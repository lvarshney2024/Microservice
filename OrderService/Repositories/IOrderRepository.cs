using MicroService.Models;
using System.Collections.Generic;

namespace MicroService.Repositories
{
    public interface IOrderRepository
    {
        Order Create(Order order);
        Order GetById(string id);
        List<Order> GetAll();
        void Update(Order order);
        void Delete(string id);
    }
}
