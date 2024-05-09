using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TicTacToeServer.Models;

namespace TicTacToeServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private static List<Game> _activeGames = new List<Game>();
        private static List<Game> _completedGames = new List<Game>();
        private static readonly string activeGamesFilePath = "activeGames.txt";
        private static readonly string completedGamesFilePath = "completedGames.txt";

        static GameController()
        {
            LoadGames();
        }

        private static void LoadGames()
        {
            if (System.IO.File.Exists(activeGamesFilePath))
            {
                var activeGameLines = System.IO.File.ReadAllLines(activeGamesFilePath);
                foreach (var line in activeGameLines)
                {
                    _activeGames.Add(JsonSerializer.Deserialize<Game>(line));
                }
            }

            if (System.IO.File.Exists(completedGamesFilePath))
            {
                var completedGameLines = System.IO.File.ReadAllLines(completedGamesFilePath);
                foreach (var line in completedGameLines)
                {
                    _completedGames.Add(JsonSerializer.Deserialize<Game>(line));
                }
            }
        }

        private static void SaveGames()
        {
            var activeGameLines = _activeGames.Select(g => JsonSerializer.Serialize(g)).ToArray();
            System.IO.File.WriteAllLines(activeGamesFilePath, activeGameLines);

            var completedGameLines = _completedGames.Select(g => JsonSerializer.Serialize(g)).ToArray();
            System.IO.File.WriteAllLines(completedGamesFilePath, completedGameLines);
        }

        [HttpGet("active")]
        public IActionResult GetActiveGames()
        {
            var gamesWithPlayerInfo = _activeGames.Select(g => new
            {
                g.Id,
                g.Players,
                HasRoom = g.Players.Count < 2
            }).ToList();
            return Ok(gamesWithPlayerInfo);
        }

        [HttpGet("completed")]
        public IActionResult GetCompletedGames()
        {
            return Ok(_completedGames);
        }

        [HttpPost("new")]
        public IActionResult NewGame([FromBody] string player)
        {
            try
            {
                var newId = _activeGames.Count > 0 ? _activeGames.Max(g => g.Id) + 1 : 1;
                var game = new Game { Id = newId, Players = new List<string> { player } };
                _activeGames.Add(game);
                SaveGames();
                return Ok(game);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("join")]
        public IActionResult JoinGame([FromBody] string player, [FromQuery] int gameId)
        {
            var game = _activeGames.FirstOrDefault(g => g.IsActive && g.Players.Count < 2 && g.Id == gameId);
            if (game == null)
            {
                return BadRequest(new { message = "Game not found or already full" });
            }

            if (!game.Players.Contains(player))
            {
                game.Players.Add(player);
                SaveGames();
            }
            return Ok(game);
        }

        [HttpPost("move")]
        public IActionResult MakeMove([FromBody] Move move)
        {
            var game = _activeGames.FirstOrDefault(g => g.IsActive && g.CurrentTurn == move.Player && g.Players.Contains(move.Player));
            if (game == null)
            {
                return BadRequest(new { message = "No active game found or not your turn or not your game" });
            }

            if (game.MakeMove(move.Row, move.Col, move.Player))
            {
                if (!game.IsActive)
                {
                    _activeGames.Remove(game);
                    _completedGames.Add(game);
                }
                SaveGames();
                return Ok(game);
            }
            return BadRequest(new { message = "Invalid move" });
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var game = _activeGames.FirstOrDefault();
            if (game != null)
            {
                return Ok(game);
            }
            return NotFound(new { message = "No active game found" });
        }
    }
}
