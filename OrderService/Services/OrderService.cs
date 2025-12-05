using MicroService.Dtos;
using MicroService.Models;
using MicroService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public Order Create(CreateOrderDto dto)
        {
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                CreatedAt = DateTime.UtcNow,
                Items = dto.Items ?? new List<OrderItem>(),
                Total = dto.Items?.Sum(i => i.Price * i.Quantity) ?? 0
            };

            return _repository.Create(order);
        }

        public Order GetById(string id)
        {
            return _repository.GetById(id);
        }

        public List<Order> GetAll()
        {
            return _repository.GetAll();
        }

        public void Update(string id, CreateOrderDto dto)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return;

            existing.CustomerId = dto.CustomerId;
            existing.Items = dto.Items ?? new List<OrderItem>();
            existing.Total = dto.Items?.Sum(i => i.Price * i.Quantity) ?? 0;

            _repository.Update(existing);
        }

        public void Delete(string id)
        {
            _repository.Delete(id);
        }
    }
}
