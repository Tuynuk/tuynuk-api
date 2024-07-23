using System.ComponentModel.DataAnnotations;
using Tuynuk.Infrastructure.Enums.Base;

namespace Tuynuk.Infrastructure.Models.Base
{
    public abstract class PersistentEntity : BaseEntity
    {
        [Required]
        public EntityState State { get; set; } = EntityState.Active;
    }
}
