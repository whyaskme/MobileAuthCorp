using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

using MACServices;

using cnt = MACServices.Constants;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;
using es = MACServices.Constants.EventStats;
using sc = MACServices.Constants.Strings;

using MongoDB.Bson;
using MongoDB.Driver;


public partial class Admin_System_ScheduledProcessing : System.Web.UI.Page
{
    private MongoDatabase mongoDatabase;

    public EventStat systemStats;

    public Int32 itemCount = 0;

    public StringBuilder sbResponse = new StringBuilder();

    public DateTime startDate = DateTime.UtcNow.AddDays(-30);
    public DateTime endDate = DateTime.UtcNow.AddDays(1);

    protected void Page_Load(object sender, EventArgs e)
    {
        // Deny access if user request not logged in
        var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
        if (!isAuthenticated)
        {
            Response.Write("Hello world!");
            Response.End();
        }

// ReSharper disable once UnusedVariable
        var selectedClientId = ObjectId.Parse(cnt.Strings.DefaultEmptyObjectId);

        divButtons.Visible = false;

        mongoDatabase = (MongoDatabase)Application[sc.MongoDB];

        try
        {
            if (!IsPostBack)
            {
                GetClientList();
            }
            else
            {
                if(dlClients.SelectedIndex > 0)
                    ProcessOwnerStats();
            }

            // Should be called monthly
            if (Request["billing"] != null)
                if (Request["billing"].ToString() == "true")
                    ProcessBilling();

            //// Should be called every 5 mins
            //if (Request["eventtypelists"] != null)
            //    if (Request["eventtypelists"].ToString() == "true")
            //        UpdateEventTypeLists();

            //// Should be called every 5 mins
            //if (Request["systemstats"] != null)
            //    if (Request["systemstats"].ToString() == "true")
            //        ProcessSystemStats();
        }
// ReSharper disable once EmptyGeneralCatchClause
// ReSharper disable once UnusedVariable
        catch (Exception ex)
        {
            var errMsg = ex.ToString();
        }
    }

    #region Process Client billing

        private void ProcessBilling()
        {

        }

    #endregion

    #region Process Stats

        public void ProcessOwnerStats()
        {
            try
            {
                // This is just a testing update
                //var clientCollection = mongoDatabase.GetCollection("Client");

                //var clientList = clientCollection.FindAllAs<Client>();
                //foreach (var currentClient in clientList)
                //{
                //    currentClient.OpenAccessServicesEnabled = false;
                //    currentClient.Update();
                //}

// ReSharper disable once UnusedVariable
                EventStat myStat = new EventStat(ObjectId.Parse(dlClients.SelectedValue), dlClients.SelectedItem.Text, es.Exceptions, 1);
            }
            catch(Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.Message;
            }
        }

    #endregion

    #region Update EventType lists

// ReSharper disable once UnusedMember.Local
    private void UpdateEventTypeLists()
    {
        // Update EventStat collection
        var eventCollection = mongoDatabase.GetCollection("Event");
        var eventTypeList = new List<string>();

        var eventTypeResult = eventCollection.Distinct("EventTypeId");
        eventTypeList.AddRange(from et in eventTypeResult where et.ToString() != "BsonNull" && et.ToString() != "" select et.ToString());

        eventTypeList.Sort();

// ReSharper disable once LocalVariableHidesMember
        var sbResponse = new StringBuilder();
        sbResponse.Append("<eventtypes>");
        foreach (var eventType in eventTypeList)
        {
            sbResponse.Append("<eventtype>");
            sbResponse.Append(eventType);
            sbResponse.Append("</eventtype>");
        }
        sbResponse.Append("</eventtypes>");
    }

    #endregion

    public void GetClientList()
    {
        dlClients.Items.Clear();

        var clientList = new MacList("", "Client", "", "_id,Name");
        foreach (var li in clientList.ListItems.Select(item => new ListItem { Text = item.Attributes["Name"].Replace("&quot;", "\"").Trim(), Value = item.Attributes["_id"].Trim() }))
        {
            if (li.Text != "")
            {
                dlClients.Items.Add(li);
            }
        }

        var li0 = new ListItem { Text = @"Select a Client (" + (dlClients.Items.Count) + @")", Value = Constants.Strings.DefaultEmptyObjectId };
        dlClients.Items.Insert(0, li0);
    }
}