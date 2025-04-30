using Connect4_BotApp.Frontend;
using System.Diagnostics;

namespace Connect4_BotApp.Backend
{
    // Handles all communication between Frontend and Backend
    internal static class API
    {
        // Starts the bot
        public static int BestMove(string[,] grid, string turn, int col)
        {
            int bestCol = Bot.StartBot(grid, turn, col);
            return bestCol;
        }

        // Returns a list of allowed moves
        public static List<int[]> ValidMoves(string[,] grid)
        {
            return GameBoard.ValidMoves(grid);
        }

        // Makes a move on grid and returns it
        public static string[,] MakeMove(string[,] grid, string turn, int col)
        {
            return GameBoard.MakeMove(grid, turn, col);
        }

        // Only method the backend is allowed to call in API, and only method 
        // allowed to use GameController. Used for debugging
        public static void DisplayMessage(string msg)
        {
            GameController.DisplayMessage(msg);
        }
    }
}