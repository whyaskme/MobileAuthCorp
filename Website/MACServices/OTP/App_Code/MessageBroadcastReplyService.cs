using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Threading;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using sr = MACServices.Constants.ServiceResponse;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using mbrsp = MACServices.Constants.MessageBroadcast.ResponseKeys;
using es = MACServices.Constants.EventStats;
using cr = MACServices.Constants.ReplyServiceRequest;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class MessageBroadcastReplyService : WebService
{
    private const string mSvcName = "MessageBroadcastReplyService";
    private const string mLogId = "MR";

    [WebMethod]
    public XmlDocument WsMessageBroadcastReplyService()
    {
        var mUtils = new Utils();
        var data = HttpContext.Current.Request.QueryString.ToString();

        // request data Dictionary
        var myData = new Dictionary<string, string> { { dk.ServiceName, mSvcName } };
        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        // log request if debug set in web.config
        var eid = mUtils.LogRequest(myData, data, mLogId);

        var mRequest = HttpContext.Current.Request.QueryString[cr.Request];
        if (String.IsNullOrEmpty(mRequest))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, 
                "Invalid request, no 'Request' in QueryString." + Environment.NewLine + data, null);
        myData.Add(dk.Request, mRequest.Trim().ToLower());

        #region VerifyOTP
        if (myData[dk.Request] == dv.VerifyOtp.ToLower())
        {
            var minpId = HttpContext.Current.Request.QueryString[cr.inpId];
            if (String.IsNullOrEmpty(minpId))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null,
                    "Invalid request, no [" + cr.inpId + "] in QueryString." 
                    + Environment.NewLine + data, null);
            
            if (minpId.Contains(":") == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null,
                    "Invalid request, invalid " + cr.inpId + "[" + minpId + "] in QueryString." 
                    + Environment.NewLine + data, null);
            var mIds = minpId.Split(':');
            
            if (mIds.Count() < 2)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null,
                    "Invalid request, invalid " + cr.inpId + "[" + minpId + "] in QueryString." 
                    + Environment.NewLine + data, null);
            myData.Add(dk.CID, mIds[0]);
            myData.Add(dk.RequestId, mIds[1]);

            var mMsg = HttpContext.Current.Request.QueryString[cr.text];
            if (String.IsNullOrEmpty(mMsg))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null,
                    "Invalid request, no [" + cr.text + "] in QueryString." + Environment.NewLine + data, null);
            myData.Add(dk.Message, mMsg);

            var myClient = mUtils.GetClientUsingClientId(myData[dk.CID]);
            if (myClient == null)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, " Invalid Client Id, " + eid, null);
            myData.Add(dk.ClientName, myClient.Name);

            // get OTP using the Request Id
            var myOtp = mUtils.GetOtpUsingRequestId(myData[dk.RequestId]);
            if (myOtp == null)
            {
                // give database indexing a chance to catch up
                var count = 1;
                while (true)
                {
                    ++count;
                    if (count < 5)
                    {
                        Thread.Sleep(1000);
                        myOtp = mUtils.GetOtpUsingRequestId(myData[dk.RequestId]);
                        if (myOtp != null)
                            break;
                    }
                    else
                    {
                        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                            myData[dk.CID], "RequestId Invalid - RequestId: " + myData[dk.RequestId], null);
                    }
                }
            }
            myData.Add(dk.ReplyUri, myOtp.ReplyUri);
            if (myData[dk.Message] == "BLANK")
            {
                var mLoopbackSetting = ConfigurationManager.AppSettings[cfg.LoopBackTest];
                if (String.IsNullOrEmpty(mLoopbackSetting) == false)
                {
                    if (mLoopbackSetting == cfg.LoopBackReplyAtGateway)
                    {
                        myData.Remove(dk.Message);
                        myData.Add(dk.Message, myOtp.Code);
                    }
                }
            }
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
                    myOtp.ErrorMsg = "Inactive, canceled!";
                    myData.Add(dk.ReplyError, myOtp.ErrorMsg);
                    // ReSharper disable once UnusedVariable
                    var myStat = new EventStat(myClient._id, myClient.Name, es.OtpInvalid, 1);
                }
                else
                {
                    // OTP Active
                    // Has the OTP Timeout
                    var myNow = DateTime.UtcNow;
                    var myTill = myOtp.EndOfLife;
                    if (myNow > myTill)
                    {
                        //yes the OTP has Timeout
                        myOtp.Active = false; // set so OTP can't be used again
                        mOtpVerifyEvent.EventTypeDesc += Constants.TokenKeys.OTPRetriesMax + myOtp.ValidationRetries
                                                         + Constants.TokenKeys.OTPCode + myData[dk.Message]
                                                         + Constants.TokenKeys.OTPRetriesCurrent + myOtp.ValidationCount
                                                         + Constants.TokenKeys.OTPExpiredTime + myOtp.EndOfLife
                                                         + Constants.TokenKeys.ClientName + myData[dk.ClientName];
                        mOtpVerifyEvent.EventTypeId = Constants.EventLog.Otp.Expired.Item1;
                        mOtpVerifyEvent.EventTypeName = Constants.EventLog.Otp.Expired.Item2;

                        myResponse.Append("<" + sr.Reply + ">" + sr.Timeout + "</" + sr.Reply + ">");

                        myOtp.ErrorMsg = "Timed out, canceled";
                        myData.Add(dk.ReplyError, myOtp.ErrorMsg);
                        // ReSharper disable once UnusedVariable
                        var myStat = new EventStat(myClient._id, myClient.Name, es.OtpExpired, 1);
                    }
                    else
                    {
                        // validate the OTP
                        if (myData[dk.Message].Contains(myOtp.Code))
                        {
                            myOtp.Active = false; // set so OTP can't be used again
                            myOtp.ProvidersReply = sr.Validated;
                            mOtpVerifyEvent.EventTypeDesc += Constants.TokenKeys.OTPRetriesMax + myOtp.ValidationRetries
                                                             + Constants.TokenKeys.OTPCode + myData[dk.Message]
                                                             + Constants.TokenKeys.OTPRetriesCurrent +
                                                             myOtp.ValidationCount
                                                             + Constants.TokenKeys.OTPExpiredTime + myOtp.EndOfLife
                                                             + Constants.TokenKeys.ClientName + myData[dk.ClientName];
                            mOtpVerifyEvent.EventTypeId = Constants.EventLog.Otp.Validated.Item1;
                            mOtpVerifyEvent.EventTypeName = Constants.EventLog.Otp.Validated.Item2;

                            myResponse.Append("<" + sr.Reply + ">" + sr.Validated + "</" + sr.Reply + ">");
                            // ReSharper disable once UnusedVariable
                            var myStat = new EventStat(myClient._id, myClient.Name, es.OtpValid, 1);
                        }
                        else
                        {
                            // OTP invalid
                            var badOtp = myData[dk.Message];
                            if (badOtp.Length > 25)
                                badOtp = badOtp.Substring(0, 25) + "...";

                            if (myOtp.ValidationCount < myOtp.ValidationRetries)
                            {
                                // Retry count not reached yet, allow retry
                                myOtp.ValidationCount++;
                                mOtpVerifyEvent.EventTypeDesc += Constants.TokenKeys.OTPCode + badOtp
                                                                 + Constants.TokenKeys.OTPRetriesMax +
                                                                 myOtp.ValidationRetries
                                                                 + Constants.TokenKeys.OTPRetriesCurrent +
                                                                 myOtp.ValidationCount
                                                                 + Constants.TokenKeys.OTPExpiredTime + myOtp.EndOfLife
                                                                 + Constants.TokenKeys.ClientName +
                                                                 myData[dk.ClientName];
                                mOtpVerifyEvent.EventTypeId = Constants.EventLog.Otp.BadOtpCode.Item1;
                                mOtpVerifyEvent.EventTypeName = Constants.EventLog.Otp.BadOtpCode.Item2;

                                var otpChars = myOtp.Code.Length;
                                myOtp.ErrorMsg = "Invalid Entry! Please enter ONLY the " + otpChars + " digit authorization code!";

                                //myOtp.ErrorMsg = "Invalid OTP retry!";
                                myData.Add(dk.ReplyError, myOtp.ErrorMsg);
                                myResponse.Append("<" + sr.Reply + ">" + sr.Invalid + "</" + sr.Reply + ">");
                                // ReSharper disable once UnusedVariable
                                var myStat = new EventStat(myClient._id, myClient.Name, es.OtpInvalid, 1);
                            }
                            else
                            {
                                // Retry count reached
                                myOtp.Active = false; // set so OTP can't be used again

                                mOtpVerifyEvent.EventTypeDesc += Constants.TokenKeys.OTPRetriesMax +
                                                                 myOtp.ValidationRetries
                                                                 + Constants.TokenKeys.OTPCode + myData[dk.Message]
                                                                 + Constants.TokenKeys.OTPRetriesCurrent +
                                                                 myOtp.ValidationCount
                                                                 + Constants.TokenKeys.OTPExpiredTime + myOtp.EndOfLife
                                                                 + Constants.TokenKeys.ClientName +
                                                                 myData[dk.ClientName];
                                mOtpVerifyEvent.EventTypeId = Constants.EventLog.Otp.TooManyRetries.Item1;
                                mOtpVerifyEvent.EventTypeName = Constants.EventLog.Otp.TooManyRetries.Item2;
                                myOtp.ErrorMsg = "Retry limit exceeded! Transaction canceled!";
                                myData.Add(dk.ReplyError, myOtp.ErrorMsg);
                                myResponse.Append("<" + sr.Reply + ">" + sr.Disabled + "</" + sr.Reply + ">");
                            } // end Retry count reached
                        } // end OTP invalid
                    } // end validate the OTP
                } // end OTP Active

                // Add the OTP event to the OTP object
                mUtils.UpdateOtpHistory(myOtp, mOtpVerifyEvent, mSvcName);
                myOtp.Update();


                if (myData.ContainsKey(dk.ReplyError))
                {   // Setup dictionary to call Message Broadcast API service
                    myData.Remove(dk.ServiceName);
                    myData.Remove(dk.ClientName);
                    myData.Remove(dk.Request);
                    myData.Add(dk.Request, "MessageBroadcast (Sms)");
                    myData.Remove(dk.Message);
                    myData.Add(dk.Message, mUtils.StringToHex(myData[dk.ReplyError]));
                    if (String.IsNullOrEmpty(myOtp.UserId) == false)
                    {
                        myData.Remove(dk.UserId);
                        myData.Add(dk.UserId, myOtp.UserId); 
                    }
                    myData.Remove(dkui.PhoneNumber);
                    myData.Add(dkui.PhoneNumber, myOtp.ToPhone);
                    if (myOtp.Active)
                    { // Not Timeout, too many retries or disabled
                        myData.Remove(dk.EnableReply);
                        myData.Add(dk.EnableReply, "true");
                    }
                    var mLoopBackState = ConfigurationManager.AppSettings[cfg.LoopBackTest];
                    if ((mLoopBackState != null) && (mLoopBackState != cfg.Disabled))
                    {
                        myData.Remove(dk.LoopBackTest);
                        myData.Add(dk.LoopBackTest, mLoopBackState);
                    }
                    try
                    {
                        var msendMessageThread = 
                            new Thread(() => sendErrorMessageThread(myData))
                            {
                                IsBackground = true
                            };
                        msendMessageThread.Start();
                    }

                    catch (Exception ex)
                    {
                        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null,
                            "Exception starting thread " + Environment.NewLine + ex.Message, null);              
                    }
                    var rply = mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    return rply;
                }

                //======== OTP was processed and Client sent a reply ur ============================
                if ((myOtp.Active == false) && (myData.ContainsKey(dk.ReplyUri)))
                {
                    var cid = myClient._id.ToString();
                    // send back to client as URL with Query String containing results
                    try
                    {
                        var mUri = mUtils.HexToString(myData[dk.ReplyUri])
                                   + "?" + cr.Request + "=" + cr.SetResponseOTP
                                   + "&" + cr.cid + "=" + cid
                                   + "&" + cr.RequestId + "=" + myOtp._id
                                   + "&" + cr.ReplyState + "=" + myOtp.ReplyState;
                        if (String.IsNullOrEmpty(myOtp.ErrorMsg) == false)
                            mUri += "&" + cr.ErrorMsg + "=" + myOtp.ErrorMsg;

                        var request = (HttpWebRequest) WebRequest.Create(mUri);
                        request.Method = "GET";
                        request.MaximumAutomaticRedirections = 4;
                        request.MaximumResponseHeadersLength = 4;
                        request.Credentials = CredentialCache.DefaultCredentials;
                        var resp = (HttpWebResponse) request.GetResponse();
                        resp.GetResponseStream();
                        resp.Close();
                    }
                    catch (Exception ex)
                    {
                        var mDetails = mSvcName + ".ReplyToClient: " +
                                       ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                        var exceptionEvent = new Event
                        {
                            EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                        };
                        exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myClient.Name;
                        exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                    }
                }
                // return the response back to the Message Broadcast
                var mResponse = mUtils.FinalizeXmlResponse(myResponse, mLogId);
                return mResponse;
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + "." + dv.VerifyOtp + " " +
                               ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                // return error to requester
                return
                    mUtils.FinalizeXmlResponseWithError(
                        mSvcName + "." + myData[dk.Request] + ", " + myData[dk.ClientName], mLogId);
            }
        }
        #endregion

        #region ADSTOP/ADENABLE

        if ((myData[dk.Request] == dv.ADSTOP.ToLower()) || (myData[dk.Request] == dv.ADENABLE.ToLower()))
        {
            var mMobilePhone = HttpContext.Current.Request.QueryString[cr.inpContactString];
            if (String.IsNullOrEmpty(mMobilePhone))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null,
                    "Invalid request, no [" + cr.inpContactString + "] in QueryString." 
                    + Environment.NewLine + data, null);

            var mEndUser = mUtils.GetEndUserByHashedPhoneNumber(mMobilePhone);
            if (mEndUser == null)
            {
                //------- Open Check -----------------------------------------------
                var myOpenRequest = new Dictionary<string, string>
                {
                    {dk.Request, myData[dk.Request]},
                    {dkui.PhoneNumber, mMobilePhone}
                };
                var reply = mUtils.ServiceRequest(
                    ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                    Constants.ServiceUrls.MacOpenEndUserServices,
                    Constants.Strings.DefaultClientId, myOpenRequest);
                if (reply.Item1 == false)
                    return mUtils.FinalizeXmlResponseWithError(reply.Item2, mLogId);
            }
            else
            {
                if (myData[dk.Request] == dv.ADSTOP.ToLower())
                        mEndUser.OtpOutAd = false;
                if (myData[dk.Request] == dv.ADENABLE.ToLower())
                        mEndUser.OtpOutAd = true;
                var euEvent = new EndUserEvent(mSvcName + ": AdPass " + myData[dk.Request]);
                mEndUser.EndUserEvents.Add(euEvent);
                mEndUser.Update();
            }
            myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Success + "</" + sr.Reply + ">");
            var mXml1 = mUtils.FinalizeXmlResponse(myResponse, mLogId);
            return mXml1;
        }

        #endregion
        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, 
                    "Invalid request, invalid 'Request' in QueryString." 
                    + Environment.NewLine + data, null);
    }

    //thread to send text message
    protected void sendErrorMessageThread(Dictionary<String, String> pData)
    {
        Thread.Sleep(2 * 1000);
        try
        {
            var mUtils = new Utils();
            var mMessageDeliveryServiceUrl = ConfigurationManager.AppSettings[cfg.MessageBroadcastAPIService];
            mUtils.ServiceRequest(mMessageDeliveryServiceUrl, pData[dk.CID], pData);
        }
        catch (Exception ex)
        {
            var mDetails = mSvcName + ".sendRetryMessageThread " 
                + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") 
                + Constants.TokenKeys.ClientName + "NA";
            var exceptionEvent = new Event
            {
                EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
            };
            exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
        }
    }

}