using System;
using MACSecurity;
using MongoDB.Bson;

namespace MACBilling
{
    public class BillAddendum : BillUtils
    {
        public BillAddendum()
        {
            _id = ObjectId.GenerateNewId();
            AddendumId = _id.ToString();

            DateCreated = DateTime.UtcNow;

            IsMinimumPriceAdjustment = false;

            HasBeenAttached = false;
            AttachedToBillId = ObjectId.Parse(BillConstants.Common.DefaultEmptyObjectId);
        }

        public BillAddendum(string ownerId)
        {
            // Read in the object data from the db and return the populated addendum
            var myAddendum = (BillAddendum)Read(ownerId, "BillAddendum");

            if (myAddendum == null)
            {
                _id = ObjectId.GenerateNewId();
                AddendumId = _id.ToString();

                DateCreated = DateTime.UtcNow;

                OwnerId = ObjectId.Parse(ownerId);

                IsMinimumPriceAdjustment = false;

                HasBeenAttached = false;
                AttachedToBillId = ObjectId.Parse(BillConstants.Common.DefaultEmptyObjectId);
            }
            else
            {
                _id = myAddendum._id;
                AddendumId = myAddendum.AddendumId;
                CreatedById = myAddendum.CreatedById;
                DateCreated = myAddendum.DateCreated;

                IsMinimumPriceAdjustment = myAddendum.IsMinimumPriceAdjustment;

                HasBeenAttached = myAddendum.HasBeenAttached;
                AttachedToBillId = myAddendum.AttachedToBillId;
                DateAttached = myAddendum.DateAttached;

                OwnerId = myAddendum.OwnerId;
                OwnerName = myAddendum.OwnerName;
                OwnerType = myAddendum.OwnerType;

                // Have to do this. Otherwise, the values get re-encrypted again during the member copy operation!
                //var tmpValue = Security.DecodeAndDecrypt(myAddendum.Amount, OwnerId.ToString());
                //tmpValue = Security.DecodeAndDecrypt(tmpValue, OwnerId.ToString());

                Amount = myAddendum.Amount; // tmpValue;
                Notes = myAddendum.Notes;
            }
        }

        public BillAddendum(string ownerId, string addendumId)
        {
// ReSharper disable once RedundantCast
// ReSharper disable once RedundantThisQualifier
            var myAddendum = (BillAddendum)this.ReadAddendum(addendumId);

            if (myAddendum == null)
            {
                _id = ObjectId.GenerateNewId();
                AddendumId = _id.ToString();

                DateCreated = DateTime.UtcNow;

                OwnerId = ObjectId.Parse(ownerId);

                IsMinimumPriceAdjustment = false;

                HasBeenAttached = false;
                AttachedToBillId = ObjectId.Parse(BillConstants.Common.DefaultEmptyObjectId);
            }
            else
            {
                _id = myAddendum._id;
                AddendumId = myAddendum.AddendumId;
                CreatedById = myAddendum.CreatedById;
                DateCreated = myAddendum.DateCreated;

                IsMinimumPriceAdjustment = myAddendum.IsMinimumPriceAdjustment;

                HasBeenAttached = myAddendum.HasBeenAttached;
                AttachedToBillId = myAddendum.AttachedToBillId;
                DateAttached = myAddendum.DateAttached;

                OwnerId = myAddendum.OwnerId;
                OwnerName = myAddendum.OwnerName;
                OwnerType = myAddendum.OwnerType;

                // Have to do this. Otherwise, the values get re-encrypted again during the member copy operation!
                //var tmpAddAmount = Security.DecodeAndDecrypt(myAddendum.Amount, OwnerId.ToString());
                //tmpAddAmount = Security.DecodeAndDecrypt(tmpAddAmount, OwnerId.ToString());

                //var tmpAddNotes = Security.DecodeAndDecrypt(myAddendum.Notes, OwnerId.ToString());
                //tmpAddNotes = Security.DecodeAndDecrypt(tmpAddNotes, OwnerId.ToString());

                Amount = myAddendum.Amount; // tmpAddAmount;
                Notes = myAddendum.Notes; // tmpAddNotes;
            }
        }

        public ObjectId _id { get; set; }

        public string AddendumId { get; set; }
        
        public ObjectId CreatedById { get; set; }
        public DateTime DateCreated { get; set; }

        public bool HasBeenAttached { get; set; }
        public bool IsMinimumPriceAdjustment { get; set; }
        public ObjectId AttachedToBillId { get; set; }
        public DateTime DateAttached { get; set; }

        public ObjectId OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerType { get; set; }

        //private string amount;
        public string Amount { get; set; }
        //public string Amount { get { return amount; } set { amount = Security.EncryptAndEncode(value, OwnerId.ToString()); } }

        //private string notes;
        public string Notes { get; set; }
        //public string Notes { get { return notes; } set { notes = Security.EncryptAndEncode(value, OwnerId.ToString()); } }
    }
}
