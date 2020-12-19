using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using MACServices;

namespace Admin.Reports.Events
{
    public partial class EventsDefault : System.Web.UI.Page
    {
        HiddenField _hiddenW;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Master != null)
            {
// ReSharper disable once UnusedVariable
                var oPanel = (Panel)Master.FindControl("pnlEventHistory");
                var bodyContainer = (Panel)Master.FindControl("BodyContainer");
                bodyContainer.Height = 1200;

                _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                _hiddenW.Value = "55020372a6e10b029035e0b4";
            }

            #region Set date range

// ReSharper disable once RedundantAssignment
            var ts = new TimeSpan(1, 0, 0, 0, 0);
            var endDate = DateTime.UtcNow;

            ts = new TimeSpan(31, 0, 0, 0, 0);
            var startDate = endDate.Add(-ts);

            if (popupDatepickerStartDate.Value == "")
                popupDatepickerStartDate.Value = startDate.ToString(CultureInfo.CurrentCulture);

            if (popupDatepickerEndDate.Value == "")
                popupDatepickerEndDate.Value = endDate.ToString(CultureInfo.CurrentCulture);

            #endregion

            SetClientList();
        }

        public void SetClientList()
        {
// ReSharper disable once NotAccessedVariable
            var clientCount = 0;
            dlClients.Items.Clear();

            var clientList = new MacList("", "Client", "", "_id,Name");
            foreach (var li in clientList.ListItems.Select(item => new ListItem { Text = item.Attributes["Name"], Value = item.Attributes["_id"] }))
            {
                clientCount++;

                if (Request["cid"] != null)
                {
                    if (li.Value == Request["cid"])
                    {
                        li.Selected = true;
                        btnEditClient.Visible = true;
                    }
                }

                dlClients.Items.Add(li);
            }

// ReSharper disable once LocalizableElement
            var rootItem = new ListItem { Text = @"Select a Client (" + (dlClients.Items.Count) + ")", Value = Constants.Strings.DefaultEmptyObjectId };
            dlClients.Items.Insert(0, rootItem);
        }
    }
}