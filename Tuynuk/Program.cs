
using Microsoft.EntityFrameworkCore;
using Tuynuk.Data;
using Tuynuk.Hubs.Sessions;
using Tuynuk.Services.Files;
using Tuynuk.Services.Sessions;

namespace Tuynuk
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
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<Random>();

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
