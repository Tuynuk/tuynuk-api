using Microsoft.EntityFrameworkCore;
using Tuynuk.Api.Data;
using Tuynuk.Api.Data.Repositories.Clients;
using Tuynuk.Api.Data.Repositories.Files;
using Tuynuk.Api.Data.Repositories.Sessions;
using Tuynuk.Api.Hubs.Sessions;
using Tuynuk.Api.Services.Files;
using Tuynuk.Api.Services.Sessions;

namespace Tuynuk
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            builder.Services.AddDbContext<TuynukDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Postgre")));
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<Random>();

            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddScoped<IFilesRepository, FilesRepository>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();

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
