using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using UserService.Models;

namespace UserService.Repositories
{
    public class InfluxUserRepository : IUserRepository
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly string _token;
        private readonly string _database;

        public InfluxUserRepository(HttpClient http, string baseUrl, string token, string database)
        {
            _http = http;
            _baseUrl = baseUrl.TrimEnd('/');
            _token = token;
            _database = database;
        }

        public User Create(User user)
        {
            long ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1_000_000;
            var sql = $"user,id={user.Id} name=\"{user.Name}\",email=\"{user.Email}\",age={user.Age}i {ts}";
            ExecuteSql(sql);
            return user;
        }

        public User GetById(string id)
        {
            var sql = $"SELECT * FROM \"user\" WHERE id = '{id}' LIMIT 1";
            var rows = QuerySql(sql);
            var row = rows.FirstOrDefault();
            if (row == null) return null;
            return new User
            {
                Id = row.ContainsKey("id") ? row["id"]?.ToString() : null,
                Name = row.ContainsKey("name") ? row["name"]?.ToString() : null,
                Email = row.ContainsKey("email") ? row["email"]?.ToString() : null,
                Age = row.ContainsKey("age") && int.TryParse(row["age"]?.ToString(), out var age) ? age : 0
            };
        }

        public List<User> GetAll()
        {
            

            var sql = "SELECT * FROM \"user\"";
            var rows = QuerySql(sql);
            var users = new List<User>();
            foreach (var row in rows)
            {
                users.Add(new User
                {
                    Id = row.ContainsKey("id") ? row["id"]?.ToString() : null,
                    Name = row.ContainsKey("name") ? row["name"]?.ToString() : null,
                    Email = row.ContainsKey("email") ? row["email"]?.ToString() : null,
                    Age = row.ContainsKey("age") && int.TryParse(row["age"]?.ToString(), out var age) ? age : 0
                });
            }
            return users;
        }

        public void Update(User user)
        {
            var sql = $"user,id={user.Id} name=\"{user.Name}\",email=\"{user.Email}\",age={user.Age}i";
            Create(user);
        }

        public void Delete(string id)
        {
            var sql = $"DELETE FROM \"user\" WHERE id = '{id}'";
            ExecuteSql(sql);
        }

        // Helper: execute non-query SQL
        private void ExecuteSql(string sql)
        {
            var url = $"{_baseUrl}/api/v3/write_lp?db={_database}"; // example endpoint; adapt to actual InfluxDB 3 Core API
            
           
            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(sql, Encoding.UTF8, "text/plain") };
            if (!string.IsNullOrWhiteSpace(_token)) req.Headers.Add("Authorization", $"Bearer {_token}");
            var resp = _http.Send(req);
            resp.EnsureSuccessStatusCode();
        }

        // Helper: query SQL and return rows as list of dictionaries
        private List<Dictionary<string, object?>> QuerySql(string sql)
        {
            var url = $"{_baseUrl}/api/v3/query_sql"; // example endpoint; adapt to actual InfluxDB 3 Core API
            var content = JsonContent.Create(new { q = sql, db = _database });
            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            if (!string.IsNullOrWhiteSpace(_token)) req.Headers.Add("Authorization", $"Bearer {_token}");
            var resp = _http.Send(req);
            resp.EnsureSuccessStatusCode();
            var body = resp.Content.ReadFromJsonAsync<List<Dictionary<string, object?>>>().GetAwaiter().GetResult();
            return body ?? new List<Dictionary<string, object?>>();
        }


    }
}
