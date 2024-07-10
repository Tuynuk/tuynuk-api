using Microsoft.EntityFrameworkCore;
using SaveFile.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace SaveFile.Models
{
    public class Session : BaseEntity
    {
        [Required]
        public string Identifier { get; set; }  
        public IEnumerable<Client> Clients { get; set; }
        public File File { get; set; }
    }
}
