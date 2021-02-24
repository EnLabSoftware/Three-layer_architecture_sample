using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace ThreeLayerSample.Domain.CustomGenerator
{
    public class MyDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            //Register custom entity generator to design-time service of EF Core
            serviceCollection.AddSingleton<ICSharpEntityTypeGenerator, MyEntityTypeGenerator>();
        }
    }
}
