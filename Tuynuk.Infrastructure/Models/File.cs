using System.ComponentModel.DataAnnotations.Schema;
using Tuynuk.Infrastructure.Models.Base;

namespace Tuynuk.Infrastructure.Models
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
