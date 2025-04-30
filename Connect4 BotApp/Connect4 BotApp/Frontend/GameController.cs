using Connect4_BotApp.API;

namespace Connect4_BotApp.Frontend
{
    // Handles game cycle (originally from Bot), user input (originally from Bot), console display logic of the old GameGrid class
    internal static class GameController
    {
        private static string[,] grid = new string[7, 6];
        private static bool gameRunning = true;
        private static string turn = "X";

        // The startup function
        private static void Main()
        {
            for (int c = 0; c < grid.GetLength(0); c++)
            {
                for (int r = 0; r < grid.GetLength(1); r++)
                {
                    grid[c, r] = " ";
                }
            }

            GameLoop();
        }


        // PUBLIC METHODS

        // Displays a string to the console
        public static void DisplayMessage(string msg)
        {
            Console.WriteLine(msg);
        }



        // PRIVATE METHODS

        // Gameplay loop here
        private static void GameLoop()
        {
            // Creates an empty board with 7 columns, 6 rows
            while (gameRunning)
            {
                TurnDisplay();

                // On turn O, run the bot
                Console.WriteLine("Starting bot search");
                int bestCol = API.API.BestMove(grid, turn);
                Console.WriteLine($"Best move is {bestCol}");

                // Gets user input on their move
                int col = Convert.ToInt16(Console.ReadLine());
                Console.WriteLine("");
                // Makes move onto the game board
                grid = API.API.MakeMove(grid, turn, col);
                // If the player won, don't switch turn
                if (!gameRunning) break;

                // Switch turn
                if (turn == "X") turn = "O";
                else turn = "X";
            }

            Console.Clear();
            DisplayGame();
            Console.WriteLine($"Game ended. Player {turn} won.");
        }

        // Displays necessary information for a player to have their turn
        private static void TurnDisplay()
        {
            Console.Clear();
            DisplayGame();
            Console.WriteLine($"Player {turn}, enter column (0-6)");

        }

        // Displays the game on screen
        private static void DisplayGame()
        {
            // Moves down the rows
            for (int r = 5; r >= 0; r--)
            {
                Console.WriteLine(new string('-', 43)); // Header of row index "r"
                string rowContent = "";

                // Row contents
                for (int c = 0; c < 7; c++)
                {
                    rowContent += $"|  {grid[c, r]}  ";
                }
                rowContent += "|";

                Console.WriteLine(rowContent);
            }
            Console.WriteLine(new string('-', 43)); // Table bottom

            /*
             *    .....
             *    -------------------     Header of row index 1
             *    |  X  |     |  O  |     Row contents
             *    -------------------     Header of row index 0
             *    |     |     |  X  |     Row contents
             *    -------------------     Table bottom
             * 
             */

        }
    }
}
