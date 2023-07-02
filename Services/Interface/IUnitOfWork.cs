namespace GeoLocationTest.Services.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransaction();

        Task Commit();

        void RollbackTransaction();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
