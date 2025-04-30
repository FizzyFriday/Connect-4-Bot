using Connect4_BotApp.API;

namespace Connect4_BotApp
{
    // Stores function related to the node and tree
    internal class Node
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
            if (this.parentNode == null)
            {
                API.DisplayMessage("Error in Node.GetInTree() - Node has no parent. Perhaps a root?");
                return false;
            }

            // Checks if the parent has this node as a child in tree
            if (this.parentNode.children.Contains(this)) return true;
            else return false;
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

/*
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
        this.potentialChildren = this.gameGrid.GetValidMoves();
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
        foreach (Node child in this.children)
        {
            // This line causes the issue. 
            // Don't allow moving to a game ending node
            if (child.postMoveState != "IP") continue;

            double uct = child.CalculateUCT();
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

    public double CalculateUCT()
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
*/