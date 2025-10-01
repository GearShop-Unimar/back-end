namespace GearShop.Repositories.Factories
{
	public class InMemoryUserRepositoryCreator : UserRepositoryCreator
	{
		public override IUserRepository CreateRepository() => new InMemoryUserRepository();
	}
}
