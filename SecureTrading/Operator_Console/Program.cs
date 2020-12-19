using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace CallWCF
{
    public class Program
    {
        private static void Main()
        {
            string requestURL = "http://localhost/STAPI/STServices.asmx/";
            
            var jsonString = "";

            var calledMethod = "HelloWorld";
            switch(calledMethod)
            {
                case "HelloWorld":
                    jsonString = new RegisterPlayer().JsonData;
                    requestURL += "HelloWorld";
                    break;

                case "RawJSON":
                    jsonString = new RegisterPlayer().JsonData;
                    requestURL += "rawJSONMSFT"; 
                    break;

                case "RegisterPlayer":
                    jsonString = new RegisterPlayer().JsonData;
                    requestURL += "stapiRegisterPlayer";
                    break;

                case "PreCheckSiteValidatePlayerRequest":
                    jsonString = new PreCheckSiteValidatePlayerRequest().JsonData;
                    requestURL += "stapiPreCheckSiteValidatePlayerRequest";
                    break;

                case "SubmitPlayerKBA":
                    //jsonString = new SubmitPlayerKBA().JsonData;
                    //requestURL += "stapiSubmitPlayerKBA";
                    break;
            }

            var sbResponse = new StringBuilder();

            var dataStream = Encoding.ASCII.GetBytes(jsonString);

            var mUrl = requestURL;
            try
            {
                var myHttpWebRequest = (HttpWebRequest) WebRequest.Create(mUrl);
                myHttpWebRequest.Method = "POST";

                // Set the content type of the data being posted.
                //myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
                myHttpWebRequest.ContentType = "application/json; charset=utf-8";
                myHttpWebRequest.ContentLength = dataStream.Length;

                var newStream = myHttpWebRequest.GetRequestStream();
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();

                using (var response = (HttpWebResponse) myHttpWebRequest.GetResponse())
                {
                    var header = response.GetResponseStream();
                    if (header == null) throw new Exception("stapiRequest returned null header!");
                    var encode = Encoding.GetEncoding("utf-8");
                    var readStream = new StreamReader(header, encode);
                    var mResponseData1 = readStream.ReadToEnd();
                    sbResponse.Append(mResponseData1);
                    response.Close();
                    readStream.Close();
                }
            }
            catch (Exception ex)
            {
                var errMsg = ex.ToString();
                sbResponse.Append("Exception: " + errMsg);
            }
            Console.WriteLine(sbResponse.ToString());
            Console.ReadLine();
        }
    }
}
