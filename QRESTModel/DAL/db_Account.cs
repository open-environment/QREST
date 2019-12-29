using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QRESTModel.BLL;

namespace QRESTModel.DAL
{
    public class UserOrgDisplayType
    {
        public Guid ORG_USER_IDX { get; set; }
        public string USER_IDX { get; set; }
        public string ORG_ID { get; set; }
        public string ORG_NAME { get; set; }
        public string USER_EMAIL { get; set; }
        public string USER_NAME { get; set; }
        public string ACCESS_LEVEL { get; set; }
        public string ACCESS_LEVEL_DESC { get; set; }
        public string STATUS_IND { get; set; }
        public string STATUS_IND_DESC { get; set; }
        public DateTime? CREATE_DT { get; set; }
    }

    public class db_Account
    {
        //***************** T_QREST_USERS ***************************************        
        public static List<T_QREST_USERS> GetT_QREST_USERS(bool ConfirmedOnly)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from u in ctx.T_QREST_USERS.AsNoTracking()
                            where (ConfirmedOnly == true ? u.EmailConfirmed == true : true)
                            orderby u.FNAME, u.LNAME
                            select u).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static T_QREST_USERS GetT_QREST_USERS_ByID(string userIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from u in ctx.T_QREST_USERS.AsNoTracking()
                            where u.USER_IDX == userIDX
                            select u).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_QREST_USERS> GetT_QREST_USERS_byOrgID(string OrgID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    if (OrgID == null)
                        return (from u in ctx.T_QREST_USERS.AsNoTracking()
                                select u).ToList();
                    else
                        return (from u in ctx.T_QREST_USERS.AsNoTracking()
                                join uo in ctx.T_QREST_ORG_USERS.AsNoTracking() on u.USER_IDX equals uo.USER_IDX
                                where uo.ORG_ID == OrgID
                                select u).ToList();

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }



