using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;



// Represents the nodes of the tree
public class Node
{
    // A total of the results from playouts (+= 1 from win, += 0.5 from draw)
    private double resultPoints = 0;
    // Represents the total playouts
    private int simCount = 0;

    // Contains the state of the game after this node's move
    // Eg, win, draw, defeat, still playing
    public string postMoveState = "";

    // Contains the children
    public List<Node> children = new List<Node>();

    // Contains the potential children that aren't nodes yet
    public List<int[]> potentialChildren = new List<int[]>();

    public string turn;
    public int[] move = new int[2];
    public Node? parentNode;
    // Represents what the board looks like for this node
    public GameGrid gameGrid;


    // Root constructor. A root doesnt have a move or a parent
    public Node(GameGrid gameGrid, string turn)
    {
        this.gameGrid = gameGrid;
        this.turn = turn;

        // Adds children of root to the tree
        List<int[]> possibleMoves = this.gameGrid.GetAllPossibleMoves();
        foreach (int[] move in possibleMoves)
        {
            Node child = new Node(this.gameGrid, turn, move, this);
            this.children.Add(child);
        }
    }

    // Main constructor
    public Node(GameGrid gameGrid, string turn, int[] move, Node? parent)
    {
        this.gameGrid = (GameGrid)gameGrid.Clone();
        this.turn = turn;
        this.move = move;
        this.parentNode = parent;
        potentialChildren = this.gameGrid.GetAllPossibleMoves();
    }


    public Node getBestUCTChild()
    {
        // Contains a node for the children and potential children not in the tree
        List<Node> childrenAndPotential = new(this.children);

        // Creates a node for all potential children
        foreach (int[] move in this.potentialChildren)
        {
            GameGrid postGrid = getPostMoveGrid();
            string postTurn = getSwitchedTurn();

            // Creates a temporarily used node. Isn't added to the tree
            Node potentialChild = new Node(postGrid, postTurn, move, this);
            childrenAndPotential.Add(potentialChild);
        }

        double bestUCT = 0;
        Node best = this.children[0];
        // Run through all nodes
        foreach (Node node in childrenAndPotential)
        {
            // calculate the uct, and compare to the best
            double uct = node.calculateUCT();
            if (uct > bestUCT)
            {
                bestUCT = uct;
                best = node;
            }
        }
        // return the node with the highest uct
        return best;
    }

    // Gets what the grid will look like after the move
    private GameGrid getPostMoveGrid()
    {
        GameGrid postMoveGrid = (GameGrid)this.gameGrid.Clone();
        postMoveGrid.MakeMove(this.move[0], this.turn);
        return postMoveGrid;
    }

    // Returns the turn of children, by switching
    private string getSwitchedTurn()
    {
        if (this.turn == "X") return "O";
        return "X";
    }

    private double calculateUCT()
    {
        // Impacts if UCT will favour high winrate, or exploration
        double explorationParameter = Math.Sqrt(2);

        // If simCount != 0, set selfSims to simCount. Else, set it to 1
        int selfSims = (simCount == 0) ? 1 : simCount;

        // Gets the value of win rate
        double winPref = resultPoints / selfSims;

        // Epsilon removes the possibility of Log(1) happening, which produces 0
        const double epsilon = 1e-6;
        // If parent isnt null, set pSims to the simCount of parent. Else, set it to the same value as selfSims
        double pSims = (this.parentNode != null) ? this.parentNode.simCount : selfSims;
        if (pSims == 0) pSims = 1;

        double naturalLog = Math.Log(pSims + epsilon);
        // Gets the value of exploration
        double explorePref = explorationParameter * Math.Sqrt(naturalLog / selfSims);

        return winPref + explorePref;
    }
}


// Contains the game board, and helping functions such as display
public class GameGrid : ICloneable
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

    // realMove indicates if this is a move that should be made to the current game
    public bool MakeMove(int col, string turn, bool realMove=false)
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
                if (realMove)
                {
                    // Checks if the game has ended
                    inPlay = CheckGameInPlay(moveOption, turn);
                }
            }
        }
        return inPlay;
    }

    private bool CheckGameInPlay(int[] move, string turn)
    {
        // Instead use the results from Bot.moveResult()
        return false;
    }
}


