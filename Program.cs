using amazon_backend.CQRS.Handlers.QueryHandlers;
using amazon_backend.Data;
using amazon_backend.Data.Dao;
using amazon_backend.Middleware;
using amazon_backend.Options.Token;
using amazon_backend.Profiles;
using amazon_backend.Services.Email;
using amazon_backend.Services.Hash;
using amazon_backend.Services.JWTService;
using amazon_backend.Services.KDF;
using amazon_backend.Services.Random;
using Microsoft.EntityFrameworkCore;
using MediatR;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using FluentValidation;
using amazon_backend.CQRS.Queries.Request;
using System.Globalization;
using amazon_backend.Services.Response;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;


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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IHashService, Md5HashService>();
            builder.Services.AddSingleton<IKdfService, HashKdfService>();
            builder.Services.AddSingleton<IRandomService, RandomService>();
            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddScoped<IValidator<GetProductsQueryRequest>, GetProductsByCategoryValidator>();
            builder.Services.AddScoped<IValidator<GetProductByIdQueryRequest>, GetProductByIdValidator>();
            builder.Services.AddScoped<IValidator<GetFilterItemsQueryRequest>, GetFilterItemsValidator>();

            // register db context
            // enabled entity framework
            string? connectionString = builder.Configuration.GetConnectionString("MySQLLocal");
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
            catch(Exception ex)
            {
                // log
            }
            // services that rely on DbContext
            builder.Services.AddScoped<ICategoryDao, CategoryDao>();
            builder.Services.AddScoped<IUserDao, UserDao>();
            builder.Services.AddScoped<IClientProfileDao, ClientProfileDao>();
            builder.Services.AddScoped<IProductDao, ProductDao>();
            builder.Services.AddScoped<IProductPropsDao, ProductPropsDao>();
            builder.Services.AddScoped<IReviewDao, ReviewDao>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.Configure<TokenOptions>(jwt);

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddScoped<TokenService>();
            builder.Services.AddAuthentication("MyCookieScheme")
            .AddCookie("MyCookieScheme", options =>
            {
                options.LoginPath = "/login";
                options.Cookie.Name = "SessionId";
            });

            builder.Services.AddSession(options =>
            {
                //options.IdleTimeout = TimeSpan.FromHours(3);
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
            builder.Services.AddSingleton<RestResponseService>(sp => new RestResponseService(sp.GetRequiredService<ILogger<RestResponseService>>()) { jsonSerializerSettings = jsonSerializerSettings });
            builder.Services.AddTransient<GetProductsQueryHandler>();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
           
            app.UseSession();

            app.UseSessionAuth();


            app.MapControllers();

            app.Run();
        }
    }
}