        //****************** T_QREST_USER_ROLES*************************************
        public static List<T_QREST_ROLES> GetT_QREST_ROLESNotInUserIDX(string userIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    //first get all roles
                    var allRoles = (from r in ctx.T_QREST_ROLES select r);

                    //next get all roles that user has
                    var rolesUserHas = from r in ctx.T_QREST_ROLES
                                       where r.T_QREST_USERS.Any(c => c.USER_IDX == userIDX)
                                       select r;

                    //then get exclusions
                    var rolesNotInRole = allRoles.Except(rolesUserHas);

                    return rolesNotInRole.OrderBy(a => a.Name).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_QREST_USERS> GetT_QREST_USERSInRole(string roleName)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from u in ctx.T_QREST_USERS
                            where u.T_QREST_ROLES.Any(c => c.Name == roleName)
                            select u).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }



        //***************** T_QREST_ORG_USERS ***************************************        
        public static Guid? InsertUpdateT_QREST_ORG_USERS(string uSER_IDX, string oRG_ID, string aCCESS_LEVEL, string sTATUS_IND, string cREATE_USER)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    T_QREST_ORG_USERS e = (from c in ctx.T_QREST_ORG_USERS
                                           where c.USER_IDX == uSER_IDX
                                           && c.ORG_ID == oRG_ID
                                           select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_ORG_USERS
                        {
                            ORG_USER_IDX = Guid.NewGuid(),
                            USER_IDX = uSER_IDX,
                            ORG_ID = oRG_ID,
                            CREATE_DT = System.DateTime.Now,
                            CREATE_USER_IDX = cREATE_USER
                        };
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.Now;
                        e.MODIFY_USER_IDX = cREATE_USER;
                    }

                    if (aCCESS_LEVEL != null) e.ACCESS_LEVEL = aCCESS_LEVEL;
                    if (sTATUS_IND != null) e.STATUS_IND = sTATUS_IND;

                    if (insInd)
                        ctx.T_QREST_ORG_USERS.Add(e);

                    ctx.SaveChanges();
                    return e.ORG_USER_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        /// <summary>
        /// Returns all people who are pending approval for organizations that the selected user is an administrator for
        /// </summary>
        /// <param name="uSER_IDX"></param>
        /// <returns></returns>
        public static List<UserOrgDisplayType> GetPendingUsersByAdminUser(string aDMIN_USER_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var x = (from auo in ctx.T_QREST_ORG_USERS
                             join uuo in ctx.T_QREST_ORG_USERS on auo.ORG_ID equals uuo.ORG_ID
                             join u in ctx.T_QREST_USERS on uuo.USER_IDX equals u.USER_IDX
                             where uuo.STATUS_IND == "P"
                             && auo.ACCESS_LEVEL == "A"
                             && auo.STATUS_IND == "A"
                             && auo.USER_IDX == aDMIN_USER_IDX
                             select new UserOrgDisplayType
                             {
                                 ORG_USER_IDX = uuo.ORG_USER_IDX,
                                 USER_IDX = uuo.USER_IDX,
                                 ORG_ID = uuo.ORG_ID,
                                 ACCESS_LEVEL = uuo.ACCESS_LEVEL,
                                 STATUS_IND = uuo.STATUS_IND,
                                 CREATE_DT = uuo.CREATE_DT,
                                 USER_NAME = u.FNAME + " " + u.LNAME
                             }).ToList();

                    return x;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        /// <summary>
        /// For global admins, shows all pending users
        /// </summary>
        /// <returns></returns>
        public static List<UserOrgDisplayType> GetAllPendingUsers()
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var x = (from uo in ctx.T_QREST_ORG_USERS
                             join u in ctx.T_QREST_USERS on uo.USER_IDX equals u.USER_IDX
                             where uo.STATUS_IND == "P"
                             select new UserOrgDisplayType
                             {
                                 ORG_USER_IDX = uo.ORG_USER_IDX,
                                 USER_IDX = uo.USER_IDX,
                                 ORG_ID = uo.ORG_ID,
                                 ACCESS_LEVEL = uo.ACCESS_LEVEL,
                                 STATUS_IND = uo.STATUS_IND,
                                 CREATE_DT = uo.CREATE_DT,
                                 USER_NAME = u.FNAME + " " + u.LNAME
                             }).ToList();

                    return x;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        /// <summary>
        /// Returns all users who are associated with an organization, optionally filtered by Access Level
        /// </summary>
        /// <param name="oRG_ID"></param>
        /// <param name="aCCESS_LEVEL"></param>
        /// <returns></returns>
        public static List<UserOrgDisplayType> GetT_QREST_ORG_USERS_ByOrgID(string oRG_ID, string aCCESS_LEVEL, string sTATUS)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from uo in ctx.T_QREST_ORG_USERS.AsNoTracking()
                            join u in ctx.T_QREST_USERS.AsNoTracking() on uo.USER_IDX equals u.USER_IDX
                            join a in ctx.T_QREST_REF_ACCESS_LEVEL.AsNoTracking() on uo.ACCESS_LEVEL equals a.ACCESS_LEVEL
                            join s in ctx.T_QREST_REF_USER_STATUS.AsNoTracking() on uo.STATUS_IND equals s.STATUS_IND
                            where uo.ORG_ID == oRG_ID
                            && (sTATUS == null ? true : uo.STATUS_IND == sTATUS)
                            && (aCCESS_LEVEL == null ? true : uo.ACCESS_LEVEL == aCCESS_LEVEL)
                            select new UserOrgDisplayType
                            {
                                ORG_USER_IDX = uo.ORG_USER_IDX,
                                USER_IDX = uo.USER_IDX,
                                USER_EMAIL = u.Email,
                                ORG_ID = uo.ORG_ID,
                                ACCESS_LEVEL = uo.ACCESS_LEVEL,
                                ACCESS_LEVEL_DESC = a.ACCESS_LEVEL_DESC,
                                STATUS_IND = uo.STATUS_IND,
                                STATUS_IND_DESC = s.STATUS_IND_DESC,
                                CREATE_DT = uo.CREATE_DT,
                                USER_NAME = u.FNAME + " " + u.LNAME
                            }).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        public static int DeleteT_QREST_ORG_USER(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_ORG_USERS rec = ctx.T_QREST_ORG_USERS.Find(id);
                    ctx.T_QREST_ORG_USERS.Remove(rec);
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }


        /// <summary>
        /// Checks if the user is an admin of a specified organization
        /// </summary>
        /// <param name="uSER_IDX"></param>
        /// <param name="oRG_ID"></param>
        /// <returns></returns>
        public static bool IsAnOrgAdmin(string uSER_IDX, string oRG_ID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    int iCount = (from a in ctx.T_QREST_ORG_USERS
                                  where a.USER_IDX == uSER_IDX
                                  && a.ACCESS_LEVEL == "A"
                                  && a.STATUS_IND == "A"
                                  && a.ORG_ID == oRG_ID
                                  select a).Count();

                    return iCount > 0;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks if the user has Level 1 rights to a specified organization
        /// </summary>
        /// <param name="uSER_IDX"></param>
        /// <param name="oRG_ID"></param>
        /// <returns></returns>
        /// 
        public static bool IsOrgLvl1(string uSER_IDX, string oRG_ID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    int iCount = (from a in ctx.T_QREST_ORG_USERS
                                  where a.USER_IDX == uSER_IDX
                                  && (a.ACCESS_LEVEL == "A" || a.ACCESS_LEVEL == "Q" || a.ACCESS_LEVEL == "U")
                                  && a.STATUS_IND == "A"
                                  && a.ORG_ID == oRG_ID
                                  select a).Count();

                    return iCount > 0;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return false;
                }
            }
        }


        /// <summary>
        /// Checks if the user has Level 2 rights to a specified organization
        /// </summary>
        /// <param name="uSER_IDX"></param>
        /// <param name="oRG_ID"></param>
        /// <returns></returns>
        /// 
        public static bool IsOrgLvl2(string uSER_IDX, string oRG_ID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    int iCount = (from a in ctx.T_QREST_ORG_USERS
                                  where a.USER_IDX == uSER_IDX
                                  && (a.ACCESS_LEVEL == "A" || a.ACCESS_LEVEL == "Q")
                                  && a.STATUS_IND == "A"
                                  && a.ORG_ID == oRG_ID
                                  select a).Count();

                    return iCount > 0;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks if the user is an admin of any organization
        /// </summary>
        /// <param name="uSER_IDX"></param>
        /// <returns></returns>
        public static bool IsAnOrgAdmin(string uSER_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    int iCount = (from a in ctx.T_QREST_ORG_USERS
                                  where a.USER_IDX == uSER_IDX
                                  && a.ACCESS_LEVEL == "A"
                                  && a.STATUS_IND == "A"
                                  select a).Count();

                    return iCount > 0;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return false;
                }
            }
        }


        /// <summary>
        /// Checks if the user belongs to any organization
        /// </summary>
        /// <param name="uSER_IDX"></param>
        /// <returns></returns>
        public static bool BelongsToAnyOrg(string uSER_IDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    int iCount = (from a in ctx.T_QREST_ORG_USERS
                                  where a.USER_IDX == uSER_IDX
                                  && a.STATUS_IND == "A"
                                  select a).Count();

                    return iCount > 0;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return false;
                }
            }
        }


        /// <summary>
        /// Checks if the user belongs to a specified organization as an active user
        /// </summary>
        /// <param name="uSER_IDX"></param>
        /// <param name="oRG_ID"></param>
        /// <param name="CanEditToo"></param>
        /// <returns>True if user is active and has access</returns>
        public static bool CanAccessThisOrg(string uSER_IDX, string oRG_ID, bool CanEditToo)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    int iCount = (from a in ctx.T_QREST_ORG_USERS
                                  where a.USER_IDX == uSER_IDX
                                  && a.ORG_ID == oRG_ID
                                  && a.STATUS_IND == "A"
                                  && (CanEditToo == true ? a.ACCESS_LEVEL != "R" : true)
                                  select a).Count();

                    return iCount > 0;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return false;
                }
            }
        }


        /// <summary>
        /// Checks if the user can access to a specified site record, and optionally whether he/she can edit the record 
        /// </summary>
        /// <param name="uSER_IDX"></param>
        /// <param name="sITE_IDX"></param>
        /// <param name="CanEditToo"></param>
        /// <returns></returns>
        public static bool CanAccessThisSite(string uSER_IDX, Guid sITE_IDX, bool CanEditToo)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    int iCount = (from a in ctx.T_QREST_ORG_USERS
                                  join s in ctx.T_QREST_SITES on a.ORG_ID equals s.ORG_ID
                                  where a.USER_IDX == uSER_IDX
                                  && s.SITE_IDX == sITE_IDX
                                  && a.STATUS_IND == "A"
                                  && (CanEditToo == true ? a.ACCESS_LEVEL != "R" : true)
                                  select a).Count();

                    return iCount > 0;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return false;
                }
            }
        }


        /// <summary>
        /// Lists all organizations which a user is associated, optionally filtering based on approval status
        /// </summary>
        /// <param name="uSER_IDX"></param>
        /// <param name="sTATUS_CD">Optionally filter based on a particular approval status</param>
        /// <returns></returns>
        public static List<UserOrgDisplayType> GetT_QREST_ORG_USERS_byUSER_IDX(string uSER_IDX, string sTATUS_CD)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var x = (from uo in ctx.T_QREST_ORG_USERS.AsNoTracking()
                             join u in ctx.T_QREST_USERS.AsNoTracking() on uo.USER_IDX equals u.USER_IDX
                             join o in ctx.T_QREST_ORGANIZATIONS.AsNoTracking() on uo.ORG_ID equals o.ORG_ID
                             join a in ctx.T_QREST_REF_ACCESS_LEVEL.AsNoTracking() on uo.ACCESS_LEVEL equals a.ACCESS_LEVEL
                             join s in ctx.T_QREST_REF_USER_STATUS.AsNoTracking() on uo.STATUS_IND equals s.STATUS_IND
                             where (sTATUS_CD != null ? uo.STATUS_IND == sTATUS_CD : true)
                             && uo.USER_IDX == uSER_IDX
                             select new UserOrgDisplayType
                             {
                                 ORG_USER_IDX = uo.ORG_USER_IDX,
                                 USER_IDX = uo.USER_IDX,
                                 ORG_ID = uo.ORG_ID,
                                 ORG_NAME = o.ORG_NAME,
                                 ACCESS_LEVEL = uo.ACCESS_LEVEL,
                                 ACCESS_LEVEL_DESC = a.ACCESS_LEVEL_DESC,
                                 STATUS_IND = uo.STATUS_IND,
                                 STATUS_IND_DESC = s.STATUS_IND_DESC,
                                 CREATE_DT = uo.CREATE_DT,
                                 USER_NAME = u.FNAME + " " + u.LNAME
                             }).ToList();

                    return x;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        /// <summary>
        /// Lists all users who do not currently belong to org
        /// </summary>
        /// <param name="uSER_IDX"></param>
        /// <returns></returns>
        public static List<T_QREST_USERS> GetT_QREST_USERS_allUsersNotInOrg(string ORG_ID)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    //first get all active users 
                    var allUsers = (from itemA in ctx.T_QREST_USERS
                                    where itemA.LockoutEndDateUtc == null
                                    select itemA);

                    //next get all users in org
                    var UsersInOrg = (from itemA in ctx.T_QREST_USERS
                                      join itemB in ctx.T_QREST_ORG_USERS on itemA.USER_IDX equals itemB.USER_IDX
                                      where itemB.ORG_ID == ORG_ID
                                      select itemA);

                    //then get exclusions
                    var usersNotInOrg = allUsers.Except(UsersInOrg);

                    return usersNotInOrg.OrderBy(a => a.Email).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }


        /// <summary>
        /// Will return true if the supplied user belongs to the org
        /// </summary>
        public static bool GetT_QREST_ORG_USERS_ByUserAndOrgID(string oRG_ID, string uSER_IDX, bool excludePending)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    var x = (from a in ctx.T_QREST_ORG_USERS
                             where a.ORG_ID == oRG_ID
                             && a.USER_IDX == uSER_IDX
                             && (excludePending == true ? a.STATUS_IND == "A" : true)
                             select a).Count();

                    return x > 0 ? true : false;

                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return false;
                }
            }
        }



        //***************** T_QREST_USER_NOTIFICATION ***************************************        
        public static T_QREST_USER_NOTIFICATION GetT_QREST_USER_NOTIFICATION_ByID(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from u in ctx.T_QREST_USER_NOTIFICATION.AsNoTracking()
                            where u.NOTIFICATION_IDX == id
                            select u).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static List<T_QREST_USER_NOTIFICATION> GetT_QREST_USER_NOTIFICATION_ByUserID(string userIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from u in ctx.T_QREST_USER_NOTIFICATION.AsNoTracking()
                            where u.USER_IDX == userIDX
                            orderby u.NOTIFY_DT descending
                            select u).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int GetT_QREST_USER_NOTIFICATION_ByUserIDUnreadCount(string userIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from u in ctx.T_QREST_USER_NOTIFICATION.AsNoTracking()
                            where u.USER_IDX == userIDX
                            && u.READ_IND == false
                            select u).Count();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

        public static List<T_QREST_USER_NOTIFICATION> GetT_QREST_USER_NOTIFICATION_ByUserIDUnreadTop3(string userIDX)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    return (from u in ctx.T_QREST_USER_NOTIFICATION.AsNoTracking()
                            where u.USER_IDX == userIDX
                            && u.READ_IND == false
                            orderby u.NOTIFY_DT descending
                            select u).Take(3).ToList();
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static Guid? InsertUpdateT_QREST_USER_NOTIFICATION(Guid? nOTIFICATION_IDX, string uSER_IDX, DateTime? nOTIFY_DT, string nOTIFY_TYPE, string nOTIFY_TITLE, string nOTIFY_DESC, 
            bool? rEAD_IND, string cREATE_USER)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    bool insInd = false;

                    T_QREST_USER_NOTIFICATION e = (from c in ctx.T_QREST_USER_NOTIFICATION
                                                   where c.NOTIFICATION_IDX == nOTIFICATION_IDX
                                                   select c).FirstOrDefault();

                    if (e == null)
                    {
                        insInd = true;
                        e = new T_QREST_USER_NOTIFICATION
                        {
                            NOTIFICATION_IDX = Guid.NewGuid(),
                            USER_IDX = uSER_IDX,
                            NOTIFY_DT = nOTIFY_DT.GetValueOrDefault(),
                            CREATE_DT = System.DateTime.Now,
                            CREATE_USERIDX = cREATE_USER,
                            READ_IND = false
                        };
                    }
                    else
                    {
                        e.MODIFY_DT = System.DateTime.Now;
                        e.MODIFY_USERIDX = cREATE_USER;
                        if (rEAD_IND != null) e.READ_IND = rEAD_IND;
                    }

                    if (nOTIFY_TYPE != null) e.NOTIFY_TYPE = nOTIFY_TYPE;
                    if (nOTIFY_TITLE != null) e.NOTIFY_TITLE = nOTIFY_TITLE;
                    if (nOTIFY_DESC != null) e.NOTIFY_DESC = nOTIFY_DESC.SubStringPlus(0,2000);

                    if (insInd)
                        ctx.T_QREST_USER_NOTIFICATION.Add(e);

                    ctx.SaveChanges();
                    return e.NOTIFICATION_IDX;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return null;
                }
            }
        }

        public static int DeleteT_QREST_USER_NOTIFICATION(Guid id)
        {
            using (QRESTEntities ctx = new QRESTEntities())
            {
                try
                {
                    T_QREST_USER_NOTIFICATION rec = ctx.T_QREST_USER_NOTIFICATION.Find(id);
                    ctx.T_QREST_USER_NOTIFICATION.Remove(rec);
                    ctx.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    logEF.LogEFException(ex);
                    return 0;
                }
            }
        }

    }
}
