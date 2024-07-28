using System.ComponentModel.DataAnnotations.Schema;
using Tuynuk.Infrastructure.Enums.Cliens;
using Tuynuk.Infrastructure.Models.Base;

namespace Tuynuk.Infrastructure.Models
{
    public class Client : BaseEntity
    {
        public Client(string connectionId, ClientType type, Guid sessionId, string publicKey) 
        {
            ConnectionId = connectionId;
            Type = type;
            SessionId = sessionId;
            PublicKey = publicKey;
        }

        public ClientType Type { get; set; }
        public string PublicKey { get; set; }

        /// <summary>
        // ConnectionId provided by SignalR
        // Only at the moment connected clients have ConnectionId
        /// </summary>
        public string ConnectionId { get; set; }

        public Guid SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session Session { get; set; }
    }
}
