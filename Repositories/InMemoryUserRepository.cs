using GearShop.Models;

namespace GearShop.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users = new()
        {
            new User { 
                Id = 1, 
                Name = "Gabriel", 
                Email = "gabriel@email.com", 
                PhoneNumber="(14) 99154-1688", 
                ProfilePicture="https://picsum.photos/200?1",
                Cpf = "12345678901",
                Estado = "SP",
                Cidade = "São Paulo",
                Cep = "01234567",
                Rua = "Rua das Flores",
                NumeroCasa = "123"
            },
            new User { 
                Id = 2, 
                Name = "Maria", 
                Email = "maria@email.com", 
                PhoneNumber="(18) 98802-7007", 
                ProfilePicture="https://picsum.photos/200?2",
                Cpf = "98765432109",
                Estado = "RJ",
                Cidade = "Rio de Janeiro",
                Cep = "20000000",
                Rua = "Avenida Central",
                NumeroCasa = "456"
            }
        };
        private int _nextId = 3;

        public Task<List<User>> GetAllAsync() => Task.FromResult(_users.ToList());
        public Task<User?> GetByIdAsync(int id) => Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        public Task<User> CreateAsync(User user) { user.Id = _nextId++; _users.Add(user); return Task.FromResult(user); }
        public Task<User?> UpdateAsync(int id, User data)
        {
            var u = _users.FirstOrDefault(x => x.Id == id);
            if (u is null) return Task.FromResult<User?>(null);
            
            u.Name = data.Name;
            u.Email = data.Email;
            u.PhoneNumber = data.PhoneNumber;
            u.ProfilePicture = data.ProfilePicture;
            u.Cpf = data.Cpf;
            u.Estado = data.Estado;
            u.Cidade = data.Cidade;
            u.Cep = data.Cep;
            u.Rua = data.Rua;
            u.NumeroCasa = data.NumeroCasa;
            
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
