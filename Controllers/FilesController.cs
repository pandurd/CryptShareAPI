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


        [HttpPost]
        [Route("share")]
        public async Task<IActionResult> PostAsync([FromBody] SharedFileEntity entity, string FileName)
        {
            entity.PartitionKey = entity.SharedEmail;
            entity.RowKey = Guid.NewGuid().ToString();

            var createdEntity = await _storageService.InsertOrMergeSharedFileAsync(entity);
            var subject = "Cyrpt Share File share";
            var content = $"File {FileName} has been shared on the site, Please visit site";
 
            CryptShareAPI.Services.EmailService.SendEmail(entity.SharedEmail, subject, content);

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

        [HttpPut]
        [Route("OTP")]
        public async Task<IActionResult> PutOTPAsync([FromBody] SharedFileEntity entity)
        {
            var random = new Random();
            var otp = random.Next(1000, 9999);
            entity.OTP = otp;

            var subject = "OTP for file";
            var content = $"OTP for opening file {otp} ";
            CryptShareAPI.Services.EmailService.SendEmail(entity.SharedEmail, subject, content);

            await _storageService.InsertOrMergeSharedFileAsync(entity);
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
