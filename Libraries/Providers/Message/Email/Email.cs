using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Mail;
using System.Collections;

namespace MACServices
{
    public class Email
    {
        //public ProviderEmail() { }

        // todo: fix when Origization.Client and result objects are is defined
        //public ??? SendByEmail(Origization.Client client, string subject, string body, string emailFrom, params string[] emailTos)
        //public bool Send(string subject, string body, string emailFrom, params string[] emailTos)
        //public bool Send(Client pClient)
        //{
        //    try
        //    {
        //        var SmtpClient = new SmtpClient
        //        {
        //            // todo: get parameters from Orginization.client object
        //            Host = "smtp.gmail.com",
        //            // todo: get parameters from Orginization.client object
        //            Port = 587,

        //            EnableSsl = true,
        //            DeliveryMethod = SmtpDeliveryMethod.Network,
        //            UseDefaultCredentials = false,
        //            Credentials = new System.Net.NetworkCredential(
        //                // todo: get parameters from Orginization.client object
        //                "userName ",
        //                "password")
        //        };

        //        MailMessage mail = new MailMessage();
        //        mail.From = new MailAddress("");
        //        //foreach (var email in emailTos)
        //        //{
        //        //    mail.To.Add(new MailAddress(email));
        //        //}

        //        //// set subject and encoding
        //        //mail.Subject = subject;
        //        mail.SubjectEncoding = System.Text.Encoding.UTF8;

        //        // set body-message and encoding
        //        mail.Body = "";// body;
        //        mail.BodyEncoding = System.Text.Encoding.UTF8;
        //        // text or html
        //        mail.IsBodyHtml = true;

        //        SmtpClient.Send(mail);
        //    }

        //    catch (SmtpException ex)
        //    {
        //        throw new ApplicationException
        //          ("SmtpException has occured: " + ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    // todo: need to return message sent object when defined
        //    return true;
        //}
    }
}
