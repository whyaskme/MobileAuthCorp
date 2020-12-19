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
    class AdminClientUser : Base
    {
        public AdminClientUser(string clientId, string endUserIpAddress)
        {
        }
    }
}
