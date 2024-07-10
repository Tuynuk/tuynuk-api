using System.ComponentModel.DataAnnotations;

namespace SaveFile.Models.Base
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
