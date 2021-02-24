using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Put into the same namespace of Entities to skip using its reference namespace on inherit
namespace ThreeLayerSample.Domain.Entities
{
    public interface IEntityBase<TKey>
    {
        TKey Id { get; set; }
    }

    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual TKey Id { get; set; }
    }
}
