using Connect4_BotApp.Frontend;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Connect4_BotApp.Backend
{
    // Handles MCTS logic / tree searching
    internal static class Bot
    {
        private static double MCTSpermittedTime = 2;



        // PUBLIC METHODS
        // Starts the bot's search
        public static int StartBot(string[,] grid, string turn, int col)
        {
            return MCTSmanager(grid, turn, col);
        }



        // PRIVATE METHODS

        // Manages and deals with results of MCTS
        private static int MCTSmanager(string[,] grid, string turn, int col)
        {
            Node root = new Node(grid, turn);

            int MCTScycles = 0;

            // Stopwatches time running
            Stopwatch timer = new Stopwatch();
            timer.Start();

            // Use HeuristicManager here to check for connect 4 for player or enemy made in 1 turn
            // Run MCTS for the allowed time
            while (timer.Elapsed.TotalSeconds < MCTSpermittedTime)
            {
                MCTS(root);
                MCTScycles++;
            }

            /*
            // This code may go in GameController, or be sent to GameController.DisplayMessage as 1 string
            // Displays the results of the search, and important debugging info.
            Console.WriteLine("--Final results--");
            int mostSims = 0;
            // Search through all possible moves
            for (int i = 0; i < root.children.Count; i++)
            {
                // Grab the node, and display info
                Node directChild = root.children[i];
                Console.WriteLine($"Sim count of column {directChild.move[0]}: {directChild.simCount}. WinPref: {directChild.resultPoints / directChild.simCount}");
                // If this node has more simulations then the best, this must be the best move
                if (directChild.simCount > mostSims)
                {
                    mostSims = directChild.simCount;
                    bestCol = directChild.move[0];
                }
            }
            // The child of root with most simulations is best move
            Console.WriteLine($"{MCTScycles} runs occured, or {MCTScycles / timer.Elapsed.TotalSeconds}per/s");
            Console.WriteLine($"Best Move - {bestCol}");
            */

            return bestCol;
        }

        // Handles the MCTS logic - Search, Expand, Simulate, Backprogate
        private static void MCTS(Node node)
        {
            // SEARCH - Searches using UCT until reaching node with no children
            while (node.children.Count > 0)
            {
                // Compare UCT of all children, and highest uct is picked
                node = BestUCTChild(node);
                if (node == null) return;
            }

            // EXPAND - Unless node ends the game, add random node to tree
            if (node.isInTree())
            {
                // Choose a random potential child
                node = node.GetRandPotential();
            }

            // Gets the result of the game after the move
            var resultAndValue = MoveResult(node);
            node.postMoveState = resultAndValue.endState;
            double value = resultAndValue.value;
            node.AddToTree();

            // SIMULATE
            // If not a game ending node, simulate
            if (node.postMoveState == "IP")
            {
                value = Rollout(node);
            }

            // BACKPROGATE
            while (node.parentNode != null)
            {
                // Save the result to each node
                node.simCount++;
                node.resultPoints += value;
                // Move up to parent
                node = node.parentNode;
            }
        }

        // Randomly searches until hitting a game ending result
        private static double Rollout(Node node)
        {
            Node rolled = node;

            // Runs until a node has no moves it can do
            while (rolled.gameGrid.GetValidMoves().Count > 0)
            {
                // Moves to a random potential child
                rolled = rolled.GetRandPotential();

                // Gets the state of the game after node
                var resultAndHeuristic = MoveResult(rolled);

                // Game ending, return result
                if (resultAndHeuristic.endState != "IP") return resultAndHeuristic.value;
            }
            return 0;
        }



        // GetBestChild for Node has been moved here to better align with responsibilities
        // Although it returns a Node's child, UCT is directly a part of MCTS and so should be in Bot
        private static Node BestUCTChild(Node node)
        {
            // Contains a node for the children and potential children not in the tree
            double bestUCT = 0;
            Node best = node.children[0];

            // Calculate the UCT for each child in tree
            foreach (Node child in node.children)
            {
                // This line causes the issue. 
                // Don't allow moving to a game ending node
                if (child.postMoveState != "IP") continue;

                double uct = node.CalculateUCT();
                // If its UCT is the highest seen, the child is the new best
                if (uct > bestUCT)
                {
                    bestUCT = uct;
                    best = child;
                }
            }

            // Since all potential children will have the simCount = 0, and so same uct
            // Only using the first potential child is needed
            if (node.potentialChildren.Count > 0)
            {
                // Create a node for the 1st potential child
                int[] potentialMove = node.potentialChildren[0];
                Node potentialChild = new Node(node.GetPostMoveGrid(), this.GetSwitchedTurn(), potentialMove, this);
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
    }
}

/*

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
            // If the player won, don't switch turn
            if (!gameRunning) break;

            // Switch turn
            if (turn == "X") turn = "O";
            else turn = "X";
        }

        Console.Clear();
        gameGrid.DisplayGame();
        Console.WriteLine($"Game ended. Player {turn} won.");
    }

    // Manages the repetitions of MCTS, calling the MCTS function
    // Deals with the results of MCTS
    static void MCTSmanager()
    {
        Node root = new Node(gameGrid, turn);
        root.postMoveState = "IP"; // The root represents current game, which is in play

        // Allowed time to run MCTS
        int permittedDuration = 2;
        int MCTScycles = 0;

        // Stopwatches time running
        Stopwatch timer = new Stopwatch();
        timer.Start();

        // Checks the immediate moves for Win in 1 or Loss in 1 
        int bestCol = GetObviousBest(root);
        if (bestCol != -1) Console.WriteLine($"Obvious best - {bestCol}");
        if (bestCol == -1)
        {
            // Run MCTS for the allowed time
            while (timer.Elapsed.TotalSeconds < permittedDuration)
            {
                MCTS(root);
                MCTScycles++;
            }
        }

        // Displays the results of the search, and important debugging info.
        Console.WriteLine("--Final results--");
        int mostSims = 0;
        // Search through all possible moves
        for (int i = 0; i < root.children.Count; i++)
        {
            // Grab the node, and display info
            Node directChild = root.children[i];
            Console.WriteLine($"Sim count of column {directChild.move[0]}: {directChild.simCount}. WinPref: {directChild.resultPoints / directChild.simCount}");
            // If this node has more simulations then the best, this must be the best move
            if (directChild.simCount > mostSims)
            {
                mostSims = directChild.simCount;
                bestCol = directChild.move[0];
            }
        }
        // The child of root with most simulations is best move
        Console.WriteLine($"{MCTScycles} runs occured, or {MCTScycles / timer.Elapsed.TotalSeconds}per/s");
        Console.WriteLine($"Best Move - {bestCol}");
    }

    // Handles the MCTS logic - Search, Expand, Simulate, Backprogate
    private static void MCTS(Node node)
    {
        // SEARCH - Searches using UCT until reaching node with no children
        while (node.children.Count > 0)
        {
            // Compare UCT of all children, and highest uct is picked
            node = node.GetBestChild();
            if (node == null) return;
        }

        // EXPAND - Unless node ends the game, add random node to tree
        if (node.GetInTree())
        {
            // Choose a random potential child
            node = node.GetRandPotential();
        }

        // Gets the result of the game after the move
        var resultAndValue = MoveResult(node);
        node.postMoveState = resultAndValue.endState;
        double value = resultAndValue.value;
        node.AddToTree();

        // SIMULATE
        // If not a game ending node, simulate
        if (node.postMoveState == "IP")
        {
            value = Rollout(node);
        }

        // BACKPROGATE
        while (node.parentNode != null)
        {
            // Save the result to each node
            node.simCount++;
            node.resultPoints += value;
            // Move up to parent
            node = node.parentNode;
        }
    }


    // Calls MoveResult, providing the necessary data as a Node
    public static (string endState, double value) MoveResult(int[] move, string moveTurn)
    {
        Node translatedNode = new Node((GameGrid)gameGrid.Clone(), moveTurn, move, null);
        var resultCache = MoveResult(translatedNode);
        return resultCache;
    }

    // Gets the state of the game after a node, if it was a Win, Draw, Loss or still in play
    public static (string endState, double value) MoveResult(Node node)
    {
        int gridMaxCol = gameGrid.grid.GetLength(0) - 1;
        int gridMaxRow = gameGrid.grid.GetLength(1) - 1;

        // The gradients to explore each direction
        int[][] positiveDirecs = new int[][]
        {
            [0, 1], [1, 1], [1, 0], [1, -1]
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
            while (node.gameGrid.grid[newSpot[0], newSpot[1]] == node.turn)
            {
                connectedCount++;
                newSpot[0] += gradient[0];
                newSpot[1] += gradient[1];
                if (validPoint(newSpot) == false) break;
            }
            return connectedCount;
        };

        int mostConnected = 1;
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

            // Saves the highest number of connections made
            if (pieceCounts > mostConnected)
            {
                mostConnected = pieceCounts;
            }
        }

        // The heuristic value of how long connections are
        // The larger the connection made, the bigger change to value.
        // A draw/in play could range from 0.4 to 0.6 instead of only 0.5
        // This makes better moves more valuable and gives the winrate calculating
        // in CalculateUCT be more accurate
        double[] heuristicValues = [0, 0.05, 0.15];

        if (mostConnected >= 4)
        {
            // If the game has ended and the winner is clear
            if (node.turn == turn) return ("W", 1);
            else return ("L", 0);
        }

        // Get the heuristic change to make
        double heuristicChange = heuristicValues[mostConnected - 1];
        // Return the general state of the game and the heuristic
        if (node.gameGrid.GetValidMoves().Count == 0) return ("D", 0.5 + heuristicChange);
        else return ("IP", 0.5 + heuristicChange);
    }

    // Randomly searches until hitting a game ending result
    private static double Rollout(Node node)
    {
        Node rolled = node;

        // Runs until a node has no moves it can do
        while (rolled.gameGrid.GetValidMoves().Count > 0)
        {
            // Moves to a random potential child
            rolled = rolled.GetRandPotential();

            // Gets the state of the game after node
            var resultAndHeuristic = MoveResult(rolled);

            // Game ending, return result
            if (resultAndHeuristic.endState != "IP") return resultAndHeuristic.value;
        }
        return 0;
    }

    private static int GetObviousBest(Node root)
    {
        // Grabs all possible moves
        List<int[]> possible = root.gameGrid.GetValidMoves();
        int bestCol = -1;

        // Run through each move
        foreach (int[] possibleMove in possible)
        {
            // Loss checking
            string enemyTurn = "O";
            if (turn == "O") enemyTurn = "X";
            // Get the move result cache
            var result = MoveResult(possibleMove, enemyTurn);

            // The win is a win for the enemy, or defeat
            if (result.endState == "L") bestCol = possibleMove[0];

            // Win checking
            result = MoveResult(possibleMove, turn);
            if (result.endState == "W") return possibleMove[0];
        }
        return bestCol;
    }
}
*/