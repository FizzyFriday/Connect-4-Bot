using Connect4_BotApp.API;
using Connect4_BotApp.Backend;

namespace Connect4_BotApp
{
    // Stores function related to the node and tree
    internal class Node
    {
        // A total from the heuristics produced by HeuristicManager
        public double rewardPoints = 0;
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
        public string[,] grid;


        // Root constructor
        public Node(string[,] grid, string turn)
        {
            this.grid = grid;
            this.turn = turn;
            this.potentialChildren = GameBoard.ValidMoves(grid);
            this.postMoveState = "IP"; // Since this is the root, the game must be in play
        }

        // Regular constructor
        public Node(string[,] grid, string turn, int[] move, Node? parentNode)
        {
            this.grid = grid;
            this.turn = turn;
            this.potentialChildren = GameBoard.ValidMoves(grid);
            this.move = move;
            this.parentNode = parentNode;
        }


        // PUBLIC METHODS

        // Returns the UCT of node
        public double CalculateUCT()
        {
            // Impacts if UCT will favour high winrate, or exploration
            double explorationParameter = Math.Sqrt(2);
            int selfSims = (simCount == 0) ? 1 : simCount;

            // Calculates exploitation part of UCT formula
            double rewardPref = rewardPoints / selfSims;

            // Calculates exploration part of UCT formula
            const double epsilon = 1e-6;
            // If parent isnt null, set pSims to the simCount of parent. Else, set it to the same value as selfSims
            double pSims = (this.parentNode != null) ? this.parentNode.simCount : selfSims;
            if (pSims == 0) pSims = 1;
            double naturalLog = Math.Log(pSims + epsilon);
            double explorationPref = explorationParameter * Math.Sqrt(Math.Log(pSims + epsilon) / selfSims);

            return rewardPref + explorationPref;
        }

        // Chooses a random child from potential children - Used in rollout
        public Node GetRandPotential()
        {
            Random rand = new Random();

            // Chooses a random move from the list of potentialChildren
            int potentialCount = this.potentialChildren.Count;
            int potentialChildIndex = rand.Next(0, potentialCount);
            int[] potentialMove = this.potentialChildren[potentialChildIndex];

            // Create the randomly chosen node
            return CreateChild(potentialMove);
        }

        // Creates a Child node - useful for when not in tree
        public Node CreateChild(int[] move)
        {
            return new Node(GetPostMoveGrid(), GetSwitchedTurn(), move, this);
        }

        // Checks if in tree - Identical to old GetInTree method
        public bool IsInTree()
        {
            // If node is the root, its in the tree
            if (this == Bot.root) return true;
            else if (this.parentNode == null)
            {
                API.API.DisplayMessage("Error in Node.GetInTree() - Node has no parent and isn't Root");
                return false;
            }

            // Checks if the parent has this node as a child in tree
            if (this.parentNode.children.Contains(this)) return true;
            else return false;
        }

        // Makes adding to the tree simpler in the Bot class
        public void AddToTree()
        {
            // Checks if the node is in tree already, to prevent repeated adding
            if (IsInTree())
            {
                API.API.DisplayMessage("Error in Node.AddToTree() - Attempted to add a node already existing in tree");
                throw new Exception();
            }

            // Checks if node has parent
            if (this.parentNode == null)
            {
                API.API.DisplayMessage("Error in Node.AddToTree() - Attempted to add a node without a parent");
                throw new Exception();
            }

            this.parentNode.children.Add(this);

            // Since when adding to tree, this is no longer a *potential* child, 
            // it would be removed from the list of potentialChildren
            this.parentNode.potentialChildren.Remove(this.move);
        }




        // PRIVATE METHODS

        // The game board after the node's move
        private string[,] GetPostMoveGrid()
        {
            string[,] postGrid = (string[,])this.grid;
            // Make move on grid
            return postGrid;
        }

        // Returns the turn of children, by switching
        private string GetSwitchedTurn()
        {
            if (this.turn == "X") return "O";
            return "X";
        }
    }
}