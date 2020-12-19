using MongoDB.Bson;

namespace MACServices
{
    public class TypeEvent
    {
        public TypeEvent(string description)
        {
            _id = ObjectId.GenerateNewId();
            Description = description;
        }

        public ObjectId _id { get; set; }
        public string Description { get; set; }
    }
}