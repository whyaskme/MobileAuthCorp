using MongoDB.Bson;
namespace STLib
{
    public class StiOperatorSite : MongoDBData
    {
        public StiOperatorSite()
        {
            _id = ObjectId.GenerateNewId();
            _t = "StiOperatorSite";

            OperatorId = "";
            SiteId = "";
            SiteUserName = "";
            SitePassword = "";
            GeoInfo = "ZsUiDymAiyVr/aQxwqC60c50qCfhJ9WPvZo3TrNAmXxD20onJILaqkmK+CGEDzr7tveVE=";
            AuthorizedIp = "184.182.215.167";
            macClientId = "";
            macClientName = "";
        }

        public ObjectId _id { get; set; }
        public string _t { get; set; }
        public string OperatorId { get; set; }
        public string SiteId { get; set; }
        public string SiteUserName { get; set; }
        public string SitePassword { get; set; }
        public string GeoInfo { get; set; }
        public string AuthorizedIp { get; set; }

        public string macClientId { get; set; }
        public string macClientName { get; set; }
    }

}
