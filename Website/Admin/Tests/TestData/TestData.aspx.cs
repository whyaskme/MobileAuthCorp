using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.IO;
using System.Web;

namespace MACUserApps.Web.Tests.TestData
{
    public partial class MACUserApps_Web_Tests_TestData_TestData : System.Web.UI.Page
    {
        private const string SelectFile = "Select file";
        private const string NoFiles = "No file";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) return;
            var fileEntries = Directory.GetFiles(HttpContext.Current.Server.MapPath("."));
            if (!fileEntries.Any())
            {
                var li = new ListItem();
                li.Text = li.Value = SelectFile;
                ddlTestFiles.Items.Add(li);
            }
            else
            {
                {
                    var li = new ListItem();
                    li.Text = li.Value = SelectFile;
                    ddlTestFiles.Items.Add(li);
                }
                foreach (var fileName in fileEntries)
                {
                    if (Path.GetExtension(fileName) == ".txt")
                    {
                        var li = new ListItem
                        {
                            Text = Path.GetFileNameWithoutExtension(fileName),
                            Value = fileName
                        };
                        ddlTestFiles.Items.Add(li);
                    }
                }
            }


        }

        protected void btnProcessFile_Click(object sender, EventArgs e)
        {
            var file = ddlTestFiles.SelectedItem.Text;
            if (file == SelectFile)
            {
                lbError.Text = @"you must select a file";
                return;
            }
            if (file == NoFiles)
            {
                lbError.Text = @"No files found";
                return;
            }
            var filePath = ddlTestFiles.SelectedValue;
            AddToLogAndDisplay("File Selected:" + file + "|Path:" + filePath);
            lbLineNumber.Text = "0";
            OutPutLine(filePath);
            btnNext.Enabled = true;
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (OutPutLine(ddlTestFiles.SelectedValue) == false)
            {
                btnNext.Enabled = false;
            }
        }

        private bool OutPutLine(string pFilePath)
        {
            var linenumber = Convert.ToInt32(lbLineNumber.Text);
            AddToLogAndDisplay("|Line " + (linenumber + 1).ToString());
            var mFile = new StreamReader(pFilePath);
            string line;
            int count = 0;
            var myLineData = new Dictionary<string, string>();
            while ((line = mFile.ReadLine()) != null)
            {
                if (linenumber == count)
                {

                    ParseLineFromFileToDictionary(line, myLineData);
                    foreach (KeyValuePair<string, string> mItem in myLineData)
                    {
                        AddToLogAndDisplay(mItem.Key + "=" + mItem.Value);
                    }
                    lbLineNumber.Text = (linenumber + 1).ToString(); 
                    return true;
                }
                ++count;
            }
            return false;
        }

        private void ParseLineFromFileToDictionary(string pLine, Dictionary<string, string> pLineData )
        {
            var lineItems = pLine.Split('|');
            if (!lineItems.Any()) return;
            foreach (var item in lineItems)
            {
                if (item.Contains("="))
                {
                    var itemkv = item.Split('=');
                    pLineData.Remove(itemkv[0]);
                    pLineData.Add(itemkv[0], itemkv[1]);
                }
            }
        }


        private void AddToLogAndDisplay(string textToAdd)
        {
            var newlog = String.Format("{0}|{1}", Session["LogText"], textToAdd);
            Session["LogText"] = newlog;
            tbLog.Text = newlog.Replace("|", Environment.NewLine);
        }
    }
}