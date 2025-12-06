using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using UserService.Models;
using System.Collections.Generic;
using System.Linq;

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
            var sql = $"INSERT INTO \"user\" (id, name, email, age) VALUES ('{user.Id}', '{user.Name}', '{user.Email}', {user.Age})";
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
            // Upsert via insert
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
            var url = $"{_baseUrl}/api/v1/sql"; // example endpoint; adapt to actual InfluxDB 3 Core API
            var content = JsonContent.Create(new { sql = sql, database = _database });
            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            if (!string.IsNullOrWhiteSpace(_token)) req.Headers.Add("Authorization", $"Bearer {_token}");

            var resp = _http.SendAsync(req).GetAwaiter().GetResult();
            var body = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request to {url} failed with {(int)resp.StatusCode} ({resp.ReasonPhrase}): {body}");
            }
        }

        // Helper: query SQL and return rows as list of dictionaries
        private List<Dictionary<string, object?>> QuerySql(string sql)
        {
            var url = $"{_baseUrl}/api/v1/sql"; // example endpoint; adapt to actual InfluxDB 3 Core API
            var content = JsonContent.Create(new { sql = sql, database = _database });
            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            if (!string.IsNullOrWhiteSpace(_token)) req.Headers.Add("Authorization", $"Bearer {_token}");

            var resp = _http.SendAsync(req).GetAwaiter().GetResult();
            var body = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request to {url} failed with {(int)resp.StatusCode} ({resp.ReasonPhrase}): {body}");
            }

            try
            {
                var parsed = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object?>>>(body);
                return parsed ?? new List<Dictionary<string, object?>>();
            }
            catch (System.Text.Json.JsonException)
            {
                // Return empty list if response isn't JSON rows - caller will handle empty result
                return new List<Dictionary<string, object?>>();
            }
        }
    }
}
