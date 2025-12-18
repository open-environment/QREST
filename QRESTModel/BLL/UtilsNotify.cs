using QRESTModel.DAL;
using System;
using System.Collections.Generic;

namespace QRESTModel.BLL
{
    public class UtilsNotify
    {
        /// <summary>
        /// Notifies a user, based on their notification preferences saved in their user profile.
        /// </summary>
        /// <param name="uSER_IDX">IDX of the person being notified.</param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <param name="attach"></param>
        /// <param name="attachFileName"></param>
        /// <param name="emailTemplateName"></param>
        /// <param name="emailParams"></param>
        /// <param name="cREATE_USER">Person creating the notification.</param>
        /// <returns></returns>
        public static bool NotifyUser(string uSER_IDX, List<string> cc, List<string> bcc, byte[] attach, string attachFileName, string emailTemplateName, Dictionary<string, string> emailParams, string cREATE_USER)
        {
            Tuple<string, string> subjbody = UtilsEmail.GetSubjBody(emailTemplateName, emailParams);
            return NotifyUser(uSER_IDX, cc, bcc, attach, attachFileName, emailTemplateName, subjbody.Item1, subjbody.Item2, cREATE_USER);
        }

        public static bool NotifyUser(string uSER_IDX, List<string> cc, List<string> bcc, byte[] attach, string attachFileName, string nOTIFY_TYPE, string nOTIFY_TITLE, string nOTIFY_DESC, string cREATE_USER)
        {
            bool overallStatus = true;

            //retrieve user profile
            T_QREST_USERS u = db_Account.GetT_QREST_USERS_ByID(uSER_IDX);
            if (u != null)
            {
                //in-app notification
                if (u.NOTIFY_APP_IND == true)
                {
                    Guid? NotifyIDX = db_Account.InsertUpdateT_QREST_USER_NOTIFICATION(null, uSER_IDX, System.DateTime.Now, nOTIFY_TYPE, nOTIFY_TITLE, nOTIFY_DESC, false, cREATE_USER);
                    if (NotifyIDX == null)
                        overallStatus = false;
                }

                //email
                if (u.NOTIFY_EMAIL_IND == true)
                {
                    bool EmailStatus = UtilsEmail.SendEmail(null, u.Email, cc, bcc, attach, attachFileName, nOTIFY_TITLE, nOTIFY_DESC);
                    if (EmailStatus == false)
                        overallStatus = false;
                }

                //sms
                //if (u.NOTIFY_SMS_IND == true)
                //{
                //    bool smsStatus = UtilsSMS.sendSMS(uSER_IDX, nOTIFY_DESC);
                //    if (smsStatus == false)
                //        overallStatus = false;
                //}

                return overallStatus;
            }
            else
                return false;
        }
    }
}
