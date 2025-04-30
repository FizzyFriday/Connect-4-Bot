using Connect4_BotApp.API;
using Connect4_BotApp.Backend;

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
        public static (string[,], bool) MakeMove(string[,] grid, string turn, int col)
        {
            // Translates the provided column into a move
            int[]? move = TranslateColToMove(grid, col);

            // If the move doesn't exist, eg column 1110343 provided
            if (move == null)
            {
                API.API.DisplayMessage("Move wasn't valid");
                return (grid, false);
            }

            // Make move and return results
            grid[move[0], move[1]] = turn;
            string state = Bot.MoveResult(grid, move, turn).endState;



            return grid;
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
    }
}