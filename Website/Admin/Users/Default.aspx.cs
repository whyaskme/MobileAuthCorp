using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using MACSecurity;
using MACServices;

using MongoDB.Bson;
using MongoDB.Web.Providers;

namespace Admin.Users
{
    public partial class Default : System.Web.UI.Page
    {
        public string loggedInAdminUserName = "";
        public string loggedInAdminFirstName = "";
        public string loggedInAdminLastName = "";
        public string loggedInAdminId = "";
        public string loggedInClientId = "";

        public string RegistrationResults;
        public string NewUserId;
        public bool BDebug = false;

        public String Tokens = "";

        public Event Event = new Event();

        public HtmlForm MyForm;

        public HiddenField hiddenF;
        public HiddenField hiddenD;
        public HiddenField hiddenE;
        public HiddenField hiddenG;
        public HiddenField hiddenH;
        public HiddenField hiddenI;
        public HiddenField hiddenJ;
        public HiddenField hiddenK;
        public HiddenField hiddenL;

        public String decryptedhiddenE = "";
        public String decryptedhiddenD = "";

        public String cssClassName = "alert-box success radius";

        HiddenField _hiddenW;

        public Utils myUtils = new Utils();

        protected void Page_Load(object sender, EventArgs e)
        {
            var IsRoleChanged = false;

            txtFirstName.Focus();

            if (Page.Master != null)
            {
                if (Master != null)
                {
                    MyForm = (HtmlForm)Page.Master.FindControl("formMain");

                    hiddenF = (HiddenField)Page.Master.FindControl("hiddenF");
                    hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");
                    hiddenE = (HiddenField)Page.Master.FindControl("hiddenE");
                    hiddenG = (HiddenField)Page.Master.FindControl("hiddenG");
                    hiddenH = (HiddenField)Page.Master.FindControl("hiddenH");
                    hiddenI = (HiddenField)Page.Master.FindControl("hiddenI");
                    hiddenJ = (HiddenField)Page.Master.FindControl("hiddenJ");
                    hiddenK = (HiddenField)Page.Master.FindControl("hiddenK");
                    hiddenL = (HiddenField)Page.Master.FindControl("hiddenL");

                    _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                    _hiddenW.Value = "5490bfb6ead63627d88e3e21";

                    var bodyContainer = (Panel)Master.FindControl("BodyContainer");
                    bodyContainer.Height = 561;
                }
            }

            loggedInAdminId = Security.DecodeAndDecrypt(hiddenE.Value, Constants.Strings.DefaultClientId);
            loggedInAdminUserName = Security.DecodeAndDecrypt(hiddenG.Value, loggedInAdminId);
            loggedInClientId = Security.DecodeAndDecrypt(hiddenD.Value, loggedInAdminId);

            loggedInAdminFirstName = Security.DecodeAndDecrypt(hiddenH.Value, loggedInAdminId);
            loggedInAdminLastName = Security.DecodeAndDecrypt(hiddenI.Value, loggedInAdminId);

            if (loggedInAdminId != "")
                Event.UserId = ObjectId.Parse(loggedInAdminId);

            if (loggedInClientId != "")
                Event.ClientId = ObjectId.Parse(loggedInClientId);

            btnSelectedGroups.Value = "0 Groups Assigned";

            userUpdateMessage.InnerHtml = "";
            userUpdateMessage.Visible = false;

            txtUsername.Disabled = true;
            spanUsername.InnerHtml = "Username (Read Only)";

            if(IsPostBack)
            {
                // Get the selected user
                switch (hiddenAA.Value)
                {
                    case "CreateUser":
                        var newUserProfile = new UserProfile("");

                        // Use email for the userName 
                        var userName = txtUsername.Value;
                        var password = txtPassword.Value;
                        var email = txtEmail.Value;
                        var securityQuestion = "Fave company";
                        var securityAnswer = "MAC";

                        MembershipCreateStatus createStatus;
                        var newUser = Membership.CreateUser(userName, password, email, securityQuestion, securityAnswer, true, out createStatus);

                        switch (createStatus)
                        {
                            case MembershipCreateStatus.Success:
                                if (newUser != null)
                                {
                                    if (newUser.ProviderUserKey != null)
                                        NewUserId = newUser.ProviderUserKey.ToString();

                                    newUser.IsApproved = chkIsApproved.Checked;
                                }

                                // Add the user's profile
                                newUserProfile = new UserProfile("")
                                {
                                    _id = ObjectId.Parse(NewUserId),
                                    UserId = ObjectId.Parse(NewUserId),
                                    Name = txtFirstName.Value + " " + txtLastName.Value,
                                    Prefix = Security.EncryptAndEncode("", NewUserId),
                                    FirstName = Security.EncryptAndEncode(txtFirstName.Value, NewUserId),
                                    MiddleName = Security.EncryptAndEncode("", NewUserId),
                                    LastName = Security.EncryptAndEncode(txtLastName.Value, NewUserId),
                                    Suffix = Security.EncryptAndEncode("", NewUserId),
                                    DateOfBirth = Security.EncryptAndEncode("1/1/1900", NewUserId)
                                };

                                var roleId = ObjectId.Parse(dlRolesAssigned.SelectedItem.Value);
                                newUserProfile.Roles.Add(roleId);

                                newUserProfile.Contact.Email = Security.EncryptAndEncode(txtEmail.Value, NewUserId);
                                newUserProfile.Contact.HomePhone = Security.EncryptAndEncode("", NewUserId);
                                newUserProfile.Contact.MobilePhone = Security.EncryptAndEncode(txtPhone.Value, NewUserId);
                                newUserProfile.Contact.WorkPhone = Security.EncryptAndEncode("", NewUserId);
                                newUserProfile.Contact.WorkExtension = Security.EncryptAndEncode("", NewUserId);

                                newUserProfile.Address.Street1 = Security.EncryptAndEncode("", NewUserId);
                                newUserProfile.Address.Street2 = Security.EncryptAndEncode("", NewUserId);
                                newUserProfile.Address.Unit = Security.EncryptAndEncode("", NewUserId);
                                newUserProfile.Address.City = Security.EncryptAndEncode("", NewUserId);
                                newUserProfile.Address.State = Security.EncryptAndEncode("", NewUserId);
                                newUserProfile.Address.Zipcode = Security.EncryptAndEncode("", NewUserId);

                                newUserProfile.IsReadOnly = Convert.ToBoolean(chkUserReadOnly.Checked);

                                newUserProfile.Create();

                                dlUserRoles.SelectedIndex = dlRolesAssigned.SelectedIndex;

                                GetUsersList();

                                // Select the new user
                                dlUsers.SelectedValue = NewUserId;
                                hiddenSelectedUserId.Value = NewUserId;

                                Tokens += Constants.TokenKeys.UserRole + dlUserRoles.SelectedItem.Text;
                                Tokens += Constants.TokenKeys.UserFullName + newUserProfile.Name;
                                Tokens += Constants.TokenKeys.UpdatedByLoggedinAdminFullName + loggedInAdminFirstName + " " + loggedInAdminLastName;

                                Event.Create(Constants.EventLog.Registration.AdminUser.Updated, Tokens);
                                break;
                            case MembershipCreateStatus.DuplicateUserName:
                                Tokens += Constants.TokenKeys.DuplicateUserName + userName;
                                Event.Create(Constants.EventLog.Registration.AdminUser.DuplicateUserName, Tokens);

                                userUpdateMessage.Visible = true;
                                userUpdateMessage.InnerHtml = "Duplicate username. Please try another.";
                                cssClassName = "alert-box warning radius";
                                break;
                            case MembershipCreateStatus.DuplicateEmail:
                                Tokens += Constants.TokenKeys.DuplicateEmail + userName;
                                Event.Create(Constants.EventLog.Registration.AdminUser.DuplicateEmail, Tokens);

                                userUpdateMessage.Visible = true;
                                userUpdateMessage.InnerHtml = "Duplicate email. Please try another.";
                                cssClassName = "alert-box warning radius";
                                break;
                            case MembershipCreateStatus.InvalidEmail:
                                Tokens += Constants.TokenKeys.InvalidEmail + userName;
                                Event.Create(Constants.EventLog.Registration.AdminUser.InvalidEmail, Tokens);

                                userUpdateMessage.Visible = true;
                                userUpdateMessage.InnerHtml = "Invalid email address. Please try another.";
                                cssClassName = "alert-box warning radius";
                                break;
                            case MembershipCreateStatus.InvalidAnswer:
                                Tokens += Constants.TokenKeys.InvalidSecurityAnswer + userName;
                                Event.Create(Constants.EventLog.Registration.AdminUser.InvalidSecurityAnswer, Tokens);

                                userUpdateMessage.Visible = true;
                                userUpdateMessage.InnerHtml = "Invalid security answer. Please try another.";
                                cssClassName = "alert-box warning radius";
                                break;
                            case MembershipCreateStatus.InvalidPassword:
                                Tokens += Constants.TokenKeys.InvalidPassword + userName;
                                Event.Create(Constants.EventLog.Registration.AdminUser.InvalidPassword, Tokens);

                                userUpdateMessage.Visible = true;
                                userUpdateMessage.InnerHtml = "Invalid password. Please try another.";
                                cssClassName = "alert-box warning radius";
                                break;
                            default:
                                Tokens += Constants.TokenKeys.DuplicateUserName + userName;
                                Event.Create(Constants.EventLog.Registration.AdminUser.UnknownRegistrationError, Tokens);

                                userUpdateMessage.Visible = true;
                                userUpdateMessage.InnerHtml = "Duplicate username. Please try another.";
                                cssClassName = "alert-box warning radius";
                                break;
                        }

                        userUpdateMessage.InnerHtml = Event.EventTypeDesc;
                        userUpdateMessage.Attributes.Add("class", cssClassName);
                        
                        GetUsersList();

                        if (NewUserId != null)
                        {
                            dlUsers.SelectedValue = NewUserId;

                            GetUserInfo();

                            hiddenAA.Value = "";
                        }
                        break;

                    case "UpdateUser":
                        var currentUserProfile = new UserProfile(dlUsers.SelectedValue);

                        currentUserProfile.IsReadOnly = Convert.ToBoolean(chkUserReadOnly.Checked);

                        var userHasBeenDisabled = false;
                        var userHasBeenReEnabled = false;

                        // Use email for the userName 
                        userName = txtUsername.Value;
                        password = txtPassword.Value;
                        email = txtEmail.Value;
                        securityQuestion = "Fave company";
                        securityAnswer = "MAC";

                        Event.ClientId = ObjectId.Parse(loggedInClientId);

                        // Get the selected User and profile
                        var currentUser = Membership.GetUser(ObjectId.Parse(dlUsers.SelectedValue), true);

                        if (!chkIsLockedOut.Checked)
                            if (currentUser != null) currentUser.UnlockUser();

                        if (currentUser != null)
                        {
                            if (!chkIsApproved.Checked && currentUser.IsApproved)
                                userHasBeenDisabled = true;

                            if (chkIsApproved.Checked && !currentUser.IsApproved)
                                userHasBeenReEnabled = true;

                            currentUser.IsApproved = chkIsApproved.Checked;
                            currentUser.Comment = "Why does the user have to have a comment to update? Dumb, dumb and dumb!";

                            var membershipProvider = new MongoDbMembershipProvider();
                            membershipProvider.UpdateUser(currentUser);

                            var pwd = currentUser.GetPassword();

                            var roleId = ObjectId.Parse(dlRolesAssigned.SelectedValue);
                            currentUserProfile.Roles.Clear();
                            currentUserProfile.Roles.Add(roleId);

                            // Update user registration
                            currentUser.Email = txtEmail.Value;

                            if (!String.IsNullOrEmpty(txtPassword.Value))
                            {
                                currentUser.ChangePassword(pwd, txtPassword.Value);
                                currentUser.ChangePasswordQuestionAndAnswer(txtPassword.Value, securityQuestion, securityAnswer);
                            }

                            // Update user profile
                            currentUserProfile.Name = txtFirstName.Value + " " + txtLastName.Value;
                            currentUserProfile.FirstName = Security.EncryptAndEncode(txtFirstName.Value, dlUsers.SelectedValue);
                            currentUserProfile.LastName = Security.EncryptAndEncode(txtLastName.Value, dlUsers.SelectedValue);

                            currentUserProfile.Contact.Email = Security.EncryptAndEncode(txtEmail.Value, dlUsers.SelectedValue);
                            currentUserProfile.Contact.HomePhone = Security.EncryptAndEncode("", dlUsers.SelectedValue);
                            currentUserProfile.Contact.MobilePhone = Security.EncryptAndEncode(txtPhone.Value, dlUsers.SelectedValue);
                            currentUserProfile.Contact.WorkPhone = Security.EncryptAndEncode("", dlUsers.SelectedValue);
                            currentUserProfile.Contact.WorkExtension = Security.EncryptAndEncode("", dlUsers.SelectedValue);

                            currentUserProfile.Address.Street1 = Security.EncryptAndEncode("", dlUsers.SelectedValue);
                            currentUserProfile.Address.Street2 = Security.EncryptAndEncode("", dlUsers.SelectedValue);
                            currentUserProfile.Address.Unit = Security.EncryptAndEncode("", dlUsers.SelectedValue);
                            currentUserProfile.Address.City = Security.EncryptAndEncode("", dlUsers.SelectedValue);
                            currentUserProfile.Address.State = Security.EncryptAndEncode("", dlUsers.SelectedValue);
                            currentUserProfile.Address.Zipcode = Security.EncryptAndEncode("", dlUsers.SelectedValue);

                            currentUserProfile.Update();
                        }

                        var oldUserProfile = new UserProfile(dlUsers.SelectedValue);

                        //var myUtils = new Utils();
                        var updatedValues = myUtils.GetObjectDifferences(oldUserProfile, currentUserProfile);

                        if (dlRolesAssigned.SelectedIndex > dlUserRoles.SelectedIndex)
                        { // Demotion
                            IsRoleChanged = true;
                            Event.EventTypeDesc += Constants.TokenKeys.PreviousRole + "(" + dlUserRoles.SelectedItem.Text.Replace("User ", "") + ")";
                            Event.EventTypeDesc += Constants.TokenKeys.NewRole + "(" + dlRolesAssigned.Text.Replace("User ", "") + ")";
                            Event.EventTypeDesc += Constants.TokenKeys.UserFullName + dlUsers.SelectedItem.Text;
                            Event.EventTypeDesc += Constants.TokenKeys.UpdatedByLoggedinAdminFullName + loggedInAdminFirstName + " " + loggedInAdminLastName;

                            Event.Create(Constants.EventLog.Assignments.RoleAssignment.Demoted, null);
                        }
                        else if (dlRolesAssigned.SelectedIndex < dlUserRoles.SelectedIndex)
                        { // Promotion
                            IsRoleChanged = true;
                            Event.EventTypeDesc += Constants.TokenKeys.PreviousRole + "(" + dlUserRoles.SelectedItem.Text.Replace("User ", "") + ")";
                            Event.EventTypeDesc += Constants.TokenKeys.NewRole + "(" + dlRolesAssigned.SelectedItem.Text.Replace("User ", "") + ")";
                            Event.EventTypeDesc += Constants.TokenKeys.UserFullName + dlUsers.SelectedItem.Text;
                            Event.EventTypeDesc += Constants.TokenKeys.UpdatedByLoggedinAdminFullName + loggedInAdminFirstName + " " + loggedInAdminLastName;

                            Event.Create(Constants.EventLog.Assignments.RoleAssignment.Promoted, null);
                        }
                        else if (userHasBeenDisabled)
                        { // User disabled and locked out
                            Event.EventTypeDesc += Constants.TokenKeys.UserRole + dlRolesAssigned.SelectedItem.Text.Replace("User ", "");
                            Event.EventTypeDesc += Constants.TokenKeys.UserFullName + dlUsers.SelectedItem.Text;
                            Event.EventTypeDesc += Constants.TokenKeys.UpdatedByLoggedinAdminFullName + loggedInAdminFirstName + " " + loggedInAdminLastName;

                            Event.Create(Constants.EventLog.Registration.AdminUser.Disabled, null);
                            cssClassName = "alert-box warning radius";
                        }
                        else if (userHasBeenReEnabled)
                        { // User has been reenabled
                            Event.EventTypeDesc += Constants.TokenKeys.UserRole + dlRolesAssigned.SelectedItem.Text.Replace("User ", "");
                            Event.EventTypeDesc += Constants.TokenKeys.UserFullName + dlUsers.SelectedItem.Text;
                            Event.EventTypeDesc += Constants.TokenKeys.UpdatedByLoggedinAdminFullName + loggedInAdminFirstName + " " + loggedInAdminLastName;

                            Event.Create(Constants.EventLog.Registration.AdminUser.Enabled, null);
                        }
                        else
                        {
                            // General user update details
                            // Reset "=" from previous log event
                            Event.EventTypeDesc = Constants.TokenKeys.UserRole + dlRolesAssigned.SelectedItem.Text.Replace("User ", "");
                            Event.EventTypeDesc += Constants.TokenKeys.UserFullName + dlUsers.SelectedItem.Text;
                            Event.EventTypeDesc += Constants.TokenKeys.UpdatedByLoggedinAdminFullName + loggedInAdminFirstName + " " + loggedInAdminLastName;
                            Event.EventTypeDesc += Constants.TokenKeys.UpdatedValues + updatedValues;

                            Event.Create(Constants.EventLog.Registration.AdminUser.Updated, null);
                        }

                        userUpdateMessage.Visible = true;

                        userUpdateMessage.InnerHtml = Event.EventTypeDesc;
                        userUpdateMessage.Attributes.Add("class", cssClassName);

                        if (IsRoleChanged)
                        {
                            GetUsersList();

                            ClearForm();
                            dlUsers.SelectedIndex = 0;

                            // Sync the roles listboxes
                            dlRolesAssigned.SelectedIndex = dlUserRoles.SelectedIndex;

                            hiddenSelectedUserId.Value = "";
                        }
                        else
                        {
                            GetUserInfo();
                        }

                        hiddenAA.Value = "";
                        break;

                    //default:
                    //    break;
                }
            }
            else
            {
                ClearForm();
                GetRolesList();
            }

            if (dlUserRoles.SelectedIndex == 1 || dlUserRoles.SelectedIndex == 2)
                btnSelectedGroups.Disabled = false;
            else
                btnSelectedGroups.Disabled = true;

            if (dlUserRoles.SelectedIndex == 3)
                btnSelectedGroups.Visible = false;
            else
                btnSelectedGroups.Visible = true;

            // Don't show roles list if Client User
            if (dlUserRoles.SelectedIndex == 4)
                divRolesAssignedList.Visible = false;
            else
                divRolesAssignedList.Visible = true;

            var decryptedUserId = Security.DecodeAndDecrypt(hiddenE.Value, Constants.Strings.DefaultClientId);

            if (Convert.ToBoolean(Security.DecodeAndDecrypt(hiddenF.Value, decryptedUserId)))
            {
                chkIsApproved.Enabled = false;
                chkIsLockedOut.Enabled = false;
                chkUserReadOnly.Enabled = false;

                dlRolesAssigned.Enabled = false;

                txtFirstName.Disabled = true;
                txtLastName.Disabled = true;
                txtEmail.Disabled = true;
                txtUsername.Disabled = true;
                txtPhone.Disabled = true;
                txtPassword.Disabled = true;

                btnSelectedGroups.Visible = false;
                btnSaveUser.Visible = false;
            }

            // If the selected user is the same as the logged in user, hide the read only bit to prevent self lockout!
            if (dlUsers.SelectedValue == loggedInAdminId)
            {
                chkUserReadOnly.Enabled = false;
                chkIsApproved.Enabled = false;
                chkIsLockedOut.Enabled = false;
                dlRolesAssigned.Enabled = false;
            }
            else
            {
                // This is the default sysadmin account. Never disable this!
                if (dlUsers.SelectedValue == "53a879d7b5655a0bcc2d567a")
                {
                    chkUserReadOnly.Enabled = false;
                    chkIsApproved.Enabled = false;
                    chkIsLockedOut.Enabled = false;
                    dlRolesAssigned.Enabled = false;
                }
                else
                {
                    chkUserReadOnly.Enabled = true;
                    chkIsApproved.Enabled = true;
                    chkIsLockedOut.Enabled = true;
                    dlRolesAssigned.Enabled = true;
                }
            }
        }

