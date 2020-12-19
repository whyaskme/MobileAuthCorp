using System;
using System.Configuration;
using System.IO;

public partial class Redirect : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            lbUrl.Text = Request.Url.AbsoluteUri;
            lbPageRequested.Text = Request.QueryString["aspxerrorpath"];
            Log("PageRequested:" + lbPageRequested.Text);
            var mUrl = lbUrl.Text.Split('?');
            lbhttp.Text = mUrl[0];
            if (String.IsNullOrEmpty(lbhttp.Text) == false)
            {
                // Don't do this
                lbhttp.Text = lbhttp.Text.Replace("redirect.aspx", "");

                if (lbPageRequested.Text.Contains("/"))
                {
                    lbPageRequested.Text = lbPageRequested.Text.ToLower();
                    var urlparts = lbPageRequested.Text.Split('/');
                    foreach (var mUrlPart in urlparts)
                    {
                        if (String.IsNullOrEmpty(mUrlPart) == false)
                        {
                            if (mUrlPart.StartsWith("demo")) continue;
                            if (mUrlPart.StartsWith("iis")) continue;

                            if (mUrlPart.Contains("_"))
                            {
                                /*
                                 * should look line A_TL_T1 or G_GL_G2
                                 * where when split on '_'
                                 * part[0] is the client key for the AdClient (from web.config)
                                 * part[1] + .aspx is the landing page
                                 * part[3] is the AdType
                                 * */
                                var parts = mUrlPart.Replace(".aspx", "").Split('_');
                                var mClientName = GetClientNameFromConfig(parts[0]);
                                if (String.IsNullOrEmpty(mClientName) == false)
                                {
                                    var myTargetUrl = lbhttp.Text +
                                                      parts[1] + ".aspx" +
                                                      "?AdClient=" + mClientName +
                                                      "&AdType=" + parts[2];
                                    Log("Redirect0To:" + myTargetUrl);
                                    lbTargetUrl.Text = myTargetUrl;
                                    Response.Redirect(myTargetUrl, false);
                                }
                            }
                            else
                            {
                                /* should look like "AT1" or "GG1" or "GGS"
                                * Where:
                                 * first character is the client key for the AdClient (from web.config)
                                 * second and third character + .aspx are the landing page
                                 * second and third character are the AdType
                                 * */
                                var mKeys = mUrlPart.Replace(".aspx", "");
                                var mClientName = GetClientNameFromConfig(mKeys);
                                var arr = mKeys.ToUpper().ToCharArray(0, 3);
                                var myTargetUrl = lbhttp.Text +
                                                  arr[1].ToString() + arr[2].ToString() + ".aspx" +
                                                  "?AdClient=" + mClientName +
                                                  "&AdType=" + arr[1].ToString() + arr[2].ToString();


                                // Not sure how this is getting injected into the url. Replace for now as a hack.
                                //myTargetUrl = myTargetUrl.Replace("IS.aspx", "");

                                Log("Redirect1To:" + myTargetUrl);


                                lbTargetUrl.Text = myTargetUrl;
                                Response.Redirect(myTargetUrl, false);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log("Redirect.Exception:" + ex.Message);
        }
        // just display 404 page
    }

    protected string GetClientNameFromConfig(string pTestAdsClientKey)
    {
        lbKey.Text = pTestAdsClientKey.ToUpper();
        var mTestAdClientList = ConfigurationManager.AppSettings["TestAdsClients"];
        lbList.Text = mTestAdClientList;
        if (String.IsNullOrEmpty(mTestAdClientList))
        {
            lbError.Text = @"No Test Ad Client List in appsettings of web.config!";
            Log(lbError.Text);
            return null;
        }
        var mTestAdClients = mTestAdClientList.Split('|');
        foreach (var mTestAdClient in mTestAdClients)
        {
            if (mTestAdClient.StartsWith(pTestAdsClientKey.ToUpper()))
            {
                if (mTestAdClient.Contains(":"))
                {
                    var mKey_ClientName = mTestAdClient.Split(':');
                    if (string.IsNullOrEmpty(mKey_ClientName[1]) == false)
                    {
                        Log(mKey_ClientName[1]);
                        return mKey_ClientName[1];
                    }
                }
            }
        }
        Log("Error: GetClientNameFromConfig(" + pTestAdsClientKey);
        lbError.Text = @"Test Client";
        return null;
    }

    protected void Log(string pEntry)
    {
        try
        {
            var mLogging = ConfigurationManager.AppSettings["Logging"];
            if (!String.IsNullOrEmpty(mLogging))
            {
                if (mLogging.ToLower() == "true")
                {
                    const string mLogDir = "c:/temp";
                    const string mFile = "redirectlog.txt";
                    //if (!Directory.Exists(mLogDir))
                    //{
                    //    Directory.CreateDirectory(mLogDir);
                    //}
                    using (var file = new StreamWriter(Path.Combine(mLogDir, mFile), true))
                    {
                        file.WriteLine(pEntry);
                    }
                }
            }
        }
        catch(Exception ex)
        {
            var errMsg = ex.Message;
        }
    }
}