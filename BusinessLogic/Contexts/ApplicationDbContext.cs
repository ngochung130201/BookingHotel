using BusinessLogic.Entities;
using BusinessLogic.Entities.Base;
using BusinessLogic.Entities.Identity;
using BusinessLogic.Services.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Contexts
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService)
        : IdentityDbContext<AppUser, AppRole, string, IdentityUserClaim<string>, IdentityUserRole<string>,
            IdentityUserLogin<string>, AppRoleClaim, IdentityUserToken<string>>(options)
    {
        #region Entities
        public virtual DbSet<Contact> Contacts { get; set; } = default!;
        public virtual DbSet<ContactInfo> ContactInfos { get; set; } = default!;
        public virtual DbSet<CronJob> CronJobs { get; set; } = default!;
        public virtual DbSet<Function> Functions { get; set; } = default!;
        public virtual DbSet<Pages> Pages { get; set; } = default!;
        public virtual DbSet<Entities.Permission> Permissions { get; set; } = default!;
        public virtual DbSet<Partners> Partners { get; set; } = default!;
        public virtual DbSet<Invoices> Invoices { get; set; } = default!;
        #endregion Entities

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = dateTimeService.Now;
                        entry.Entity.CreatedBy = currentUserService.UserName ?? "System";
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = dateTimeService.Now;
                        entry.Entity.LastModifiedBy = currentUserService.UserName ?? "System";
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,5)");
            }
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(entity =>
            {
                entity.ToTable(name: "Users", "Identity");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<AppRole>(entity =>
            {
                entity.ToTable(name: "Roles", "Identity");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles", "Identity");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims", "Identity");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins", "Identity");
            });

            builder.Entity<AppRoleClaim>(entity =>
            {
                entity.ToTable(name: "RoleClaims", "Identity");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens", "Identity");
            });
        }
    }
}