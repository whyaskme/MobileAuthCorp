using System;
using System.Net;
using System.Web.UI;
using System.IO;

public partial class ResultsPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var mEFileName = (Request.QueryString["f"]);
        var mFileName = MACSecurity.Security.DecodeAndDecrypt(mEFileName, MACServices.Constants.Strings.DefaultClientId);
        var mFileType = Path.GetExtension(mFileName);
        if (!String.IsNullOrEmpty(mFileType))
        {
            if (mFileType.ToLower() == ".png")
            {
                Contentdiv.Visible = false;
                xmldiv.Visible = false;
                pngdiv.InnerHtml = "<img src='" + mFileName.Replace("\\", "/") + "' />";
                return;
            }
            var mFileContent = System.IO.File.ReadAllText(mFileName);
            if (!String.IsNullOrEmpty(mFileContent))
            {
                if (mFileType.ToLower() == ".xml")
                {
                    Contentdiv.Visible = false;
                    pngdiv.Visible = false;
                    var NL = Environment.NewLine;
                    lit1.Text = (WebUtility.HtmlEncode(mFileContent)).Replace("&gt;&lt;", "&gt;<br />&lt;")
                        .Replace(NL, "<br />");
                    return;
                }
                // must be html
                xmldiv.Visible = false;
                pngdiv.Visible = false;
                Contentdiv.InnerHtml = mFileContent;
            }
        }
    }
}