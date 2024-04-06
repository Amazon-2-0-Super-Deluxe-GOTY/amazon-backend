using amazon_backend.Data;
using amazon_backend.Data.Dao;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;


namespace amazon_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            

            // register db context
            // enabled entity framework
            string? connectionString = builder.Configuration.GetConnectionString("MySQLLocal");
            if (connectionString == null)
            {
                throw new Exception("No connection string in appsettings.json");
            }
            builder.Services.AddDbContext<DataContext>(
                options => options.
                    UseMySQL(connectionString)
                );

            // services that rely on DbContext
            builder.Services.AddScoped<ICategoryDAO, CategoryDao>();
            builder.Services.AddScoped<IUserDao, UserDao>();
            builder.Services.AddScoped<IClientProfileDao, ClientProfileDao>();
            builder.Services.AddScoped<IProductDao, ProductDao>();
            builder.Services.AddScoped<IProductPropsDao, ProductPropsDao>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
