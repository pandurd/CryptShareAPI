using CryptShareAPI.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace CryptShareAPI.Services
{
	public class TableStorageService : ITableStorageService
	{
		private const string FilesTableName = "Files";
		private const string SharedFilesTableName = "SharedFiles";
		private readonly IConfiguration _configuration;

		public TableStorageService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task<List<FileEntity>> RetrieveAsyncByEmail(string email)
		{
			var storageAccount = CloudStorageAccount.Parse(_configuration["StorageConnectionString"]);
			var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
			var table = tableClient.GetTableReference(FilesTableName);

			TableContinuationToken token = null;
			var entities = new List<FileEntity>();
			do
			{
				var query = new TableQuery<FileEntity>();
				query.Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, email));

				var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
				entities.AddRange(queryResult.Results);
				token = queryResult.ContinuationToken;
			} while (token != null);


			return entities;
		}

		public async Task<List<FileEntity>> RetrieveExpiredAsync()
		{
			var storageAccount = CloudStorageAccount.Parse(_configuration["StorageConnectionString"]);
			var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
			var table = tableClient.GetTableReference(FilesTableName);

			TableContinuationToken token = null;
			var entities = new List<FileEntity>();
			do
			{
				var query = new TableQuery<FileEntity>();
				query.Where(TableQuery.GenerateFilterCondition("ExpireDateTime", QueryComparisons.LessThanOrEqual, DateTime.Now.Ticks.ToString()));

				var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
				entities.AddRange(queryResult.Results);
				token = queryResult.ContinuationToken;
			} while (token != null);


			return entities;
		}

		public async Task<List<SharedFileEntity>> RetrieveSharedAsyncByEmail(string email)
		{
			var storageAccount = CloudStorageAccount.Parse(_configuration["StorageConnectionString"]);
			var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
			var table = tableClient.GetTableReference(SharedFilesTableName);

			TableContinuationToken token = null;
			var entities = new List<SharedFileEntity>();
			do
			{
				var query = new TableQuery<SharedFileEntity>();
				query.Where(TableQuery.GenerateFilterCondition("SharedEmail", QueryComparisons.Equal, email));

				var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
				entities.AddRange(queryResult.Results);
				token = queryResult.ContinuationToken;
			} while (token != null);


			return entities;
		}

		public async Task<FileEntity> RetrieveAsyncByFile(Guid guid, string email)
		{
			var retrieveOperation = TableOperation.Retrieve<FileEntity>(email, guid.ToString());
			return await ExecuteTableOperation(retrieveOperation) as FileEntity;
		}

		public async Task<SharedFileEntity> RetrieveSharedAsyncByFile(Guid guid, string email)
		{
			var retrieveOperation = TableOperation.Retrieve<SharedFileEntity>(email, guid.ToString());
			return await ExecuteTableOperation(retrieveOperation) as SharedFileEntity;
		}

		public async Task<FileEntity> InsertOrMergeAsync(FileEntity entity)
		{
			var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
			return await ExecuteTableOperation(insertOrMergeOperation) as FileEntity;
		}
		public async Task<SharedFileEntity> InsertOrMergeSharedFileAsync(SharedFileEntity entity)
		{
			var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
			return await ExecuteTableOperation(insertOrMergeOperation) as SharedFileEntity;
		}

		public async Task<FileEntity> DeleteAsync(FileEntity entity)
		{
			var deleteOperation = TableOperation.Delete(entity);
			return await ExecuteTableOperation(deleteOperation) as FileEntity;
		}
		public async Task<SharedFileEntity> DeleteSharedFileAsync(SharedFileEntity entity)
		{
			var deleteOperation = TableOperation.Delete(entity);
			return await ExecuteTableOperation(deleteOperation) as SharedFileEntity;
		}
		private async Task<object> ExecuteTableOperation(TableOperation tableOperation)
		{
			var table = await GetCloudTable();
			var tableResult = await table.ExecuteAsync(tableOperation);
			return tableResult.Result;
		}

		private async Task<CloudTable> GetCloudTable()
		{
			var storageAccount = CloudStorageAccount.Parse(_configuration["StorageConnectionString"]);
			var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
			var table = tableClient.GetTableReference(FilesTableName);
			await table.CreateIfNotExistsAsync();
			return table;
		}
    }
}
