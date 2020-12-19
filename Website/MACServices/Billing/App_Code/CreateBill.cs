using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using MongoDB.Bson;
using MongoDB.Driver.Builders;

using MACServices;
using MACBilling;

using dk = MACServices.Constants.Dictionary.Keys;

/// <summary>
/// Summary description for CreateBill
/// </summary>
[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
 
[System.Web.Script.Services.ScriptService]
public class CreateBill : WebService {

    public int dateToRun = Convert.ToInt16(ConfigurationManager.AppSettings["BillRunDate"]);

    public int currentDate = DateTime.UtcNow.Day;
    public int currentMonth = DateTime.UtcNow.Month;
    public int currentYear = DateTime.UtcNow.Year;

    public int daysInMonth = 0;

    public bool encryptBillDetails = true;

    public Utils myUtils = new Utils();
    public BillUtils myBillUtils = new BillUtils();

    public StringBuilder sbResponse = new StringBuilder();

    public int i = 0;

    public decimal billTotal = 0.00M;
    public decimal clientTotal = 0.00M;
    public decimal groupTotal = 0.00M;

    public string clientId = "";
    public string groupId = "";

    public int numberBillClients = 0;

    public string emptyId = Constants.Strings.DefaultEmptyObjectId;

    public string archiveId = Constants.Strings.DefaultEmptyObjectId;

    public bool saveToArchive = false;

    public bool isTimeToRun = false;

    public bool isUsingDefaultEmail = false;

    public CreateBill () {}

    [WebMethod]
    public XmlDocument WsCreateBill(string SaveToArchive)
    {
        daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);

        if (SaveToArchive != "")
            saveToArchive = Convert.ToBoolean(SaveToArchive);

        myUtils.InitializeXmlResponse(sbResponse);

        try
        {
            sbResponse.Append("<billcollection savetoarchive='" + saveToArchive + "' datetorun='" + dateToRun + "' total=''>");

            isTimeToRun = IsDateToRun();

            // Testing. Don't forget to comment out when finished
            if (!saveToArchive)
                isTimeToRun = true;

            if (isTimeToRun)
            {
                sbResponse.Append("<result status='Executed " + DateTime.UtcNow + "' />");

                // Initialize collection structures
                List<Tuple<string, ObjectId>> myClientsList = GetClientList();
                List<Tuple<string, ObjectId>> myGroupsList = GetGroupList();

                // Process groups first
                ProcessGroupBills(myGroupsList);

                // Now process clients
                ProcessClientBills(myClientsList);
            }
            else
            {
                sbResponse.Append("<result status='Not time to run' />");
            }
        }
        catch(Exception ex)
        {
            var errMsg = ex.ToString();
        }

        // Update header
        sbResponse.Replace("<billcollection savetoarchive='" + saveToArchive + "' datetorun='" + dateToRun + "' total=''>", "<billcollection savetoarchive='" + saveToArchive + "' datetorun='" + dateToRun + "' total='" + myBillUtils.FormatMoney(billTotal) + "'>");

        sbResponse.Append("</billcollection>");

        var rsp = myUtils.FinalizeXmlResponse(sbResponse, "CB");

        // Convert to html using xslt and send output to accounting@mobileauthcorp.com
        var MessageSubject = "MAC Billing Summary";
        var MessageBody = sbResponse.ToString();

        myUtils.SendGenericEmail(Constants.Strings.DefaultClientId, "Client", Constants.Strings.DefaultFromEmail, Constants.Strings.DefaultAccountingEmail, MessageSubject, MessageBody, true);

        return rsp;
    }

    public bool IsDateToRun()
    {
        if (currentDate == dateToRun)
            return true;
        return false;
    }

    public bool BillAlreadyProcessed(ObjectId ownerId, string ownerType)
    {
        var billHasBeenProcessed = false;

        var billQuery = Query.And(Query.EQ("OwnerId", ownerId), Query.EQ("_t", "BillArchive"));
        var sortBy = SortBy.Descending("DateCreated");

        var billHistory = myUtils.mongoDBConnectionPool.GetCollection("Archive").FindAs<BillArchive>(billQuery).SetSortOrder(sortBy);
        foreach (BillArchive billArchive in billHistory)
        {
            billHasBeenProcessed = true;
        }

        return billHasBeenProcessed;
    }

    public List<Tuple<string, ObjectId>> GetClientList()
    {
        List<Tuple<string, ObjectId>> myClientsList = new List<Tuple<string, ObjectId>>();

        var clientList = new MacList("", "Client", "", "_id,Name");
        foreach (var li in clientList.ListItems.Select(item => new ListItem { Text = item.Attributes["Name"].Replace("&quot;", "\"").Trim(), Value = item.Attributes["_id"].Trim() }))
        {
            Tuple<string, ObjectId> currentClient = new Tuple<string, ObjectId>(li.Text.Replace("\t", ""), ObjectId.Parse(li.Value));
            myClientsList.Add(currentClient);
        }

        return myClientsList;
    }

    public List<Tuple<string, ObjectId>> GetGroupList()
    {
        List<Tuple<string, ObjectId>> myGroupsList = new List<Tuple<string, ObjectId>>();

        var groupList = new MacList("", "Group", "", "_id,Name");
        foreach (var li in groupList.ListItems.Select(item => new ListItem { Text = item.Attributes["Name"].Replace("&quot;", "\"").Trim(), Value = item.Attributes["_id"].Trim() }))
        {
            Tuple<string, ObjectId> currentGroup = new Tuple<string, ObjectId>(li.Text.Replace("\t", ""), ObjectId.Parse(li.Value));
            myGroupsList.Add(currentGroup);
        }
        return myGroupsList;
    }

    public void ProcessClientBills(List<Tuple<string, ObjectId>>  myClientsList)
    {
        // Reset counter
        i = 0;

        sbResponse.Append("<bills type='Client' count='0' total='0'>");

        foreach (Tuple<string, ObjectId> currentClient in myClientsList)
        {
            var clientName = currentClient.Item1.Replace("'", "").Replace("&", "");
            var clientId = currentClient.Item2;

            BillClient myClientBill = new BillClient("Current (Pending) Charges", clientId.ToString(), emptyId);
            BillConfig myClientBillConfig = new BillConfig(clientId.ToString(), "Client", emptyId);

            var currentBillAlreadyProcessed = BillAlreadyProcessed(clientId, "Client");
            if (!currentBillAlreadyProcessed)
            {

                if (!myClientBillConfig.IncludeInGroupBill)
                {
                    if (saveToArchive)
                        archiveId = SaveClientBillArchive(myClientBill);

                    billTotal += myClientBill.Total;
                    clientTotal += myClientBill.Total;

                    sbResponse.Append("<bill status='Processed' archiveid='" + archiveId + "' clientname='" + clientName + "' clientid='" + clientId + "' billid='" + myClientBill._id + "' billtotal='" + myBillUtils.FormatMoney(myClientBill.Total) + "' />");

                    i++;
                }
            }
            else
            {
                sbResponse.Append("<bill status='Already Processed' archiveid='archiveId' clientname='" + clientName + "' clientid='" + clientId + "' />");
            }
        }

        // Finalize counts
        sbResponse.Replace("<bills type='Client' count='0' total='0'>", "<bills type='Client' count='" + i + "' total='" + myBillUtils.FormatMoney(clientTotal) + "'>");

        sbResponse.Append("</bills>");
    }

    public void ProcessGroupBills(List<Tuple<string, ObjectId>> myGroupsList)
    {
        // Reset counter
        i = 0;

        int billClientsCount;

        sbResponse.Append("<bills type='Group' count='0' total='0'>");

        foreach (Tuple<string, ObjectId> currentGroup in myGroupsList)
        {
            billClientsCount = 0;

            var groupName = currentGroup.Item1.Replace("'", "").Replace("&", "");
            var groupId = currentGroup.Item2;

            BillGroup myGroupBill = new BillGroup("Current (Pending) Charges", groupId.ToString(), emptyId);

            var currentBillAlreadyProcessed = BillAlreadyProcessed(groupId, "Group");
            if (!currentBillAlreadyProcessed)
            {
                if (saveToArchive)
                    archiveId = SaveGroupBillArchive(myGroupBill);

                billTotal += myGroupBill.Total;
                groupTotal += myGroupBill.Total;

                // Mark each client as processed in myClientsList 
                foreach (BillClient currentClientBill in myGroupBill.ClientBills)
                {
                    BillConfig currentClientConfig = new BillConfig(currentClientBill.OwnerId.ToString(), "Client", emptyId);

                    if (currentClientConfig.IncludeInGroupBill && currentClientConfig.BillToGroupId == groupId)
                        billClientsCount++;
                }

                sbResponse.Append("<bill status='Processed' archiveid='" + archiveId + "' groupname='" + groupName + "' groupid='" + groupId + "' members='" + myGroupBill.ClientBills.Count + "' membersincluded='" + billClientsCount + "' billid='" + myGroupBill._id + "' billtotal='" + myBillUtils.FormatMoney(myGroupBill.Total) + "' />");

                i++;
            }
            else
            {
                sbResponse.Append("<bill status='Already Processed' archiveid='archiveId' groupname='" + groupName + "' groupid='" + groupId + "' />");
            }
        }

        sbResponse.Replace("<bills type='Group' count='0' total='0'>", "<bills type='Group' count='" + i + "' total='" + myBillUtils.FormatMoney(groupTotal) + "'>");

        sbResponse.Append("</bills>");
    }

    public string SaveClientBillArchive(BillClient myBill)
    {
        try
        {
            clientId = myBill.OwnerId.ToString();

            Client myClient = new Client(clientId);

            //myBill._id = myBill._id;
            myBill.ClientId = ObjectId.Parse(clientId);

            foreach (var currAddendum in myBill.Addendums)
            {
                if (currAddendum.IsMinimumPriceAdjustment)
                {
                    // Do nothing. This is a non-persisted minimum price adjustment addendum
                }
                else
                {
                    // Update status of addendum
                    currAddendum._id = ObjectId.Parse(currAddendum.AddendumId);
                    currAddendum.AttachedToBillId = myBill._id;
                    currAddendum.DateAttached = DateTime.UtcNow;
                    currAddendum.HasBeenAttached = true;
                    currAddendum.CreatedById = ObjectId.Parse(emptyId);

                    // Update the addendum in the db
                    currAddendum.UpdateAddendum(currAddendum, currAddendum._id.ToString());
                }
            }

            var myArchive = new BillArchive();

            myArchive.ForMonthYear = myBill.DateStart.ToString("MM/yyyy");

            //myArchive.OwnerId = myBill.ClientId;
            myArchive.OwnerId = myBill.ClientId.ToString();

            var myJsonBill = (new JavaScriptSerializer()).Serialize(myBill);

            myArchive._id = myBill._id;

            //if (encryptBillDetails)
            //    myArchive.BillDetails = MACSecurity.Security.EncryptAndEncode(myJsonBill, myBill.ClientId.ToString());
            //else
                myArchive.BillDetails = myJsonBill;

            myArchive.Amount = myBill.Total;

            myArchive.CreatedById = ObjectId.Parse(emptyId);

            myArchive.DateCreated = DateTime.UtcNow;
            myArchive.DateDue = myBill.DateDue;

            //myArchive.OwnerId = myClient._id;
            myArchive.OwnerId = myClient._id.ToString();
            myArchive.OwnerName = myClient.Name.Trim();
            myArchive.OwnerType = "Client";

            myArchive.Archive(myArchive);

            archiveId = myArchive._id.ToString();

            var adminFirstName = "Billing";
            var adminLastName = "Service";

            // Log the billing event
            var myBillEvent = new Event();
            myBillEvent.EventTypeDesc = Constants.TokenKeys.BillAmount + "<a href='/Admin/Billing/Group/Default.aspx?bid=" + myBill._id + "&pa=ViewBillDetails'>" + myBillUtils.FormatMoney(Convert.ToDecimal(myBill.Total.ToString())) + "</a>";
            myBillEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myArchive.OwnerName.Trim();
            myBillEvent.EventTypeDesc += Constants.TokenKeys.EventGeneratedByName + (adminFirstName + " " + adminLastName);

            myBillEvent.EventTypeName = Constants.EventLog.Billing.Created.Item2;
            myBillEvent.EventTypeId = Constants.EventLog.Billing.Created.Item1;
            //myBillEvent.ClientId = myArchive.OwnerId;
            myBillEvent.ClientId = ObjectId.Parse(myArchive.OwnerId);

            myBillEvent.Create(Constants.EventLog.Billing.Created, null);

            SendBillNotification(archiveId, myArchive.OwnerId.ToString(), "Client");
        }
        catch (Exception ex)
        {
            var errMsg = ex.ToString();
            return archiveId;
        }

        return archiveId;
    }

    public string SaveGroupBillArchive(BillGroup myBill)
    {
        try
        {
            groupId = myBill.OwnerId.ToString();

            Group myGroup = new Group(groupId);

            //myBill._id = myBill._id;
            //myBill.GroupId = ObjectId.Parse(groupId);
            myBill.GroupId = groupId;

            foreach (var currAddendum in myBill.Addendums)
            {
                if (currAddendum.IsMinimumPriceAdjustment)
                {
                    // Do nothing. This is a non-persisted minimum price adjustment addendum
                }
                else
                {
                    // Update status of addendum
                    currAddendum._id = ObjectId.Parse(currAddendum.AddendumId);
                    currAddendum.AttachedToBillId = myBill._id;
                    currAddendum.DateAttached = DateTime.UtcNow;
                    currAddendum.HasBeenAttached = true;
                    currAddendum.CreatedById = ObjectId.Parse(emptyId);

                    // Update the addendum in the db
                    currAddendum.UpdateAddendum(currAddendum, currAddendum._id.ToString());
                }
            }

            var myArchive = new BillArchive();

            myArchive.ForMonthYear = myBill.DateStart.ToString("MM/yyyy");

            myArchive.OwnerId = myBill.GroupId;

            var myJsonBill = (new JavaScriptSerializer()).Serialize(myBill);

            myArchive._id = myBill._id;

            //if (encryptBillDetails)
            //    myArchive.BillDetails = MACSecurity.Security.EncryptAndEncode(myJsonBill, myBill.GroupId.ToString());
            //else
                myArchive.BillDetails = myJsonBill;

            myArchive.Amount = myBill.Total;

            myArchive.CreatedById = ObjectId.Parse(emptyId);

            myArchive.DateCreated = DateTime.UtcNow;
            myArchive.DateDue = myBill.DateDue;

            //myArchive.OwnerId = myGroup._id;
            myArchive.OwnerId = myGroup._id.ToString();
            myArchive.OwnerName = myGroup.Name.Trim();
            myArchive.OwnerType = "Group";

            myArchive.Archive(myArchive);

            archiveId = myArchive._id.ToString();

            var adminFirstName = "Billing";
            var adminLastName = "Service";

            // Log the billing event
            var myBillEvent = new Event();
            myBillEvent.EventTypeDesc = Constants.TokenKeys.BillAmount + "<a href='/Admin/Billing/Group/Default.aspx?bid=" + myBill._id + "&pa=ViewBillDetails'>" + myBillUtils.FormatMoney(Convert.ToDecimal(myBill.Total.ToString())) + "</a>";
            myBillEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myArchive.OwnerName.Trim();
            myBillEvent.EventTypeDesc += Constants.TokenKeys.EventGeneratedByName + (adminFirstName + " " + adminLastName);

            myBillEvent.EventTypeName = Constants.EventLog.Billing.Created.Item2;
            myBillEvent.EventTypeId = Constants.EventLog.Billing.Created.Item1;
            //myBillEvent.ClientId = myArchive.OwnerId;
            myBillEvent.ClientId = ObjectId.Parse(myArchive.OwnerId);

            myBillEvent.Create(Constants.EventLog.Billing.Created, null);

            SendBillNotification(archiveId, myArchive.OwnerId.ToString(), "Group");
        }
        catch(Exception ex)
        {
            var errMsg = ex.ToString();
            return archiveId;
        }

        return archiveId;
    }

    public void SendBillNotification(string archiveId, string ownerId, string ownerType)
    {
        isUsingDefaultEmail = false;

        var currentServer = "http://" + HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

        string MessageSubject;
        string MessageBody;

        var myBillConfig = new BillConfig(ownerId, ownerType, emptyId);

        List<string> notifyUsers = new List<string>();

        if (!string.IsNullOrEmpty(myBillConfig.NotifyUserIds))
        {
            if (myBillConfig.NotifyUserIds.Contains("|"))
            {
                var arrUserIds = myBillConfig.NotifyUserIds.Split('|');
                foreach (string currentUserId in arrUserIds)
                {
                    if (currentUserId != "")
                    {
                        UserProfile myProfile = new UserProfile(currentUserId);
                        var userEmail = MACSecurity.Security.DecodeAndDecrypt(myProfile.Contact.Email, currentUserId);
                        notifyUsers.Add(userEmail);
                    }
                }
            }
            else
            {
                notifyUsers.Add(Constants.Strings.DefaultAccountingEmail);
            }
        }
        else
        {
            notifyUsers.Add(Constants.Strings.DefaultAccountingEmail);
            isUsingDefaultEmail = true;
        }

        // Send bill now
        foreach (var currEmailAddress in notifyUsers)
        {
            MessageSubject = "";
            MessageBody = "";

            if (isUsingDefaultEmail)
            {
                MessageSubject += "(Bill recipients not defined) - ";
                MessageBody += "<div>(Bill recipients not defined)</div>";
            }

            MessageSubject += myBillConfig.OwnerName.Trim() + ", your current MAC bill is ready for review";

            MessageBody = "<div>Please click the link below to log into your account and view the current bill for (" + myBillConfig.OwnerName.Trim() + ").</div>";
            MessageBody += "<div><a href='" + currentServer + "/Admin/Security/Default.aspx?uname=" + currEmailAddress.Trim() + "&bid=" + archiveId + "&pa=ViewBillDetails'>Log In</a></div>";

            myUtils.SendGenericEmail(ownerId, ownerType, Constants.Strings.DefaultFromEmail, currEmailAddress.Trim(), MessageSubject, MessageBody, true);
        }
    }
}
