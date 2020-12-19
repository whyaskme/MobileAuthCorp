using System;
using System.Configuration;
using System.Text;

using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
namespace Admin.Tests.AWS
{
    public partial class OtpValidation : System.Web.UI.Page
    {
        public string macServicesUrl = ConfigurationManager.AppSettings["MacServicesUrl"];

        public StringBuilder sbResponse = new StringBuilder();

        public Int16 autoRetryCount = Convert.ToInt16(ConfigurationManager.AppSettings["AutoRetryCount"]);
        public Int16 currRetryCount = 0;

        public string cid;
        public string requestId;
        public string otpCode;
        public string validationResult;
        public string userIp;

        public string serviceReply = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            cid = Request["cid"];
            requestId = Request["requestid"];
            otpCode = Request["otp"];
            validationResult = "<span style='color: #ff0000;'>False</span>";
            userIp = Request.ServerVariables["LOCAL_ADDR"];

            serviceReply = sendOtpValidationRequest();
            if (serviceReply.Contains("Validated"))
            {
                validationResult = "<span style='color: #058f1f;'>True</span>";
            }
            else
            {
                validationResult = "<span style='color: #ff0000;'>" + serviceReply + "</span>";

                if (!serviceReply.Contains("Inactive"))
                {
                    // Retry
                    if (autoRetryCount > 0)
                    {
                        currRetryCount++;

                        while (currRetryCount < autoRetryCount)
                        {
                            serviceReply = sendOtpValidationRequest();
                            if (serviceReply.Contains("Validated"))
                            {
                                validationResult = "<span style='color: #058f1f;'>True</span>";
                                return;
                            }
                            else
                                currRetryCount++;
                        }

                        if (serviceReply.Contains("Validated"))
                            validationResult = "<span style='color: #058f1f;'>True</span>";
                        else // We've exhausted retries and need to bailout here...
                            validationResult = "<span style='color: #ff0000;'>" + serviceReply + "</span>";
                    }
                }
            }

            sbResponse.Append("<div>Valid?  " + validationResult + "</div>");

            divOtpValidation.InnerHtml = sbResponse.ToString();
        }

        public string sendOtpValidationRequest()
        {
            sbResponse.Append("<div>Try #: " + (currRetryCount+1) + "</div>");
            sbResponse.Append("<div>Server: " + userIp + "</div>");
            sbResponse.Append("<div>CID: " + cid + "</div>");
            sbResponse.Append("<div>RequestId: " + requestId + "</div>");
            sbResponse.Append("<div>OTP: " + otpCode + "</div>");
            sbResponse.Append("<hr />");

            var myMacotp = new MacOtp.MacOtp();
            // Validate the OTP
            serviceReply = myMacotp.VerifyOtp(macServicesUrl, cid, requestId, otpCode);

            return serviceReply;
        }
    }
}