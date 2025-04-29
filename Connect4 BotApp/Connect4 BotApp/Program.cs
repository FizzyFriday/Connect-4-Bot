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

    public bool MakeMove(int col, string turn)
    {
        List<int[]> moveOptions = GetAllPossibleMoves();
        // Run through all moves, and check if a move's column matches col
        // Make move, and check if game ended. Return the result.
        return false;
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
            // Display the board

            // Gets user input on their move
            int col = Convert.ToInt16(Console.ReadLine());
            // Makes move on gameGrid, and checks if game ended
            gameGrid.MakeMove(col, turn);

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
