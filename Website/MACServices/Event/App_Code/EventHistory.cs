using System;
using System.Text;
using System.Xml;
using System.Web.Services;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;

/// <summary>
/// Summary description for EventHistory
/// </summary>
[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class EventHistory : WebService {

    [WebMethod]
    public XmlDocument WsEventHistory(string clientId, string objectType, string startRecordNumber, string numberOfRecords, string sortField, string sortDirection, string startDate, string endDate)
    {
        var mUtils = new Utils();

        var sbResponse = new StringBuilder();

        // For debugging only
        if (String.IsNullOrEmpty(clientId))
            clientId = "";

        if (String.IsNullOrEmpty(objectType))
            objectType = "";

        if (String.IsNullOrEmpty(startRecordNumber))
            startRecordNumber = "0";

        if (String.IsNullOrEmpty(numberOfRecords))
            numberOfRecords = "10";

        if (String.IsNullOrEmpty(sortField))
            sortField = "Date";

        if (String.IsNullOrEmpty(sortDirection))
            sortDirection = "Desc";

        mUtils.InitializeXmlResponse(sbResponse);
        try
        {
            var currentRecord = 0;
            var eventList = new EventList(
                clientId, 
                objectType, 
                Convert.ToInt16(startRecordNumber), 
                Convert.ToInt16(numberOfRecords), 
                sortField, 
                sortDirection,
                startDate,
                endDate,
                "_id,Date,ObjectType,Details");

            foreach (var item in eventList.ListItems)
            {
                if (currentRecord == 0)
                    sbResponse.Append("<events totalrecords='" + item.Attributes["EventCount"] + "'>");

                sbResponse.Append("<event id='" + item.Attributes["_id"] + "'>");
                sbResponse.Append("<date>" + item.Attributes["Date"] + "</date>");
                sbResponse.Append("<details>" + item.Attributes["Details"].Replace("'", "").Replace("&","") + "</details>");
                sbResponse.Append("</event>");

                currentRecord++;

                if (currentRecord == eventList.ListItems.Count)
                    sbResponse.Append("</events>");
            }

            var rsp = mUtils.FinalizeXmlResponse(sbResponse, "EH");
            return rsp;
        }
        catch (Exception ex)
        {
            var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
            var exceptionEvent = new Event
            {
                EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
            };
            exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

            return mUtils.FinalizeXmlResponseWithError(String.Format("Exception: {0}", ex.Message), "EH");
        }
    }
}
