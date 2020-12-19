using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace MACServices
{
    public class SSLSupport
    {
        private static readonly string[] TrustedHosts = new[] 
        {
          "localhost", 
          "demo.mobileauthcorp.com", 
          "polaris.mobileauthcorp.com", 
          "qa.mobileauthcorp.com", 
          "test.mobileauthcorp.com", 
          "www.mobileauthcorp.com"
        };

        public SSLSupport()
        {

        }

        public static void EnableTrustedHosts()
        {
            ServicePointManager.ServerCertificateValidationCallback =
            (sender, certificate, chain, errors) =>
            {
                if (errors == SslPolicyErrors.None)
                {
                    return true;
                }

                var request = sender as HttpWebRequest;
                if (request != null)
                {
                    return TrustedHosts.Contains(request.RequestUri.Host);
                }

                return false;
            };
        }
    }
}
