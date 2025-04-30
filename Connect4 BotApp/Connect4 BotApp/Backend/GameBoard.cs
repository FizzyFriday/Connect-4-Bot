// Contains non UI or gameplay logic methods of old GameGrid class such as GetValidMoves

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
    }
}