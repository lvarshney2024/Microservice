using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using UserService.Models;
using System.Collections.Generic;
using System.Linq;

namespace UserService.Repositories
{
    public class InfluxUserRepository : IUserRepository
    {
        private readonly InfluxDBClient _client;
        private readonly string _bucket;
        private readonly string _org;

        public InfluxUserRepository(InfluxDBClient client, string bucket, string org)
        {
            _client = client;
            _bucket = bucket;
            _org = org;
        }

        public UserService.Models.User Create(Models.User user)
        {
            var point = PointData.Measurement("user")
                .Tag("id", user.Id)
                .Field("name", user.Name)
                .Field("email", user.Email)
                .Field("age", user.Age);
            _client.GetWriteApi().WritePoint(point, _bucket, _org);
            return user;
        }

        public UserService.Models.User GetById(string id)
        {
            var query = $"from(bucket: '{_bucket}') |> range(start: -1y) |> filter(fn: (r) => r[\"id\"] == '{id}')";
            var tables = _client.GetQueryApi().QueryAsync(query, _org).Result;
            var record = tables.SelectMany(t => t.Records).FirstOrDefault();
            if (record == null) return null;
            return new UserService.Models.User
            {
                Id = record.GetValueByKey("id")?.ToString(),
                Name = record.GetValueByKey("name")?.ToString(),
                Email = record.GetValueByKey("email")?.ToString(),
                Age = int.TryParse(record.GetValueByKey("age")?.ToString(), out var age) ? age : 0
            };
        }

        public List<UserService.Models.User> GetAll()
        {
            var query = $"from(bucket: '{_bucket}') |> range(start: -1y) |> filter(fn: (r) => r._measurement == 'user')";
            var tables = _client.GetQueryApi().QueryAsync(query, _org).Result;
            var users = new List<UserService.Models.User>();
            foreach (var record in tables.SelectMany(t => t.Records))
            {
                users.Add(new UserService.Models.User
                {
                    Id = record.GetValueByKey("id")?.ToString(),
                    Name = record.GetValueByKey("name")?.ToString(),
                    Email = record.GetValueByKey("email")?.ToString(),
                    Age = int.TryParse(record.GetValueByKey("age")?.ToString(), out var age) ? age : 0
                });
            }
            return users;
        }

        public void Update(UserService.Models.User user)
        {
            Create(user); // Overwrite by writing again
        }

        public void Delete(string id)
        {
            // InfluxDB is not designed for deletes, but you can drop data by predicate
            // This is a placeholder for actual delete logic
        }
    }
}
