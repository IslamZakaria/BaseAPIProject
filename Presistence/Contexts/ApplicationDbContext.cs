using Core.Entities.BaseEntities;
using Core.Interfaces.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Presistence.Contexts
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IDateTimeService _dateTime;
        private readonly IAuthenticatedUserService _authenticatedUser;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
                                    IDateTimeService dateTime,
                                    IAuthenticatedUserService authenticatedUser) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _dateTime = dateTime;
            _authenticatedUser = authenticatedUser;
        }
        public IDbConnection Connection => Database.GetDbConnection();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                }

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = _dateTime.NowUtc;
                    entry.Entity.CreatedBy = _authenticatedUser.UserId ?? "Website Copolitan";
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedAt = _dateTime.NowUtc;
                    entry.Entity.LastModifiedBy = _authenticatedUser.UserId ?? "Website Copolitan";
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            //All Decimals will have 18,2 Range
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }
            base.OnModelCreating(builder);


            #region Set OnDeleteToRestrict
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // equivalent of modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
                entityType.SetTableName(entityType.DisplayName());

                // equivalent of modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
                entityType.GetForeignKeys()
                    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                    .ToList()
                    .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);
            }
            #endregion
        }
    }
}
