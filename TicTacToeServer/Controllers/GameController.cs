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
                var game = new Game { Id = newId };
                game.Players.Add(player);
                game.CurrentTurn = player;
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
            var game = _activeGames.FirstOrDefault(g => g.IsActive && g.Id == gameId);
            if (game == null)
            {
                return BadRequest(new { message = "Game not found or already full" });
            }

            if (!game.Players.Contains(player))
            {
                if (game.Players.Count < 2)
                {
                    game.Players.Add(player);
                }
                else
                {
                    return BadRequest(new { message = "Game is already full" });
                }
            }

            SaveGames();
            return Ok(game);
        }

        /*
        [HttpPost("move")]
        public IActionResult MakeMove([FromBody] Move move)
        {
            Console.WriteLine($"Received move: Player {move.Player}, Row {move.Row}, Col {move.Col}, Game ID: {move.GameId}");

            // Find the game by ID
            var game = _activeGames.FirstOrDefault(g => g.Id == move.GameId && g.IsActive && g.Players.Contains(move.Player));
            if (game == null)
            {
                Console.WriteLine("No active game found or player not in game");
                Console.WriteLine($"Active games count: {_activeGames.Count}");
                foreach (var g in _activeGames)
                {
                    Console.WriteLine($"Game ID: {g.Id}, Players: {string.Join(", ", g.Players)}, Current Turn: {g.CurrentTurn}, Is Active: {g.IsActive}");
                }
                return BadRequest(new { message = "No active game found or not your turn or not your game" });
            }

            // Determine player's symbol (X or O)
            string playerSymbol = game.Players.IndexOf(move.Player) == 0 ? "X" : "O";

            if (game.CurrentTurn != playerSymbol)
            {
                Console.WriteLine($"Invalid turn: It's {game.CurrentTurn}'s turn, but {move.Player} ({playerSymbol}) tried to move.");
                return BadRequest(new { message = "It's not your turn" });
            }

            if (game.MakeMove(move.Row, move.Col, move.Player,playerSymbol))
            {
                if (!game.IsActive)
                {
                    _activeGames.Remove(game);
                    _completedGames.Add(game);
                }
                SaveGames();
                Console.WriteLine($"Move successful: Player {move.Player} moved to Row {move.Row}, Col {move.Col}");
                Console.WriteLine($"Next turn: {game.CurrentTurn}");
                return Ok(game);
            }

            Console.WriteLine("Invalid move attempted");
            return BadRequest(new { message = "Invalid move" });
        }
        */
        [HttpPost("move")]
        public IActionResult MakeMove([FromBody] Move move)
        {
            Console.WriteLine($"Received move: Player {move.Player}, Row {move.Row}, Col {move.Col}, Game ID: {move.GameId}");

            // Find the game
            var game = _activeGames.FirstOrDefault(g => g.Id == move.GameId && g.IsActive && g.Players.Contains(move.Player));

            // Check if game is valid and player is part of the game
            if (game == null)
            {
                Console.WriteLine("No active game found or player not in game");
                return BadRequest(new { message = "No active game found or not your turn or not your game" });
            }
            if (!game.Players.Contains(move.Player))
            {
                return BadRequest(new { message = "You are not a part of this game." });
            }

            // Ensure there are at least two players in the game
            if (game.Players.Count < 2)
            {
                Console.WriteLine("Not enough players to make a move");
                return BadRequest(new { message = "Not enough players in the game" });
            }

            // Determine player's symbol (X or O)
            string playerSymbol = game.Players.IndexOf(move.Player) == 0 ? "X" : "O";

            // Attempt to make the move
            if (game.MakeMove(move.Row, move.Col, move.Player, playerSymbol))
            {
                // If the game is now inactive, move it to completed games
                if (!game.IsActive)
                {
                    _activeGames.Remove(game);
                    _completedGames.Add(game);
                    SaveGames();

                    // Create a response that includes the final board state and the outcome
                    var response = new
                    {
                        Game = game,
                        Message = game.Winner != null ? $"Game over: {game.Winner} wins!" : "Game over: Draw!"
                    };

                    // Ideally, you'd use SignalR or another real-time tech to notify both players
                    // For now, we return the same response to the requester, which should be handled client-side
                    return Ok(response);
                }
                SaveGames();
                Console.WriteLine($"Move successful: Player {move.Player} moved to Row {move.Row}, Col {move.Col}");
                Console.WriteLine($"Next turn: {game.CurrentTurn}");
                return Ok(new { Game = game, Message = $"Next turn: {game.CurrentTurn}" });
            }

            Console.WriteLine("Invalid move attempted");
            return BadRequest(new { message = "Invalid move" });
        }





        [HttpGet("status")]
        public IActionResult GetStatus([FromQuery] int gameId)
        {
            var game = _activeGames.FirstOrDefault(g => g.Id == gameId);
            if (game != null)
            {
                return Ok(game);
            }
            return NotFound(new { message = "No active game found" });
        }
    }
}
