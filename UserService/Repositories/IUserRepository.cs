using UserService.Models;
using System.Collections.Generic;

namespace UserService.Repositories
{
    public interface IUserRepository
    {
        User Create(User user);
        User GetById(string id);
        List<User> GetAll();
        void Update(User user);
        void Delete(string id);
    }
}
