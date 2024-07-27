using System.ComponentModel.DataAnnotations;
using Tuynuk.Infrastructure.Models.Base;

namespace Tuynuk.Infrastructure.Models
{
    public class Session : BaseEntity
    {
        /// <summary>
        /// Stored as a SHA256 hash to prevent identification and connection to sessions from database access
        /// </summary>
        [Required]
        public string Identifier { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public File File { get; set; }
    }
}
