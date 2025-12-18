using System;
using System.Data.Entity.Validation;
using System.Runtime.CompilerServices;

namespace QRESTModel.DAL
{
    class logEF
    {
        /// <summary>
        /// General purpose logging of any Entity Framework methods to database
        /// </summary>
        /// <param name="ex">Exception to log</param>
        public static void LogEFException(Exception ex, [CallerMemberName] string caller = "")
        {
            string err = "";

            if (ex is DbEntityValidationException dbex)
            {
                foreach (var eve in dbex.EntityValidationErrors)
                {
                    err += "[Entity Error] " + eve.Entry.Entity.GetType().Name;
                    foreach (var ve in eve.ValidationErrors)
                        err += " [Property]: " + ve.PropertyName + " [Error]: " + ve.ErrorMessage;
                }
            }
            else
            {
                Exception realerror = ex;
                while (realerror.InnerException != null)
                    realerror = realerror.InnerException;

                err = realerror.Message ?? "Unknown error";
            }

            db_Ref.CreateT_QREST_SYS_LOG(null, "ERROR", $"[EF][{caller}]: {err}");
        }
    }
}
