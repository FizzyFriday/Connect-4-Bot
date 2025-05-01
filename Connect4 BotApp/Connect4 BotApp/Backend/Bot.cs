using Connect4_BotApp.API;
using System.Diagnostics;

namespace Connect4_BotApp.Backend
{
    // Handles MCTS logic / tree searching
    internal static class Bot
    {
        private static double MCTSpermittedTime = 2;

        // Contains the information from API
        // Removes the need for calling every method with this information as parameters
        private static (string[,] grid, string turn) inputCache = new();

        // Contains root of tree. The reference helps with Node.IsInTree method
        public static Node? root;


        // PUBLIC METHODS
        // Starts the bot's search
        public static int StartBot(string[,] grid, string turn)
        {
            inputCache = (grid, turn);
            int bestCol = MCTSmanager();
            return bestCol;
        }



        // PRIVATE METHODS

        // Manages and deals with results of MCTS
        private static int MCTSmanager()
        {
            Console.WriteLine("Begin");
            root = new Node(inputCache.grid, inputCache.turn);

            int MCTScycles = 0;

            // Stopwatches time running
            Stopwatch timer = new Stopwatch();
            timer.Start();

            // Use HeuristicManager here to check for connect 4 for player or enemy made in 1 turn
            // Run MCTS for the allowed time
            while (timer.Elapsed.TotalSeconds < MCTSpermittedTime)
            {
                MCTS(root);
                Console.WriteLine(root.simCount);
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

            int mostSims = 0;
            int bestCol = -1;
            for (int i = 0; i < root.children.Count; i++)
            {
                // Grab the node, and display info
                Node directChild = root.children[i];
                // If this node has more simulations then the best, this must be the best move
                if (directChild.simCount > mostSims)
                {
                    mostSims = directChild.simCount;
                    bestCol = directChild.move[0];
                }
            }
            Console.WriteLine("Result");

            return bestCol;
        }

        // Handles the MCTS logic - Search, Expand, Simulate, Backprogate
        private static void MCTS(Node node)
        {
            Console.WriteLine("Start");
            // SEARCH - Searches using UCT until reaching node with no children
            while (node.children.Count > 0)
            {
                // Compare UCT of all children, and highest uct is picked
                node = BestUCTChild(node);
                if (node == null) return;
                Console.WriteLine("Got");
            }

            // EXPAND - Unless node ends the game, add random node to tree
            if (node.IsInTree())
            {
                Console.WriteLine("Expand");
                // Choose a random potential child
                node = node.GetRandPotential();
            }
            if (node != root) Console.WriteLine("Success");

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
            Console.WriteLine("Rollout");

            // Runs until a node has no moves it can do
            while (GameBoard.ValidMoves(rolled.grid).Count > 0)
            {
                // Moves to a random potential child
                rolled = rolled.GetRandPotential();

                // Gets the state of the game after node
                var resultAndHeuristic = MoveResult(rolled);

                // Game ending, return result
                if (resultAndHeuristic.endState != "IP") return resultAndHeuristic.value;
            }
            Console.WriteLine("Ran");
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
                Node potentialChild = node.CreateChild(potentialMove);

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


        // How MoveResult is called by Bot and other classes should be simplified
        // Move it into HeuristicManager?

        // Calls MoveResult, providing the necessary data as a Node. Used by other components
        //public static (string endState, double value) MoveResult(string[,] grid, int[] move, string moveTurn)
        //{
        //    Node translatedNode = new Node(grid, moveTurn, move, null);
        //    var resultCache = MoveResult(translatedNode);
        //    return resultCache;
        //}
    }
}