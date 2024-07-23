namespace Tuynuk.Api.Services.Base
{
    public interface IBaseService<TService>
    {
        IConfiguration Configuration { get; }
        ILogger<TService> Logger { get; }
        HttpContext HttpContext { get; }
    }
}
