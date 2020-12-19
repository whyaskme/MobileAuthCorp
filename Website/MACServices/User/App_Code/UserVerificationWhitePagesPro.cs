using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.Services;
using System.Xml;
using System.Net;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class UserVerificationWhitePagesPro : WebService
{
    public Object PropertyValue { get; set; }
    public virtual Type PropertyType { get; set; }

    private const string mSvcName = "UserVerificationWhitePagesPro";
    private const string mLogId = "WP";

    [WebMethod]
    public XmlDocument WsUserVerificationWhitePagesPro(string data)
    {
        var mUtils = new Utils();

        Client myClient;
        // request data Dictionary
        var myData = new Dictionary<string, string>();

        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        // check if debug
        if (data.Length < 24)
        {
            #region Test

            //==== Debug request  =============================================
            data = data.ToLower();
            var myClientName = "";
            var comment = "";
            // set the client name
            if (data.StartsWith("b")) myClientName = "Box Office";
            else if (data.StartsWith("c")) myClientName = "Casino";
            else if (data.StartsWith("d")) myClientName = "Discount Shoe Warehouse";
            else if (data.StartsWith("e")) myClientName = "Email Only";
            else if (data.StartsWith("f")) myClientName = "Food City";
            else if (data.StartsWith("k")) myClientName = "Kohl's";
            else if (data.StartsWith("t")) myClientName = "Target #222";
            else if (data.StartsWith("s")) myClientName = "Sam's Club";
            else if (data.StartsWith("w"))
            {
                myClientName = data.StartsWith("wo") ? "Walmart Online" : "Walmart #303";
            }

            // default user
            myData.Add(dkui.FirstName, "Terry");
            myData.Add(dkui.LastName, "Davis");
            myData.Add(dkui.PhoneNumber, "4802684076");
            myData.Add(dkui.DOB, "7/31/1946");
            myData.Add(dkui.EmailAddress, "tdavis@mobileauthcorp.com");
            myData.Add(dkui.UID, "tdavis@mobileauthcorp.com");

            if (data.Contains("1")) // user 1
            {
                comment = "valid mobile phone number";
            }
            else if (data.Contains("2"))
            {
                comment = "valid landline phone number";
                myData.Remove(dkui.PhoneNumber);
                myData.Add(dkui.PhoneNumber, "4805855801");
            }
            else if (data.Contains("3"))
            {
                comment = "no area code in phone number";
                myData.Remove(dkui.PhoneNumber);
                myData.Add(dkui.PhoneNumber, "2684076");
            }
            else if (data.Contains("4"))
            {
                comment = "last name mismatch";
                myData.Remove(dkui.FirstName);
                myData.Remove(dkui.LastName);
                myData.Add(dkui.FirstName, "Bob");
                myData.Add(dkui.LastName, "White");
            }
            else if (data.Contains("5"))
            {
                comment = "invalid phone number";
                myData.Remove(dkui.PhoneNumber);
                myData.Remove(dkui.FirstName);
                myData.Remove(dkui.LastName);
                myData.Add(dkui.FirstName, "Joe");
                myData.Add(dkui.LastName, "Baranauskas");
                myData.Add(dkui.PhoneNumber, "5129710910");
            }
            else if (data.Contains("6"))
            {
                //SUSAN	KETOLA	98908	0368 19391123
                comment = "LN-REST";
                myData.Remove(dkui.DOB);
                myData.Remove(dkui.FirstName);
                myData.Remove(dkui.LastName);
                myData.Remove(dkui.PhoneNumber);

                myData.Add(dkui.FirstName, "SUSAN");
                myData.Add(dkui.LastName, "KETOLA");
                myData.Add(dkui.DOB, "11/23/1939");
                myData.Add(dkui.ZipCode, "98908");
                myData.Add(dkui.SSN4, "0368");
                myData.Add(dkui.PhoneNumber, "5011234567");
                myData.Add(dk.Protocol, "REST");
            }
            else if (data.Contains("7"))
            {
                // MAITE       CRAMER      94949	0912    19411223
                comment = "LN-REST";
                myData.Remove(dkui.DOB);
                myData.Remove(dkui.FirstName);
                myData.Remove(dkui.LastName);
                myData.Remove(dkui.PhoneNumber);

                myData.Add(dkui.FirstName, "MAITE");
                myData.Add(dkui.LastName, "CRAMER");
                myData.Add(dkui.DOB, "12/23/1941");
                myData.Add(dkui.SSN4, "0912");
                myData.Add(dkui.ZipCode, "94949");
                myData.Add(dkui.PhoneNumber, "5011234567");
                myData.Add(dk.Protocol, "REST");
            }
            else if (data.Contains("8"))
            {
                //SUSAN	KETOLA	98908	0368 19391123
                comment = "LN-SOAP";
                myData.Remove(dkui.DOB);
                myData.Remove(dkui.FirstName);
                myData.Remove(dkui.LastName);
                myData.Remove(dkui.PhoneNumber);

                myData.Add(dkui.FirstName, "SUSAN");
                myData.Add(dkui.LastName, "KETOLA");
                myData.Add(dkui.DOB, "11/23/1939");
                myData.Add(dkui.ZipCode, "98908");
                myData.Add(dkui.SSN4, "0368");
                myData.Add(dkui.PhoneNumber, "5011234567");
                myData.Add(dk.Protocol, "SOAP");
            }
            else if (data.Contains("9"))
            {
                // DIXIE       LILEIKIS	92316	0930    1941122
                comment = "LN-SOAP";
                myData.Remove(dkui.DOB);
                myData.Remove(dkui.FirstName);
                myData.Remove(dkui.LastName);
                myData.Remove(dkui.PhoneNumber);

                myData.Add(dkui.FirstName, "MAITE");
                myData.Add(dkui.LastName, "CRAMER");
                myData.Add(dkui.DOB, "12/23/1941");
                myData.Add(dkui.SSN4, "0912");
                myData.Add(dkui.ZipCode, "94949");
                myData.Add(dkui.PhoneNumber, "5011234567");
                myData.Add(dk.Protocol, "SOAP");
            }

            myClient = mUtils.GetClientUsingClientName(myClientName);
            if (myClient == null)
                return mUtils.FinalizeXmlResponseWithError("Debug: Could not get client by name: " + myClientName, mLogId);
            myResponse.Append("<Debug>Run - Client" + myClientName + " - " + comment + "</Debug>");

            #endregion
        }
        else
        {
            var request = mUtils.GetIdDataFromRequest(data);
            if (String.IsNullOrEmpty(request.Item1))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null,
                    "Corrupt data!" + Environment.NewLine + data, "1");

            try
            {
                var sd = Security.DecodeAndDecrypt(request.Item2, request.Item1);
                myData.Clear();
                myData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(sd);
            }
            catch
            {
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1,
                    "Corrupt data!" + Environment.NewLine + data, "2");
            }
            myClient = mUtils.ValidateClient(request.Item1);
            if (myClient == null) return mUtils.EmptyXml();
            if (myData.ContainsKey(dk.ClientName) == false)
                myData.Add(dk.ClientName, myClient.Name);
        }
        myData.Remove(dk.ServiceName);
        myData.Add(dk.ServiceName, mSvcName);

        // log request if debug set in web.config
        mUtils.LogRequest(myData, data, mLogId);

        //Check Ip
        var mResult = mUtils.CheckClientIp(myClient);
        if (mResult.Item1 == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], mResult.Item2, null);

        if (!myData.ContainsKey(dkui.LastName))
        {
            myResponse.Append("<" + sr.Reply + ">Failed Last name required</" + sr.Reply + ">");
            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
        }

        if (!myData.ContainsKey(dkui.PhoneNumber))
        {
            myResponse.Append("<" + sr.Reply + ">Verification, Failed Phone number required</" + sr.Reply + ">");
            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
        }

        foreach (var mVerificationProvider in myClient.VerificationProviders)
        {
            if (mVerificationProvider.Name == myData[dk.VerificationProviderName])
            {
                if (mVerificationProvider.Enabled == false)
                {
                    myResponse.Append("<" + sr.Reply + ">Verification, Disabled!</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
                var mLoopback = ConfigurationManager.AppSettings[cfg.LoopBackTest];
                if (!String.IsNullOrEmpty(mLoopback))
                {
                    if (mLoopback != cfg.Disabled)
                    {
                        myResponse.Append("<" + sr.Reply + ">Verification, Disabled!</" + sr.Reply + ">");
                        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    }
                }
                // Loopback not disabled, do vericication
                var sUrl = string.Format("{0}/{1}/{2}?api_key={3};phone={4}",
                    /*0*/mVerificationProvider.BaseUrl,
                    /*1*/mVerificationProvider.SearchType,
                    /*2*/mVerificationProvider.ApiVersion,
                    /*3*/mVerificationProvider.ApiKey,
                    /*4*/myData[dkui.PhoneNumber]);

                // example url: https://proapi.whitepages.com/reverse_phone/1.1/?api_key=defa7058bc71ada1b60000216700c1d2;phone=4805551212

                try
                {
                    var myWebRequest = WebRequest.Create(sUrl);

                    // If required by the server, set the credentials.
                    myWebRequest.Credentials = CredentialCache.DefaultCredentials;
                    // Get the response.
                    var myWebResponse = myWebRequest.GetResponse();
                    //var ResponseStatus = ((HttpWebResponse)myWebResponse).StatusDescription;
                    var response = myWebResponse.GetResponseStream();
                    var xmlDoc = new XmlDocument();
                    if (response != null) xmlDoc.Load(response);
                    var elemList = xmlDoc.GetElementsByTagName("wp:result");
                    var xmlAttributeCollection = elemList[0].Attributes;
                    if (xmlAttributeCollection != null)
                    {
                        var reply = xmlAttributeCollection["wp:type"].Value;
                        //var code = xmlAttributeCollection["wp:code"].Value;
                        //var message = xmlAttributeCollection["wp:message"].Value;
                        //var billable = xmlAttributeCollection["wp:billable"].Value;
                        if (reply.ToLower() != "success")
                        {
                            // not a success reply must be error
                            elemList = xmlDoc.GetElementsByTagName("wp:message");
                            myResponse.Append("<" + sr.Reply + ">Verification, Failed " + elemList[0].InnerXml +
                                              "</" + sr.Reply + ">");
                            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                        }
                    }
                    // sucess
                    var lastname = "N/A";
                    //var carrier = "N/A";
                    var phonetype = "N/A";
                    elemList = xmlDoc.GetElementsByTagName("wp:lastname");
                    if (elemList.Count != 0)
                        lastname = elemList[0].InnerXml; // last name on account

                    //elemList = xmlDoc.GetElementsByTagName("wp:carrier");
                    //if (elemList.Count != 0)
                    //    carrier = elemList[0].InnerXml; // carrier
                    elemList = xmlDoc.GetElementsByTagName("wp:phone");
                    if (elemList.Count != 0)
                    {
                        var attr = elemList[0].Attributes;
                        if (attr != null && attr.Count > 1)
                            phonetype = attr[1].InnerText; // type of phone
                    }
                    //
                    // check the results
                    //
                    // check if is there a last name on the account
                    if (lastname == "N/A")
                        myResponse.Append("<" + sr.Reply + ">Verification, Failed no name on account!</" + sr.Reply + ">");

                        // check if mobile device
                    else if (phonetype.ToLower() != "mobile") // has to ba a mobile phone
                        myResponse.Append("<" + sr.Reply + ">Verification, Failed invalid phone type " + phonetype + "!</" + sr.Reply + ">");

                        // check if last name of user being registered is the same as last name on account
                    else if (lastname.ToLower() != myData[dkui.LastName].ToLower())
                        myResponse.Append("<" + sr.Reply + ">Verification, Failed last name on account[" +
                                          myData[dkui.LastName] + "]!</" + sr.Reply + ">");

                    else
                        myResponse.Append("<" + sr.Reply + ">Verification, Verified</" + sr.Reply + ">");
                }
                catch (Exception ex)
                {

                    var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    if (myData.ContainsKey(dk.ClientName))
                        exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                    myResponse.Append("<" + sr.Reply + ">Verification, Web Request failed, exception:" +
                        ex.Message + "!</" + sr.Reply + ">");      
                }
            }
        }
        myResponse.Append("<" + sr.Reply + ">Verification, Failed verification provider lookup /Reply>");
        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
    }

    public static bool ValidateServerCertificate(object sender,
        System.Security.Cryptography.X509Certificates.X509Certificate certification,
        System.Security.Cryptography.X509Certificates.X509Chain chain,
        System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}