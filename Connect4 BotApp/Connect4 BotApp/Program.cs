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
        int[] pieceCounts = new int[8];
        int gridMaxCol = grid.GetLength(0)-1;
        int gridMaxRow = grid.GetLength(1)-1;

        // The gradients to explore each direction
        int[][] direcs = new int[][]
        {
            new int[] { 0, 1 },
            new int[] { 1, 1 },
            new int[] { 1, 0 },
            new int[] { 1, -1 },
            new int[] { 0, -1 },
            new int[] { -1, -1 },
            new int[] { -1, 0 },
            new int[] { -1, 1 },
        };

        // Removes the repeated use of the long if in-bounds check
        Func<int[], bool> validPoint = (spot) =>
        {
            if (spot[0] > gridMaxCol || spot[0] < 0) return false;
            if (spot[1] > gridMaxRow || spot[1] < 0) return false;
            return true;
        };

        // The index to put the piece count into
        int pieceCountsIndex = 0;
        // Run through all directions
        foreach (var direc in direcs)
        {
            int[] newSpot = (int[])move.Clone();
            int pieceCount = 0;

            do
            {
                Console.WriteLine($"Checking {newSpot[0]}, {newSpot[1]}");
                // If the piece matches the player's piece, add to count. Else end loop
                if (grid[newSpot[0], newSpot[1]] == turn) pieceCount++;
                else break;

                // Move to next spot
                newSpot[0] += direc[0];
                newSpot[1] += direc[1];
            }
            while (validPoint(newSpot));

            // Gets the first column and row when going in chosen direction
            pieceCounts[pieceCountsIndex] = pieceCount;
            pieceCountsIndex++;

        }

        // For testing
        for (int i = 0; i < pieceCounts.Length; i++)
        {
            Console.WriteLine(pieceCounts[i]);
        }

        // Use the values in pieceCounts to determine if there is 4 in a row

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

            // Gets user input on their move
            int col = Convert.ToInt16(Console.ReadLine());
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
