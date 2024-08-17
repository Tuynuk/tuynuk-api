using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Tuynuk.Api.Data;
using Tuynuk.Api.Data.Repositories.Clients;
using Tuynuk.Api.Data.Repositories.Files;
using Tuynuk.Api.Data.Repositories.Sessions;
using Tuynuk.Api.Extensions;
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

            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            string postgreConnectionString = builder.Configuration.GetConnectionString("POSTGRES");
            builder.Services.AddDbContext<TuynukDbContext>(o => o.UseNpgsql(postgreConnectionString));
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<Random>();

            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddScoped<IFilesRepository, FilesRepository>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddHangfire(x =>
                        x.UsePostgreSqlStorage(l => l.UseNpgsqlConnection(postgreConnectionString)));
            builder.Services.AddHangfireServer();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            long maxRequestBodySize = builder.Configuration.GetValue<long>("Kestrel:MaxRequestBodySizeInBytes");
            builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = maxRequestBodySize);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.Services.MigrateDatabase<TuynukDbContext>();

            app.UseHangfireDashboard();
            RecurringJob.AddOrUpdate<ISessionService>("RemoveAbandonedSessions", l => l.RemoveAbandonedSessionsAsync(), Cron.Hourly());

            app.UseAuthorization();

            app.UseGlobalErrorHandler();

            app.MapHub<SessionHub>("hubs/session");

            app.MapControllers();

            app.Run();
        }
    }
}
