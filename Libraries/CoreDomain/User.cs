using System.Collections.Generic;
using MongoDB.Bson;

namespace MACServices
{
    public class User : Base
    {
        public List<Event> UserEvents = new List<Event>();

        public User()
        {
            _id = ObjectId.GenerateNewId();
            UserId = _id;
            UserInfo = new UserProfile(Constants.Strings.DefaultEmptyObjectId);
            SecurityInfo = new UserSecurity();
        }

        public ObjectId UserId { get; set; }
        public bool Enabled { get; set; }
        public string UserIpAddress { get; set; }
        public string[] UserRoles { get; set; }

        public UserProfile UserInfo { get; set; }
        public UserSecurity SecurityInfo { get; set; }
    }
}