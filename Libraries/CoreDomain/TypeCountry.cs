using MongoDB.Bson;

namespace MACServices
{
    public class Country
    {
        public Country(string name)
        {
            _id = ObjectId.GenerateNewId();
            Name = name;
        }

        public ObjectId _id { get; set; }
        public string Name { get; set; }
    }
}