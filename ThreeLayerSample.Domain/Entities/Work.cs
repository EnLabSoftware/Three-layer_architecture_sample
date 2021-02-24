#nullable disable

namespace ThreeLayerSample.Domain.Entities
{
    public partial class Work : EntityBase<int>
    {
        
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
