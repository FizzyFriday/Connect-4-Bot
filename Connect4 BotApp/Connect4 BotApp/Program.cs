using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml.Schema;



// Represents the nodes of the tree
public class Node
{
    // A total of the results from playouts (+= 1 from win, += 0.5 from draw)
    public double resultPoints = 0;
    // Represents the total playouts
    public int simCount = 0;

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
    }

    // Main constructor
    public Node(GameGrid gameGrid, string turn, int[] move, Node? parent)
    {
        this.gameGrid = (GameGrid)gameGrid.Clone();
        this.turn = turn;
        this.move = move;
        this.parentNode = parent;
        potentialChildren = this.gameGrid.GetValidMoves();
    }


    // Public Methods

    // Returns the child with the highest UCT, in the tree or potential
    public Node GetBestChild()
    {
        // Contains a node for the children and potential children not in the tree
        double bestUCT = 0;
        Node best = this.children[0];

        // Calculate the UCT for each child in tree
        Console.WriteLine(this.children.Count);
        foreach (Node child in this.children)
        {
            // This line causes the issue
            // Don't allow moving to a game ending node
            if (child.postMoveState != "IP") continue;

            double uct = child.CalculateUCT();
            Console.WriteLine(uct);
            // If its UCT is the highest seen, the child is the new best
            if (uct > bestUCT)
            {
                bestUCT = uct;
                best = child;
            }
        }

        // Since all potential children will have the simCount = 0, and so same uct
        // Only using the first potential child is needed
        if (this.potentialChildren.Count > 0)
        {
            // Create a node for the 1st potential child
            int[] potentialMove = this.potentialChildren[0];
            Node potentialChild = new Node(this.GetPostMoveGrid(), this.GetSwitchedTurn(), potentialMove, this);
            // Calculate uct for the potential child
            double uct = potentialChild.CalculateUCT();
            // If the potential child is the best option, return this node
            if (uct > bestUCT)
            {
                return potentialChild;
            }
        }

        return best;
    }

    // Chooses a random child from potential children
    public Node GetRandPotential()
    {
        // Selects a random potential children
        Random rand = new Random();

        // Chooses a random move from the list of potentialChildren
        int potentialCount = this.potentialChildren.Count;
        int potentialChildIndex = rand.Next(0, potentialCount);
        int[] potentialMove = this.potentialChildren[potentialChildIndex];

        // Create a node for the child
        Node newChild = new Node(this.GetPostMoveGrid(), this.GetSwitchedTurn(), potentialMove, this);
        return newChild;
    }

    // Makes adding to the tree simpler in the Bot class
    public void AddToTree()
    {
        // Checks if the node is in tree already, to prevent repeated adding
        if (this.GetInTree())
        {
            throw new Exception("Error in Node.AddToTree() - Attempted to add a node already existing in tree");
        }

        // Checks if node has parent
        if (this.parentNode == null)
        {
            throw new Exception("Error in Node.AddToTree() - Attempted to add a node without a parent");
        }

        // References parent node
        Node parent = this.parentNode;
        // Adds to tree
        parent.children.Add(this);

        // Since when adding to tree, this is no longer a *potential* child, 
        // it would be removed from the list of potentialChildren
        parent.potentialChildren.Remove(this.move);
    }

    public bool GetInTree()
    {
        if (this.parentNode == null)
        {
            throw new Exception("Error in Node.GetInTree() - Node has no parent. Perhaps a root?");
        }

        // Checks if the parent has this node as a child in tree
        if (this.parentNode.children.Contains(this)) return true;
        else return false;
    }


    // Private methods

    // Gets what the grid will look like after the move
    private GameGrid GetPostMoveGrid()
    {
        GameGrid postMoveGrid = (GameGrid)this.gameGrid.Clone();
        postMoveGrid.MakeMove(this.move[0], this.turn);
        return postMoveGrid;
    }

    // Returns the turn of children, by switching
    private string GetSwitchedTurn()
    {
        if (this.turn == "X") return "O";
        return "X";
    }

    private double CalculateUCT()
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
        Console.WriteLine(winPref + explorePref);
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
    }

    // realMove indicates if this is a move that should be made to the current game
    public bool MakeMove(int col, string turn, bool realMove=false)
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
                    string afterMoveState = Bot.MoveResult(moveOption, turn);
                    // If game not in play - game ended
                    if (afterMoveState != "IP") return false;
                }
            }
        }
        return true;
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
            // Makes move onto the game board
            gameRunning = gameGrid.MakeMove(col, turn, true);

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
        int allowedMCTSReps = 100;
        int MCTSran = 0;
        Node root = new Node(gameGrid, turn);

        // Runs 500 times, and adds to the counter
        while (MCTSran < allowedMCTSReps)
        {
            MCTS(root);
            MCTSran++;
        }

        // The child of root with most simulations is best move
        foreach (Node directChild in root.children)
        {
            Console.WriteLine(directChild.simCount);
        }
    }

    // Handles the MCTS logic - Search, Expand, Simulate, Backprogate
    private static void MCTS(Node node)
    {
        // SEARCH - Searches using UCT until reaching node with no children
        while (node.children.Count > 0)
        {
            // Compare UCT of all children, and highest uct is picked
            node = node.GetBestChild();
        }

        // On 1st iteration - node = root

        // EXPAND - Unless node ends the game, add random node to tree
        // This leaf isn't in the tree, so add to tree
        if (node.parentNode == null) return; // Shouldn't run as leaf shouldn't be the root because of SEARCH
        if (!node.GetInTree())
        {
            // Get the state of game after the node's move
            node.postMoveState = MoveResult(node);
            // Add to tree
            node.AddToTree();
            // Remove from leaf's potential children

            // A node can't run SIMULATION if it ends the game
            if (node.postMoveState != "IP") return;
        }
        // A leaf already in the tree, then pick a random move to extend
        // leaf has 0 children in leaf.children
        else if (node.postMoveState == "IP")
        {
            // Choose a random potential child
            node = node.GetRandPotential();
            node.AddToTree();
        }

        // SIMULATE
        // Get the results of the simulation
        string result = Rollout(node);

        // BACKPROGATE - Send the results up the tree
        double resultPoints = 0;
        // Assign the rollout result to a number
        if (result == "W") resultPoints = 1;
        if (result == "D") resultPoints = 0.5;

        // Runs up the tree
        while (node.parentNode != null)
        {
            // Save the result to each node
            node.simCount++;
            node.resultPoints += resultPoints;
            // Move up to parent
            node = node.parentNode;
        }

        // Node is now the root
        node.simCount++;
        node.resultPoints += resultPoints;
    }


    // Calls MoveResult, providing the necessary data as a Node
    public static string MoveResult(int[] move, string moveTurn)
    {
        Node translatedNode = new Node((GameGrid)gameGrid.Clone(), moveTurn, move, null);
        return MoveResult(translatedNode);
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
        if (node.gameGrid.GetValidMoves().Count == 0) return "D";

        // Otherwise, still in play
        return "IP";
    }

    // Randomly searches until hitting a game ending result
    private static string Rollout(Node node)
    {
        Node rolled = node;

        // Runs until a node has no moves it can do
        while (rolled.gameGrid.GetValidMoves().Count > 0)
        {
            // Moves to a random potential child
            rolled = rolled.GetRandPotential();

            // Gets the state of the game after node
            string result = MoveResult(rolled);

            // Game ending, return result
            if (result != "IP") return result;
        }
        // If the search hit a deadend with no moves, its a draw
        return "D";
    }
}
