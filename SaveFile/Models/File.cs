using SaveFile.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaveFile.Models
{
    public class File : BaseEntity
    {
        public string Content { get; set; }

        public string Extension { get; set; }

        public string Name { get; set; }

        public Guid SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session Session { get; set; }
    }
}
