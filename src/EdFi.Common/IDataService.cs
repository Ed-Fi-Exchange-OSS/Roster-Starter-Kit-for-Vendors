using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Roster.Data;
using EdFi.Roster.Models;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Common
{
    public interface IDataService
    {
        Task<IEnumerable<TEntity>> ReadAllAsync<TEntity>() where TEntity : class;

        Task SaveAsync<TDataIn>(List<TDataIn> entities, bool doNotDeleteExistingRecords = false) where TDataIn : class;

        Task SaveAsync<TDataIn>(TDataIn entity) where TDataIn : ChangeQuery;

        void ClearRecords<TDataIn>() where TDataIn : class;
    }

    public class DataService : IDataService
    {
        private readonly BaseDbContext _dbContext;

        public DataService(BaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TEntity>> ReadAllAsync<TEntity>() where TEntity : class
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task SaveAsync<TDataIn>(List<TDataIn> entities, bool doNotDeleteExistingRecords = false) where TDataIn : class
        {
            if (!doNotDeleteExistingRecords)
            {
                ClearRecords<TDataIn>();
            }

            foreach (var entity in entities)
            {
               await _dbContext.Set<TDataIn>().AddAsync(entity);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveAsync<TDataIn>(TDataIn entity) where TDataIn : ChangeQuery
        {
            var existingRecord = await _dbContext.Set<TDataIn>()
                .FirstOrDefaultAsync(q => q.ResourceType == entity.ResourceType);

            if (existingRecord != null)
            {
                existingRecord.ChangeVersion = entity.ChangeVersion;
            }
            else
            {
                await _dbContext.Set<TDataIn>().AddAsync(entity);
            }

            await _dbContext.SaveChangesAsync();
        }

        public void ClearRecords<TDataIn>() where TDataIn : class
        {
            _dbContext.Set<TDataIn>().RemoveRange(_dbContext.Set<TDataIn>());
            _dbContext.SaveChanges();
        }
    }
}
