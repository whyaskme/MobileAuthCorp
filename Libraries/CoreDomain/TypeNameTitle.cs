using MongoDB.Bson;

namespace MACServices
{
    public class TypeNameTitle
    {
        public TypeNameTitle(string description)
        {
            _id = ObjectId.GenerateNewId();
            Description = description;
        }

        public ObjectId _id { get; set; }
        public string Description { get; set; }
    }
}