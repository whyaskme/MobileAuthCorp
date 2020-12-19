using System.Collections.Generic;
using MongoDB.Bson;

namespace MACServices
{
    public class Base
    {
        public Base()
        {
            Relationships = new List<Relationship>();
            Name = "";
        }

        #region Properties

        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public List<Relationship> Relationships { get; set; }
        public ObjectId ClientId { get; set; }

        public void Create()
        {
            var mUtils = new Utils();
            mUtils.ObjectCreate(this);
        }

        public object Read()
        {
            var mUtils = new Utils();
            var objectType = GetType().ToString().ToLower().Replace("macservices.", "");
            switch (objectType)
            {
                case "administrator":
                    return mUtils.ObjectRead(_id.ToString(), objectType, "");

                case "client":
                    return mUtils.ObjectRead(_id.ToString(), objectType, "");

                case "group":
                    return mUtils.ObjectRead(_id.ToString(), objectType, "");

                case "userprofile":
                    return mUtils.ObjectRead(_id.ToString(), objectType, "");
            }

            //return mUtils.ObjectRead(_id.ToString(), objectType, "");
            return this;
        }

        public string Update()
        {
            var mUtils = new Utils();
            return mUtils.ObjectUpdate(this, _id.ToString());
        }

        public void Delete()
        {
        }

        public void Disable()
        {
        }

        #endregion
    }
}