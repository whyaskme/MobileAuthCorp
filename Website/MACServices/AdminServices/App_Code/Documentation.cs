using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;

using MACServices;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;

using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]

public class Documentation : System.Web.Services.WebService {

    public Documentation () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public XmlDocument GetDocumentation(string TopicId, string GetSubTopics) 
    {
        var myUtils = new Utils();
        var myHelpUtils = new HelpUtils();

        var _topicId = TopicId;
        var _getSubTopics = false;

        if (string.IsNullOrEmpty(TopicId))
            _topicId = "54984113ead6361fcc0b42b5"; //5490bfd0ead63627d88e3e23

        if (!string.IsNullOrEmpty(GetSubTopics))
            _getSubTopics = Convert.ToBoolean(GetSubTopics);

        // start the XML response
        var sbResponse = new StringBuilder();

        myUtils.InitializeXmlResponse(sbResponse);

        HelpTopic myTopic = new HelpTopic(_topicId);

        sbResponse.Append("<topics topic='" + myTopic.Category + "' topicdesc='" + myTopic.Description + "' getsubtopics='" + _getSubTopics + "'>");

        sbResponse.Append(" <topic id='" + myTopic._id + "'>");

        var topicDetails = myTopic.Details;
        if (string.IsNullOrEmpty(topicDetails))
            topicDetails = "No content";

        sbResponse.Append("     <description>" + myTopic.Description + "</description>");
        sbResponse.Append("     <details>" + topicDetails + "</details>");
        sbResponse.Append(" </topic>");

        if (_getSubTopics)
        {
            var query = Query.EQ("Category", myTopic.Category);
            var sortBy = SortBy.Ascending("SubCategory");

            var subTopics = myHelpUtils.mongoDBConnectionPool.GetCollection("Help").FindAs<HelpTopic>(query).SetSortOrder(sortBy);
            foreach (HelpTopic currentSubTopic in subTopics)
            {
                if (currentSubTopic.Description != myTopic.Description)
                {
                    sbResponse.Append(" <topic id='" + currentSubTopic._id + "'>");
                    sbResponse.Append("     <description>" + FormatTopicTitle(currentSubTopic.Description) + "</description>");
                    sbResponse.Append("     <details>" + currentSubTopic.Details + "</details>");
                    sbResponse.Append(" </topic>");
                }
            }
        }

        sbResponse.Append("</topics>");

        var rsp = myUtils.FinalizeXmlResponse(sbResponse, "DC");
        return rsp;
    }

    public string FormatTopicTitle(string topicTitle)
    {
        topicTitle = topicTitle.Replace("1) ", "");
        topicTitle = topicTitle.Replace("2) ", "");
        topicTitle = topicTitle.Replace("3) ", "");
        topicTitle = topicTitle.Replace("4) ", "");
        topicTitle = topicTitle.Replace("5) ", "");
        topicTitle = topicTitle.Replace("6) ", "");
        topicTitle = topicTitle.Replace("7) ", "");
        topicTitle = topicTitle.Replace("8) ", "");
        topicTitle = topicTitle.Replace("9) ", "");
        topicTitle = topicTitle.Replace("10) ", "");

        return topicTitle;
    }
}
