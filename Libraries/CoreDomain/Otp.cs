using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace MACServices
{
    public class Otp : Base
    {
        public Otp(string clientId)
        {
            _id = ObjectId.GenerateNewId();
            ClientId = ObjectId.Parse(clientId);
            UserIpAddress = String.Empty;
            CodeHistory = new List<Event>();
            ErrorMsg = String.Empty;
            Message = String.Empty;
            UserName = String.Empty;
            TrxType = 0;
            ProvidersName = String.Empty;
            DeliveryMethod = Constants.DeliveryMethod.Sms.Item1;
            DeliveryMethodId = Constants.DeliveryMethod.Sms.Item2;
            ProvidersReply = String.Empty;
            ValidationCount = 0;
            ValidationRetries = 3;
            Active = true;
            ProviderTryList = String.Empty;
            EndOfLife = DateTime.UtcNow.AddMinutes(3);
            AdDetails = new AdDetails();
            MsgSegmentCount = 1;
            FirstTimeCarrierInfoSent = false;    // false = send carrier info, true = don't send
            RequestType = String.Empty;
            ReplyState = String.Empty;          // FI Only - State of the reply message process
            ReplyUri = String.Empty;            // FI Only - Client's reply Uri
        }

        public Otp()
        {
            //throw new NotImplementedException();
        }

        public string UserId { get; set; }
        public string UserIpAddress { get; set; }
        public string ToEmail { get; set; }
        public string ToPhone { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string ProvidersName { get; set; }
        public string DeliveryMethod { get; set; }
        public ObjectId DeliveryMethodId { get; set; }
        public string ProvidersReply { get; set; }
        public string ErrorMsg { get; set; }
        public double Cost { get; set; }
        public string TrxDetail { get; set; }
        public int TrxType { get; set; }
        public DateTime EndOfLife { get; set; }
        public bool UserOtpOutAd { get; set; }
        public bool Active { get; set; }
        public int ValidationCount { get; set; }
        public int ValidationRetries { get; set; }
        public string ProviderTryList { get; set; }
        public AdDetails AdDetails { get; set; }
        public List<Event> CodeHistory { get; set; }
        public int MsgSegmentCount { get; set; }
        public string UserName { get; set; }
        public bool FirstTimeCarrierInfoSent { get; set; }
        public string RequestType { get; set; }
        public string ReplyState { get; set; }
        public string ReplyUri { get; set; }
    }
}