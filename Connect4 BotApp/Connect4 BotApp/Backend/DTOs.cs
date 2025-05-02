using Connect4_BotApp.API;
using Connect4_BotApp.Backend;
using System.Runtime.CompilerServices;

namespace Connect4_BotApp
{
    internal class GameSit
    {
        public string[,] grid;
        public string turn;

        public GameSit(string[,] grid, string turn)
        {
            this.grid = grid;
            this.turn = turn;
        }
    }
}