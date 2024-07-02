using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.Indexing.Elasticsearch.Exceptions;
using AElf.Indexing.Elasticsearch.Options;
using AElf.Indexing.Elasticsearch.Provider;
using Microsoft.Extensions.Options;
using Nest;
using Volo.Abp.Domain.Entities;

namespace AElf.Indexing.Elasticsearch
{
    public class NESTRepository<TEntity, TKey> : NESTReaderRepository<TEntity, TKey>,
        INESTRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        public NESTRepository(IEsClientProvider esClientProvider,IOptionsSnapshot<IndexSettingOptions> indexSettingOptions, string index = null, string type = null)
            : base(esClientProvider, indexSettingOptions, index, type)
        {
        }

        /// <summary>
        /// AddOrUpdate Document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task AddOrUpdateAsync(TEntity model, string index = null)
        {
            var indexName = index?? IndexName;
            var client = GetElasticClient();
            var exits = client.DocumentExists(DocumentPath<TEntity>.Id(new Id(model)), dd => dd.Index(indexName));

            if (exits.Exists)
            {
                var result = await client.UpdateAsync(DocumentPath<TEntity>.Id(new Id(model)),
                    ss => ss.Index(indexName).Doc(model).RetryOnConflict(3).Refresh(IndexSettingOptions.Refresh));

                if (result.IsValid) return;
                throw new ElasticSearchException($"Update Document failed at index{indexName} :" +
                                                 result.ServerError.Error.Reason);
            }
            else
            {
                var result = await client.IndexAsync(model, ss => ss.Index(indexName).Refresh(IndexSettingOptions.Refresh));
                if (result.IsValid) return;
                throw new ElasticSearchException($"Insert Docuemnt failed at index {indexName} :" +
                                                 result.ServerError.Error.Reason);
            }
        }
        
        /// <summary>
        /// Add Document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task AddAsync(TEntity model, string index = null)
        {
            var indexName = index?? IndexName;
            var client = GetElasticClient();
            var result = await client.IndexAsync(model, ss => ss.Index(indexName).Refresh(IndexSettingOptions.Refresh));
            if (result.IsValid) return;
            throw new ElasticSearchException($"Insert Docuemnt failed at index {indexName} :" +
                                             result.ServerError.Error.Reason);
        }

        /// <summary>
        /// Update Document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task UpdateAsync(TEntity model, string index = null)
        {
            var indexName = index?? IndexName;
            var client = GetElasticClient();
            var result = await client.UpdateAsync(DocumentPath<TEntity>.Id(new Id(model)),
                ss => ss.Index(indexName).Doc(model).RetryOnConflict(3).Refresh(IndexSettingOptions.Refresh));

            if (result.IsValid) return;
            throw new ElasticSearchException($"Update Document failed at index{indexName} :" +
                                             result.ServerError.Error.Reason);
        }
        
        /// <summary>
        /// Delete Document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task DeleteAsync(TKey id, string index = null)
        {
            var indexName = index?? IndexName;
            var client = GetElasticClient();
            var response = await client.DeleteAsync(new DeleteRequest(indexName, new Id(new {id = id.ToString()})) {Refresh = IndexSettingOptions.Refresh});
            if (response.ServerError == null)
            {
                return;
            }
            throw new Exception($"Delete Docuemnt at index {indexName} :{response.ServerError.Error.Reason}");
        }

        /// <summary>
        /// Delete Document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task DeleteAsync(TEntity model, string index = null)
        {
            var indexName = index?? IndexName;
            var client = GetElasticClient();
            var response = await client.DeleteAsync(new DeleteRequest(indexName, new Id(model)) {Refresh = IndexSettingOptions.Refresh});
            if (response.ServerError == null)
            {
                return;
            }
            throw new Exception($"Delete Docuemnt at index {indexName} :{response.ServerError.Error.Reason}");
        }
        
        /// <summary>
        /// Batch add and modify
        /// </summary>
        /// <param name="list"></param>
        /// <exception cref="ElasticSearchException"></exception>
        public async Task BulkAddOrUpdateAsync(List<TEntity> list, string index = null)
        {
            var indexName = index?? IndexName;
            var client = GetElasticClient();
            var bulk = new BulkRequest(indexName)
            {
                Operations = new List<IBulkOperation>(),
                Refresh = IndexSettingOptions.Refresh
            };
            foreach (var item in list)
            {
                bulk.Operations.Add(new BulkIndexOperation<TEntity>(item));
            }
            var response = await client.BulkAsync(bulk);
            if (!response.IsValid)
            {
                throw new ElasticSearchException(
                    $"Bulk InsertOrUpdate Docuemnt failed at index {indexName} :{response.ServerError.Error.Reason}");
            }
        }

        /// <summary>
        /// Deleting Documents in Batches
        /// </summary>
        /// <param name="list"></param>
        /// <exception cref="ElasticSearchException"></exception>
        public async Task BulkDeleteAsync(List<TEntity> list, string index = null)
        {
            var indexName = index?? IndexName;
            var client = GetElasticClient();
            var bulk = new BulkRequest(indexName)
            {
                Operations = new List<IBulkOperation>(),
                Refresh = IndexSettingOptions.Refresh
            };
            foreach (var item in list)
            {
                bulk.Operations.Add(new BulkDeleteOperation<TEntity>(new Id(item)));
            }

            var response = await client.BulkAsync(bulk);
            if (response.ServerError == null)
            {
                return;
            }
            throw new ElasticSearchException(
                    $"Bulk Delete Docuemnt at index {indexName} :{response.ServerError.Error.Reason}");
        }

        public async Task BulkUpdateAsync(List<TEntity> list, string index = null)
        {
            var indexName = index?? IndexName;
            var client = GetElasticClient();
            var bulk = new BulkRequest(indexName)
            {
                Operations = new List<IBulkOperation>(),
                Refresh = IndexSettingOptions.Refresh
            };
            
            foreach (var item in list)
            {
                var updateOperation = new BulkUpdateOperation<TEntity,TEntity>(new Id(item))
                {
                    Doc = item,
                    Index = indexName
                };
                bulk.Operations.Add(updateOperation);
            }
            
            var response = await client.BulkAsync(bulk);
            if (!response.IsValid)
            {
                throw new ElasticSearchException(
                    $"Bulk Update Document failed at index {indexName} :{response.ServerError.Error.Reason}");
            }
        }
    }
}