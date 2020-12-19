using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using sr = MACServices.Constants.ServiceResponse;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class OpenClientServices : WebService 
{
    private const string mSvcName = "RegisterOasClient";
    private const string mLogId = "OC";
    public Object PropertyValue { get; set; }
    public virtual Type PropertyType { get; set; }

    [WebMethod]
    public XmlDocument WsOpenClientServices(string data)
    {
        var mUtils = new Utils();

        var myData = new Dictionary<string, string> { { dk.ServiceName, mSvcName } };

        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        var myrequestData = data;

        var requestWasFrom = HttpContext.Current.Request.ServerVariables["HTTP_HOST"];

        var request = mUtils.GetIdDataFromRequest(myrequestData);
        if (String.IsNullOrEmpty(request.Item1))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1, "Corrupt data" + Environment.NewLine + data, null);

        // decrypt and parse request data
        if (mUtils.DecryptAndParseRequestData(request.Item1, request.Item2, myData, char.Parse(dk.KVSep)) == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1, "Corrupt data" + Environment.NewLine + data, null);

        if (!myData.ContainsKey(dk.Request))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1, "Corrupt data" + Environment.NewLine + data, null);

        // log request if debug set in web.config
        var eid = mUtils.LogRequest(myData, data, mLogId);

        #region Register
        if (myData[dk.Request].Contains(dv.Register))
        {
            try
            {
                if (!myData.ContainsKey(dk.Name))
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1,
                        "Client's Name is required!", null);
                var myOasClientName = myData[dk.Name];

                myResponse.Append("<" + sr.Name + ">" + myOasClientName + "</" + sr.Name + ">");

                if (!myData.ContainsKey(dk.FullyQualifiedDomainName))
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1,
                        "Client's Fully Qualified Domain Name is required!", null);

                var myOasClient = mUtils.GetOasClientByClientId(request.Item1);
                if (myOasClient == null)
                {
                    myOasClient = new OasClientList(request.Item1)
                    {
                        Name = myOasClientName,
                        FullyQualifiedDomainName = myData[dk.FullyQualifiedDomainName]
                    };

                    var myEvent = new OasClientEvent(requestWasFrom + " " + myData[dk.Request]);
                    myOasClient.EndUserEvents.Add(myEvent);
                    mUtils.ObjectCreate(myOasClient);
                    myResponse.Append("<" + sr.Name + ">" + myOasClient.Name + "</" + sr.Name + ">");
                    myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Registered + "</" + sr.Reply +
                                      ">");
                }
                else // already registered... enable
                {
                    myOasClient.FullyQualifiedDomainName = myData[dk.FullyQualifiedDomainName];
                    myOasClient.Enabled = true;
                    var myEvent = new OasClientEvent(requestWasFrom + " " + myData[dk.Request] + "enabled");
                    myOasClient.EndUserEvents.Add(myEvent);
                    mUtils.ObjectUpdate(myOasClient, myOasClient._id.ToString());
                    myResponse.Append("<" + sr.Name + ">" + myOasClient.Name + "</" + sr.Name + ">");
                    myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Enabled + "</" + sr.Reply + ">");
                }
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + dv.Register + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + exceptionEvent.EventTypeDesc, mLogId);
            }
        }
        #endregion
        #region Enable
    
        if (myData[dk.Request].Contains(dv.Enable))
        {
            try
            {
                var myOasClientToEnable = mUtils.GetOasClientByClientId(request.Item1);
                if (myOasClientToEnable == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1,
                        "Invalid client ID " + request.Item1, null);
                if (myOasClientToEnable.Enabled == false)
                {
                    myOasClientToEnable.Enabled = true;
                    var myEvent = new OasClientEvent(requestWasFrom + " " + myData[dk.Request]);
                    myOasClientToEnable.EndUserEvents.Add(myEvent);
                    mUtils.ObjectUpdate(myOasClientToEnable, myOasClientToEnable._id.ToString());
                }
                myResponse.Append("<" + sr.Name + ">" + myOasClientToEnable.Name + "</" + sr.Name + ">");
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Enabled + "</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + dv.Enable + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + exceptionEvent.EventTypeDesc, mLogId);
            }
        }
        #endregion
        #region Disable
        if (myData[dk.Request].Contains(dv.Disable))
        {
            try
            {
                var myClientToDisable = mUtils.GetOasClientByClientId(request.Item1);
                if (myClientToDisable == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1,
                        "Invalid client ID " + request.Item1, null);
                myResponse.Append("<" + sr.Name + ">" + myClientToDisable.Name + "</" + sr.Name + ">");

                if (myClientToDisable.Enabled)
                {
                    myClientToDisable.Enabled = false;
                    var myEvent = new OasClientEvent(requestWasFrom + " " + myData[dk.Request]);
                    myClientToDisable.EndUserEvents.Add(myEvent);
                    mUtils.ObjectUpdate(myClientToDisable, myClientToDisable._id.ToString());
                }
                myResponse.Append("<" + sr.Name + ">" + myClientToDisable.Name + "</" + sr.Name + ">");
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Disabled + "</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + dv.Disable + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + exceptionEvent.EventTypeDesc, mLogId);
            }
        }
        #endregion
        #region Delete
        if (myData[dk.Request].Contains(dv.Delete))
        {
            try
            {
                var myClientToDisable = mUtils.GetOasClientByClientId(request.Item1);
                if (myClientToDisable == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1,
                        "Invalid client ID " + request.Item1, null);
                myResponse.Append("<" + sr.Name + ">" + myClientToDisable.Name + "</" + sr.Name + ">");
                var result = mUtils.DeleteOasClient(request.Item1);
                myResponse.Append("<" + sr.Reply + ">" + result + "</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + dv.Delete + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + exceptionEvent.EventTypeDesc, mLogId);
            }
        }
        #endregion
        #region Exists
        if (myData[dk.Request].Contains(dv.Exists))
        {
            try
            {
                var myClientToCheck = mUtils.GetOasClientByClientId(request.Item1);
                if (myClientToCheck == null)
                {
                    myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.NotRegistered + "</" + sr.Reply +">");
                }
                else
                {
                    myResponse.Append("<Enabled>" + myClientToCheck.Enabled.ToString() + "</Enabled>");
                    myResponse.Append("<Date>" + myClientToCheck.Date.ToShortDateString() + "</Date>");
                    myResponse.Append("<" + dk.FullyQualifiedDomainName + ">" + myClientToCheck.FullyQualifiedDomainName + "</" + dk.FullyQualifiedDomainName + ">");
                    myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Registered + "</" + sr.Reply + ">");
                }
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + dv.Exists + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + exceptionEvent.EventTypeDesc, mLogId);
            }

        }
        #endregion

        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1,
            "Invalid request[" + myData[dk.Request] + "], " + eid, null);
    }
}

