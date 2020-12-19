using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Web;
using MongoDB.Web.Providers;

namespace MACServices
{
    public class Consumer : User
    {
        public Consumer()
        { }
        public Consumer(string clientId, string endUserIpAddress)
        {
            this.ClientId = ObjectId.Parse(clientId);
            this.RecordOperations.CreateDate = DateTime.UtcNow;
            this.RecordOperations.CreatedByIpAddress = endUserIpAddress;
            this.RecordOperations.IsActivated = true;
            this.RecordOperations.IsDeleted = false;
        }
    }
}
