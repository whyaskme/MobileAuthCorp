using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallWCF
{
    public class RegisterPlayer
    {
        public RegisterPlayer()
        {
            JsonData += "{";
            JsonData += "\"operatorId\":\"188\",";
            JsonData += "\"siteId\":\"227\",";
            JsonData += "\"siteUsername\":\"188642015227\",";
            JsonData += "\"sitePwd\":\"@8cIa6NBTdTZjzb7\",";
            JsonData += "\"geoComplyEncryptedPacket\":\"ZsUiDymAiyVr/aQxwqC60c50qCfhJ9WPvZo3TrNAmXxD20onJILaqkmK+CGEDzr7tveVE=\",";
            JsonData += "\"userName\":\"tdavis@mobileauthcorp.com\",";
            JsonData += "\"ipAddress\":\"184.182.215.167\",";
            JsonData += "\"playerDetails\": ";
            JsonData += "{";
            JsonData += "\"userName\":\"Joe-20154825104\",";
            JsonData += "\"firstName\":\"Joe\",";
            JsonData += "\"middleInitial\":\"J\",";
            JsonData += "\"lastName\":\"Baranauskas\",";
            JsonData += "\"gender\":\"Male\",";
            JsonData += "\"dob\":\"07/19/1963\",";
            JsonData += "\"emailAddress\":\"jbaranauskas@mobileauthcorp.com\",";
            JsonData += "\"playerAddress1\":\"7275 E Gold Dust\",";
            JsonData += "\"playerAddress2\":\"#104\",";
            JsonData += "\"city\":\"Paradise Valley\",";
            JsonData += "\"county\":\"Maricopa\",";
            JsonData += "\"state\":\"Arizona\",";
            JsonData += "\"zipCode\":\"85258\",";
            JsonData += "\"country\":\"United States\",";
            JsonData += "\"mobileNo\":\"+1-480-634-0702\",";
            JsonData += "\"landLineNo\":\"+1-480-939-2980\",";
            JsonData += "\"ssn\":\"452-45-2741\",";
            JsonData += "\"dlNumber\":\"D08719647\",";
            JsonData += "\"dlIssuingState\":\"Arizona\",";
            JsonData += "\"ipAddress\":\"184.182.215.167\"";
            JsonData += "}";
            JsonData += "}";
        }

        public string JsonData { get; set; }
    }

    public class PreCheckSiteValidatePlayerRequest
    {
        public PreCheckSiteValidatePlayerRequest()
        {
            JsonData += "{";
            JsonData += "\"operatorId\":\"188\",";
            JsonData += "\"siteId\":\"227\",";
            JsonData += "\"siteUsername\":\"188642015227\",";
            JsonData += "\"sitePwd\":\"@8cIa6NBTdTZjzb7\",";
            JsonData += "\"userName\":\"johnsmith1\",";
            JsonData += "\"ipAddress\":\"209.237.227.195\",";
            JsonData += "\"geoComplyEncryptedPacket\":\"ZsUiDymAiyVr/aQxwqC60c50qCfhJ9WPvZo3TrNAmXxD20onJILaqkmK+CGEDzr7tvey1k=\",";
            JsonData += "\"deviceFingerPrint\":\"value\"";
            JsonData += "}";
        }

        public string JsonData { get; set; }
    }

    public class SubmitPlayerKBA
    {
        public string operatorId { get; set; }
        public string siteId { get; set; }
        public string siteUsername { get; set; }
        public string sitePwd { get; set; }
        public string requestToken { get; set; }
        public KBAAnswer[] answer { get; set; }
    }

    public class KBAAnswer
    {
        public string answerId { get; set; }
        public string questionId { get; set; }
    }
}
