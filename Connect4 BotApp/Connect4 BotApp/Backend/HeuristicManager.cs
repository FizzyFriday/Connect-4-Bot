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
