using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using MACBilling;
using MACSecurity;
using MACServices;

using MongoDB.Bson;
using MongoDB.Web.Providers;

namespace Admin.Users
{
    public partial class ClientUsers : System.Web.UI.Page
    {
        public string RegistrationResults;
        public string NewUserId;

        public bool BDebug = true;

        public String Tokens = "";

        public Event Event = new Event();

        public HtmlForm MyForm;

        public HiddenField HiddenLoggedInAdminId;
        public HiddenField hiddenH;
        public HiddenField hiddenI;

        public string securityQuestion = "Fave company";
        public string securityAnswer = "MAC of course!";

        // Use email for the userName 
        public string userName = "";
        public string password = "1212";
        public string email = "";

        public string loggedInAdminId = "";
        public string loggedInAdminFirstName = "";
        public string loggedInAdminLastName = "";

        public bool createRelationship = true;

        public Utils mUtils = new Utils();

        public Client MyClient;

        public UserProfile userProfile;

        protected void Page_Load(object sender, EventArgs e)
        {
            BDebug = true;

            email = txtEmail.Value;
            userName = email;
            password = "1212";

            if (Page.Master != null)
            {
                if (Master != null)
                {
                    MyForm = (HtmlForm)Page.Master.FindControl("formMain");
                    HiddenLoggedInAdminId = (HiddenField)Page.Master.FindControl("hiddenE");
                    hiddenH = (HiddenField)Page.Master.FindControl("hiddenH");
                    hiddenI = (HiddenField)Page.Master.FindControl("hiddenI");
                }
            }

            if (Request["clientId"] != null)
            {
                hiddenD.Value = Request["clientId"];
                Event.ClientId = ObjectId.Parse(hiddenD.Value);
            }

            if (Request["loggedInAdminId"] != null)
                if (Request["loggedInAdminId"] != "")
                    loggedInAdminId = Request["loggedInAdminId"];

            loggedInAdminId = MACSecurity.Security.DecodeAndDecrypt(loggedInAdminId, Constants.Strings.DefaultClientId);

            loggedInAdminFirstName = MACSecurity.Security.DecodeAndDecrypt(hiddenH.Value, loggedInAdminId);
            loggedInAdminLastName = MACSecurity.Security.DecodeAndDecrypt(hiddenH.Value, loggedInAdminId);

            var userIsReadOnly = "";
            if (!String.IsNullOrEmpty(Request["userisreadonly"]))
                userIsReadOnly = Request["userisreadonly"];

            userIsReadOnly = MACSecurity.Security.DecodeAndDecrypt(userIsReadOnly, loggedInAdminId);

            MyClient = new Client(hiddenD.Value);

            userUpdateMessage.InnerHtml = "";
            userUpdateMessage.Visible = false;

            if (IsPostBack)
            {
                // Get the selected user
                //GetUsersList();
            }
            else
            {
                txtFirstName.Focus();
                ClearForm();
                GetUsersList();
            }

            if (dlUsers.SelectedIndex == 0)
            {
                btnRemove.Enabled = false;
                btnCancel.Enabled = false;
            }
            else
            {
                btnRemove.Enabled = true;
                btnCancel.Enabled = true;
            }

            if (Convert.ToBoolean(userIsReadOnly))
            {
                dlUsers.Enabled = false;
                txtFirstName.Disabled = true;
                txtLastName.Disabled = true;
                txtEmail.Disabled = true;
                txtPhone.Disabled = true;

                btnRegister.Visible = false;
                btnRemove.Visible = false;
                btnCancel.Visible = false;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            userUpdateMessage.Visible = false;
            userUpdateMessage.InnerHtml = "";

            ClearForm();
            GetUsersList();

            dlUsers.SelectedIndex = 0;

            btnCancel.Enabled = false;
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            // Remove relationships between client and user
            createRelationship = false;

            // Fetch the user's profile
            var userId = mUtils.GetUserIdByUserName(txtEmail.Value);

            userProfile = new UserProfile(userId)
            {
                _id = ObjectId.Parse(userId),
                UserId = ObjectId.Parse(userId),
                Name = txtFirstName.Value + " " + txtLastName.Value,
                Prefix = Security.EncryptAndEncode("", userId),
                FirstName = Security.EncryptAndEncode(txtFirstName.Value, userId),
                MiddleName = Security.EncryptAndEncode("", userId),
                LastName = Security.EncryptAndEncode(txtLastName.Value, userId),
                Suffix = Security.EncryptAndEncode("", userId),
                DateOfBirth = Security.EncryptAndEncode("1/1/1900", userId)
            };

            mUtils.ManageObjectRelationships_UserAndClient(ObjectId.Parse(loggedInAdminId), createRelationship, userProfile, MyClient);

            // Add the Client User to the BillingConfig sendto list
            var myConfig = new BillConfig(hiddenD.Value, "Client", loggedInAdminId);

            var userEmail = Security.DecodeAndDecrypt(userProfile.Contact.Email, userProfile.UserId.ToString());
            myConfig.BillingSendTo = myConfig.BillingSendTo.Replace(userEmail + ", ", "");

            myConfig.UpdateConfig(myConfig, myConfig.OwnerId.ToString());

            userUpdateMessage.Visible = true;
            userUpdateMessage.InnerHtml = "Client access permissions removed for User";

            GetUsersList();

            ClearForm();
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {

// ReSharper disable once RedundantAssignment
            var userId = "";

            if (Master == null) return;
// ReSharper disable once UnusedVariable
            var updateMsg = (TextBox)Master.FindControl("divServiceResponseMessage");

            // Register the user
            try
            {
                #region ASPNet Membership

                if (IsPostBack)
                {
                    if (btnRegister.Text.Contains("Register"))
                    {
                        MembershipCreateStatus createStatus;
                        var newUser = Membership.CreateUser(userName, password, email, securityQuestion, securityAnswer, true, out createStatus);

                        switch (createStatus)
                        {
                            case MembershipCreateStatus.Success:
                                if (newUser != null)
                                {
                                    if (newUser.ProviderUserKey != null)
                                        NewUserId = newUser.ProviderUserKey.ToString();
                                }

                                // Add the user's profile
                                userProfile = new UserProfile("")
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

                                var roleId = ObjectId.Parse(Constants.Strings.DefaultClientUserId);
                                userProfile.Roles.Add(roleId);

                                userProfile.Contact.Email = Security.EncryptAndEncode(txtEmail.Value, NewUserId);
                                userProfile.Contact.HomePhone = Security.EncryptAndEncode("", NewUserId);
                                userProfile.Contact.MobilePhone = Security.EncryptAndEncode(txtPhone.Value, NewUserId);
                                userProfile.Contact.WorkPhone = Security.EncryptAndEncode("", NewUserId);
                                userProfile.Contact.WorkExtension = Security.EncryptAndEncode("", NewUserId);

                                userProfile.Address.Street1 = Security.EncryptAndEncode("", NewUserId);
                                userProfile.Address.Street2 = Security.EncryptAndEncode("", NewUserId);
                                userProfile.Address.Unit = Security.EncryptAndEncode("", NewUserId);
                                userProfile.Address.City = Security.EncryptAndEncode("", NewUserId);
                                userProfile.Address.State = Security.EncryptAndEncode("", NewUserId);
                                userProfile.Address.Zipcode = Security.EncryptAndEncode("", NewUserId);

                                userProfile.Create();

                                // Create the relationship between user and client
                                mUtils.ManageObjectRelationships_UserAndClient(ObjectId.Parse(loggedInAdminId), createRelationship, userProfile, MyClient);

                                GetUsersList();

                                Tokens += Constants.TokenKeys.UserRole + Constants.Strings.DefaultClientUserId;
                                Tokens += Constants.TokenKeys.UserFullName + userProfile.Name;
                                Tokens += Constants.TokenKeys.UpdatedByLoggedinAdminFullName + loggedInAdminFirstName + " " + loggedInAdminLastName;

                                Event.Create(Constants.EventLog.Registration.AdminUser.Updated, Tokens);

                                userUpdateMessage.Visible = true;
                                userUpdateMessage.InnerHtml = "User granted access permissions for client (" + MyClient.Name + ")";
                                break;
                            case MembershipCreateStatus.DuplicateUserName:
                                // Fetch the user's profile
                                userId = mUtils.GetUserIdByUserName(userName);

                                userProfile = new UserProfile(userId)
                                {
                                    _id = ObjectId.Parse(userId),
                                    UserId = ObjectId.Parse(userId),
                                    Name = txtFirstName.Value + " " + txtLastName.Value,
                                    Prefix = Security.EncryptAndEncode("", userId),
                                    FirstName = Security.EncryptAndEncode(txtFirstName.Value, userId),
                                    MiddleName = Security.EncryptAndEncode("", userId),
                                    LastName = Security.EncryptAndEncode(txtLastName.Value, userId),
                                    Suffix = Security.EncryptAndEncode("", userId),
                                    DateOfBirth = Security.EncryptAndEncode("1/1/1900", userId)
                                };

                                // Create the relationship between user and client
                                mUtils.ManageObjectRelationships_UserAndClient(ObjectId.Parse(loggedInAdminId), createRelationship, userProfile, MyClient);

                                Tokens += Constants.TokenKeys.ClientName + MyClient.Name;
                                Tokens += Constants.TokenKeys.UserFullName + userProfile.Name;
                                Tokens += Constants.TokenKeys.UpdatedByLoggedinAdminFullName + loggedInAdminFirstName + " " + loggedInAdminLastName;

                                Event.Create(Constants.EventLog.Registration.ClientUser.Added, Tokens);

                                userUpdateMessage.Visible = true;
                                userUpdateMessage.InnerHtml = "User granted access permissions for client (" + MyClient.Name + ")";
                                break;
                            case MembershipCreateStatus.DuplicateEmail:
                                // Fetch the user's profile
                                userId = mUtils.GetUserIdByUserName(userName);

                                userProfile = new UserProfile(userId)
                                {
                                    _id = ObjectId.Parse(userId),
                                    UserId = ObjectId.Parse(userId),
                                    Name = txtFirstName.Value + " " + txtLastName.Value,
                                    Prefix = Security.EncryptAndEncode("", userId),
                                    FirstName = Security.EncryptAndEncode(txtFirstName.Value, userId),
                                    MiddleName = Security.EncryptAndEncode("", userId),
                                    LastName = Security.EncryptAndEncode(txtLastName.Value, userId),
                                    Suffix = Security.EncryptAndEncode("", userId),
                                    DateOfBirth = Security.EncryptAndEncode("1/1/1900", userId)
                                };

                                // Create the relationship between user and client
                                mUtils.ManageObjectRelationships_UserAndClient(ObjectId.Parse(loggedInAdminId), createRelationship, userProfile, MyClient);

                                Tokens += Constants.TokenKeys.UserRole + Constants.Strings.DefaultClientUserId;
                                Tokens += Constants.TokenKeys.UserFullName + userProfile.Name;
                                Tokens += Constants.TokenKeys.UpdatedByLoggedinAdminFullName + loggedInAdminFirstName + " " + loggedInAdminLastName;

                                Event.Create(Constants.EventLog.Registration.ClientUser.Added, Tokens);

                                userUpdateMessage.Visible = true;
                                userUpdateMessage.InnerHtml = "User granted access permissions for client (" + MyClient.Name + ")";
                                break;
                            case MembershipCreateStatus.InvalidEmail:
                                Tokens += Constants.TokenKeys.InvalidEmail + userName;
                                Event.Create(Constants.EventLog.Registration.AdminUser.InvalidEmail, Tokens);

                                userUpdateMessage.Visible = true;
                                userUpdateMessage.InnerHtml = "Invalid email address. Please try another";
                                break;
                            case MembershipCreateStatus.InvalidAnswer:
                                Tokens += Constants.TokenKeys.InvalidSecurityAnswer + userName;
                                Event.Create(Constants.EventLog.Registration.AdminUser.InvalidSecurityAnswer, Tokens);

                                userUpdateMessage.Visible = true;
                                userUpdateMessage.InnerHtml = "Invalid security answer. Please try another";
                                break;
                            case MembershipCreateStatus.InvalidPassword:
                                Tokens += Constants.TokenKeys.InvalidPassword + userName;
                                Event.Create(Constants.EventLog.Registration.AdminUser.InvalidPassword, Tokens);

                                userUpdateMessage.Visible = true;
                                userUpdateMessage.InnerHtml = "Invalid password. Please try another";
                                break;
                            //default:
                                // Create the relationship between user and client
                                //mUtils.ManageObjectRelationships_AdminAndClient(ObjectId.Parse(loggedInAdminId), createRelationship, userProfile, MyClient);
                                //break;
                        }
                        // Add the Client User to the BillingConfig sendto list
                        var myConfig = new BillConfig(hiddenD.Value, "Client", loggedInAdminId);

                        var userEmail = Security.DecodeAndDecrypt(userProfile.Contact.Email, userProfile.UserId.ToString());
                        myConfig.BillingSendTo += userEmail + ", ";

                        myConfig.UpdateConfig(myConfig, myConfig.OwnerId.ToString());

                        // Show updated list
                        GetUsersList();

                        hiddenSelectedUserId.Value = userProfile.UserId.ToString();

                        dlUsers.SelectedValue = hiddenSelectedUserId.Value;

                        GetUserInfo(dlUsers.SelectedValue);
                    }
                    else
                    {
                        var currentUserProfile = new UserProfile(dlUsers.SelectedValue);

                        Event.ClientId = ObjectId.Parse(hiddenD.Value);

                        //var _event = new Event {UserId = ObjectId.Parse(hiddenE.Value)};
                        if (btnRegister.Text.Contains("Update"))
                        {
                            // Get the selected User and profile
                            var currentUser = Membership.GetUser(ObjectId.Parse(dlUsers.SelectedValue), true);

                            if (currentUser != null)
                            {
                                currentUser.IsApproved = true;
                                currentUser.Comment = "Why does the user have to have a comment to update? Dumb, dumb and dumb!";

                                var membershipProvider = new MongoDbMembershipProvider();
                                membershipProvider.UpdateUser(currentUser);

// ReSharper disable once UnusedVariable
                                var pwd = currentUser.GetPassword();

                                currentUserProfile = new UserProfile(dlUsers.SelectedValue);

                                // Update user registration
                                currentUser.Email = txtEmail.Value;

                                // Update user profile
                                currentUserProfile.Name = txtFirstName.Value + " " + txtLastName.Value;
                                currentUserProfile.FirstName = Security.EncryptAndEncode(txtFirstName.Value, dlUsers.SelectedValue);
                                currentUserProfile.LastName = Security.EncryptAndEncode(txtLastName.Value, dlUsers.SelectedValue);

                                currentUserProfile.Contact.Email = Security.EncryptAndEncode(txtEmail.Value, dlUsers.SelectedValue);
                                currentUserProfile.Contact.MobilePhone = Security.EncryptAndEncode(txtPhone.Value, dlUsers.SelectedValue);

                                currentUserProfile.Update();
                            }

                            var oldUserProfile = new UserProfile(dlUsers.SelectedValue);

                            var myUtils = new Utils();
                            var updatedValues = myUtils.GetObjectDifferences(oldUserProfile, currentUserProfile);


                            // General user update details
                            Event.EventTypeDesc = Constants.TokenKeys.UserRole + "Client User";
                            Event.EventTypeDesc += Constants.TokenKeys.UserFullName + dlUsers.SelectedItem.Text;
                            Event.EventTypeDesc += Constants.TokenKeys.UpdatedByLoggedinAdminFullName + loggedInAdminFirstName + " " + loggedInAdminLastName;
                            Event.EventTypeDesc += Constants.TokenKeys.UpdatedValues + updatedValues;

                            Event.Create(Constants.EventLog.Registration.AdminUser.Updated, null);

                            userUpdateMessage.Visible = true;

                            userUpdateMessage.InnerHtml = "Client User account updated";

                            GetUsersList();

                            dlUsers.SelectedValue = hiddenSelectedUserId.Value;

                            GetUserInfo(dlUsers.SelectedValue);
                        }
                    }
                }

                if (dlUsers.SelectedIndex > 0)
                {
                    GetUserInfo(dlUsers.SelectedValue);
                    btnRemove.Enabled = true;
                    btnCancel.Enabled = true;
                }
                else
                {
                    GetUsersList();
                    btnRemove.Enabled = false;
                    btnCancel.Enabled = false;
                }

                #endregion
            }
            catch (Exception ex)
            {
                userUpdateMessage.InnerHtml = @"System exception: " + ex.Message;

                var exceptionEvent = new Event();
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, Constants.TokenKeys.ExceptionDetails + ex.ToString());

                userUpdateMessage.InnerHtml = exceptionEvent.EventTypeDesc;
            }
        }

        public void GetUsersList()
        {
            dlUsers.Items.Clear();

// ReSharper disable once UnusedVariable
            var clientUserList = new List<UserProfile>();

            var defaultItem = new ListItem();
// ReSharper disable once LocalizableElement
            defaultItem.Text = "Select a User";
            defaultItem.Value = Constants.Strings.DefaultEmptyObjectId;
            dlUsers.Items.Add(defaultItem);

            var createItem = new ListItem();
            // ReSharper disable once LocalizableElement
            createItem.Text = "Create new User";
            createItem.Value = Constants.Strings.DefaultEmptyObjectId;
            dlUsers.Items.Add(createItem);

            var clientUsers = mUtils.GetOwnerUsers(hiddenD.Value, "Client", "User");

            foreach(var currentUser in clientUsers)
            {
                var tmpUser = currentUser.Split('|');

                var li = new ListItem();
                li.Text = tmpUser[0];
                li.Value = tmpUser[1];

                dlUsers.Items.Add(li);
            }
        }

        protected void dlUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (dlUsers.SelectedIndex)
            {
                case 0: // Reset
                    ClearForm();
                    break;

                case 1: // Create User
                    ClearForm();
                    break;

                default: // Get User Info
                    GetUserInfo(dlUsers.SelectedValue);
                    hiddenSelectedUserId.Value = dlUsers.SelectedValue;
                    break;
            }
        }

