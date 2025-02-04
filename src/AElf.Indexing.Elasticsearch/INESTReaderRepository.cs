using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nest;
using Volo.Abp.Domain.Entities;

namespace AElf.Indexing.Elasticsearch
{
    public interface INESTReaderRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
    {
        Task<TEntity> GetAsync(TKey id, string index = null);

        Task<TEntity> GetAsync(Func<QueryContainerDescriptor<TEntity>, QueryContainer> filterFunc = null,
            Func<SourceFilterDescriptor<TEntity>, ISourceFilter> includeFieldFunc = null,
            Expression<Func<TEntity, object>> sortExp = null, SortOrder sortType = SortOrder.Ascending,
            string index = null);

        // Task<TEntity> GetByExpAsync(Expression<Func<TEntity, bool>> filterExp = null,
        //     Expression<Func<TEntity, object>> includeFieldExp = null,
        //     Expression<Func<TEntity, object>> sortExp = null, SortOrder sortType = SortOrder.Ascending);

        // Task<Tuple<long, List<TEntity>>> GetListByExpAsync(Expression<Func<TEntity, bool>> filterExp = null,
        //     Expression<Func<TEntity, object>> includeFieldExp = null,
        //     Expression<Func<TEntity, object>> sortExp = null, SortOrder sortType = SortOrder.Ascending
        //     , int limit = 10, int skip = 0);

        Task<Tuple<long, List<TEntity>>> GetListAsync(
            Func<QueryContainerDescriptor<TEntity>, QueryContainer> filterFunc = null,
            Func<SourceFilterDescriptor<TEntity>, ISourceFilter> includeFieldFunc = null,
            Expression<Func<TEntity, object>> sortExp = null, SortOrder sortType = SortOrder.Ascending
            , int limit = 1000, int skip = 0, string index = null);

        Task<Tuple<long, List<TEntity>>> GetSortListAsync(
            Func<QueryContainerDescriptor<TEntity>, QueryContainer> filterFunc = null,
            Func<SourceFilterDescriptor<TEntity>, ISourceFilter> includeFieldFunc = null,
            Func<SortDescriptor<TEntity>, IPromise<IList<ISort>>> sortFunc = null
            , int limit = 1000, int skip = 0, string index = null);

        Task<ISearchResponse<TEntity>> SearchAsync(SearchDescriptor<TEntity> query,
            int skip, int size, string index = null, string[] includeFields = null,
            string preTags = "<strong style=\"color: red;\">", string postTags = "</strong>", bool disableHigh = false,
            params string[] highField);

        Task<CountResponse> CountAsync(
            Func<QueryContainerDescriptor<TEntity>, QueryContainer> query, string indexPrefix = null);
        // Task<CountResponse> CountByExpAsync(
        //     Expression<Func<TEntity, bool>> filterExp);
        Task<Tuple<long, List<TEntity>>> GetListByLucenceAsync(string filter, Func<SortDescriptor<TEntity>, IPromise<IList<ISort>>> sortFunc = null,
            int limit = 1000, int skip = 0, string index = null);

    }
}