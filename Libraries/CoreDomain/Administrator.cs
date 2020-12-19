using System;
using MongoDB.Bson;

namespace MACServices
{
    public class Administrator : User
    {
        public Administrator(string adminId)
        {
            if (String.IsNullOrEmpty(adminId))
            {
                _id = ObjectId.GenerateNewId();
                Enabled = true;
                Name = "";
                var arrRoles = new[] {"User (Consumer)", "User (System Administrator)"};
                UserRoles = arrRoles;
            }
            else
            {
                _id = ObjectId.Parse(adminId);

                var myAdministrator = (User) Read();
                Enabled = myAdministrator.Enabled;
                Name = myAdministrator.Name;
                Relationships = myAdministrator.Relationships;
            }
        }
    }
}