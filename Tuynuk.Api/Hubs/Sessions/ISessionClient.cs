namespace Tuynuk.Api.Hubs.Sessions
{
    public interface ISessionClient
    {
        Task OnSessionReady(string oppositeClientPublicKey);
        Task OnSessionCreated(string sessionIdentifier);
        Task OnFileUploaded(Guid fileId);
    }
}
