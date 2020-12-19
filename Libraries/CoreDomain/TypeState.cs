using MongoDB.Bson;

namespace MACServices
{
    public class State
    {
        public State(string name)
        {
            _id = ObjectId.GenerateNewId();
            Name = name;
            Description = "";
            Abbreviation = "";
        }

        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
    }
}