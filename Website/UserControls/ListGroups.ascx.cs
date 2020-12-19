using System;
using System.Configuration;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using MACServices;

public partial class UserControls_ListGroups : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        dlGroups.Items.Clear();

        var liRoot = new ListItem();

        liRoot.Text = @"Please select a Group";
        liRoot.Value = @"-1";
        dlGroups.Items.Add(liRoot);

        var groupList = new MacListAdHoc("Group", "_t", "Group", false, "");

        // Here we need to process the xml response into a List collection
        var xmlGroupsDoc = groupList.ListXml;
        var xmlGroups = xmlGroupsDoc.GetElementsByTagName("group");

        for (int i = 0; i < xmlGroups.Count; i++)
        {
            var li = new ListItem();

            li.Text = xmlGroups[i].Attributes["name"].Value;
            li.Value = xmlGroups[i].Attributes["id"].Value;

            dlGroups.Items.Add(li);
        }
    }
}