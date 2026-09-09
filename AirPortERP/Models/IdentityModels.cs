using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;

namespace AirPortERP.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //[Required]
        //public Nullable<int> CompanyInfoId { get; set; }
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
            : base("DefaultConnection", throwIfV1Schema: false)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .ToTable("AspNetUsers", "Identity");

            modelBuilder.Entity<IdentityRole>()
                .ToTable("AspNetRoles", "Identity");

            modelBuilder.Entity<IdentityUserRole>()
                .ToTable("AspNetUserRoles", "Identity");

            modelBuilder.Entity<IdentityUserClaim>()
                .ToTable("AspNetUserClaims", "Identity");

            modelBuilder.Entity<IdentityUserLogin>()
                .ToTable("AspNetUserLogins", "Identity");
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();

        }

        public System.Data.Entity.DbSet<AirPortERP.ViewModels.PrintLandingViewModel> PrintLandingViewModels { get; set; }

        public System.Data.Entity.DbSet<AirPortERP.Models.AccountGroup> AccountGroups { get; set; }
    }
}