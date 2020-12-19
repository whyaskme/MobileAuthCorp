using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;

namespace MACEndUserVerification
{
    public class EndUserVerification
    {
        public static Tuple<bool, string> VerifyEndUser(Client pClient, string pEndUserInformation)
        {
            var myEndUserData = new Dictionary<string, string>();
            var userInfo = pEndUserInformation.Split(char.Parse(dk.ItemSep));
            if (!userInfo.Any())
                return new Tuple<bool, string>(false, "Bad End User Info");
            foreach (var item in userInfo)
            {
                if (item.Contains(":")) {
                    return new Tuple<bool, string>(false, "Bad End User Info item [" + item + "]");
                }
                var kv = item.Split(':');
                myEndUserData.Remove(kv[0]);
                myEndUserData.Add(kv[0], kv[1]);
            }

            // ToDo:
            return null; // pClient.VerificationProviders.Name == "WhitePagesPro" ? VerifyUsingWhitePagesPro(pClient, myEndUserData) : new Tuple<bool, string>(false, "no support fo verification provider: " + pClient.VerificationProviders.Name);
        }

        private static Tuple<bool, string> VerifyUsingWhitePagesPro(Client pClient, Dictionary<string, string> pEndUserInfo)
        {
            // check for required info for WhitePagesPRO
            if (!pEndUserInfo.ContainsKey(dkui.LastName))
            {
                return new Tuple<bool, string>(false, "Last name required");
            } 
            if (!pEndUserInfo.ContainsKey(dkui.PhoneNumber))
            {
                return new Tuple<bool, string>(false, "Phone Number required");
            } 
            // Make call to WhitePagesPro
            // enample: https://proapi.whitepages.com/reverse_phone/1.1/?api_key=defa7058bc71ada1b60000216700c1d2;phone=4802684076
            
            // get Url 
            const string url = @"https://proapi.whitepages.com/reverse_phone/1.1/";
            //string Url = pClient.VerificationProviders.BaseUrl;
            
            // ToDo:
            //var data = String.Format("api_key={0};phone={1}", pClient.VerificationProviders.ApiKey, pEndUserInfo.ContainsKey(dkui.PhoneNumber));
            var data = "???";
            // make web service call
            var dataStream = Encoding.UTF8.GetBytes(data);
            var webRequest = WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = dataStream.Length;
            var newStream = webRequest.GetRequestStream();
            // Send the data.
            newStream.Write(dataStream, 0, dataStream.Length);
            newStream.Close();
            var res = webRequest.GetResponse();
            var response = res.GetResponseStream();
            var xmlDoc = new XmlDocument();
            if (response != null) xmlDoc.Load(response);
            var elemList = xmlDoc.GetElementsByTagName("wp");
            if (elemList.Count < 1)
                return new Tuple<bool, string>(false, "Invalid reply");
            // need to do verification here
            // is thisa mobile phone
            // is it active



            return new Tuple<bool, string>(true, elemList[0].InnerXml);
        }

        //private static Tuple<bool, string> VerifyUsingLexusNexis(Client pClient, Dictionary<string, string> pEndUserInfo)
        //{
        //    return new Tuple<bool, string>(false, "not implemented");
        //}

    }
}