using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

public partial class Admin_Tests_AdminOTPTests_EditDocTemplates : System.Web.UI.Page
{
    private const string Test = "Doc";
    private const string SelectClient = "Select Client";
    private const string NoClient = "No Clients";
    private const string SelectGroup = "Select Group";
    private const string NoGroups = "No Groups";

    protected void Page_Load(object sender, EventArgs e)
    {
        Session["LogText"] = "";
        if (IsPostBack) return;
        GetClients();
        SetGroups();
    }

    protected void btnGetDocTemplates_Click(object sender, EventArgs e)
    {

        var cid = GetSelectedClientId(ddlClient.SelectedValue);
        if (string.IsNullOrEmpty(cid)) return;

        var mUtils = new Utils();
        var mClient = mUtils.GetClientUsingClientId(cid);
        if (mClient == null)
        {
            lbError.Text = @"Can't get Client";
            return;
        }
        lbError.Text = "";
        hiddenTemplates.Value = "";
        foreach (var mTemplate in mClient.DocumentTemplates)
        {
            var key = mTemplate.MessageClass;
            var Value = mUtils.StringToHex(
                    dk.ItemSep + "des" + dk.KVSep + mUtils.StringToHex(mTemplate.MessageDesc) +
                    dk.ItemSep + "fmt" + dk.KVSep + mUtils.StringToHex(mTemplate.MessageFormat) +
                    dk.ItemSep + "fadr" + dk.KVSep + mUtils.StringToHex(mTemplate.MessageFromAddress) +
                    dk.ItemSep + "fname" + dk.KVSep + mUtils.StringToHex(mTemplate.MessageFromName)
                );
            hiddenTemplates.Value += dk.ItemSep + key + dk.KVSep + Value;
        }
        hiddenTemplates.Value = hiddenTemplates.Value.Trim(char.Parse(dk.ItemSep));
        fillTableFromHiddenField();
    }

    private void fillTableFromHiddenField()
    {
        var mUtils = new Utils();
        // get template classes
        var mDocClasses = hiddenTemplates.Value.Split(char.Parse(dk.ItemSep));
        foreach (string mDocClass in mDocClasses)
        {
            var mKeyValue = mDocClass.Split(char.Parse(dk.KVSep));
            var mKey = mKeyValue[0];
            var mValue = mUtils.HexToString(mKeyValue[1]);
            var mValues = mValue.Split(char.Parse(dk.ItemSep));
            var kdesc =  mValues[0];
            var vdesc = mUtils.HexToString(mValues[1]);
        }

    }

            //    {
            //    var mRow = new TableRow();

            //    var classCell = new TableCell();
            //    classCell.Text = mTemplate.MessageClass;
            //    var uWidth = new Unit(50, UnitType.Point);
            //    classCell.Width = uWidth;
            //    mRow.Cells.Add(classCell);

            //    var descCell = new TableCell();
            //    descCell.Text = mTemplate.MessageDesc;
            //    mRow.Cells.Add(descCell);

            //    var edit = new CheckBox();
            //    edit.Text = mTemplate.MessageClass;
            //    edit.


            //    edit.Text = mTemplate.MessageFormat;
            //    edit.Value = mTemplate.MessageClass;
            //    edit.Click += new EventHandler(this.edit_Click);

            //    var fmtCell = new TableCell();
            //    fmtCell.Controls.Add(edit);
            //    mRow.Cells.Add(fmtCell);
              
            //    tbDocs.Rows.Add(mRow);
            //}

    protected void edit_Click(Object sender, EventArgs e)
    {
        //do amazing stuff
    }

    #region Selected
   
    public void ddlClient_Selected(object sender, EventArgs e)
    {
        SetGroups();
    }
    #endregion

    #region Helper Methods

  /// <summary> Get selected Client Id </summary>
    private string GetSelectedClientId(string pClientDetails)
    {
        if (pClientDetails == SelectClient)
        {
            lbError.Text = @"You must select a client!";
            return null;
        }
        var cd = pClientDetails.Split(char.Parse(dk.ItemSep));
        return cd[0];
    }

