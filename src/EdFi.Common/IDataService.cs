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

        Task SaveAsync(ChangeQuery entity);

        Task<int> AddOrUpdateAllAsync<TDataIn>(List<TDataIn> entities) where TDataIn : RosterDataRecord;

        Task DeleteAllAsync<TDataIn>(List<string> resourceIds) where TDataIn : RosterDataRecord;

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

        public async Task SaveAsync(ChangeQuery entity)
        {
            var existingRecord = await _dbContext.Set<ChangeQuery>()
                .SingleOrDefaultAsync(q => q.ResourceType == entity.ResourceType);

            if (existingRecord != null)
            {
                existingRecord.ChangeVersion = entity.ChangeVersion;
            }
            else
            {
                await _dbContext.Set<ChangeQuery>().AddAsync(entity);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> AddOrUpdateAllAsync<TDataIn>(List<TDataIn> entities) where TDataIn : RosterDataRecord
        {
            var addedRecords = 0;
            foreach (var entity in entities)
            {
                var existing = await _dbContext.Set<TDataIn>()
                    .SingleOrDefaultAsync(q => q.ResourceId != null && q.ResourceId == entity.ResourceId);

                if (existing != null)
                {
                    existing.Content = entity.Content;
                }
                else
                {
                    await _dbContext.Set<TDataIn>().AddAsync(entity);
                    addedRecords++;
                }
            }
            await _dbContext.SaveChangesAsync();
            return addedRecords;
        }

        public async Task DeleteAllAsync<TDataIn>(List<string> resourceIds) where TDataIn : RosterDataRecord
        {
            foreach (var resourceId in resourceIds)
            {
                var existing = await _dbContext.Set<TDataIn>()
                    .SingleOrDefaultAsync(q => q.ResourceId != null && q.ResourceId == resourceId);

                if (existing != null)
                {
                    _dbContext.Set<TDataIn>().Remove(existing);
                }
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
