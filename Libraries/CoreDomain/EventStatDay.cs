using System;

namespace MACServices
{
    public class EventStatDay
    {
        public EventStatDay()
        {
            Date = DateTime.UtcNow.Date;

            // Advertising
            AdMessageSent = 0;
            AdEnterOtpScreenSent = 0;
            AdVerificationScreenSent = 0;

            // End User
            EndUserRegister = 0;
            EndUserVerify = 0;

            // Events
            Events = 0;
            Exceptions = 0;

            // Delivery
            OtpSentEmail = 0;
            OtpSentSms = 0;
            OtpSentVoice = 0;

            // Validation
            OtpValid = 0;
            OtpInvalid = 0;
            OtpExpired = 0;
        }

        public DateTime Date { get; set; }

        public Int32 AdMessageSent { get; set; }
        public Int32 AdEnterOtpScreenSent { get; set; }
        public Int32 AdVerificationScreenSent { get; set; }

        public Int32 EndUserRegister { get; set; }
        public Int32 EndUserVerify { get; set; }

        public Int32 Events { get; set; }
        public Int32 Exceptions { get; set; }

        public Int32 OtpSentEmail { get; set; }
        public Int32 OtpSentSms { get; set; }
        public Int32 OtpSentVoice { get; set; }
        public Int32 OtpValid { get; set; }
        public Int32 OtpInvalid { get; set; }
        public Int32 OtpExpired { get; set; }
    }
}
