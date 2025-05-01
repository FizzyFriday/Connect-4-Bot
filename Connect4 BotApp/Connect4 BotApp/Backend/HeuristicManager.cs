using Connect4_BotApp.API;


namespace Connect4_BotApp
{
    // Handles the heuristics for evaluating a move
    // Such as:
    //    Blocking a move
    //    Spotting a winning move
    //    Trap detection logic
    //    Making connect 2's & 3's

    internal class HeuristicManager
    {
        // Represents turn of currentPlayer
        private static string currentTurn = "";


        // PUBLIC METHODS

        // Runs all the heuristic calls and returns results
        public static double GetHeuristics(Node node, string currentPlayerTurn)
        {
            currentTurn = currentPlayerTurn;

            // Main heuristics here
            return -1;
        }

        // Returns the game state after a node's move
        // moveByWho - if the move would be a win if current player
        // played the move, or if set to enemy player's a loss for the current player if
        // enemy played the move
        public static string EndState(string[,] grid, int[] move, string moveByWho)
        {
            int largestConnection = ConnectHeuristic(grid, move, moveByWho);

            if (largestConnection >= 4)
            {
                // Connect 4 move played by current player
                if (moveByWho == currentTurn) return "W";
                // Connect 4 move played by other player
                else return "L";
            }
            else if (GameBoard.ValidMoves(grid).Count == 0) return "D";
            // Still in play
            else return "IP";
        }

        // Returns best move if Win in 1 or Loss in 1
        // Use sparingly, preferably only on Root
        public static int QuickBestResponse(string[,] grid, int[] move)
        {
            // Grabs all possible moves
            List<int[]> possible = GameBoard.ValidMoves(grid);

            // Check for any Win in 1
            foreach (int[] possibleMove in possible)
            {
                // Gets state of game after move, return the move if results in win
                string endState = EndState(grid, move, currentTurn);
                if (endState == "W") return possibleMove[0];
            }

            // Check for any Loss in 1 threats
            foreach (int[] possibleMove in possible)
            {
                // Gets the turn of enemy
                string enemyTurn = "O";
                if (currentTurn == "O") enemyTurn = "X";

                // Gets state of game after move, return the move if
                // results in win for enemy player, or loss
                string endState = EndState(grid, move, enemyTurn);
                if (endState == "W") return possibleMove[0];
            }

            // No obvious best move
            return -1;
        }



        // PRIVATE METHODS

        // Returns endState from move and the connection made
        // playedByWho - The turn of player making move,
        // currentTurn - Turn of the current player, which doesnt change at any point
        private static int ConnectHeuristic(string[,] grid, int[] move, string playedByWho)
        {
            int gridMaxCol = grid.GetLength(0) - 1;
            int gridMaxRow = grid.GetLength(1) - 1;

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
                while (grid[newSpot[0], newSpot[1]] == playedByWho)
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
                int[] posNext = [move[0] + direc[0], move[1] + direc[1]];
                int[] negNext = [move[0] + negativeDirec[0], move[1] + negativeDirec[1]];

                // Adds the count of connected pieces
                pieceCounts += countLoop(posNext, direc);
                pieceCounts += countLoop(negNext, negativeDirec);

                // Saves the highest number of connections made
                if (pieceCounts > mostConnected)
                {
                    mostConnected = pieceCounts;
                }
            }

            //double[] heuristicValues = [0, 0.05, 0.15, 1];

            return mostConnected;

            // The heuristic value of how long connections are
            // The larger the connection made, the bigger change to value.
            // A draw/in play could range from 0.4 to 0.6 instead of only 0.5
            // This makes better moves more valuable and gives the winrate calculating
            // in CalculateUCT be more accurate
            //double[] heuristicValues = [0, 0.05, 0.15];

            //if (mostConnected >= 4)
            //{
            //    // If the game has ended and the winner is clear
            //    if (node.turn == inputCache.turn) return ("W", 1);
            //    else return ("L", 0);
            //}

            // Get the heuristic change to make
            //double heuristicChange = heuristicValues[mostConnected - 1];
            // Return the general state of the game and the heuristic
            //if (GameBoard.ValidMoves(node.grid).Count == 0) return ("D", 0.5 + heuristicChange);
            //else return ("IP", 0.5 + heuristicChange);
        }
    }
}
