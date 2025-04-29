using System;
using System.Runtime.CompilerServices;


// Represents the nodes of the tree
public class Node
{
    // Contains the children
    public List<Node> children = new List<Node>();

    // Contains the state of the game after this node's move
    // Eg, win, draw, defeat, still playing
    public string postMoveGameState = "";
}


// Contains the game board, and helping functions such as display
public class GameGrid
{
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

    // Runs through the game board, determining where pieces can go
    public List<int[]> GetAllPossibleMoves()
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
    }

    public bool MakeMove(int col, string turn)
    {
        List<int[]> moveOptions = GetAllPossibleMoves();
        bool inPlay = true;

        foreach (int[] moveOption in moveOptions)
        {
            // If the chosen move is a possible move
            if (col == moveOption[0])
            {
                // Makes the move
                grid[col, moveOption[1]] = turn;

                // Checks if the game has ended
                inPlay = CheckGameInPlay(moveOption, turn);
            }
        }
        return inPlay;
    }

    private bool CheckGameInPlay(int[] move, string turn)
    {
        int gridMaxCol = grid.GetLength(0)-1;
        int gridMaxRow = grid.GetLength(1)-1;

        // The gradients to explore each direction
        int[][] positiveDirecs = new int[][]
        {
            new int[] { 0, 1 },
            new int[] { 1, 1 },
            new int[] { 1, 0 },
            new int[] { 1, -1 },
        };

        // Removes the repeated use of the long if in-bounds check
        Func<int[], bool> validPoint = (spot) =>
        {
            if (spot[0] > gridMaxCol || spot[0] < 0) return false;
            if (spot[1] > gridMaxRow || spot[1] < 0) return false;
            return true;
        };

        // Does the looped counting
        Func<int[], int[], int> countLoop = (newSpot, gradient) =>
        {
            int loopPieceCount = 0;

            return loopPieceCount;
        };

        // Run through all directions
        foreach (var direc in positiveDirecs)
        {
            // Represents direc in the opposite direction
            int[] negativeDirec = new int[2] { direc[0] * -1, direc[1] * -1 };
            int pieceCounts = 1; // Set to 1 as the placed piece counts to a connect 4

            // Gets where the 1st next spot is when moving towards the gradient and opposite of it
            int nextPositiveSpotCol = move[0] + direc[0];
            int nextPositiveSpotRow = move[1] + direc[1];
            int nextNegativeSpotCol = move[0] + negativeDirec[0];
            int nextNegativeSpotRow = move[1] + negativeDirec[1];

            int[] nextPosSpot = new int[2] { nextPositiveSpotCol, nextPositiveSpotRow };
            int[] nextNegSpot = new int[2] { nextNegativeSpotCol, nextNegativeSpotRow };


            // Set pieceCount to the result of countLoop for direc
            // Add the result of countLoop for negativeDirec

            // If pieceCount >= 4, return false
        }

        return true;
    }
}


// The main class, processing tree searching and evaluation
public static class Bot
{ 
    public static int maxDepth = 3;
    public static GameGrid gameGrid;
    public static Node root;
    public static bool gameRunning = true;
    private static string turn = "X";

    static void Main()
    {
        // Creates an empty board with 7 columns, 6 rows
        gameGrid = new GameGrid(7, 6);
        root = new Node();

        while (gameRunning)
        {
            gameGrid.DisplayGame();
            Console.WriteLine($"Player {turn}, enter column (0-6)");

            // Gets user input on their move
            int col = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("");
            // Makes move on gameGrid, and checks if game ended
            gameRunning = gameGrid.MakeMove(col, turn);

            if (turn == "X") turn = "O";
            else turn = "X";
        }
    }

    // Handles the MCTS logic - Search, Expand, Simulate, Backprogate
    static void MCTS()
    { 
        // SEARCH
        // Searches through the tree until reaching a leaf node - no children

        // EXPAND
        // Unless a node that ends the game, add a random node to tree

        // SIMULATE
        // Run a rollout

        // BACKPROGATE
        // Send the rollout results up the tree
    }
}
