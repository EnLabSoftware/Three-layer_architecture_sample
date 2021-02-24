#nullable disable

namespace ThreeLayerSample.Domain.Entities
{
    public partial class VwWork : EntityBase<int>
    {
        
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
