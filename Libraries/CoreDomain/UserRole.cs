using MongoDB.Bson;

namespace MACServices
{
    public class UserRole
    {
        public UserRole()
        {
            _id = ObjectId.GenerateNewId();
            ApplicationName = "MAC_R1";
            Role = "";
            SortOrder = 1;
        }

        public ObjectId _id { get; set; }
        public string ApplicationName { get; set; }
        public string Role { get; set; }
        public int SortOrder { get; set; }
    }
}