using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web.Services;
using System.Xml;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cs = MACServices.Constants.Strings;
using es = MACServices.Constants.EventStats;
using sr = MACServices.Constants.ServiceResponse;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class ValidateOtp : WebService {

    private const string mSvcName = "ValidateOtp";
    private const string mLogId = "RO";

    [WebMethod]
    public XmlDocument WsValidateOtp(string data)
    {
        var mUtils = new Utils();

        // request data Dictionary
        var myData = new Dictionary<string, string> { { dk.ServiceName, mSvcName } };

        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        #region Decode/decrypt request data and log
        if (data.StartsWith("99"))
        {
            var requestData = data.Substring(2, data.Length - 2); // dump the 99 from front
            var request = mUtils.GetIdDataFromRequest(requestData);

            // parse string(data) and add to the dictionary
            if (mUtils.ParseIntoDictionary(mUtils.HexToString(request.Item2), myData, char.Parse(dk.KVSep)) == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    request.Item1, "requestData (Corrupt)" + Environment.NewLine + data, "99");
        }
        else
        { // normal processing data is encrypted
            var request = mUtils.GetIdDataFromRequest(data);
            if (String.IsNullOrEmpty(request.Item1))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    null, request.Item2 + Environment.NewLine + data, "1");

            // decrypt, parse string and add to the dictionary
            if (mUtils.DecryptAndParseRequestData(request.Item1, request.Item2, myData, Convert.ToChar(dk.KVSep)) == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    request.Item1, "requestData (Corrupt)" + Environment.NewLine + data, "2");
        }

        // log request if debug set in web.config
        var eid = mUtils.LogRequest(myData, data, mLogId);

        #endregion

        if (myData.ContainsKey(dk.Request) == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, " Request no Request, " + eid, "1");

        if (myData.ContainsKey(dk.CID) == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, " Request no CID, " + eid, "4");

        var myClient = mUtils.GetClientUsingClientId(myData[dk.CID]);
        if (myClient == null)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, " Invalid Client Id, " + eid, "4");

        if (myData.ContainsKey(dk.ClientName) == false)
            myData.Add(dk.ClientName, myClient.Name);

        if (myData[dk.Request] == dv.Ping)
        {
            myResponse.Append("<" + sr.Reply + ">" + sr.Success + "</" + sr.Reply + ">");
            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
        }

        if (myData[dk.Request] != dv.VerifyOtp)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, " Invalid request[" + myData[dk.Request] + "], " + eid, "2");

        if (myData.ContainsKey(dk.RequestId) == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], "Request Data (No RequestId)", "3");

        if (myData.ContainsKey(dk.OTP) == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], "OTP missing in request data", "5");

        if (myData[dk.OTP].Length < 1)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], "OTP length", "6");

        #region Check Ip
        var mResult = mUtils.CheckClientIp(myClient);
        if (mResult.Item1 == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], mResult.Item2, null);
        #endregion

        var requestId = myData[dk.RequestId];
        if (myData[dk.RequestId].Contains("("))        // remove delivery method
            requestId = myData[dk.RequestId].Replace("(" + cs.Sms + ")", "")
                                                .Replace("(" + cs.Voice + ")", "")
                                                .Replace("(" + cs.Email + ")", "");

        // get OTP using the Request Id
        var myOtp = mUtils.GetOtpUsingRequestId(requestId);
        if (myOtp == null)
        {
            // give database indexing a chance to catch up
            Thread.Sleep(1000);
            // try again
            myOtp = mUtils.GetOtpUsingRequestId(requestId);
            if (myOtp == null)
            {
                // give database indexing a chance to catch up
                Thread.Sleep(1000);
                // try again
                myOtp = mUtils.GetOtpUsingRequestId(requestId);
                if (myOtp == null)
                {
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        myData[dk.CID], "RequestId (Invalid) - " + myData[dk.ClientName] + " - RequestId: " + requestId, "7");
                }
            }
        }
        // has to be from the same client/Group that requested the OTP
        if (!String.Equals(myOtp.ClientId.ToString(), myData[dk.CID], StringComparison.CurrentCultureIgnoreCase))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                myData[dk.CID], "Client Id (Invalid for this OTP)", "8");

        try
        {
            var mOtpVerifyEvent = new Event
            {
                ClientId = myOtp.ClientId,
                EventTypeDesc = Constants.TokenKeys.RequestId + myOtp._id
            };

            // is the OTP still active
            if (myOtp.Active == false)
            {
                mOtpVerifyEvent.EventTypeDesc += Constants.TokenKeys.OTPRetriesMax + myOtp.ValidationRetries
                                            + Constants.TokenKeys.OTPRetriesCurrent + myOtp.ValidationCount
                                            + Constants.TokenKeys.ClientName + myData[dk.ClientName];
                mOtpVerifyEvent.EventTypeId = Constants.EventLog.Otp.Inactive.Item1;
                mOtpVerifyEvent.EventTypeName = Constants.EventLog.Otp.Inactive.Item2;
                myResponse.Append("<" + sr.Reply + ">" + sr.Inactive + "</" + sr.Reply + ">");

                // ReSharper disable once UnusedVariable
                var myStat = new EventStat(myClient._id, myClient.Name, es.OtpInvalid, 1);
            }
            else
            {   // OTP Active
                // Has the OTP Timeout
                var myNow = DateTime.UtcNow;
                var myTill = myOtp.EndOfLife;
                if (myNow > myTill)
                {  //yes the OTP has Timeout
                    myOtp.Active = false; // set so OTP can't be used again
                    mOtpVerifyEvent.EventTypeDesc += Constants.TokenKeys.OTPRetriesMax + myOtp.ValidationRetries
                                        + Constants.TokenKeys.OTPCode + myData[dk.OTP]
                                        + Constants.TokenKeys.OTPRetriesCurrent + myOtp.ValidationCount
                                        + Constants.TokenKeys.OTPExpiredTime + myOtp.EndOfLife
                                        + Constants.TokenKeys.ClientName + myData[dk.ClientName];
                    mOtpVerifyEvent.EventTypeId = Constants.EventLog.Otp.Expired.Item1;
                    mOtpVerifyEvent.EventTypeName = Constants.EventLog.Otp.Expired.Item2;
                    myResponse.Append("<" + sr.Reply + ">" + sr.Timeout + "</" + sr.Reply + ">");

                    // ReSharper disable once UnusedVariable
                    var myStat = new EventStat(myClient._id, myClient.Name, es.OtpExpired, 1);
                }
                else
                {   // validate the OTP
                    if (myData[dk.OTP] == myOtp.Code)
                    {
                        myOtp.Active = false; // set so OTP can't be used again
                        mOtpVerifyEvent.EventTypeDesc += Constants.TokenKeys.OTPRetriesMax + myOtp.ValidationRetries
                                            + Constants.TokenKeys.OTPCode + myData[dk.OTP]
                                            + Constants.TokenKeys.OTPRetriesCurrent + myOtp.ValidationCount
                                            + Constants.TokenKeys.OTPExpiredTime + myOtp.EndOfLife
                                            + Constants.TokenKeys.ClientName + myData[dk.ClientName];
                        mOtpVerifyEvent.EventTypeId = Constants.EventLog.Otp.Validated.Item1;
                        mOtpVerifyEvent.EventTypeName = Constants.EventLog.Otp.Validated.Item2;
                        myResponse.Append("<" + sr.Reply + ">" + sr.Validated + "</" + sr.Reply + ">");

                        // ReSharper disable once UnusedVariable
                        var myStat = new EventStat(myClient._id, myClient.Name, es.OtpValid, 1);
                    }
                    else
                    {  // OTP invalid
                        var badOtp = myData[dk.OTP];
                        if (badOtp.Length > 25)
                            badOtp = badOtp.Substring(0, 25) + "...";

                        if (myOtp.ValidationCount < myOtp.ValidationRetries)
                        { // Retry count not reached yet, allow retry
                            myOtp.ValidationCount++;
                            mOtpVerifyEvent.EventTypeDesc += Constants.TokenKeys.OTPCode + badOtp
                                        + Constants.TokenKeys.OTPRetriesMax + myOtp.ValidationRetries
                                        + Constants.TokenKeys.OTPRetriesCurrent + myOtp.ValidationCount
                                        + Constants.TokenKeys.OTPExpiredTime + myOtp.EndOfLife
                                        + Constants.TokenKeys.ClientName + myData[dk.ClientName];
                            mOtpVerifyEvent.EventTypeId = Constants.EventLog.Otp.BadOtpCode.Item1;
                            mOtpVerifyEvent.EventTypeName = Constants.EventLog.Otp.BadOtpCode.Item2;
                            myResponse.Append("<" + sr.Reply + ">" + sr.Invalid + "</" + sr.Reply + ">");

                            // ReSharper disable once UnusedVariable
                            var myStat = new EventStat(myClient._id, myClient.Name, es.OtpInvalid, 1);
                        }
                        else
                        {   // Retry count reached
                            myOtp.Active = false; // set so OTP can't be used again

                            mOtpVerifyEvent.EventTypeDesc += Constants.TokenKeys.OTPRetriesMax + myOtp.ValidationRetries
                                        + Constants.TokenKeys.OTPCode + myData[dk.OTP]
                                        + Constants.TokenKeys.OTPRetriesCurrent + myOtp.ValidationCount
                                        + Constants.TokenKeys.OTPExpiredTime + myOtp.EndOfLife
                                        + Constants.TokenKeys.ClientName + myData[dk.ClientName];
                            mOtpVerifyEvent.EventTypeId = Constants.EventLog.Otp.TooManyRetries.Item1;
                            mOtpVerifyEvent.EventTypeName = Constants.EventLog.Otp.TooManyRetries.Item2;
                            
                            myResponse.Append("<" + sr.Reply + ">" + sr.Disabled + "</" + sr.Reply + ">");
                        } // end Retry count reached
                    } // end OTP invalid
                } // end validate the OTP
            } // end OTP Active

            // Add the OTP event to the OTP object
            mUtils.UpdateOtpHistory(myOtp, mOtpVerifyEvent, mSvcName);

           // Add details to the response
            myResponse.Append(AddDetailsToResponse(myClient, myData, myOtp));

            // return the response back to the requester
            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
        }
        catch (Exception ex)
        {
            var mDetails = mSvcName + "." + dv.VerifyOtp + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
            var exceptionEvent = new Event
            {
                EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
            };
            if (myData.ContainsKey(dk.ClientName))
                exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
            exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

            // return error to requester
            return mUtils.FinalizeXmlResponseWithError(mSvcName + "." + myData[dk.Request] + ", " + myData[dk.ClientName], mLogId);
        }
    }

    protected string AddDetailsToResponse(Client pClient, Dictionary<String, String> pData, Otp pOtp)
    {
        var mDetails = new StringBuilder();
        mDetails.Append("<" + sr.Details + ">");
        {
            mDetails.Append(sr.Request + dk.KVSep + pData[dk.Request]);
            mDetails.Append(dk.ItemSep + dk.ClientName + dk.KVSep + pClient.Name);
            mDetails.Append(dk.ItemSep + dk.ReplyStatus + dk.KVSep + pOtp.Active);
            mDetails.Append(dk.ItemSep + sr.DeliveryMethod + dk.KVSep + pOtp.DeliveryMethod);
            mDetails.Append(dk.ItemSep + sr.TLM + dk.KVSep + pClient.OtpSettings.Timeout);
            mDetails.Append(dk.ItemSep + dk.OTPRetriesMax + dk.KVSep + pOtp.ValidationRetries);
            mDetails.Append(dk.ItemSep + dk.OTPExpiredTime + dk.KVSep + pOtp.EndOfLife);
        }
        mDetails.Append("</" + sr.Details + ">");
        return mDetails.ToString();
    }

 
}