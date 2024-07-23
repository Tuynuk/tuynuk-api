using System.ComponentModel.DataAnnotations.Schema;
using Tuynuk.Infrastructure.Enums.Cliens;
using Tuynuk.Infrastructure.Models.Base;

namespace Tuynuk.Infrastructure.Models
{
    public class Client : BaseEntity
    {
        public ClientType Type { get; set; }
        public string PublicKey { get; set; }

        public string ConnectionId { get; set; }

        public Guid SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session Session { get; set; }
    }
}
