using System.Collections.Generic;
using System.Threading.Tasks;
using ThreeLayerSample.Domain.Entities;

namespace ThreeLayerSample.Domain.Interfaces.Services
{
    public interface IWorkService
    {
        /// <summary>
        /// Get all items of Work table
        /// </summary>
        /// <returns></returns>
        Task<IList<Work>> GetAll();
        Task<Work> GetOne(int workId);
        Task Update(Work work);
        Task Add(Work work);
        Task Delete(int workId);
    }
}
