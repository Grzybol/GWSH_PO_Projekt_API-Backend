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

        public string CurrentTurn { get; set; } = null;
        public string Winner { get; set; } = null;
        public bool IsActive { get; set; } = true;
        public List<string> Players { get; set; } = new List<string>();

        /*
        public bool MakeMove(int row, int col, string player, string playerSymbol)
        {
            if (IsActive && row >= 0 && row < 3 && col >= 0 && col < 3 && Board[row][col] == null && player == CurrentTurn)
            {
                Console.WriteLine($"test");
                Board[row][col] = playerSymbol;
                if (CheckWin(playerSymbol))
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
        */
        public bool MakeMove(int row, int col, string player, string playerSymbol)
        {
            if (IsActive && row >= 0 && row < 3 && col >= 0 && col < 3 && Board[row][col] == null && Players.Contains(player) && player == CurrentTurn)
            {
                Console.WriteLine($"Player {player} is making a move with symbol {playerSymbol} at row {row}, col {col}, IsActive: {IsActive}");
                Board[row][col] = playerSymbol;
                if (CheckWin(playerSymbol))
                {
                    Winner = player;
                    IsActive = false;
                    Console.WriteLine($"Player {player} won. IsActive: {IsActive}");
                }
                else if (Board.SelectMany(x => x).All(x => x != null))
                {
                    //remis
                    IsActive = false;
                    Console.WriteLine($"Draw");
                }
                else
                {
                    // Find the index of the current player and switch to the next player
                    int currentPlayerIndex = Players.IndexOf(player);
                    int nextPlayerIndex = (currentPlayerIndex + 1) % Players.Count;
                    CurrentTurn = Players[nextPlayerIndex];
                    Console.WriteLine($"Next turn: {CurrentTurn} (Player {Players[nextPlayerIndex]})");
                }
                return true;
            }
            else
            {
                Console.WriteLine($"Invalid move attempt: It's {CurrentTurn}'s turn, but {player} ({playerSymbol}) tried to move.");
            }
            return false;
        }


        private bool CheckWin(string playerSymbol)
        {
            // Check rows, columns, and diagonals for a win
            return (Board.Any(row => row.All(cell => cell == playerSymbol)) ||
                    Enumerable.Range(0, 3).Any(col => Board.All(row => row[col] == playerSymbol)) ||
                    Enumerable.Range(0, 3).All(i => Board[i][i] == playerSymbol) ||
                    Enumerable.Range(0, 3).All(i => Board[i][2 - i] == playerSymbol));
        }
    }
}
