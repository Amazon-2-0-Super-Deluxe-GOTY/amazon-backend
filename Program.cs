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
using MySql.Data.MySqlClient;


namespace amazon_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var jwt = builder.Configuration.GetSection("JwtBearer");
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IHashService, Md5HashService>();
            builder.Services.AddSingleton<IKdfService, HashKdfService>();
            builder.Services.AddSingleton<IRandomService, RandomService>();
            builder.Services.AddSingleton<IEmailService, EmailService>();



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
            catch(Exception ex)
            {
                // log
            }
            // services that rely on DbContext
            builder.Services.AddScoped<ICategoryDAO, CategoryDao>();
            builder.Services.AddScoped<IUserDao, UserDao>();
            builder.Services.AddScoped<IClientProfileDao, ClientProfileDao>();
            builder.Services.AddScoped<IProductDao, ProductDao>();
            builder.Services.AddScoped<IProductPropsDao, ProductPropsDao>();
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
