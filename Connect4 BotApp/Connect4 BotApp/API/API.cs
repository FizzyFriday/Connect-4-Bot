using Connect4_BotApp.Frontend;
using Connect4_BotApp.Backend;

namespace Connect4_BotApp.API
{
    // Handles all communication between Frontend and Backend
    internal static class API
    {
        // Starts the bot, and returns the best move
        public static int BestMove(string[,] grid, string turn)
        {
            int bestCol = Bot.StartBot(grid, turn);
            return bestCol;
        }

        // Returns if a column is an allowed move
        public static bool ValidateMove(string[,] grid, int col)
        {
            return GameBoard.ValidateCol(grid, col);
        }

        // Returns grid after making move, and if game ended
        public static (string[,] grid, bool gameEnded) MakeMove(string[,] grid, string turn, int col)
        {
            int[]? move = GameBoard.TranslateColToMove(grid, col);
            // If the column couldn't be translated to a  move
            if (move == null)
            {
                DisplayMessage("Error - Column not valid");
                return (grid, false);
            }

            return GameBoard.MakeMove(grid, turn, move);
        }

        // Only method the backend is allowed to call in API, and only method 
        // allowed to use GameController. Used for debugging
        public static void DisplayMessage(string msg)
        {
            GameController.DisplayMessage(msg);
        }
    }
}