        private void GetUserInfo(string userId)
        {
            var currentUser = Membership.GetUser(ObjectId.Parse(userId), false);
            if (currentUser != null)
            {
                spanDateRegistered.InnerHtml = currentUser.CreationDate.ToString(CultureInfo.CurrentCulture);
                spanLastLogin.InnerHtml = currentUser.LastLoginDate.ToString(CultureInfo.CurrentCulture);

                var currentUserProfile = new UserProfile(userId);
                txtFirstName.Value = Security.DecodeAndDecrypt(currentUserProfile.FirstName, userId);
                txtLastName.Value = Security.DecodeAndDecrypt(currentUserProfile.LastName, userId);
                txtEmail.Value = Security.DecodeAndDecrypt(currentUserProfile.Contact.Email, userId);
                txtPhone.Value = Security.DecodeAndDecrypt(currentUserProfile.Contact.MobilePhone, userId);
            }

            btnRegister.Text = @"Update";
        }

        private void ClearForm()
        {
            spanDateRegistered.InnerHtml = DateTime.Now.ToString();
            spanLastLogin.InnerHtml = DateTime.Now.ToString();

            hiddenSelectedUserId.Value = "";

            BDebug = false;

            if (BDebug)
            {
                txtEmail.Value = "user@client.com";
                txtFirstName.Value = "Client";
                txtLastName.Value = "User";
                txtPhone.Value = "480-263-4076";
            }
            else
            {
                txtFirstName.Value = "";
                txtLastName.Value = "";
                txtEmail.Value = "";
                txtPhone.Value = "";
            }

            btnCancel.Enabled = false;
            btnRemove.Enabled = false;

            btnRegister.Text = @"Register";
        }
    }
}