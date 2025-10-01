using GearShop.Models;

namespace GearShop.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users = new()
        {
            new User { Id = 1, Name = "Gabriel", Email = "gabriel@email.com", phoneNumber="(14) 99154-1688", profilePicture="https://picsum.photos/200?1" },
            new User { Id = 2, Name = "Maria",   Email = "maria@email.com",   phoneNumber="(18) 98802-7007", profilePicture="https://picsum.photos/200?2" }
        };
        private int _nextId = 3;

        public Task<List<User>> GetAllAsync() => Task.FromResult(_users.ToList());
        public Task<User?> GetByIdAsync(int id) => Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        public Task<User> CreateAsync(User user) { user.Id = _nextId++; _users.Add(user); return Task.FromResult(user); }
        public Task<User?> UpdateAsync(int id, User data)
        {
            var u = _users.FirstOrDefault(x => x.Id == id);
            if (u is null) return Task.FromResult<User?>(null);
            u.Name = data.Name; u.Email = data.Email; u.phoneNumber = data.phoneNumber; u.profilePicture = data.profilePicture;
            return Task.FromResult<User?>(u);
        }
        public Task<bool> DeleteAsync(int id)
        {
            var u = _users.FirstOrDefault(x => x.Id == id);
            if (u is null) return Task.FromResult(false);
            _users.Remove(u); return Task.FromResult(true);
        }
        public Task<bool> EmailExistsAsync(string email, int? exceptId = null) =>
            Task.FromResult(_users.Any(u => u.Email.ToLower() == email.ToLower() && (!exceptId.HasValue || u.Id != exceptId)));
    }
}
