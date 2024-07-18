using Tuynuk.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuynuk.Models
{
    public class File : BaseEntity
    {
        public byte[] Content { get; set; } 

        public string Name { get; set; }

        public string HMAC { get; set; }

        public Guid SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session Session { get; set; }
    }
}
