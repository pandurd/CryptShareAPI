using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CryptShareAPI.Models;
using CryptShareAPI.Services;

namespace CryptShareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController  : ControllerBase
    {
        private readonly ITableStorageService _storageService;

        public FilesController(ITableStorageService storageService)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        [HttpGet]
        [Route("email/{email}")]
        public async Task<IActionResult> GetAsyncByEmail(string email)
        {
            return Ok(await _storageService.RetrieveAsyncByEmail(email));
        }

        [HttpGet]
        [Route("shared/{email}")]
        public async Task<IActionResult> GetSharedAsyncByEmail(string email)
        {
            return Ok(await _storageService.RetrieveSharedAsyncByEmail(email));
        }

        [HttpGet]
        [Route("Expired")]
        public async Task<IActionResult> GetExpired()
        {
            return Ok(await _storageService.RetrieveExpiredAsync());
        }

        [HttpGet]
        [Route("id/{email}/{FileGuid}")]
        public async Task<IActionResult> GetAsyncByFile(string email, Guid FileGuid)
        {
            return Ok(await _storageService.RetrieveAsyncByFile(FileGuid, email));
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> PostAsync([FromBody] FileEntity entity)
        {
            entity.PartitionKey = entity.Email;
            entity.FileGuid = Guid.NewGuid();
            entity.RowKey = entity.FileGuid.ToString();
            entity.ExpireDateTime = entity.Expire.Ticks;

            var createdEntity = await _storageService.InsertOrMergeAsync(entity);

            return Ok(createdEntity);
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] FileEntity entity)
        {
            entity.PartitionKey = entity.Email;
            entity.RowKey = entity.FileGuid.ToString();

            await _storageService.InsertOrMergeAsync(entity);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([FromQuery] Guid FileGuid, string email)
        {
            var entity = await _storageService.RetrieveAsyncByFile(FileGuid, email);
            await _storageService.DeleteAsync(entity);
            return NoContent();
        }

        [HttpDelete]
        [Route("Expired")]
        public async Task<IActionResult> DeleteExpiredAsync()
        {
            var expired = await _storageService.RetrieveExpiredAsync();
            
            foreach(var entity in expired)
                await _storageService.DeleteAsync(entity);

            return NoContent();
        }
    }
}
