using Microsoft.EntityFrameworkCore;
using Tuynuk.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Tuynuk.Models
{
    public class Session : BaseEntity
    {
        [Required]
        public string Identifier { get; set; }  
        public IEnumerable<Client> Clients { get; set; }
        public File File { get; set; }
    }
}