        public void GetRolesList()
        { 
            dlUserRoles.Items.Clear();

            var rootItem = new ListItem {Text = @"Select a Role", Value = Constants.Strings.DefaultEmptyObjectId};
            dlUserRoles.Items.Add(rootItem);

            List<string> userRoles = myUtils.GetUserRoles();
            foreach(string currentRole in userRoles)
            {
                var tmpVal = currentRole.Split('|');

                var li = new ListItem();
                li.Attributes.Add("id", "role"+ tmpVal[0]);
                li.Text = tmpVal[0] + @" - " + tmpVal[1];
                li.Value = tmpVal[2];

                dlUserRoles.Items.Add(li);
                dlRolesAssigned.Items.Add(li);
            }
        }

        public void GetUsersList()
        {
            dlUsers.Items.Clear();

            var li0 = new ListItem();
            li0.Text = @"Select a " + CleanListItem(dlUserRoles.SelectedItem.Text);
            li0.Value = Constants.Strings.DefaultEmptyObjectId;
            dlUsers.Items.Add(li0);

            var groupAdminList = new MacListAdHoc("UserProfile", "Roles", dlUserRoles.SelectedValue, false, "");

            // Here we need to process the xml response into a List collection
            var xmlGroupAdminDoc = groupAdminList.ListXml;
            var xmlGroupAdmins = xmlGroupAdminDoc.GetElementsByTagName("userprofile");

            foreach (var li in from XmlNode currentAdmin in xmlGroupAdmins where currentAdmin.Attributes != null select currentAdmin.Attributes["id"].Value into adminId let userProfile = new UserProfile(adminId) let adminName = Security.DecodeAndDecrypt(userProfile.FirstName, adminId) + " " + Security.DecodeAndDecrypt(userProfile.LastName, adminId) select new ListItem {Text = adminName, Value = adminId})
            {
                dlUsers.Items.Add(li);
            }
        }

