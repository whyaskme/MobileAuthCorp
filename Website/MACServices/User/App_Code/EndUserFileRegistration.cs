using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Xml;
using System.IO;
using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using es = MACServices.Constants.EventStats;
using sr = MACServices.Constants.ServiceResponse;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class EndUserFileRegistration : WebService
{
    public Object PropertyValue { get; set; }
    public virtual Type PropertyType { get; set; }

    private const string mSvcName = "EndUserFileRegistration";
    private const string mLogId = "FR";

    [WebMethod]
    public XmlDocument WsEndUserFileRegistration(string data)
    {
        var mUtils = new Utils();

        // request data Dictionary
        var myData = new Dictionary<string, string> { { dk.ServiceName, mSvcName } };

        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        // isloate ID from data
        var req = mUtils.GetIdDataFromRequest(data);
        if (String.IsNullOrEmpty(req.Item1))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null,
                req.Item2 + Environment.NewLine + data, "1");

        if (mUtils.DecryptAndParseRequestData(req.Item1, req.Item2, myData, char.Parse(dk.KVSep)) == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                req.Item1, "Corrupt data!" + Environment.NewLine + data, "2");

        // log request if debug set in web.config
        mUtils.LogRequest(myData, data, mLogId);

        if (!myData.ContainsKey(dk.Request))
            return mUtils.FinalizeXmlResponseWithError("Request type required!" + Environment.NewLine + data, mLogId);

        if (!myData.ContainsKey(dk.CID))
            return mUtils.FinalizeXmlResponseWithError("Client Id required!" + Environment.NewLine + data, mLogId);

        // id must be valid client
        var myClient = mUtils.ValidateClient(req.Item1);
        if (myClient == null) return mUtils.EmptyXml();

        //Check Ip
        var mResult = mUtils.CheckClientIp(myClient);
        if (mResult.Item1 == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], mResult.Item2, null);

        if (myData.ContainsKey(dk.RegistrationType) == false)
            return mUtils.FinalizeXmlResponseWithError("Registration Type required!" + Environment.NewLine + data, mLogId);

        if (!myData.ContainsKey(dk.FileType))
            return mUtils.FinalizeXmlResponseWithError("File type required!" + Environment.NewLine + data, mLogId);

        var myFileType = myData[dk.FileType];

        if (!myData.ContainsKey(dk.FileName))
            return mUtils.FinalizeXmlResponseWithError("File Name required!" + Environment.NewLine + data, mLogId);

        var fileName = myData[dk.FileName];

        //if (!myData.ContainsKey(dk.UploadFolder))
        //    return mUtils.FinalizeXmlResponseWithError("Upload Folder required!" + Environment.NewLine + data, mLogId);

        var fullPathToFile = Path.Combine(Server.MapPath("/"), "Temp", fileName);

        //if (File.Exists(fullPathToFile) == false)
        //    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myClient.ClientId.ToString(), 
        //        "No file at:" + fullPathToFile, null);

        switch (myFileType.ToLower())
        {
            #region xml
            case "xml":
            {
                // Process End User Records
                var endUserNodeProcessedCount = 0;
                var myRegDoc = new XmlDocument();
                try
                {
                    myRegDoc.Load(fullPathToFile);
                }
                catch (Exception ex)
                {
                    File.Delete(fullPathToFile);
                    myResponse.Append("<" + sr.Reply + ">Error:" + ex.Message + "</" +
                                      sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }

                var elemList = myRegDoc.GetElementsByTagName("EndUser");
                var endUserNodeCount = elemList.Count;
                if (endUserNodeCount < 1)
                {
                    File.Delete(fullPathToFile);
                    myResponse.Append("<" + sr.Reply + ">No EndUser nodes in file.</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
                //=================================================
                // loop to process one or more end user nodes
                myResponse.Append("<" + sr.Reply + ">");
                foreach (XmlNode myNode in elemList)
                {
                    var myUserData = new Dictionary<string, string>
                    {
                        {dk.Request, myData[dk.Request]},
                        {dk.CID, myData[dk.CID]}
                    };

                    if (myData.ContainsKey(dk.GroupId))
                        myUserData.Add(dk.GroupId, myData[dk.GroupId]);

                    if (myData.ContainsKey(dk.RegistrationType))
                        myUserData.Add(dk.RegistrationType, myData[dk.RegistrationType]);
                    else
                        myUserData.Add(dk.RegistrationType, dv.ClientRegister);

                    if (myNode.Attributes != null)
                    {
                        foreach (XmlNode node in myNode.Attributes)
                        {
                            var key = node.Name;
                            var value = node.InnerText;
                            myUserData.Remove(key);
                            myUserData.Add(key, value);
                        }

                        var childNodes = myNode.ChildNodes;
                        if (childNodes.Count > 0)
                        {
                            foreach (
                                var cNode in
                                    childNodes.Cast<XmlNode>()
                                        .Where(cNode => cNode.Name == "Address")
                                        .Where(cNode => cNode.Attributes != null))
                            {
                                if (cNode.Attributes == null) continue;
                                for (var x = 0; x < cNode.Attributes.Count; ++x)
                                {
                                    var key = cNode.Attributes[x].Name;
                                    var value = cNode.Attributes[x].InnerText;
                                    myUserData.Remove(key);
                                    myUserData.Add(key, value);
                                }
                            }
                        }
                    }

                    var ret = mUtils.Check4MininumEndUserInfo(myUserData);
                    if (ret.Item1 == false)
                    {
                        myResponse.Append("<EndUser Name='" + myUserData[dkui.FirstName] +
                                          " " + myUserData[dkui.LastName] + "' Status='" + ret.Item2 + "' />");
                        continue;
                    }
                    myUserData.Add(dk.UserId, Security.GetHashString(myUserData[dkui.LastName] + myUserData[dkui.UID]));
                    var myEndUser = new EndUser();
                    if (myData[dk.RegistrationType] == dv.OpenRegister)
                    {
                        ret = mUtils.ServiceRequest(
                            ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                            Constants.ServiceUrls.MacOpenEndUserServices,
                            myClient.ClientId.ToString(), myUserData);
                        myResponse.Append("<EndUser Name='" + myUserData[dkui.FirstName] + " " +
                                          myUserData[dkui.LastName] + "' Status='" + ret.Item2 + "' />");
                        continue;
                    }
                    // check if user exists
                    var rtn = mUtils.GetHashedIdBasedOnRegistrationType(myUserData);
                    if (ret.Item1 == false)
                    {
                        myResponse.Append("<" + sr.BadNode + ">Existing Hashed Id, " + ret.Item2 + "</" + sr.BadNode + ">");
                        continue;
                    }
                    var endUser = mUtils.GetEndUserByHashedUserId(rtn.Item2);
                    if (endUser != null)
                    {
                        myResponse.Append("<EndUser Name='" + myUserData[dkui.FirstName] + " " +
                                          myUserData[dkui.LastName] + "' Status=' User Exists' />");
                        continue;
                    }
                    ret = mUtils.PopulateEndUserObject(myEndUser, myUserData);
                    if (ret.Item1 == false)
                    {
                        myResponse.Append("<" + sr.BadNode + ">Missing End User info," + ret.Item2 + "</" + sr.BadNode + ">");
                        continue;
                    }
                    // File Registration, set active
                    myEndUser.Active = true;
                    myEndUser.State = Constants.EndUserStates.Registered;
                    var myEvent = new EndUserEvent(mSvcName +
                                                   ":" + myEndUser.RegistrationType +
                                                   ", State=" + myEndUser.State +
                                                   ", Ad Opt-Out=" + myEndUser.OtpOutAd);
                    myEndUser.EndUserEvents.Add(myEvent);
                    mUtils.ObjectCreate(myEndUser);
                    myResponse.Append("<EndUser Name='" +
                                      myUserData[dkui.FirstName] + " " +
                                      myUserData[dkui.LastName] +
                                      ":" + myEndUser.RegistrationType +
                                      ", State=" + myEndUser.State +
                                      ", Ad Opt-Out=" + myEndUser.OtpOutAd + "' />");
                    ++endUserNodeProcessedCount;

                    EventLogRegistration(myClient, myData);

                } // end xml file type
                File.Delete(fullPathToFile);
                myResponse.Append(
                    String.Format(
                        "<" + sr.Result + ">File processed, Record Count:{0} Processed:{1}</" + sr.Result + ">" +
                        sr.Reply + ">",
                        endUserNodeCount, endUserNodeProcessedCount));
                myResponse.Append("</" + sr.Reply + ">");
                var XmlReply = mUtils.FinalizeXmlResponse(myResponse, mLogId);
                return XmlReply;
            }
            #endregion

            #region Text File Character seperated fields and values 
            case "cvs":
            case "txt":
            {

                var endLineCount = 0;
                if (File.Exists(fullPathToFile) == false)
                    return mUtils.FinalizeXmlResponseWithError(mSvcName + " No file @ " + fullPathToFile, mLogId);
                myResponse.Append("<" + sr.Reply + ">");
                string line;
                // Read the file and display it line by line.
                var mFile = new StreamReader(fullPathToFile);
                while ((line = mFile.ReadLine()) != null)
                {
                    if (String.IsNullOrWhiteSpace(line)) continue;
                    if (!line.StartsWith(dk.Register)) continue;

                    ++endLineCount;
                    var myUserData = new Dictionary<string, string>();
                    if (mUtils.ParseIntoDictionary(line, myUserData, char.Parse(dk.KVSep)) == false)
                    {
                        myResponse.Append("<" + sr.BadLine + ">Parsing error, " + line + "</" + sr.BadLine + ">");
                        continue;
                    }
                    if (myUserData.ContainsKey(dk.Request) == false)
                        myUserData.Add(dk.Request, myData[dk.Request]); 
                    if (myUserData.ContainsKey(dk.CID) == false)
                        myUserData.Add(dk.CID, myData[dk.CID]);
                    if (myUserData.ContainsKey(dk.RegistrationType) == false)
                        myUserData.Add(dk.RegistrationType, myData[dk.RegistrationType]);

                    if (myData.ContainsKey(dk.GroupId))
                        myUserData.Add(dk.GroupId, myData[dk.GroupId]);

                    if (!myUserData.ContainsKey(dk.UserId))
                    {
                        if (myUserData.ContainsKey(dkui.UID))
                            myUserData.Add(dk.UserId,
                                Security.GetHashString(myUserData[dkui.LastName].ToLower() +
                                                       myUserData[dkui.UID].ToLower()));
                        else
                            myUserData.Add(dk.UserId,
                                Security.GetHashString(myUserData[dkui.LastName].ToLower() +
                                                       myUserData[dkui.EmailAddress].ToLower()));
                    }


                    if (myData[dk.RegistrationType] == dv.OpenRegister)
                    {
                        // Open Registration
                        var ret = mUtils.ServiceRequest(
                            ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                            Constants.ServiceUrls.MacOpenEndUserServices,
                                myClient.ClientId.ToString(), myUserData);
                        myResponse.Append("<EndUser Name='" + myUserData[dkui.FirstName] + " " +
                                          myUserData[dkui.LastName] + "' Status='" + ret.Item2 + "' />");
                        continue;
                    }

                    // check if user exists
                    var rtn = mUtils.GetHashedIdBasedOnRegistrationType(myUserData);
                    if (rtn.Item1 == false)
                    {
                        myResponse.Append("<" + sr.BadLine + ">Existing Hashed Id, " + rtn.Item2 + "</" + sr.BadLine + ">");
                        continue;
                    }
                    var endUser = mUtils.GetEndUserByHashedUserId(rtn.Item2);
                    if (endUser != null)
                    {
                        myResponse.Append("<EndUser Name='" + myUserData[dkui.FirstName] + " " +
                                          myUserData[dkui.LastName] + "' Status=' User Exists' />");
                        continue;
                    }
                    var myEndUser = new EndUser();
                    rtn = mUtils.PopulateEndUserObject(myEndUser, myUserData);
                    if (rtn.Item1 == false)
                    {
                        myResponse.Append("<" + sr.BadLine + ">Missing End User info," + rtn.Item2 + "</" + sr.BadLine + ">");
                        continue;
                    }
                    // Client or Group Registration
                    // File Registration, set active
                    myEndUser.Active = true;
                    myEndUser.State = Constants.EndUserStates.Registered;
                    var myEvent = new EndUserEvent(mSvcName +
                                                   ":" + myEndUser.RegistrationType +
                                                   ", State=" + myEndUser.State +
                                                   ", Ad Opt-Out=" + myEndUser.OtpOutAd);

                    myEndUser.EndUserEvents.Add(myEvent);
                    // File Registration, set active
                    myEndUser.Active = true;
                    mUtils.ObjectCreate(myEndUser);

                    myResponse.Append("<EndUser Name='" +
                                      myUserData[dkui.FirstName] + " " +
                                      myUserData[dkui.LastName] +
                                      ":" + myEndUser.RegistrationType +
                                      ", State=" + myEndUser.State +
                                      ", Ad Opt-Out=" + myEndUser.OtpOutAd + "' />");


                    // ReSharper disable once UnusedVariable
                    var myStat = new EventStat(myClient._id, myClient.Name, es.EndUserRegister, 1);

                    EventLogRegistration(myClient, myData);

                }
                mFile.Close();

                File.Delete(fullPathToFile);
                myResponse.Append("<" + sr.Result + ">File processed, Line Count:" + endLineCount + "</" + sr.Result + ">");
                myResponse.Append("</" + sr.Reply + ">");
                var XmlReply = mUtils.FinalizeXmlResponse(myResponse, mLogId);
                return XmlReply;
            }
            #endregion

            #region Serialized Dictionary file

            case "json":
            {
                File.Delete(fullPathToFile);
                myResponse.Append("Serialized Dictionary file processing not implemented.</" +
                                  sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            #endregion

            default:
                File.Delete(fullPathToFile);
                myResponse.Append("File type [" + myFileType + "] not supported.</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
        }
    }

    /// <summary>Log the registration event</summary>
    protected void EventLogRegistration(Client pClient, Dictionary<String, String> pData)
    {
        var registrationEvent = new Event
        {
            ClientId = pClient.ClientId,
            EventTypeDesc = Constants.TokenKeys.UserRole + "EndUser"
        };
        if (Convert.ToBoolean(ConfigurationManager.AppSettings[cfg.Debug]))
            registrationEvent.EventTypeDesc += Constants.TokenKeys.UserFullName 
                + pData[dkui.FirstName] + " " +  pData[dkui.LastName];
        registrationEvent.EventTypeDesc += Constants.TokenKeys.ClientName + pClient.Name;
        registrationEvent.Create(Constants.EventLog.Registration.EndUser.Created, null);
    }
}