using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

namespace System
{
    public partial class UploadFile : Page
    {
        Utils myUtils = new Utils();

        string adminUserId = "";
        string adminFirstName = "";
        string adminLastName = "";

        UserProfile adminProfile; 

        string ownerId = "";
        string ownerType = "";

        protected void Page_Load(object sender, EventArgs e)
        {

            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                // Boot this guy out. Not authenticated!
                divUpload.Visible = false;
                Response.Write("Not authorized");
                Response.End();
            }
            else
            {
                if (Request["userId"] != null)
                {
                    adminUserId = Request["userId"].ToString();

                    adminUserId = MACSecurity.Security.DecodeAndDecrypt(adminUserId, Constants.Strings.DefaultClientId);

                    adminProfile = new UserProfile(adminUserId);

                    adminFirstName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.FirstName, adminUserId);
                    adminLastName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.LastName, adminUserId);
                }

                if (Request["ownerId"] != null)
                    ownerId = Request["ownerId"].ToString();

                hiddenOwnerId.Value = ownerId;

                if (Request["ownerType"] != null)
                    ownerType = Request["ownerType"].ToString();

                hiddenOwnerType.Value = ownerType;

                if (IsPostBack) // Run the upload and update process
                {
                    try
                    {
                        if (IsPostBack)
                        {
                            //check to make sure a file is selected
                            if (FileUpload1.HasFile)
                            {
                                //create the path to save the file to
                                var tmpVal = FileUpload1.FileName.Split('.');

                                var pathName = "/Images/OwnerLogos/";
                                var fileName = ownerType + "-" + ownerId + "." + tmpVal[1];

                                string fullFileName = Path.Combine(Server.MapPath("~" + pathName), fileName);

                                FileUpload1.SaveAs(fullFileName);

                                switch (ownerType)
                                {
                                    case "Client":
                                        Client myClient = new Client(ownerId);
                                        myClient.OwnerLogoUrl = pathName + fileName;
                                        myClient.Update();
                                        break;

                                    case "Group":
                                        Group myGroup = new Group(ownerId);
                                        myGroup.OwnerLogoUrl = pathName + fileName;
                                        myGroup.Update();
                                        break;
                                }
                            }

                            ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
                        }
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception ex)
                    {
                        var errMsg = ex.ToString();
                    }
                }
                else // 
                {

                }
            }  
        }
    }
}