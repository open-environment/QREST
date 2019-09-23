using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QREST.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Extended Properties
        public string FNAME { get; set; }
        public string LNAME { get; set; }
        public string TITLE { get; set; }
        public bool? NOTIFY_APP_IND { get; set; }
        public bool? NOTIFY_EMAIL_IND { get; set; }
        public bool? NOTIFY_SMS_IND { get; set; }
        public DateTime? LAST_LOGIN_DT { get; set; }
        public string CREATE_USER_IDX { get; set; }
        public DateTime? CREATE_DT { get; set; }
        public string MODIFY_USER_IDX { get; set; }
        public DateTime? MODIFY_DT { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("IdentityConnection", throwIfV1Schema: false)
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //change table names
            modelBuilder.Entity<ApplicationUser>().ToTable("T_QREST_USERS");
            modelBuilder.Entity<IdentityRole>().ToTable("T_QREST_ROLES");
            modelBuilder.Entity<IdentityUserRole>().ToTable("T_QREST_USER_ROLES");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("T_QREST_USER_LOGINS");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("T_QREST_USER_CLAIMS");

            //change column names
            modelBuilder.Entity<ApplicationUser>().Property(p => p.Id).HasColumnName("USER_IDX");
            modelBuilder.Entity<IdentityRole>().Property(p => p.Id).HasColumnName("ROLE_IDX");
            modelBuilder.Entity<IdentityUserRole>().Property(p => p.UserId).HasColumnName("USER_IDX");
            modelBuilder.Entity<IdentityUserRole>().Property(p => p.RoleId).HasColumnName("ROLE_IDX");
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

}