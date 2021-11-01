using System;
using Microsoft.Azure.Cosmos.Table;

namespace CryptShareAPI.Models
{
    public class SharedFileEntity : TableEntity
    {
        public Guid Id { get; set; }
        public Guid FileGuid { get; set; }
        public string SharedEmail { get; set; }
        public long OTP { get; set; }
    }
}
