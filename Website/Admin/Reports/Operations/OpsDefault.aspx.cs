using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using MACSecurity;
using MACServices;
using cs = MACServices.Constants.Strings;
using dk = MACServices.Constants.Dictionary.Keys;
using dkui = MACServices.Constants.Dictionary.Userinfo;

using MACOperationalTestLib;

namespace Admin.Reports.Operations
{
    public partial class OpsDefault : System.Web.UI.Page
    {
        private HiddenField _hiddenW;
        public const string PS = "Please Select";
        public const string None = "None";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)return;

            if (Master != null)
            {
                // ReSharper disable once UnusedVariable
                var oPanel = (Panel) Master.FindControl("pnlEventHistory");
                var bodyContainer = (Panel) Master.FindControl("BodyContainer");
                bodyContainer.Height = 1200;

                _hiddenW = (HiddenField) Master.FindControl("hiddenW");
                _hiddenW.Value = "55020381a6e10b029035e0b5";
            }
            UpdateTestSystemForm();
        }

        protected void UpdateTestSystemForm()
        {
            var mOpTestSystem = GetOperationalTestObject();
            if (mOpTestSystem == null)
            {
                lbViewFileError.Text = @"Unable to get Operational Test documnet from database, Please retry!";
                return;
            }
            lbViewFileError.Text = String.Empty;
            // Set default results retrival window
            txtResultsStartDate.Value = DateTime.UtcNow.AddDays(mOpTestSystem.ResultsLookupDays * -1).ToString("MM/dd/yy 00:00:00");
            txtResultsEndDate.Value = DateTime.UtcNow.ToString("MM/dd/yy HH:mm:ss");
            SetDDLFromOperationalTestObject(mOpTestSystem);
        }

        protected void SetDDLFromOperationalTestObject(OperationalTest pOpTestSystem)
        {
            var mOpTestSystem = pOpTestSystem;
            if (pOpTestSystem == null)
            {
                mOpTestSystem = GetOperationalTestObject();
            }
            dlResultsTabSystemsUnderTest.Items.Clear();
            dlResultsTabTestRuns.Items.Clear();
            {
                var mLi = new ListItem();
                mLi.Text = mLi.Value = None;
                dlResultsTabSystemsUnderTest.Items.Add(mLi);
                dlResultsTabTestRuns.Items.Add(mLi);
            }
            if (mOpTestSystem.SystemsUnderTestList.Any())
            {
                dlResultsTabSystemsUnderTest.Items.Clear();
                {
                    var mLi = new ListItem();
                    mLi.Text = mLi.Value = PS;
                    dlResultsTabSystemsUnderTest.Items.Add(mLi);
                }
                foreach (var mSUT in mOpTestSystem.SystemsUnderTestList)
                {
                    dlResultsTabSystemsUnderTest.Items.Add(mSUT.SystemName);
                    var mLi = new ListItem {Text = mSUT.SystemName};
                    //Contacts
                    if (!mSUT.ContactList.Any())
                    {
                        mLi.Value = None;
                    }
                    else
                    {
                        foreach (var mContact in mSUT.ContactList)
                        {
                            var fn = Security.DecodeAndDecrypt(mContact.eFirstName, cs.DefaultClientId);
                            var ln = Security.DecodeAndDecrypt(mContact.eLastName, cs.DefaultClientId);
                            var em = Security.DecodeAndDecrypt(mContact.eEmailAddress, cs.DefaultClientId);
                            var pn = Security.DecodeAndDecrypt(mContact.ePhoneNumber, cs.DefaultClientId);
                            mLi.Value += @"~"
                                        + dkui.FirstName + fn.Replace(" ","") + dk.ItemSep
                                        + dkui.LastName + ln.Replace(" ", "") + dk.ItemSep
                                        + dkui.EmailAddress + em + dk.ItemSep
                                        + dkui.PhoneNumber + pn +
                                        dk.ItemSep + @"SMSContact" + mContact.SendSMS +
                                        dk.ItemSep + @"EmailContact" + mContact.SendEmail;
                        }
                        mLi.Value = mLi.Value.Trim('~');
                    }
                }
            }
        }

        #region Button event handlers

        //========= Test Results event handlers =================================
        protected void btnRetrieveTestRuns_Click(object sender, EventArgs e)
        {
            lbViewFileError.Text = String.Empty;
            lbErrorTestResults.Text = String.Empty;
            if (dlResultsTabSystemsUnderTest.SelectedItem.Text == PS)
            {
                dlResultsTabTestRuns.Items.Clear();
                var mLi = new ListItem();
                mLi.Text = mLi.Value = None;
                dlResultsTabTestRuns.Items.Add(mLi);
                return;
            }
            var mSUTName = dlResultsTabSystemsUnderTest.SelectedItem.Text;
            DateTime SD;
            DateTime ED;
            // txtResultsStartDate  txtResultsEndDate
            try
            {
                SD = Convert.ToDateTime(txtResultsStartDate.Value);
            }
            catch
            {
                lbErrorTestResults.Text = @"Invalid Start Date";
                return;
            }
            try
            {
                ED = Convert.ToDateTime(txtResultsEndDate.Value);
            }
            catch
            {
                lbErrorTestResults.Text = @"Invalid End Date";
                return;
            }
            if (ED < SD)
            {
                lbErrorTestResults.Text = @"Start Date can't be later than end date";
                return;
            }

            var mOpUtils = new OperationalTestUtils();
            var TestRuns = mOpUtils.GetTestRunsBySystemUnderTestNameAndDateRange(mSUTName, SD, ED, null);
            if (!TestRuns.Any() )
            {
                lbErrorTestResults.Text = @"No Test Runs for the System Under Test that is selected!";
                return;    
            }
            dlResultsTabTestRuns.Items.Clear();
            {
                var mLi = new ListItem();
                mLi.Text = mLi.Value = PS;
                dlResultsTabTestRuns.Items.Add(mLi);
            }
            foreach (var TestRun in TestRuns)
            {
                var mLi = new ListItem();
                mLi.Text = TestRun.Key;
                mLi.Value = TestRun.Value;
                dlResultsTabTestRuns.Items.Add(mLi);
            }
        }

        protected void dlResultsTabTestRuns_Changed(object sender, EventArgs e)
        {
            lbErrorTestResults.Text = String.Empty;
            if (dlResultsTabTestRuns.SelectedItem.Text == PS)
            {
                var mLi = new ListItem();
                mLi.Text = mLi.Value = None;
                return;
            }
            // Valid test run selected, go get list of files
            var mTestRunId = dlResultsTabTestRuns.SelectedValue;
            var mOpUtils = new OperationalTestUtils();
            var mOperationalTestResults = mOpUtils.GetOperationalTestResultsById(mTestRunId);
            if (mOperationalTestResults == null) 
            {
                var mLi = new ListItem();
                mLi.Text = mLi.Value = None;
                lbErrorTestResults.Text = @"Could not retrive test results for " + dlResultsTabTestRuns.SelectedItem.Text;
                return;    
            }
            txtResultsTestName.Value = mOperationalTestResults.TestName;
            txtResultsTestScript.Value = mOperationalTestResults.ScriptFile;
            txtResultsOverall.Value = mOperationalTestResults.ResultsStatement;

            if (mOperationalTestResults.ResultFileList.Count < 1) return;

            var first = true;
            hiddenFileList.Value = string.Empty;
            var mLinks = new StringBuilder();
            foreach (OperationalTestResultFile mFile in mOperationalTestResults.ResultFileList)
            {
                var mEFileToView = Security.EncryptAndEncode(
                        mFile.Name, Constants.Strings.DefaultClientId);

                var mEResultTestId = Security.EncryptAndEncode(
                        mOperationalTestResults._id.ToString(), Constants.Strings.DefaultClientId);
                if (first)
                {
                    mLinks.Append("<Label>Result Files (Click to view)</label>");
                    mLinks.Append("<a href='ReportPage.aspx?f=" + mEFileToView + "&i=" + mEResultTestId +
                                  "' target='_blank' >" + mFile.Name + "</a>");
                    first = false;
                }
                else
                {
                    mLinks.Append("&nbsp-&nbsp");
                    mLinks.Append("<a href='ReportPage.aspx?f=" + mEFileToView + "&i=" + mEResultTestId +
                                  "' target='_blank' >" + mFile.Name + "</a>");
                }
            }
            divFiles.InnerHtml = hiddenFileList.Value = mLinks.ToString();
        }
       
        #endregion

        #region Service Calls
        protected OperationalTest GetOperationalTestObject()
        {
            try
            {
                var mOtUtils = new OperationalTestUtils();
                return mOtUtils.Read();
            }
            catch (Exception ex)
            {
                lbViewFileError.Text = ex.Message;
            }
            return null;
        }

        #endregion
    }
}