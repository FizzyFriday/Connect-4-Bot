using System;


// Represents the nodes of the tree
public class Node
{ 
    
}


// Contains the game board, and helping functions such as display
public class GameGrid
{
    // 2D array representing the game board and pieces
    public string[,] gameGrid;

    public GameGrid(int colCount, int rowCount)
    {
        gameGrid = new string[colCount, rowCount];
        for (int c = 0; c < colCount; c++)
        {
            for (int r = 0; r < rowCount; r++)
            {
                gameGrid[c, r] = " ";
            }
        }
    }
}


// The main class, processing tree searching and evaluation
public static class Bot
{ 

}
