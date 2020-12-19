using MongoDB.Bson;

namespace MACServices
{
    public class Member
    {
        public Member()
        {
            MemberId = ObjectId.GenerateNewId();
            MemberType = "";
            MemberHierarchy = "";
        }

        public ObjectId MemberId { get; set; }
        public string MemberType { get; set; }
        public string MemberHierarchy { get; set; }
    }
}