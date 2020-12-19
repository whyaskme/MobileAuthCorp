using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;

namespace STLib
{
    public class TestCreditCard : MongoDBData
    {
        public TestCreditCard()
        {
            _id = ObjectId.GenerateNewId();
            _t = "TestCreditCard";

            macPlayerId = ObjectId.Empty;

            cardType = "";
            cardNumber = "";
            cardExpires = "";
            cardCCV = "";

            registeredDate = "";

            stiPlayer = new StiPlayer();
            events = new List<StiEvent>();
        }

        public ObjectId _id { get; set; }
        public string _t { get; set; }

        public ObjectId macPlayerId { get; set; }

        public string cardType { get; set; }
        public string cardNumber { get; set; }
        public string cardExpires { get; set; }
        public string cardCCV { get; set; }
        public string registeredDate { get; set; }

        public StiPlayer stiPlayer { get; set; }
        public List<StiEvent> events { get; set; }
    }

}
