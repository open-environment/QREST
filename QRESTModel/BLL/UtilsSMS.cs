using QRESTModel.DAL;
using System;
using System.Linq;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace QRESTModel.BLL
{
    public class UtilsSMS
    {

        public static bool sendSMS(string userIDX, string msg)
        {
            try
            {
                //retrieve user profile
                T_QREST_USERS u = db_Account.GetT_QREST_USERS_ByID(userIDX);
                if (u != null)
                {
                    string justNumbers = new String(u.PhoneNumber.Where(Char.IsDigit).ToArray());

                    if (justNumbers.Length == 10)
                        justNumbers = "1" + justNumbers;

                    //************* GET SMTP SERVER SETTINGS ****************************
                    string accountSid = db_Ref.GetT_QREST_APP_SETTING("SMS_SID");
                    string authToken = db_Ref.GetT_QREST_APP_SETTING("SMS_AUTH_TOKEN");
                    string phoneFrom = db_Ref.GetT_QREST_APP_SETTING("SMS_PHONE_NUM");

                    TwilioClient.Init(accountSid, authToken);

                    var message = MessageResource.Create(
                        body: msg,
                        from: new Twilio.Types.PhoneNumber("+" + phoneFrom),
                        to: new Twilio.Types.PhoneNumber("+" + justNumbers)
                    );
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
