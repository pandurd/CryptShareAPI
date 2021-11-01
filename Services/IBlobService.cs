using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptShareAPI.Services
{
    public interface IBlobService
    {
        Task<bool> SaveBlob();
    }
}
