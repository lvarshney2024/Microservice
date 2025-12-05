using MongoDB.Bson;
using MongoDB.Driver;
using MicroService.Models;
using System.Collections.Generic;
using System.Linq;

namespace MicroService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _collection;

        public OrderRepository(MongoDbService mongoDbService)
        {
            _collection = mongoDbService.GetCollection<Order>("Orders");
        }

        public Order Create(Order order)
        {
            _collection.InsertOne(order);
            return order;
        }

        public Order GetById(string id)
        {
            return _collection.Find(o => o.Id == id).FirstOrDefault();
        }

        public List<Order> GetAll()
        {
            return _collection.Find(new BsonDocument()).ToList();
        }

        public void Update(Order order)
        {
            _collection.ReplaceOne(o => o.Id == order.Id, order);
        }

        public void Delete(string id)
        {
            _collection.DeleteOne(o => o.Id == id);
        }
    }
}
