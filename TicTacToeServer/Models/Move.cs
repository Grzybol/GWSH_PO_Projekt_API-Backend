namespace TicTacToeServer.Models
{
    public class Move
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public string Player { get; set; }
        public int GameId { get; set; } // Ensure GameId property is present
    }
}
