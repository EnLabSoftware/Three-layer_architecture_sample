using System.Collections.Generic;
using System.Threading.Tasks;
using ThreeLayerSample.Domain.Entities;
using ThreeLayerSample.Domain.Interfaces;
using ThreeLayerSample.Infrastructure.Extensions;

namespace ThreeLayerSample.Infrastructure.Repositories
{
    public static class WorkRepository
    {
        /// <summary>
        /// Get all items of work table from stored procedure
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static async Task<IList<Work>> GetAll(this IRepository<Work> repository)
        {
            var works = new List<Work>();
            await repository.DbContext.LoadStoredProc("spGetWorks")
                //.WithSqlParam("sampleParam", "sampleValue") // Sample code to add params to provided stored procedure
                .ExecuteStoredProcAsync(result =>
                {
                    // Read value to list
                    works = result.ReadNextListOrEmpty<Work>();
                    // Sample code to read to value
                    // var getOne = result.ReadToValue<int>() 
                });


            return works;
        }
    }
}
