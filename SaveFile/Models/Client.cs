using SaveFile.Enums;
using SaveFile.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaveFile.Models
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
