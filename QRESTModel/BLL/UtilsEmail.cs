using QRESTModel.DAL;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace QRESTModel.BLL
{
    public class UtilsEmail
    {
        public static Tuple<string, string> GetSubjBody(string emailTemplateName, Dictionary<string, string> emailParams)
        {
            //************GET EMAIL CONTENT FROM TEMPLATE******************************
            T_QREST_EMAIL_TEMPLATE _temp = db_Ref.GetT_QREST_EMAIL_TEMPLATE_ByName(emailTemplateName);
            if (_temp != null)
            {
                string subj = _temp.SUBJ;
                string body = _temp.MSG;

                foreach (KeyValuePair<string, string> _item in emailParams)
                    body = body.Replace("{" + _item.Key + "}", _item.Value);

                return Tuple.Create(subj, body);
            }
            else
                db_Ref.CreateT_QREST_SYS_LOG("EMAIL", "ERROR", "No email template found for " + emailTemplateName);


            //FAILED IF GOT THIS FAR
            return null;
        }

        /// <summary>
        /// Sends out an email from the application. Returns true if successful.
        /// </summary>
        /// <param name="from">Email address of sender. Leave null to use default.</param>
        /// <param name="to">Email address sending to</param>
        /// <param name="cc">Carbon copied on the email</param>
        /// <param name="bcc">Blind carbon copied on the email</param>
        /// <param name="attach">Attachment as byte array</param>
        /// <param name="attachFileName">Attachment file name including extension e.g. test.doc</param>
        /// <param name="subject">Name of the email template to use</param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static bool SendEmail(string from, string to, List<string> cc, List<string> bcc, byte[] attach, string attachFileName, string subj, string body)
        {
            try
            {
                //************* GET SMTP SERVER SETTINGS ****************************
                string mailServer = db_Ref.GetT_QREST_APP_SETTING("EMAIL_SERVER");
                string Port = db_Ref.GetT_QREST_APP_SETTING("EMAIL_PORT") ?? "25";
                string smtpUser = db_Ref.GetT_QREST_APP_SETTING("EMAIL_SECURE_USER");
                string smtpUserPwd = db_Ref.GetT_QREST_APP_SETTING("EMAIL_SECURE_PWD");

                //*************SET MESSAGE SENDER IF NOT SUPPLIED*********************  
                from = from ?? db_Ref.GetT_QREST_APP_SETTING("EMAIL_FROM");

                //************** SEND EMAIL EITHER USING SENDGRID OR LOCAL SMTP ******
                bool SendStatus = false;
                if (mailServer == "smtp.sendgrid.net")
                    SendStatus = SendGridEmail(from, to, cc, bcc, subj, null, smtpUserPwd, body).GetAwaiter().GetResult();
                else
                    SendStatus = SendSMTPEmail(from, to, cc, bcc, attach, attachFileName, mailServer, subj, body);

                //*************LOG EMAIL SENT****************************************
                db_Ref.CreateT_QREST_SYS_LOG_EMAIL(from, to, null, subj, body, "EMAIL");

                return SendStatus;
            }
            catch (Exception ex)
            {
                db_Ref.CreateT_QREST_SYS_LOG("EMAIL", "ERROR", "[" + to + "] " + ex.Message);
            }

            //FAILED IF GOT THIS FAR
            return false;
        }

        /// <summary>
        /// Sends out an email from the application. Returns true if successful.
        /// </summary>
        /// <param name="from">Email address of sender. Leave null to use default.</param>
        /// <param name="to">Email address sending to</param>
        /// <param name="cc">Carbon copied on the email</param>
        /// <param name="bcc">Blind carbon copied on the email</param>
        /// <param name="attach">Attachment as byte array</param>
        /// <param name="attachFileName">Attachment file name including extension e.g. test.doc</param>
        /// <param name="emailTemplateName">Name of the email template to use</param>
        /// <param name="emailParams"></param>
        /// <returns></returns>
        public static bool SendEmail(string from, string to, List<string> cc, List<string> bcc, byte[] attach, string attachFileName, string emailTemplateName, Dictionary<string, string> emailParams)
        {
            //************GET EMAIL CONTENT FROM TEMPLATE******************************
            Tuple<string, string> subjbody = GetSubjBody(emailTemplateName, emailParams);

            if (subjbody != null)
                return SendEmail(from, to, cc, bcc, attach, attachFileName, subjbody.Item1, subjbody.Item2);
            else
                return false;
        }

        /// <summary>
        /// Sends out an email using local SMTP server
        /// </summary>
        /// <returns>true if successful</returns>
        private static bool SendSMTPEmail(string from, string to, List<string> cc, List<string> bcc, byte[] attach, string attachFileName, string mailServer, string subj, string body)
        {
            //************COMPOSE SMTP MAIL MESSAGE******************************
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage
            {
                Subject = subj,
                Body = body,
                IsBodyHtml = true,
                From = new System.Net.Mail.MailAddress(from)
            };
            message.To.Add(to);

            if (cc != null)
                foreach (string cc1 in cc)
                    message.CC.Add(cc1);

            if (bcc != null)
                foreach (string bcc1 in bcc)
                    message.Bcc.Add(bcc1);

            //*************ATTACHMENT ADDING**********************
            if (attach != null)
                message.Attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(attach), attachFileName));

            //*************SEND EMAIL*********************  
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(mailServer);
            smtp.Send(message);

            return true;
        }

        /// <summary>
        /// Sends out an email using SendGrid. 
        /// </summary>
        /// <returns>true if successful</returns>
        private static async Task<bool> SendGridEmail(string from, string to, List<string> cc, List<string> bcc, string subj, string body, string apiKey, string bodyHTML = null)
        {
            try
            {
                var client = new SendGridClient(apiKey);

                //******************** CONSTRUCT EMAIL ********************************************
                // Create the email object first, then add the properties.
                var msg = new SendGridMessage();

                // Add message properties.
                msg.Subject = subj;
                if (bodyHTML == null)
                    msg.AddContent(MimeType.Text, body);
                else
                    msg.AddContent(MimeType.Html, bodyHTML);
                msg.From = new EmailAddress(from);
                msg.AddTo(to);

                foreach (string cc1 in cc ?? Enumerable.Empty<string>())
                    msg.AddCc(cc1);

                foreach (string bcc1 in bcc ?? Enumerable.Empty<string>())
                    msg.AddBcc(bcc1);


                //******************** SEND EMAIL ****************************************************
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);


                //******************** RETURN RESPONSE ***********************************************
                if (response.StatusCode == HttpStatusCode.Accepted)
                    return true;
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    db_Ref.CreateT_QREST_SYS_LOG(from, "EMAIL ERR", "Sendgrid call fails authorization");
                else
                    db_Ref.CreateT_QREST_SYS_LOG(from, "EMAIL ERR", "Unknown send error");

                return false;
                //************************************************************************************

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    db_Ref.CreateT_QREST_SYS_LOG(from, "EMAIL ERR", ex.InnerException.ToString());
                else
                    db_Ref.CreateT_QREST_SYS_LOG(from, "EMAIL ERR", ex.Message ?? "Unknown exception");

                return false;
            }
        }

    }
}
