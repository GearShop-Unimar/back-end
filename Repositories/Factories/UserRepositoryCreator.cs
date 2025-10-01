namespace GearShop.Repositories.Factories
{
    public abstract class UserRepositoryCreator
    {
        public abstract IUserRepository CreateRepository();
    }
}
