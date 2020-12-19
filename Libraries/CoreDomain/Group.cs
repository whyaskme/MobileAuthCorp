using System;
using System.Configuration;
using MongoDB.Bson;

namespace MACServices
{
    public class Group : Base
    {
        public Group() { }

        public Group(string groupId)
        {
            if (String.IsNullOrEmpty(groupId))
            {
                _id = ObjectId.GenerateNewId();
                Organization = new Organization();
                Enabled = true;
                Name = String.Empty;
                MacOasServicesUrl = ConfigurationManager.AppSettings["DefaultMacOasServicesUrl"];
                GroupType = "RootGroup";
                CreatedDate = DateTime.UtcNow;
                CreatedById = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
                UpdatedDate = DateTime.UtcNow;
                UpdatedById = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
                DisabledDate = DateTime.MinValue;
                DisabledById = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);

                OwnerLogoUrl = Constants.Common.EmptyOwnerLogoUrl;
            }
            else
            {
                _id = ObjectId.Parse(groupId);

                var myGroup = (Group) Read();
                if (myGroup == null) return;
                Organization = myGroup.Organization;
                Enabled = myGroup.Enabled;
                Name = myGroup.Name;
                MacOasServicesUrl = myGroup.MacOasServicesUrl;
                GroupType = myGroup.GroupType;
                CreatedDate = myGroup.CreatedDate;
                CreatedById = myGroup.CreatedById;
                UpdatedDate = myGroup.CreatedDate;
                UpdatedById = myGroup.CreatedById;
                DisabledDate = myGroup.CreatedDate;
                DisabledById = myGroup.CreatedById;
                Relationships = myGroup.Relationships;

                OwnerLogoUrl = myGroup.OwnerLogoUrl ?? Constants.Common.EmptyOwnerLogoUrl;
            }
        }

        public Organization Organization { get; set; }
        public bool Enabled { get; set; }
        public string GroupType { get; set; }
        public DateTime CreatedDate { get; set; }
        public ObjectId CreatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ObjectId UpdatedById { get; set; }
        public DateTime DisabledDate { get; set; }
        public ObjectId DisabledById { get; set; }
        public string MacOasServicesUrl { get; set; }
        public string OwnerLogoUrl { get; set; }
    }
}