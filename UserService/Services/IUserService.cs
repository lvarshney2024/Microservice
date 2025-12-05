using UserService.Dtos;
using UserService.Models;
using System.Collections.Generic;

namespace UserService.Services
{
    public interface IUserService
    {
        User Create(CreateUserDto dto);
        User GetById(string id);
        List<User> GetAll();
        void Update(string id, CreateUserDto dto);
        void Delete(string id);
    }
}
