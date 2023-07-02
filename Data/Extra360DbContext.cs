using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace GeoLocationTest.Data
{
    public abstract class Extra360DbContext : DbContext
    {
        public Extra360DbContext(DbContextOptions options) : base(options)
        {

        }

        private IDbContextTransaction _currentTransaction;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public async Task BeginTransaction()
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("A transaction already exists");
            }

            _currentTransaction = await Database
                .BeginTransactionAsync(IsolationLevel.ReadCommitted)
                .ConfigureAwait(false);
        }

        public Task Commit()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException($"Transaction does not exist.");
            }

            return CommitTransaction(_currentTransaction);
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.GetCustomAttributes(typeof(AuditableAttribute), true).Length > 0)
                {
                    modelBuilder.Entity(entityType.Name).Property<DateTime>(AuditableAttribute.CreateDateFieldName);
                    modelBuilder.Entity(entityType.Name).Property<int>(AuditableAttribute.CreatedByFieldName);
                    modelBuilder.Entity(entityType.Name).Property<DateTime?>(AuditableAttribute.EditDateFieldName);
                    modelBuilder.Entity(entityType.Name).Property<int?>(AuditableAttribute.EditedByFieldName);
                }
            }

            base.OnModelCreating(modelBuilder);

        }

        private async Task CommitTransaction(IDbContextTransaction transaction)
        {
            try
            {
                await SaveChangesAsync().ConfigureAwait(false);
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        private void UpdateAuditFields()
        {
            ChangeTracker.DetectChanges();
            var timestamp = DateTime.Now;
            foreach (var entry in ChangeTracker
            .Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                if (entry.Entity.GetType().GetCustomAttributes(typeof(AuditableAttribute), true).Length > 0)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property(AuditableAttribute.CreateDateFieldName).CurrentValue = timestamp;
                    }
                    else
                    {
                        entry.Property(AuditableAttribute.EditDateFieldName).CurrentValue = timestamp;
                    }
                }
            }
        }
    }
}