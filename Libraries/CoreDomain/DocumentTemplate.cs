using System;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MACServices
{
    public class DocumentTemplate
    {
        public DocumentTemplate()
        {
            _id = ObjectId.GenerateNewId();
            _t = "DocumentTemplate";
            MessageClass = "";
            MessageDesc = "";
            MessageFormat = "";
            MessageFromAddress = "";
            MessageFromName = "";
        }

        //public DocumentTemplate(string templateType)
        //{
        //    _id = ObjectId.GenerateNewId();
        //    _t = "DocumentTemplate";
        //    MessageClass = "";
        //    MessageDesc = templateType;
        //    MessageFormat = "";
        //    MessageFromAddress = "";
        //    MessageFromName = "";
        //}


        public DocumentTemplate(string messageClass)
        {
            Utils myUtils = new Utils();

            DocumentTemplate myTemplate;

            try
            {
                var query = Query.EQ("MessageClass", messageClass);
                var mongoCollection = myUtils.mongoDBConnectionPool.GetCollection("TypeDefinitions");

                myTemplate = mongoCollection.FindOneAs<DocumentTemplate>(query);

                _id = myTemplate._id;

                MessageClass = myTemplate.MessageClass;
                MessageDesc = myTemplate.MessageDesc;
                MessageFormat = myTemplate.MessageFormat;
                MessageFromAddress = myTemplate.MessageFromAddress;
                MessageFromName = myTemplate.MessageFromName;
            }
            catch (Exception ex)
            {
                var errMsg = ex.ToString();
            }
        }

        public ObjectId _id { get; set; }
        public string _t { get; set; }
        public string MessageClass { get; set; }
        public string MessageDesc { get; set; }
        public string MessageFormat { get; set; }
        public string MessageFromAddress { get; set; }
        public string MessageFromName { get; set; }
    }
}