using MongoDB.Bson;

namespace MACServices
{
    public class TypeUser
    {
        public TypeUser(string description)
        {
            _id = ObjectId.GenerateNewId();
            Description = description;
        }

        public ObjectId _id { get; set; }
        public string Description { get; set; }
    }
}