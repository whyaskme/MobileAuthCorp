using System;
using System.Configuration;
using System.Text;

using MACServices;

namespace Admin.Tests.AWS
{
    public partial class Default : System.Web.UI.Page
    {
        StringBuilder sbResponse = new StringBuilder();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var myMacOtp = new MacOtp.MacOtp();
                //var sReply = myMacOtp.SendOtpToAdminUser(ConfigurationManager.AppSettings["MacServicesUrl"], "system@mobileauthcorp.com", Constants.Strings.DefaultClientId);

                var sReply = myMacOtp.SendOtpToAdminUser(ConfigurationManager.AppSettings["MacServicesUrl"], "system@mobileauthcorp.com", Constants.Strings.DefaultClientId);

                if (sReply.StartsWith("Error"))
                {
                    var validationEvent = new Event();
                    validationEvent.Create(Constants.EventLog.Exceptions.General, Constants.TokenKeys.ExceptionDetails + sReply);

                    sbResponse.Append(sReply);


                }
                else
                {
                    var tmpOtp = sReply.Split('~');
                    var tmpRequestId = tmpOtp[0].Split('=');

                    var requestId = tmpRequestId[1];
                    var requestOtp = tmpOtp[1].Replace("Otp=", "");

                    sbResponse.Append("<div id='divServer'>");
                    sbResponse.Append("<span style='color: #000000;'>Server:</span> " + Request.ServerVariables["LOCAL_ADDR"]);
                    sbResponse.Append("</div>");
                    sbResponse.Append("<div id='divRequestId'>");
                    sbResponse.Append("<span style='color: #000000;'>RequestId:</span> " + requestId);
                    sbResponse.Append("</div>");
                    sbResponse.Append("<div id='divRequestOtp'>");
                    sbResponse.Append("<span style='color: #000000;'>OTP Code:</span> " + requestOtp);
                    sbResponse.Append("</div>");

                    // Redirect to Otp Validation
                    //Response.Redirect("OTP-Validation.aspx?requestId=" + requestId + "&Otp=" + requestOtp);
                    Response.Redirect("OTP-Validation.aspx?cid=" + Constants.Strings.DefaultClientId + "&requestid=" + requestId + "&otp=" + requestOtp, true);
                    Response.End();
                }

                divOTPRequestResult.InnerHtml = sbResponse.ToString();
            }
            catch (Exception ex)
            {
                var exceptionEvent = new Event();
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, Constants.TokenKeys.ExceptionDetails + ex);
            }
        }
    }
}