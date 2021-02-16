using System;

// https://www.chessprogramming.org/Getting_Started

namespace chess
{
  class Game
  {
    // https://en.wikipedia.org/wiki/Board_representation_(computer_chess)#Square_list
    private int[,] board;

    private bool whitesTurn; // if true, it is white's turn, else - black's

    public Game(bool withAi)
    {
      if (withAi) throw new Exception("Ai is not supported yet");

      NewBoard();
      whitesTurn = true;
    }

    private void NewBoard()
    {
      board = new int[12, 12] {
        {7,7,7,7,7,7,7,7,7,7,7,7},
        {7,7,7,7,7,7,7,7,7,7,7,7},
        {7,7,-4,-2,-3,-5,-6,-3,-2,-4,7,7},
        {7,7,-1,-1,-1,-1,-1,-1,-1,-1,7,7},
        {7,7,0,0,0,0,0,0,0,0,7,7},
        {7,7,0,0,0,0,0,0,0,0,7,7},
        {7,7,0,0,0,0,0,0,0,0,7,7},
        {7,7,0,0,0,0,0,0,0,0,7,7},
        {7,7,1,1,1,1,1,1,1,1,7,7},
        {7,7,4,2,3,5,6,3,2,4,7,7},
        {7,7,7,7,7,7,7,7,7,7,7,7},
        {7,7,7,7,7,7,7,7,7,7,7,7}
      };
    }

    public void DisplayBoard()
    {
      Console.Clear();
      for (int y = 2; y < 10; y++)
      {
        for (int x = 2; x < 10; x++)
        {
          Console.Write("{0} ", board[y, x]);
        }
        Console.WriteLine("");
      }
    }
  }
}
