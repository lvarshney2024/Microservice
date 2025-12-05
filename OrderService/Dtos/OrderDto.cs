using System;
using System.Collections.Generic;
using MicroService.Models;

namespace MicroService.Dtos
{
    public class OrderDto
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal Total { get; set; }
    }

    public class CreateOrderDto
    {
        public string CustomerId { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}