// The main class, processing tree searching and evaluation
public static class Bot
{ 
    public static int maxDepth = 3;
    public static GameGrid gameGrid;
    public static bool gameRunning = true;
    private static string turn = "X";

    static void Main()
    {
        // Creates an empty board with 7 columns, 6 rows
        gameGrid = new GameGrid(7, 6);

        while (gameRunning)
        {
            Console.Clear();
            gameGrid.DisplayGame();
            MCTSmanager();
            Console.WriteLine($"Player {turn}, enter column (0-6)");

            // Gets user input on their move
            int col = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("");
            // Makes move on gameGrid, and checks if game ended
            gameRunning = gameGrid.MakeMove(col, turn);

            if (turn == "X") turn = "O";
            else turn = "X";
        }

        Console.Clear();
        gameGrid.DisplayGame();
        Console.WriteLine("Game ended");
    }

    // Manages the repetitions of MCTS, calling the MCTS function
    // Deals with the results of MCTS
    static void MCTSmanager()
    {
        // The allowed repetitions of mcts
        int allowedMCTSReps = 10;
        int MCTSran = 0;
        Node root = new Node(gameGrid, turn);
        

        // Runs 500 times, and adds to the counter
        while (MCTSran < allowedMCTSReps)
        {
            MCTS(root);
            MCTSran++;
        }

        // Use the results
    }

    // Handles the MCTS logic - Search, Expand, Simulate, Backprogate
    static void MCTS(Node node)
    {
        // node = Root
        Console.WriteLine("MCTS called");

        // SEARCH
        // Searches through the tree until reaching a leaf node - no children
        while (node.children.Count > 0)
        {
            // Compare UCT of all children, and highest uct is picked
            node = node.getBestUCTChild();
        }

        // node = Leaf

        // EXPAND
        // Unless a node that ends the game, add a random node to tree

        // SIMULATE
        // Run a rollout

        // BACKPROGATE
        // Send the rollout results up the tree
    }

    // Gets the state of the game after a node, if it was a Win, Draw, Loss or still in play
    public static string MoveResult(Node node)
    {
        int gridMaxCol = gameGrid.grid.GetLength(0) - 1;
        int gridMaxRow = gameGrid.grid.GetLength(1) - 1;

        // The gradients to explore each direction
        int[][] positiveDirecs = new int[][]
        {
            [0, 1],
            [1, 1],
            [1, 0],
            [1, -1]
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
            // If this direction isnt valid, return 0
            if (validPoint(newSpot) == false) return 0;

            int connectedCount = 0;

            // Runs until out of bounds or piece isn't owned by player
            while (gameGrid.grid[newSpot[0], newSpot[1]] == turn)
            {
                connectedCount++;
                newSpot[0] += gradient[0];
                newSpot[1] += gradient[1];
                if (validPoint(newSpot) == false) break;
            }
            return connectedCount;
        };

        // Run through all directions
        foreach (var direc in positiveDirecs)
        {
            // Represents direc in the opposite direction
            int[] negativeDirec = [direc[0] * -1, direc[1] * -1];
            int pieceCounts = 1; // Set to 1 as the placed piece counts to a connect 4

            // Gets where the 1st next spot is when moving towards the gradient and opposite of it
            int[] posNext = [node.move[0] + direc[0], node.move[1] + direc[1]];
            int[] negNext = [node.move[0] + negativeDirec[0], node.move[1] + negativeDirec[1]];

            // Adds the count of connected pieces
            pieceCounts += countLoop(posNext, direc);
            pieceCounts += countLoop(negNext, negativeDirec);

            // A connect 4 was made
            if (pieceCounts >= 4)
            {
                // The turn doesn't match the player - Enemy made the 4
                if (node.turn != turn) return "W"; // L

                // turn matches the player - player made the 4
                if (node.turn == turn) return "L";
            }
        }
        
        // If there are no possible moves, and wasnt a Win or Loss, it must be a draw
        if (node.gameGrid.GetAllPossibleMoves().Count == 0) return "D";

        // Otherwise, still in play
        return "IP";
    }
}
