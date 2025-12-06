using UserService.Dtos;
using UserService.Models;
using UserService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public User Create(CreateUserDto dto)
        {
            var user = new User
            {
                Id = dto.Id,
                Name = dto.Name,
                Email = dto.Email,
                Age = dto.Age
            };
            return _repository.Create(user);
        }

        public User GetById(string id)
        {
            return _repository.GetById(id);
        }

        public List<User> GetAll()
        {
            return _repository.GetAll();
        }

        public void Update(string id, CreateUserDto dto)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return;
            existing.Name = dto.Name;
            existing.Email = dto.Email;
            existing.Age = dto.Age;
            _repository.Update(existing);
        }

        public void Delete(string id)
        {
            _repository.Delete(id);
        }
    }
}
