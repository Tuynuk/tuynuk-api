using System.ComponentModel.DataAnnotations;
using Tuynuk.Infrastructure.Models.Base;

namespace Tuynuk.Infrastructure.Models
{
    public class Session : BaseEntity
    {
        [Required]
        public string Identifier { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public File File { get; set; }
    }
}
