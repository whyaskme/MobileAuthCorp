using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using MACServices;

using cnt = MACServices.Constants.EventLog;

namespace UserControls
{
    public partial class EventTypes : UserControl
    {
        public static string MacServicesUrl = ConfigurationManager.AppSettings["MacServicesUrl"];

        protected void Page_Load(object sender, EventArgs e)
        {
            dlEventTypes.Items.Clear();
            var li = new ListItem {Text = @"All Events", Value = @"All Events"};
            dlEventTypes.Items.Add(li);

            //foreach (PropertyInfo property in cnt.GetType().GetProperties())
            //{
            //    if (statName == property.Name)
            //    {
            //        Int32 value = (Int32)property.GetValue(myNewStat);
            //        property.SetValue(myNewStat, value += incrementValue);
            //    }
            //}

            //foreach(var currentEventType in cnt.Client)
            //{
            //    var tmpVal = currentEventType;
            //}

            //foreach (var prop in typeof(cnt).GetProperties())
            //{
            //    if (prop.CanRead)
            //    {
            //        var value = prop.GetValue(instance, null) as string;
            //        //var item = new DropDownItem();
            //        //item.Description = value;
            //        //item.Value = value;
            //        //collection.Add(item);
            //    }
            //}
        }
    }
}