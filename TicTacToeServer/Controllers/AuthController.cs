using Microsoft.AspNetCore.Mvc;
using TicTacToeServer.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TicTacToeServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private static Dictionary<string, string> _users = new Dictionary<string, string>();
        private static readonly string usersFilePath = "users.txt";

        static AuthController()
        {
            LoadUsers();
        }

        private static void LoadUsers()
        {
            if (System.IO.File.Exists(usersFilePath))
            {
                var userLines = System.IO.File.ReadAllLines(usersFilePath);
                foreach (var line in userLines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        _users[parts[0]] = parts[1];
                    }
                }
            }
        }

        private static void SaveUsers()
        {
            var userLines = _users.Select(u => $"{u.Key},{u.Value}").ToArray();
            System.IO.File.WriteAllLines(usersFilePath, userLines);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (_users.ContainsKey(user.Username))
            {
                return BadRequest(new { message = "User already exists" });
            }
            _users[user.Username] = user.Password;
            SaveUsers();
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            if (_users.TryGetValue(user.Username, out var password))
            {
                if (password == user.Password)
                {
                    return Ok(new { message = "Login successful" });
                }
                return Unauthorized(new { message = "Invalid password" });
            }
            return Unauthorized(new { message = "Invalid username" });
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
