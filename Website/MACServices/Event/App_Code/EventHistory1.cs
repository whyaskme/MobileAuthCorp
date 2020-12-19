using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web.Services;

using MACServices;
using tokenKeys = MACServices.Constants.TokenKeys;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class EventHistory1 : WebService {

    [WebMethod]
    public XmlDocument WsEventHistory1(string data)
    {
        var mUtils = new Utils();

        // start the XML response
        var sbResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(sbResponse);
        
        var myData = new Dictionary<string, string> {{dk.ServiceName, "EventHistory1"}, {dk.Debug, "yes"}};
        if (data.Length <4)
        {
            string parameters = "Request=";
            if (data.StartsWith("1")) {
                parameters += "GetHistory";
                if (data.Contains("a"))
                    parameters += "|clientId=000000000000000000000000";
                else
                    parameters += "|clientId=111111111111111111111111";
                if (data.EndsWith("o")) 
                    parameters += "|ObjectType=Otp";
                else if (data.EndsWith("e"))
                    parameters += "|ObjectType=EndUser";
                else
                    parameters += "|ObjectType=Client";
                parameters += "|StartRecordNumber=0";
                parameters += "|NumberOfRecords=9";
                parameters += "|SortField=Date";
                parameters += "|SortDirection=Desc";
                parameters += "|StartDate=";
                parameters += "|EndDate=";
            }
            else if (data.StartsWith("2"))
            {
                parameters += "GetEventTypes";
            }
            else
            {
                return mUtils.EventLogError_FinalizeXmlResponse(myData[dk.ServiceName],
                        null, "Debug request invalid!", null);
            }
            data = parameters;
        }
        if (mUtils.ParseIntoDictionary(data, myData, '=') == false)
            return mUtils.EventLogError_FinalizeXmlResponse(myData[dk.ServiceName],
                                    null, "Corrupt or bad request data!", null);
        XmlDocument rsp;
        try
        {
            switch (myData[dk.Request])
            {
                case "GetHistory":
                {
                    var currentRecord = 0;
                    var eventList = new EventList(
                        myData["clientId"],
                        myData["ObjectType"],
                        Convert.ToInt16(myData["StartRecordNumber"]),
                        Convert.ToInt16(myData["NumberOfRecords"]),
                        myData["SortField"],
                        myData["SortDirection"],
                        myData["StartDate"].Trim(),
                        myData["EndDate"].Trim(),
                        //myData["RefreshEventTypeListBox"],
                        "_id,Date,EventTypeId,EventTypeName,EventTypeDesc");

                    foreach (var item in eventList.ListItems)
                    {
                        if (currentRecord == 0)
                            sbResponse.Append("<events totalrecords='" + item.Attributes["EventCount"] + "'>");

                        sbResponse.Append("<event id='" + item.Attributes["_id"] + "'>");
                        sbResponse.Append("<date>" + item.Attributes["Date"] + "</date>");
                        sbResponse.Append("<details>");
                        sbResponse.Append("<![CDATA[");
                        sbResponse.Append(item.Attributes["EventTypeDesc"].Replace("'", "").Replace("&", ""));
                        sbResponse.Append("]]>");
                        sbResponse.Append("</details>");
                        sbResponse.Append("</event>");

                        currentRecord++;

                        if (currentRecord == eventList.ListItems.Count)
                            sbResponse.Append("</events>");
                    }

                    if (Convert.ToBoolean(myData["RefreshEventTypeListBox"]))
                    {
                        // Get event types here...
                        DateTime startDate = String.IsNullOrEmpty(myData["StartDate"].Trim()) ? DateTime.UtcNow.AddDays(-30) : DateTime.Parse(myData["StartDate"].Trim());
                        DateTime endDate = String.IsNullOrEmpty(myData["EndDate"].Trim()) ? DateTime.UtcNow.AddDays(1) : DateTime.Parse(myData["EndDate"].Trim());

                        var result = DateTime.Compare(startDate, endDate);
                        if (result == 0)
                        {
                            endDate = endDate.AddDays(1); // start date is the same as end date
                            endDate = endDate.AddSeconds(-1);
                        }
                        else if (result > 0)
                            startDate = endDate.AddDays(-1); // start date is greater than end date

                        var eventTypesList = mUtils.GetEventTypes(myData["clientId"], startDate, endDate);

                        sbResponse.Append(eventTypesList);
                    }

                    rsp = mUtils.FinalizeXmlResponse(sbResponse, String.Empty);
                    return rsp;
                }
            }
            return mUtils.FinalizeXmlResponseWithError(myData[dk.ServiceName] + " invalid request " + myData[dk.Request], String.Empty);
        }
        catch (Exception ex)
        {
            var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
            var exceptionEvent = new Event
            {
                EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
            };
            exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

            return mUtils.FinalizeXmlResponseWithError(myData[dk.ServiceName] + ".Exception: " + ex.Message, String.Empty);
        }
    }
}
