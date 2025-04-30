// Contains non UI or gameplay logic methods of old GameGrid class such as GetValidMoves

using Connect4_BotApp.Backend;

namespace Connect4_BotApp
{
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

        // Makes a move on the real game board. Should it have game end checking?
        public static string[,] MakeMove(string[,] grid, string turn, int col)
        {
            List<int[]> moveOptions = Backend.Bot.ValidMoves(grid);

            bool moveSuccess = false;
            foreach (int[] moveOption in moveOptions)
            {
                // If the chosen move is a possible move
                if (col == moveOption[0])
                {
                    grid[col, moveOption[1]] = turn;
                    moveSuccess = true;
                    // Check if game ended
                }
            }
            // If the move wasn't an option, display this wasn't valid
            if (!moveSuccess)
            {
                Bot.DisplayMessage("Move wasn't valid");
            }
            return grid;
        }
    }
}