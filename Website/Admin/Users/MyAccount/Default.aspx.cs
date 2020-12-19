using System;
using System.Globalization;
using System.Web.Security;
using System.Web.UI.WebControls;
using MACSecurity;
using MACServices;
using MongoDB.Bson;

namespace Admin.Users.MyAccount
{
    public partial class Default : System.Web.UI.Page
    {
        public string loggedInAdminUserName = "";
        public string loggedInAdminId = "";
        public string loggedInClientId = "";

        HiddenField _hiddenD;
        HiddenField _hiddenE;
        HiddenField _hiddenG;
        HiddenField _hiddenH;
        HiddenField _hiddenI;
// ReSharper disable once NotAccessedField.Local
        HiddenField _hiddenJ;
// ReSharper disable once NotAccessedField.Local
        HiddenField _hiddenK;
        HiddenField _hiddenL;
// ReSharper disable once NotAccessedField.Local
        HiddenField _hiddenM;

        HiddenField _hiddenW;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Master == null) return;

            _hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");
            _hiddenE = (HiddenField)Page.Master.FindControl("hiddenE");
            _hiddenG = (HiddenField)Page.Master.FindControl("hiddenG");
            _hiddenH = (HiddenField)Page.Master.FindControl("hiddenH");
            _hiddenI = (HiddenField)Page.Master.FindControl("hiddenI");
            _hiddenJ = (HiddenField)Page.Master.FindControl("hiddenJ");
            _hiddenK = (HiddenField)Page.Master.FindControl("hiddenK");
            _hiddenL = (HiddenField)Page.Master.FindControl("hiddenL");
            _hiddenM = (HiddenField)Page.Master.FindControl("hiddenM");

            _hiddenW = (HiddenField)Master.FindControl("hiddenW");
            _hiddenW.Value = "54a9afc7ead6361b78f8f45d";

            var master = (MasterPages.AdminConsole)Page.Master;
            master.SetHiddenFields();

            var pwd = "";
            var securityQuestion = "Fave company";
            var securityAnswer = "MAC";

            var currentUserProfile = new UserProfile("");

            loggedInAdminId = MACSecurity.Security.DecodeAndDecrypt(_hiddenE.Value, Constants.Strings.DefaultClientId);
            loggedInAdminUserName = MACSecurity.Security.DecodeAndDecrypt(_hiddenG.Value, loggedInAdminId);
            loggedInClientId = MACSecurity.Security.DecodeAndDecrypt(_hiddenD.Value, loggedInAdminId);

            // Get the loggedin User
            var currentUser = Membership.GetUser(loggedInAdminUserName);
            if (currentUser != null)
            {
                pwd = currentUser.GetPassword();
                currentUserProfile = new UserProfile(loggedInAdminId);
            }

            var newPwd = txtPassword.Value;
            if (newPwd == null || newPwd == "")
                newPwd = "1212";

            if (!IsPostBack)
            {
                GetUserInfo();
            }
            else
            {
                // Update user registration
                if (currentUser != null)
                {
                    currentUser.Email = txtEmail.Text;

                    currentUser.ChangePassword(pwd, newPwd);
                    currentUser.ChangePasswordQuestionAndAnswer("password", securityQuestion, securityAnswer);
                }

                // Update user profile
                currentUserProfile.Name = txtFirstName.Text + " " + txtLastName.Text;
                currentUserProfile.FirstName = Security.EncryptAndEncode(txtFirstName.Text, loggedInAdminId);
                currentUserProfile.LastName = Security.EncryptAndEncode(txtLastName.Text, loggedInAdminId);

                currentUserProfile.Contact.Email = Security.EncryptAndEncode(txtEmail.Text, loggedInAdminId);
                currentUserProfile.Contact.HomePhone = Security.EncryptAndEncode("", loggedInAdminId);
                currentUserProfile.Contact.MobilePhone = Security.EncryptAndEncode(txtPhone.Text, loggedInAdminId);
                currentUserProfile.Contact.WorkPhone = Security.EncryptAndEncode("", loggedInAdminId);
                currentUserProfile.Contact.WorkExtension = Security.EncryptAndEncode("", loggedInAdminId);

                currentUserProfile.Address.Street1 = Security.EncryptAndEncode("", loggedInAdminId);
                currentUserProfile.Address.Street2 = Security.EncryptAndEncode("", loggedInAdminId);
                currentUserProfile.Address.Unit = Security.EncryptAndEncode("", loggedInAdminId);
                currentUserProfile.Address.City = Security.EncryptAndEncode("", loggedInAdminId);
                currentUserProfile.Address.State = Security.EncryptAndEncode("", loggedInAdminId);
                currentUserProfile.Address.Zipcode = Security.EncryptAndEncode("", loggedInAdminId);

                currentUserProfile.Update();

                var _event = new Event();

                var tokens = "";
                tokens += Constants.TokenKeys.UserRole + _hiddenL.Value.Replace("User ", "");
                tokens += Constants.TokenKeys.UserFullName + currentUserProfile.Name;
                tokens += Constants.TokenKeys.UpdatedByLoggedinAdminFullName + _hiddenH.Value + " " + _hiddenI.Value;

                _event.ClientId = ObjectId.Parse(loggedInClientId);
                _event.UserId = ObjectId.Parse(loggedInAdminId);

                _event.Create(Constants.EventLog.Registration.AdminUser.Updated, tokens);

                var updateMsg = (TextBox)Page.Master.FindControl("divServiceResponseMessage");
                updateMsg.Text = _event.EventTypeDesc;
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
        }

        private void GetUserInfo()
        {
            var currentUser = Membership.GetUser(loggedInAdminUserName);
            if (currentUser != null)
            {
                txtUsername.Text = currentUser.UserName;
                //txtSecurityQuestion.Text = currentUser.PasswordQuestion;

                // Only expose this if localhost
                var currentHost = Request.ServerVariables[MACServices.Constants.WebConfig.HostInfo.RequestVariables.ServerName].ToString();
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

                chkIsApproved.Checked = currentUser.IsApproved;
                chkIsLockedOut.Checked = currentUser.IsLockedOut;

                spanDateRegistered.InnerHtml = currentUser.CreationDate.ToString(CultureInfo.CurrentCulture);
                spanLastLogin.InnerHtml = currentUser.LastLoginDate.ToString(CultureInfo.CurrentCulture);
            }

            var currentUserProfile = new UserProfile(loggedInAdminId);
            txtFirstName.Text = Security.DecodeAndDecrypt(currentUserProfile.FirstName, loggedInAdminId);
            txtLastName.Text = Security.DecodeAndDecrypt(currentUserProfile.LastName, loggedInAdminId);
            txtEmail.Text = Security.DecodeAndDecrypt(currentUserProfile.Contact.Email, loggedInAdminId);
            txtPhone.Text = Security.DecodeAndDecrypt(currentUserProfile.Contact.MobilePhone, loggedInAdminId);

            h3NameTitle.InnerHtml = txtFirstName.Text + " " + txtLastName.Text;

            Session["Relationships"] = currentUserProfile.Relationships;
        }
    }
}