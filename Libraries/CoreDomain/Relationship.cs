using MongoDB.Bson;

namespace MACServices
{
    public class Relationship
    {
        public Relationship()
        {
            Enabled = true;
        }

        public bool Enabled { get; set; }
        public ObjectId MemberId { get; set; }
        public string MemberType { get; set; }
        public string MemberHierarchy { get; set; }
    }
}