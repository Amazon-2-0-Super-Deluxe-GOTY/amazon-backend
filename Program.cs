using amazon_backend.Data;
using amazon_backend.Data.Dao;
using amazon_backend.Options.Token;
using amazon_backend.Profiles;
using amazon_backend.Services.Email;
using amazon_backend.Services.Hash;
using amazon_backend.Services.JWTService;
using amazon_backend.Services.KDF;
using amazon_backend.Services.Random;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.Globalization;
using amazon_backend.Services.Response;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using amazon_backend.Services.AWSS3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace amazon_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var jwt = builder.Configuration.GetSection("JwtBearer");
            // Add services to the container.
            ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IHashService, Md5HashService>();
            builder.Services.AddSingleton<IKdfService, HashKdfService>();
            builder.Services.AddSingleton<IRandomService, RandomService>();
            builder.Services.AddSingleton<IEmailService, EmailService>();

            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
            // register db context
            // enabled entity framework
            string? connectionString = builder.Configuration.GetConnectionString("MySQL");
            if (connectionString == null)
            {
                throw new Exception("No connection string in appsettings.json");
            }
            try
            {
                builder.Services.AddDbContext<DataContext>(
                    options => options.
                        UseMySQL(connectionString)
                    );
            }
            catch (Exception ex)
            {
                throw new Exception("Can't connect to MySql Server");
            }
            // services that rely on DbContext
            #region DAO's
            builder.Services.AddScoped<ICategoryDao, CategoryDao>();
            builder.Services.AddScoped<IUserDao, UserDao>();
            builder.Services.AddScoped<IProductDao, ProductDao>();
            builder.Services.AddScoped<IProductPropsDao, ProductPropsDao>();
            builder.Services.AddScoped<IReviewDao, ReviewDao>();
            #endregion 

            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.Configure<TokenOptions>(jwt);

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddScoped<TokenService>();

            var secretKey = jwt.GetValue<string>("SecretKey");
            if (secretKey == null)
            {
                throw new Exception("The secret key cannot be null or empty. Please provide a valid secret in the TokenOptions.");
            }
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            builder.Services.AddSingleton(sp => new RestResponseService(sp.GetRequiredService<ILogger<RestResponseService>>(), jsonSerializerSettings));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.Services.AddS3Client(builder.Configuration);
            builder.Services.AddSingleton<IS3Service, S3Service>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.MapControllers();

            app.Run();
        }
    }
}
