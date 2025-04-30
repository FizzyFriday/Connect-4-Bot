using Connect4_BotApp.Backend;

namespace Connect4_BotApp.Frontend
{
    // Handles game cycle (originally from Bot), user input (originally from Bot), console display logic of the old GameGrid class
    internal static class GameController
    {
        // Any initialization occurs here
        static void Main()
        {
            DisplayGame();
        }

        // Gameplay loop here
        private static void GameLoop()
        { 
        
        }

        // obvious
        private static void DisplayGame()
        {
            int a = Backend.Bot.testc();
            Console.WriteLine(a);
        }







        // GameGrid code v

        /*
        // 2D array representing the game board and pieces
        public string[,] grid;

        public GameGrid(int colCount, int rowCount)
        {
            grid = new string[colCount, rowCount];

            // Sets each position of board to represent an empty piece
            for (int c = 0; c < colCount; c++)
            {
                for (int r = 0; r < rowCount; r++)
                {
                    grid[c, r] = " ";
                }
            }
        }

        // Deep copies a GameGrid object
        public object Clone()
        {
            var newObj = new GameGrid(this.grid.GetLength(0), this.grid.GetLength(1))
            {
                grid = (string[,])grid.Clone()
            };
            return newObj;
        }


        // Runs through the game board, determining where pieces can go
        public List<int[]> GetValidMoves()
        {
            List<int[]> options = new List<int[]>();
            for (int col = 0; col < 7; col++)
            {
                int currentRow = 5; // Highest row index
                                    // Checks if the top spot is empty
                if (grid[col, currentRow] != " ") continue;

                // Move down gradually, finding the lowest point that is empty as this is where the piece would go
                while (grid[col, currentRow - 1] == " ")
                {
                    currentRow--;
                    // Prevents checking negative indices
                    if (currentRow == 0) break;
                }

                // Set the column and row of the move into an array
                int[] moveOption = new int[2] { col, currentRow };
                options.Add(moveOption);
            }
            return options;
        }

        // Displays the game on screen
        public void DisplayGame()
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
        /*
        }

        // realMove indicates if this is a move that should be made to the current game
        public bool MakeMove(int col, string turn, bool realMove = false)
        {
            List<int[]> moveOptions = GetValidMoves();

            foreach (int[] moveOption in moveOptions)
            {
                // If the chosen move is a possible move
                if (col == moveOption[0])
                {
                    // Makes the move
                    grid[col, moveOption[1]] = turn;
                    if (realMove)
                    {
                        // Get the state of the game after the move
                        string afterMoveState = Bot.MoveResult(moveOption, turn).endState;
                        // If game not in play - game ended
                        if (afterMoveState != "IP") return false;
                    }
                }
            }
            return true;
        }
        */
    }
}
