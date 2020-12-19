using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

public partial class Admin_Tests_TieredModelTests_TieredTest : System.Web.UI.Page
{
    private const string Test = "Tier";
    private const string OTPS = "OTPSent";
    private const string EUR = "EndUserReg";
    private const string AS = "AdsSent";
    private const string AC = "AdsClicked";
    private const string AF = "AdsFilled";

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnSerialize_Click(object sender, EventArgs e)
    {
        string value = Request["OTPT1"].ToString();
        var mCosting = new Dictionary<string, string>();
        if (!String.IsNullOrEmpty(OTPT1.Text))
        {
            mCosting.Add(OTPS, getTiers(OTPS, "OTPT"));
        }
        if (!String.IsNullOrEmpty(EUR1.Text))
        {
            mCosting.Add(EUR, getTiers(EUR, "EUR"));
        }
        if (!String.IsNullOrEmpty(AS1.Text))
        {
            mCosting.Add(AS, getTiers(AS, "AS"));
        }
        var mSer = JsonConvert.SerializeObject(mCosting);
        AddToLogAndDisplay(mSer);
        txtSer.Text = mSer;
    }

    private string getTiers(string costtier, string tbname)
    {
        var mTiers = new Dictionary<string, string>();
        lbError.Text = costtier + @".";

        var mPreviousRange = 0;
        for (var x = 1; x < 6; ++x)
        {
            var mTBID = tbname + x;

            var tiervalue = getTierValueById(mTBID);
            if (string.IsNullOrEmpty(tiervalue)) break; // quit if no value

            var mRangeCharge = tiervalue.Split(':'); // Range[0] Charge[1]
            if (mRangeCharge.Count() != 2)
            {
                lbError.Text += tbname + x + @" Invalid";
                break; // must be 2 Range & Charge
            }
            if (!IsNumeric(mRangeCharge[0]))
            {
                lbError.Text += tbname + x + @" Range not numeric";
                break; // must be 2 Range & Charge
            }
            int mCurrentRange;
            Int32.TryParse(mRangeCharge[0], out mCurrentRange);
            if (mCurrentRange < mPreviousRange)
            {
                lbError.Text += tbname + x + @" Range not numeric";
                break; // must be 2 Range & Charge
            }
            mPreviousRange = mCurrentRange;
            if (!IsCurrency(mRangeCharge[1]))
            {
                lbError.Text += tbname + x + @" Charge is not currency";
                break; // must be 2 Range & Charge
            }
            mTiers.Add(mRangeCharge[0], mRangeCharge[1]);
        }
        var mSer = JsonConvert.SerializeObject(mTiers);
        AddToLogAndDisplay(mSer);
        return mSer;
    }

    private string getTierValueById(string pId)
    {
        var tiercontrol = (TextBox)FindControl(pId);
        if (tiercontrol != null)
            return tiercontrol.Text;
        switch (pId)
        {
            case "OTPT1":
                return OTPT1.Text;
            case "OTPT2":
                return OTPT2.Text;
            case "OTPT3":
                return OTPT3.Text;
            case "OTPT4":
                return OTPT4.Text;
            case "OTPT5":
                return OTPT5.Text;
            case "OTPT6":
                return OTPT6.Text;

            case "EUR1":
                return EUR1.Text;
            case "EUR2":
                return EUR2.Text;
            case "EUR3":
                return EUR3.Text;
            case "EUR4":
                return EUR4.Text;
            case "EUR5":
                return EUR5.Text;
            case "EUR6":
                return EUR6.Text;

            case "AS1":
                return AS1.Text;
            case "AS2":
                return AS2.Text;
            case "AS3":
                return AS3.Text;
            case "AS4":
                return AS4.Text;
            case "AS5":
                return AS5.Text;
            case "AS6":
                return AS6.Text;

        }
        return null;
    }
   
    private bool IsNumeric(string value)
    {
        int number;
        return Int32.TryParse(value, out number);
    }
    private bool IsCurrency(string value)
    {
        decimal number;
        return Decimal.TryParse(value, out number);
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
}