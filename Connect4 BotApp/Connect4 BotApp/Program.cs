using System;
using System.Runtime.CompilerServices;


// Represents the nodes of the tree
public class Node
{ 
    
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

        while (gameRunning)
        {
            // Display the board

            // Gets user input on their move
            int col = Convert.ToInt16(Console.ReadLine());

            // Make move on gameGrid, and checks if game ended

            if (turn == "X") turn = "O";
            else turn = "X";
        }
    }

}
