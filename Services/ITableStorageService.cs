using CryptShareAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptShareAPI.Services
{
    public interface ITableStorageService
    {
        Task<List<FileEntity>> RetrieveAsyncByEmail(string email);
        Task<List<FileEntity>> RetrieveExpiredAsync();
        Task<List<SharedFileEntity>> RetrieveSharedAsyncByEmail(string email);
        Task<FileEntity> RetrieveAsyncByFile(Guid FileGuid, string email);
        Task<FileEntity> InsertOrMergeAsync(FileEntity entity);
        Task<SharedFileEntity> InsertOrMergeSharedFileAsync(SharedFileEntity entity);
        Task<FileEntity> DeleteAsync(FileEntity entity);
        Task<SharedFileEntity> DeleteSharedFileAsync(SharedFileEntity entity);
    }
}
