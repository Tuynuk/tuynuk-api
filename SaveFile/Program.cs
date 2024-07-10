
using Microsoft.EntityFrameworkCore;
using SaveFile.Data;
using SaveFile.Hubs.Sessions;
using SaveFile.Services.Sessions;

namespace SaveFile
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Postgre")));
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<Random>();
           
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapHub<SessionHub>("hubs/session");

            app.MapControllers();

            app.Run();
        }
    }
}
