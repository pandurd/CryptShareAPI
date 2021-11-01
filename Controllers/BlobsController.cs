using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

using System.Security.Cryptography;
using CryptShareAPI.Services;
using System.Text;
using System.IO;
using System;
using SharpAESCrypt;

namespace CryptShareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobsController : ControllerBase
    {
        private readonly BlobServiceClient _client;
        private readonly string _password = "password";
        private readonly string containerName = "files";
        public BlobsController(BlobServiceClient client)
        {
            _client = client;
        }

        [HttpGet]
        public async IAsyncEnumerable<string> GetAsync()
        {
            BlobContainerClient container = _client.GetBlobContainerClient(containerName);
            await foreach (var blob in container.GetBlobsAsync())
            {
                yield return blob.Name;
            }
        }

        [HttpPost]
        [Route("CheckEncryption")]
        public IActionResult CheckEncryption(IFormFile file)
        {
            //var fileString = new StringBuilder();
            //using (var reader = new StreamReader(file.OpenReadStream()))
            //{
            //    while (reader.Peek() >= 0)
            //        fileString.AppendLine(reader.ReadLine());
            //}

            //var bytes = Encoding.ASCII.GetBytes(fileString.ToString());
            //var encodedString = Convert.ToBase64String(bytes);

            //var encrypted = EncryptionService.Encrypt(encodedString, _password);
            //var roundtrip = EncryptionService.Decrypt(encrypted, _password);
            //return File(roundtrip, file.ContentType, file.FileName);

            
            var outputStream = new MemoryStream();
            var st = new MemoryStream();
            Stream aesStream = new SharpAESCrypt.SharpAESCrypt("password", file.OpenReadStream(), OperationMode.Encrypt);
            Stream decr = new SharpAESCrypt.SharpAESCrypt("password", aesStream, OperationMode.Decrypt);
            // SharpAESCrypt.SharpAESCrypt.Encrypt("password", file.OpenReadStream(), outputStream);
            //outputStream.Position = 0;
            //outputStream.Flush();
            //SharpAESCrypt.SharpAESCrypt.Decrypt("password", outputStream, st);
            aesStream.Dispose();
            decr.Dispose();
            return File(decr, file.ContentType, file.FileName);

        }

        [HttpGet("{name}")]
        public async Task<FileResult> GetAsync(string name)
        {
            BlobContainerClient container = _client.GetBlobContainerClient(containerName);
            BlobClient blob = container.GetBlobClient(name);
            Response<BlobDownloadInfo> downloadInfo = await blob.DownloadAsync();

            //var encryptedFileString = new StringBuilder();
            //using (var reader = new StreamReader(downloadInfo.Value.Content))
            //{
            //    while (reader.Peek() >= 0)
            //        encryptedFileString.AppendLine(reader.ReadLine());
            //}

            //var content = EncryptionService.Decrypt(encryptedFileString.ToString(), _password);
            //var file = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var stream = EncryptionService.Decrypt(downloadInfo.Value.Content, _password);
            return File(stream, downloadInfo.Value.ContentType, name);
        }

        [HttpPost]
        public async Task PostAsync(IFormFile file)
        {
            BlobContainerClient container = _client.GetBlobContainerClient(containerName);

            var stream = EncryptionService.Encrypt(file.OpenReadStream(), _password);

            var fileString = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    fileString.AppendLine(reader.ReadLine());
            }

            //var encryptedFileString = EncryptionService.Encrypt(fileString.ToString(), _password);

            //var encryptedFile = new MemoryStream(Encoding.UTF8.GetBytes(encryptedFileString));

            await container.UploadBlobAsync(file.FileName, stream);
        }

        [HttpDelete("{name}")]
        public async Task DeleteAsync(string name)
        {
            BlobContainerClient container = _client.GetBlobContainerClient(containerName);
            BlobClient blob = container.GetBlobClient(name);
            await blob.DeleteAsync();
        }
    }
}