    /// <summary> Get end user's last name </summary>
    private string GetUserInfo(string pKey, string pUserDetails)
    {
        var mDetails = pUserDetails.Split(char.Parse(dk.ItemSep));
        if (mDetails.Any())
        {
            foreach (var mDetail in mDetails)
            {
                if (mDetail.StartsWith(pKey))
                {
                    return mDetail.Replace(pKey + dk.KVSep, "");
                }
            }
        }
        return null;
    }

    private string GetSelectedGroupId()
    {
        if (ddlGroups.Visible)
        {
            if (ddlGroups.SelectedValue != NoGroups)
            {
                if (ddlGroups.SelectedValue != SelectGroup)
                {
                    var gd = ddlGroups.SelectedValue.Split(char.Parse(dk.ItemSep));
                    return gd[0];
                }
            }
        }
        return null;
    }

    private void SetGroups()
    {

        lbGroup.Visible = false;
        ddlGroups.Visible = false;

        ddlGroups.Items.Clear();
        var values = ddlClient.SelectedValue.Split(Char.Parse(dk.ItemSep));
        if (values.Count() < 2)
        {
            ddlGroups.Items.Add(NoGroups);
            return;
        }
        var skipcid = true; // first entry is always the clientId
        foreach (var group in values)
        {
            var grouplist = new ListItem();
            if (skipcid)
            {
                grouplist.Text = grouplist.Value = SelectGroup;
                skipcid = false;
            }
            else
            {
                lbGroup.Visible = true;
                ddlGroups.Visible = true;
                var name_id = group.Split('=');
                grouplist.Text = name_id[0];
                grouplist.Value = name_id[1];
            }
            ddlGroups.Items.Add(grouplist);
        }
    }

    private void GetClients()
    {
        try
        {
            var request = WebRequest.Create(ConfigurationManager.AppSettings[cfg.MacServicesUrl] + TestLib.TestConstants.GetTestClientsInfoUrl);
            request.Method = "Post";
            request.ContentLength = 0;
            var res = request.GetResponse();
            var response = res.GetResponseStream();
            var xmlDoc = new XmlDocument();
            if (response != null) xmlDoc.Load(response);
            var elemList = xmlDoc.GetElementsByTagName(sr.Error);
            if (elemList.Count != 0)
            {
                lbError.Text = String.Format("Error: returned from GetClients service {0}", elemList[0].InnerXml);
                return;
            }

            ddlClient.Items.Clear();
            elemList = xmlDoc.GetElementsByTagName("Client");
            if (elemList.Count != 0)
            {
                var li1 = new ListItem
                {
                    Text = SelectClient,
                    Value = SelectClient
                };
                ddlClient.Items.Add(li1);
                foreach (XmlNode myClientNode in elemList)
                {
                    var li = new ListItem
                    {
                        Text = myClientNode.ChildNodes[0].InnerXml,
                        Value = myClientNode.ChildNodes[1].InnerXml
                    };
                    var gelemList = myClientNode.ChildNodes[2].ChildNodes;
                    foreach (XmlNode gnode in gelemList)
                    {
                        li.Value += dk.ItemSep + gnode.ChildNodes[0].InnerText;
                    }
                    ddlClient.Items.Add(li);
                }
            }
            else
            {
                var li0 = new ListItem
                {
                    Text = NoClient,
                    Value = NoClient
                };
                ddlClient.Items.Add(li0);
            }
        }
        catch (Exception ex)
        {
            AddToLogAndDisplay(ex.ToString());
            lbError.Text = @"GetClients service error";
        }
    }

    
    protected void btnClearLog_Click(object sender, EventArgs e)
    {
        Session["LogText"] = "";
        AddToLogAndDisplay("btnClearLog");
    }
    private void AddToLogAndDisplay(string textToAdd)
    {
        var newlog = String.Format("{0}|{1}.{2}", Session["LogText"], Test, textToAdd);
        Session["LogText"] = newlog;
        tbLog.Text = newlog.Replace("|", Environment.NewLine);
    }

    #endregion
    
}