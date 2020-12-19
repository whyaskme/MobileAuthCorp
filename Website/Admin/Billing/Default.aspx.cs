using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using MACBilling;
using MACServices;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using Newtonsoft.Json;

using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

using cnt = MACBilling.BillConstants;
using cs = MACServices.Constants.Strings;

public partial class Default : System.Web.UI.Page
{
    public HtmlForm _myForm { get; set; }

    HiddenField _hiddenE;
    HiddenField _hiddenH;
    HiddenField _hiddenI;
    HiddenField _hiddenD;
    HiddenField _hiddenT;
    HiddenField _userIpAddress;
    HiddenField _hiddenUserRole;

    HiddenField _hiddenV;

    HiddenField _hiddenW;

    Utils mUtils = new Utils();
    BillUtils myBillUtils = new BillUtils();

    MongoDatabase mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
    //MongoCollection mongoCollection;

    public string parentContainerId = "";
    public Decimal groupRate = 0.00M;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.Master != null)
        {
            if (Master != null)
            {
                _myForm = (HtmlForm)Page.Master.FindControl("formMain");

                _hiddenE = (HiddenField)Page.Master.FindControl("hiddenE");
                _hiddenH = (HiddenField)Page.Master.FindControl("hiddenH");
                _hiddenI = (HiddenField)Page.Master.FindControl("hiddenI");
                _hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");
                _hiddenT = (HiddenField)Page.Master.FindControl("hiddenT");
                _hiddenUserRole = (HiddenField)Page.Master.FindControl("hiddenL");
                _userIpAddress = (HiddenField)Master.FindControl("hiddenM");

                _hiddenV = (HiddenField)Master.FindControl("hiddenV");

                _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                _hiddenW.Value = "54fdf2abea6a57089cdd3f6a";
            }
        }
    }
}