using System;
using System.Collections.Generic;

using MongoDB.Bson;

namespace STLib
{
    public class StiWrapperState : MongoDBData
    {
        public StiWrapperState(string stateId)
        {
            if (String.IsNullOrEmpty(stateId))
            {
                _id = ObjectId.GenerateNewId();
                _t = "WrapperState";

                Enabled = true;
                IsValid = true;
                
                methodName = "";
                macNextMethod = "";
                macClientId = "";
                macClientName = "";
                macRequestId = "";
                macOTP = "";
                macEnterOtpAd = "";

                // Secure Trading Info
                stResponseToken = "";
                stOperatorId = "";
                stSiteId = "";
                stUserName = "";
                stPlayerId = "";
                stPlayerIp = "";
                stReferenceNumber = "";
                stRequestJson = "";
                stResponseJson = "";
                stReturnCode = "";
                stRecordStatus = "";

                //transaction info
                stProcess = "";
                stTransactionRequest = "";
                stTransactionType = "";
                stTransactionAmount= "";

                stTimeStamp = DateTime.UtcNow;

                Events = new List<StiEvent>();
            }
            else
            {
                // Lookup and return the stored state
                _id = ObjectId.Parse(stateId);
                _t = "WrapperState";

                // Read in the object data from the db and return the populated object
                var myState = (StiWrapperState)Read(stateId);
                if (myState == null)
                {
                    IsValid = false;
                    return;
                }
                IsValid = true;

                Enabled = myState.Enabled;
                if (!Enabled)
                {
                    return;
                }

                // MAC Info
                methodName = myState.methodName;
                macNextMethod = myState.macNextMethod;
                macClientId = myState.macClientId;
                macClientName = myState.macClientName;
                macRequestId = myState.macRequestId;
                macOTP = myState.macOTP;
                macEnterOtpAd = myState.macEnterOtpAd;

                // Secure Trading Info
                stResponseToken = myState.stResponseToken;
                stOperatorId = myState.stOperatorId;
                stSiteId = myState.stSiteId;
                stUserName = myState.stUserName;
                stPlayerId = myState.stPlayerId;
                stPlayerIp = myState.stPlayerIp;
                stReferenceNumber = myState.stReferenceNumber;
                stRequestJson = myState.stRequestJson;
                stReturnCode = myState.stReturnCode;
                stResponseJson = myState.stResponseJson;
                stRecordStatus = myState.stRecordStatus;
                stProcess = myState.stProcess;
                stTransactionRequest = myState.stTransactionRequest;
                stTransactionType = myState.stTransactionType;
                stTransactionAmount = myState.stTransactionAmount;

                stTimeStamp = myState.stTimeStamp;

                Events = myState.Events;
            }
        }

        public ObjectId _id { get; set; }
        public string _t { get; set; }
        public bool Enabled { get; set; }
        public bool IsValid { get; set; }

        // MAC Info
        public string methodName { get; set; }
        public string macNextMethod { get; set; }
        public string macClientId { get; set; }
        public string macClientName { get; set; }
        public string macRequestId { get; set; }
        public string macOTP { get; set; }          // if message delivery set to loopback
        public string macEnterOtpAd { get; set; }   // if ad enabled on client

        // Secure Trading Info
        public string stResponseToken { get; set; }
        public string stOperatorId { get; set; }
        public string stSiteId { get; set; }
        public string stReferenceNumber { get; set; }
        public string stRequestJson { get; set; }
        public string stReturnCode { get; set; }
        public string stResponseJson { get; set; }
        public string stRecordStatus { get; set; }
        public string stUserName { get; set; }
        public string stPlayerId { get; set; }
        public string stPlayerIp { get; set; }

        // transaction info
        public string stProcess { get; set; }
        public string stTransactionRequest { get; set; }
        public string stTransactionType { get; set; }
        public string stTransactionAmount { get; set; }

        public DateTime stTimeStamp { get; set; }

        public List<StiEvent> Events { get; set; }

    }
}
