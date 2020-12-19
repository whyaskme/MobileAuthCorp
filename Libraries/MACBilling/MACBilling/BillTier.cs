using MACSecurity;
namespace MACBilling
{
    public class BillTier : BillUtils
    {
        public BillTier(string ownerId, string tierType, string tierValues)
        {
            OwnerId = ownerId;
            TierType = tierType;
            TierValues = tierValues;
        }

        public string OwnerId { get; set; }
        public string TierType { get; set; }

        private string tiervalues;
        public string TierValues { get { return tiervalues; } set { tiervalues = Security.EncryptAndEncode(value, OwnerId); } }
    }
}
