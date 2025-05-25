using ERPtask.models;
using ERPtask.Repositrories;
using System.Data.SqlClient;
namespace ERPtask.servcies
{
    public class CostEntryService
    {
        private readonly CostEntryRepository _repository;

        public CostEntryService(CostEntryRepository repository)
        {
            _repository = repository;
        }

        public async Task<CostEntry> CreateCostEntry(CostEntry costEntry)
        {
            return await _repository.CreateCostEntry(costEntry);
        }

        public async Task<CostEntry> GetCostEntryById(Guid id)
        {
            return await _repository.GetCostEntryById(id);
        }

        public async Task<IEnumerable<CostEntry>> GetAllCostEntries()
        {
            return await _repository.GetAllCostEntries();
        }

        public async Task<bool> UpdateCostEntry(CostEntry costEntry)
        {
            return await _repository.UpdateCostEntry(costEntry);
        }

        public async Task<bool> DeleteCostEntry(Guid id)
        {
            return await _repository.DeleteCostEntry(id);
        }
    }
}
