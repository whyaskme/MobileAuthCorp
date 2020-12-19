using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.UI;

using MACBilling;
using MACSecurity;
using MACServices;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using cfg = MACServices.Constants.WebConfig;

namespace Reset
{
    public partial class IPs : Page
    {
        Utils mUtils = new Utils();

        public UserProfile adminProfile;

        public string adminUserId = "";
        public string adminFirstName = "";
        public string adminLastName = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            if (Request["userId"] != null)
            {
                adminUserId = Request["userId"].ToString();

                adminUserId = Security.DecodeAndDecrypt(adminUserId, Constants.Strings.DefaultClientId);

                adminProfile = new UserProfile(adminUserId);

                adminFirstName = Security.DecodeAndDecrypt(adminProfile.FirstName, adminUserId);
                adminLastName = Security.DecodeAndDecrypt(adminProfile.LastName, adminUserId);
            }

            if (IsPostBack) // Run the process
            {
                try
                {
                    MongoCollection mongoCollection = mUtils.mongoDBConnectionPool.GetCollection("Client");
                    var query = Query.EQ("_t", "Client");

                    var clientList = mongoCollection.FindAs<Client>(query);
                    foreach (Client currentClient in clientList)
                    {
                        currentClient.AllowedIpList = txtIPAddresses.Text;
                        currentClient.Update();

                        // Log the changes
                        var ipEvent = new Event
                        {
                            ClientId = currentClient._id,
                            UserId = ObjectId.Parse(adminUserId),
                            EventTypeDesc = Constants.TokenKeys.OwnerType + "Client"
                                            + Constants.TokenKeys.OwnerName + currentClient.Name
                                            + Constants.TokenKeys.AllowedIPAddresses + txtIPAddresses.Text.Replace("|", ", ")
                                            + Constants.TokenKeys.EventGeneratedByName + adminFirstName + " " + adminLastName
                        };
                        ipEvent.Create(Constants.EventLog.System.AllowedIPsUpdated, null);
                    }

                    mongoCollection = mUtils.mongoDBConnectionPool.GetCollection("Billing");
                    query = Query.And(Query.EQ("OwnerType", "Group"), Query.EQ("OwnerType", "Group"));

                    var groupConfigList = mongoCollection.FindAs<BillConfig>(query);
                    foreach (BillConfig currentConfig in groupConfigList)
                    {
                        currentConfig.AllowedIpList = txtIPAddresses.Text;
                        currentConfig.UpdateConfig(currentConfig, currentConfig.OwnerId.ToString());

                        // Log the changes
                        var ipEvent = new Event
                        {
                            ClientId = currentConfig.OwnerId,
                            UserId = ObjectId.Parse(adminUserId),
                            EventTypeDesc = Constants.TokenKeys.OwnerType + "Group"
                                            + Constants.TokenKeys.OwnerName + currentConfig.OwnerName
                                            + Constants.TokenKeys.AllowedIPAddresses + txtIPAddresses.Text.Replace("|", ", ")
                                            + Constants.TokenKeys.EventGeneratedByName + adminFirstName + " " + adminLastName
                        };
                        ipEvent.Create(Constants.EventLog.System.AllowedIPsUpdated, null);
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch(Exception ex)
                {
                    var errMsg = ex.ToString();
                }

                ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
            }
            else // 
            {

            }
        }
    }
}