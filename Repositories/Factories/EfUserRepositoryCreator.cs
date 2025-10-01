using GearShop.Data;

namespace GearShop.Repositories.Factories
{
    public class EfUserRepositoryCreator : UserRepositoryCreator
    {
        private readonly AppDbContext _db;
        public EfUserRepositoryCreator(AppDbContext db) => _db = db;
        public override IUserRepository CreateRepository() => new EfUserRepository(_db);
    }
}
