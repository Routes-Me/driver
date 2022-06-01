namespace driver_service.Abstraction
{
    public interface IUnitOfWork
    {
        IDriverRepository DriverRepository { get; }
        void BeginTransaction();
        void Commit();
        void Rollback();

        void Save();

    }
}
