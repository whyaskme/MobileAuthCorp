using MongoDB.Bson;

namespace MACServices
{
    public class RegistrationType
    {
        public RegistrationType(string name)
        {
            _id = ObjectId.GenerateNewId();
            Name = name;
            Description = "";
        }

        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}