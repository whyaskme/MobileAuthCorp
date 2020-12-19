using System;
using MongoDB.Bson;

namespace MACServices
{
    public class BaseRecordOperations
    {
        #region Properties

        public DateTime CreateDate { get; set; }
        public ObjectId CreatedById { get; set; }
        public string CreatedByIpAddress { get; set; }
        public DateTime ReadDate { get; set; }
        public ObjectId ReadById { get; set; }
        public string ReadByIpAddress { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ObjectId UpdatedById { get; set; }
        public string UpdatedByIpAddress { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedDate { get; set; }
        public ObjectId DeletedById { get; set; }
        public string DeletedByIpAddress { get; set; }
        public bool IsActivated { get; set; }
        public DateTime ActivatedDate { get; set; }
        public ObjectId ActivatedById { get; set; }
        public string ActivatedByIpAddress { get; set; }
        public DateTime DeactivatedDate { get; set; }
        public ObjectId DeactivatedById { get; set; }
        public string DeactivatedByIpAddress { get; set; }

        #endregion

        public BaseRecordOperations()
        {
            CreateDate = DateTime.UtcNow;
            CreatedById = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
            CreatedByIpAddress = String.Empty;
            ReadDate = new DateTime();
            ReadById = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
            ReadByIpAddress = String.Empty;
            UpdatedDate = new DateTime();
            UpdatedById = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
            UpdatedByIpAddress = String.Empty;
            IsDeleted = false;
            DeletedDate = new DateTime();
            DeletedById = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
            DeletedByIpAddress = String.Empty;
            IsActivated = true;
            ActivatedDate = new DateTime();
            ActivatedById = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
            ActivatedByIpAddress = String.Empty;
            DeactivatedDate = new DateTime();
            DeactivatedById = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
            DeactivatedByIpAddress = String.Empty;
        }
    }
}