using MicroService.Dtos;
using MicroService.Models;
using System.Collections.Generic;

namespace MicroService.Services
{
    public interface IOrderService
    {
        Order Create(CreateOrderDto dto);
        Order GetById(string id);
        List<Order> GetAll();
        void Update(string id, CreateOrderDto dto);
        void Delete(string id);
    }
}
