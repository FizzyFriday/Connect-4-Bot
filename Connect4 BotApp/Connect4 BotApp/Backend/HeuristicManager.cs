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
        // PUBLIC METHODS

        // Runs all the heuristic calls and returns results
        public static double GetHeuristics(Node node)
        {


            return -1;
        }



        // Returns endStae from move and the connection made
        // currentTurn - Turn of the current player, which doesnt change at any point
        public static string EndState(Node node, string currentTurn)
        {
            int gridMaxCol = node.grid.GetLength(0) - 1;
            int gridMaxRow = node.grid.GetLength(1) - 1;

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
                while (node.grid[newSpot[0], newSpot[1]] == node.turn)
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

            if (mostConnected >= 4 && node.turn != currentTurn) return "L";
            else if (mostConnected >= 4 && node.turn == currentTurn) return "L";
            else if (GameBoard.ValidMoves(node.grid).Count == 0) return "D";
            else return "IP";
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

        /*
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
        */
    }
}
