using System.ComponentModel.DataAnnotations;

namespace Tuynuk.Models.Base
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
