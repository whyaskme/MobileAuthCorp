using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;

namespace SMS
{
    public class Http
    {
        // todo: fix when Origization.Client and result objects are is defined
        //public override ??? Send(Origization.Client client, string to, string body)
        public bool Send(string toPhone, string textMessage)
        {
            //todo: get from organization.client when defined
            string url = "http://api.gateway160.com/client/sendmessage";
            string accountName = "MAC";
            string key = "9ea2b492-ae5f-41b2-afc2-9cdbc2398188";
            string countryCode = "US";

            try
            {
                byte[] response = null;
                using (WebClient myWebclient = new WebClient())
                {
                    response = myWebclient.UploadValues(url, new NameValueCollection() {
                        { "accountName", accountName },
                        { "key", key },
                        { "phoneNumber", toPhone },
                        { "countryCode", countryCode },
                        { "message", textMessage },
                        { "isUnicode", "0" }
                    });
                }
                if (response.ToString() == "1")
                    // todo: fix return when result object is defined
                    return true;
                    //return SMSResult.Success;

                else if (response.ToString() == "0")
                {
                    // todo: fix return when result object is defined
                    return false;
                    //error = "invalid account name/key";
                    //return SMSResult.InvalidAccount;
                }
                else if (response.ToString() == "-1")
                {
                    // todo: fix return when result object is defined
                    return false;
                    //error = "Gateway error";
                    //return SMSResult.Error;
                }
            }
            catch { /* ignore */}
            // todo: fix return when result object is defined
            return false;
            //error = "Can not send ProviderSms by Gateway160";
            //return SMSResult.Error;
        }
    }
}
