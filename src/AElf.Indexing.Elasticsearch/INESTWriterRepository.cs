using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace AElf.Indexing.Elasticsearch
{
    public interface INESTWriterRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
    {
        Task AddOrUpdateAsync(TEntity model, string index = null);
        
        Task AddAsync(TEntity model, string index = null);
        
        Task UpdateAsync(TEntity model, string index = null);

        Task DeleteAsync(TKey id, string index = null);
        
        Task DeleteAsync(TEntity model, string index = null);
        
        Task BulkAddOrUpdateAsync(List<TEntity> list, string index = null);

        Task BulkDeleteAsync(List<TEntity> list, string index = null);

        Task BulkUpdateAsync(List<TEntity> list, string index = null);
    }
}