using System;
using System.Net;
using System.Text;
using System.Web.UI;
using System.IO;
using MongoDB.Driver;


public partial class Admin_Reports_Operations_ReportPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var mEFileName = (Request.QueryString["f"]);
        var mETestRunId = Request.QueryString["i"];
        var mFileName = MACSecurity.Security.DecodeAndDecrypt(mEFileName, MACServices.Constants.Strings.DefaultClientId);
        var mTestRunId = MACSecurity.Security.DecodeAndDecrypt(mETestRunId, MACServices.Constants.Strings.DefaultClientId);

        var mFileType = Path.GetExtension(mFileName);

        var mOpUtils = new MACOperationalTestLib.OperationalTestUtils();
        var mFileContent = mOpUtils.GetResultsFileContent(mTestRunId, mFileName);
        if (String.IsNullOrEmpty(mFileContent) == false)
        {
            if (!String.IsNullOrEmpty(mFileType))
            {
                if (mFileType.ToLower() == ".xml")
                {
                    var NL = Environment.NewLine;
                    Contentdiv.Visible = false;
                    pngdiv.Visible = false;
                    lit1.Text = (WebUtility.HtmlEncode(mFileContent)).Replace("&gt;&lt;", "&gt;<br />&lt;").Replace(NL, "<br />");
                    return;
                }
                if (mFileType.ToLower() == ".png")
                {
                    Contentdiv.Visible = false;
                    xmldiv.Visible = false;
                    mFileContent =
                        @"R0lGODlhEAAOALMAAOazToeHh0tLS/7LZv/0jvb29t/f3//Ub//ge8WSLf/rhf/3kdbW1mxsbP//mf///yH5BAAAAAAALAAAAAAQAA4AAARe8L1Ekyky67QZ1hLnjM5UUde0ECwLJoExKcppV0aCcGCmTIHEIUEqjgaORCMxIC6e0CcguWw6aFjsVMkkIr7g77ZKPJjPZqIyd7sJAgVGoEGv2xsBxqNgYPj/gAwXEQA7";
                    pngdiv.InnerHtml = "<img src='data:image/png;base64," + mFileContent + "' >";

                    //pngdiv.InnerHtml =
                    //    "<img src='data:image/gif;base64,R0lGODlhEAAOALMAAOazToeHh0tLS/7LZv/0jvb29t/f3//Ub//ge8WSLf/rhf/3kdbW1mxsbP//mf///yH5BAAAAAAALAAAAAAQAA4AAARe8L1Ekyky67QZ1hLnjM5UUde0ECwLJoExKcppV0aCcGCmTIHEIUEqjgaORCMxIC6e0CcguWw6aFjsVMkkIr7g77ZKPJjPZqIyd7sJAgVGoEGv2xsBxqNgYPj/gAwXEQA7' width='16' height='14' >";
                    return;
                }
            }
            // must be html
            xmldiv.Visible = false;
            pngdiv.Visible = false;
            Contentdiv.InnerHtml = mFileContent;  
        }


    }

}