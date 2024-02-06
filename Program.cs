using amazon_backend.Data;
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
            String? connectionString = builder.Configuration.GetConnectionString("MySQL");
            if (connectionString != null)
            {
                builder.Services.AddDbContext<DataContext>(
                    options => options.
                        UseMySQL(connectionString)
                    );
            }
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
