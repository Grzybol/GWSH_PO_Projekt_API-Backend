namespace TicTacToeServer.Models
{
    public class Game
    {
        public int Id { get; set; }
        public List<List<string>> Board { get; set; } = new List<List<string>>
        {
            new List<string> { null, null, null },
            new List<string> { null, null, null },
            new List<string> { null, null, null }
        };

        public string CurrentTurn { get; set; } = "X";
        public string Winner { get; set; } = null;
        public bool IsActive { get; set; } = true;
        public List<string> Players { get; set; } = new List<string>();

        public bool MakeMove(int row, int col, string player)
        {
            if (IsActive && row >= 0 && row < 3 && col >= 0 && col < 3 && Board[row][col] == null && player == CurrentTurn)
            {
                Board[row][col] = player;
                if (CheckWin(player))
                {
                    Winner = player;
                    IsActive = false;
                }
                else if (Board.SelectMany(x => x).All(x => x != null))
                {
                    IsActive = false;
                }
                else
                {
                    CurrentTurn = (CurrentTurn == "X" ? "O" : "X");
                }
                return true;
            }
            return false;
        }


        private bool CheckWin(string player)
        {
            // Check rows, columns, and diagonals for a win
            return (Board.Any(row => row.All(cell => cell == player)) ||
                    Enumerable.Range(0, 3).Any(col => Board.All(row => row[col] == player)) ||
                    Enumerable.Range(0, 3).All(i => Board[i][i] == player) ||
                    Enumerable.Range(0, 3).All(i => Board[i][2 - i] == player));
        }
    }
}
