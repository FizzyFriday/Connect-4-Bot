using Connect4_BotApp.API;

namespace Connect4_BotApp
{
    // Contains non UI or gameplay logic methods of old GameGrid class such as GetValidMoves
    internal static class GameBoard
    {
        public static List<int[]> ValidMoves(string[,] grid)
        {
            List<int[]> options = new();

            for (int col = 0; col < grid.GetLength(0); col++)
            {
                int currentRow = grid.GetLength(1) - 1;
                if (grid[col, currentRow] != " ") continue;

                // Move down gradually, finding the lowest point that is empty as this is where the piece would go
                while (grid[col, currentRow - 1] == " ")
                {
                    currentRow--;
                    // Prevents checking negative indices
                    if (currentRow == 0) break;
                }

                // Set the column and row of the move into an array
                int[] moveOption = [col, currentRow];
                options.Add(moveOption);
            }
            return options;
        }

        // Makes a move on the real game board
        // Returns the new game board and if the game ended
        public static (string[,] grid, bool gameEnded) MakeMove(string[,] grid, string turn, int[] move)
        {
            List<int[]> possibleMoves = ValidMoves(grid);
            if (possibleMoves.Contains(move) == false)
            {
                API.API.DisplayMessage("API.MakeMove called with invalid move. Use API.ColumnToMove for valid move. Move rejected.");
                return (grid, false);
            }

            // Make move and return results
            grid[move[0], move[1]] = turn;
            string state = HeuristicManager.EndState(grid, move, turn);
            if (state == "IP") return (grid, true);
            else return (grid, false);
        }

        public static int[]? TranslateColToMove(string[,] grid, int col)
        {
            List<int[]> moveOptions = ValidMoves(grid);

            // Run through all valid moves
            foreach (int[] moveOption in moveOptions)
            {
                // If the chosen move is a possible move
                if (col == moveOption[0])
                {
                    // Return the column and row
                    return new int[] { col, moveOption[1] };
                }
            }

            API.API.DisplayMessage("Column provided invalid");
            return null;
        }

        // Checks if a column is an allowed move
        public static bool ValidateCol(string[,] grid, int col)
        {
            List<int[]> ValidMoves = GameBoard.ValidMoves(grid);

            // Run through all possible moves. If the column is allowed return true
            foreach (int[] possibleMove in ValidMoves)
            {
                if (col == possibleMove[0]) return true;
            }
            return false;
        }
    }
}