        private string CleanListItem(string listItemText)
        {
            listItemText = listItemText.Replace("1 - ", "");
            listItemText = listItemText.Replace("2 - ", "");
            listItemText = listItemText.Replace("3 - ", "");
            listItemText = listItemText.Replace("4 - ", "");
            listItemText = listItemText.Replace("5 - ", "");
            listItemText = listItemText.Replace("6 - ", "");
            listItemText = listItemText.Replace("7 - ", "");
            listItemText = listItemText.Replace("8 - ", "");
            listItemText = listItemText.Replace("9 - ", "");
            listItemText = listItemText.Replace("10 - ", "");

            return listItemText;
        }

        protected void dlUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dlUsers.SelectedIndex > 0)
            {
                hiddenSelectedUserId.Value = dlUsers.SelectedValue;
                GetUserInfo();
            }
            else
            {
                hiddenSelectedUserId.Value = "";
                ClearForm();
            }
        }

        protected void dlUserRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearForm();

            dlUsers.Items.Clear();

            dlRolesAssigned.SelectedIndex = dlUserRoles.SelectedIndex;

            switch(dlUserRoles.SelectedIndex)
            {
                case 0: // Nothing selected
                    dlUsers.Visible = false;
                    pnlRegistration.Visible = false;
                    break;

                case 1: // System Admin
                    dlUsers.Visible = true;
                    pnlRegistration.Visible = true;
                    GetUsersList();
                    break;

                case 2: // Group Admin
                    dlUsers.Visible = true;
                    pnlRegistration.Visible = true;
                    GetUsersList();
                    break;

                case 3: // Client Admin
                    dlUsers.Visible = true;
                    pnlRegistration.Visible = true;
                    GetUsersList();
                    break;

                case 4: // Accounting User
                    dlUsers.Visible = true;
                    pnlRegistration.Visible = true;
                    GetUsersList();
                    break;

                case 5: // Client User
                    dlUsers.Visible = true;
                    pnlRegistration.Visible = true;
                    GetUsersList();
                    break;

                case 6: // Group User
                    dlUsers.Visible = true;
                    pnlRegistration.Visible = true;
                    GetUsersList();
                    break;

                case 7: // Operations User
                    dlUsers.Visible = true;
                    pnlRegistration.Visible = true;
                    GetUsersList();
                    break;

                case 8: // View Only
                    dlUsers.Visible = true;
                    pnlRegistration.Visible = true;
                    GetUsersList();
                    break;
            }
        }

        private void GetUserInfo()
        {
            txtUserID.Text = hiddenSelectedUserId.Value;// userId;

            var currentUser = Membership.GetUser(ObjectId.Parse(hiddenSelectedUserId.Value), false);
            if (currentUser != null)
            {
                chkIsApproved.Checked = currentUser.IsApproved;
                chkIsLockedOut.Checked = currentUser.IsLockedOut;

                txtUsername.Value = currentUser.UserName;

                // Only expose this if localhost
                var currentHost = Request.ServerVariables[Constants.WebConfig.HostInfo.RequestVariables.ServerName].ToString();
                if (currentHost == "localhost" || currentHost == "127.0.0.1" || currentHost == "::1")
                {
                    divPasswordControls.Visible = true;
                    txtPassword.Value = currentUser.GetPassword();
                }
                else
                {
                    // Don't show
                    divPasswordControls.Visible = false;
                }

                spanDateRegistered.InnerHtml = currentUser.CreationDate.ToString(CultureInfo.CurrentCulture);
                spanLastLogin.InnerHtml = currentUser.LastLoginDate.ToString(CultureInfo.CurrentCulture);
            }

            var currentUserProfile = new UserProfile(hiddenSelectedUserId.Value);

            chkUserReadOnly.Checked = currentUserProfile.IsReadOnly;
            if (chkUserReadOnly.Checked)
            {
                chkUserReadOnly.Text = @"<span style='color: #ff0000;'>User is set to read only!</span>";
            }
            else
            {
                chkUserReadOnly.Text = @"<span style='color: #4d4d4d;'>Check to make User read only</span>";
            }

            txtFirstName.Value = Security.DecodeAndDecrypt(currentUserProfile.FirstName, hiddenSelectedUserId.Value);
            txtLastName.Value = Security.DecodeAndDecrypt(currentUserProfile.LastName, hiddenSelectedUserId.Value);
            txtEmail.Value = Security.DecodeAndDecrypt(currentUserProfile.Contact.Email, hiddenSelectedUserId.Value);
            txtPhone.Value = Security.DecodeAndDecrypt(currentUserProfile.Contact.MobilePhone, hiddenSelectedUserId.Value);

            h3NameTitle.InnerHtml = txtFirstName.Value + " " + txtLastName.Value;

            var groupCount = 0;
// ReSharper disable once NotAccessedVariable
            var clientCount = 0;

            Session["Relationships"] = currentUserProfile.Relationships;
            foreach (Relationship currentRelationship in currentUserProfile.Relationships)
            {
                switch (currentRelationship.MemberType)
                {
                    case "Client":
                        clientCount++;
                        break;
                    case "Group":
                        groupCount++;
                        break;
                }
            }

            btnSaveUser.Value = @"Update " + txtFirstName.Value + @"'s Account";

            if (groupCount == 1)
                btnSelectedGroups.Value = groupCount + " Group Assigned";
            else
                btnSelectedGroups.Value = groupCount + " Groups Assigned";

        }

        private void ClearForm()
        {
            if (BDebug)
            {
                txtFirstName.Value = @"Test";
                txtLastName.Value = @"User";
                txtEmail.Value = @"user@test.com";
                txtPhone.Value = @"555-555-1212";
                txtUsername.Value = txtEmail.Value;
                txtPassword.Value = "123456";
            }
            else
            {
                spanDateRegistered.InnerHtml = "";
                spanLastLogin.InnerHtml = "";

                txtFirstName.Value = "";
                txtLastName.Value = "";
                txtEmail.Value = "";
                txtPhone.Value = "";
                txtUsername.Value = "";
                txtPassword.Value = "";
            }

            txtUserID.Text = "";
            h3NameTitle.InnerHtml = "";

            chkIsApproved.Checked = true;
            chkIsLockedOut.Checked = false;

            chkUserReadOnly.Checked = false;

            btnSaveUser.Value = @"Register User";

            txtUsername.Disabled = false;
            spanUsername.InnerHtml = "Username";
        }
    }
}