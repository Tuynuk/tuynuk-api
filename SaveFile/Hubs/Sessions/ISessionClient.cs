namespace SaveFile.Hubs.Sessions
{
    public interface ISessionClient
    {
        Task OnSessionReady(string oppositeClientPublicKey);
        Task OnSessionCreated(string sessionIdentifier);
    }
}
