using Microsoft.Azure.Cosmos.Table;
using System;

namespace CryptShareAPI.Models
{
    public class FileEntity : TableEntity
    {
        public string FileName { get; set; }
        public Guid FileGuid { get; set; }
        public string Email { get; set; }
        public long ExpireDateTime { get; set; }
        public DateTime Expire { get; set; }
    }
}
