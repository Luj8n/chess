using System;
using System.Collections.Generic;

// https://www.chessprogramming.org/Getting_Started

namespace chess
{
  struct Position
  {
    // board's (12x12) coords
    public int x;
    public int y;
  }
  class Game
  {
    // https://en.wikipedia.org/wiki/Board_representation_(computer_chess)#Square_list
    // but it's flipped: white is top, balck is bottom (easier this way)
    private int[,] board;

    private bool whitesTurn; // if true, it is white's turn, else - black's

    public Game(bool withAi)
    {
      if (withAi) throw new Exception("Ai is not supported yet");

      NewBoard();
      whitesTurn = true;
    }

    public Position[] ConvertMove(string[] move)
    {
      // move[0] - a2, move[1] - a4
      Position fromPos = new Position();
      Position toPos = new Position();
      fromPos.x = (char.ToUpper(move[0][0])) - 63; // converts character to its alphabet integer position
      fromPos.y = int.Parse(move[0][1].ToString()) + 1;
      toPos.x = (char.ToUpper(move[1][0])) - 63;
      toPos.y = int.Parse(move[1][1].ToString()) + 1;
      return new Position[2] { fromPos, toPos };
    }

    private void NewBoard()
    {
      board = new int[12, 12] {
        {7,7,7,7,7,7,7,7,7,7,7,7},
        {7,7,7,7,7,7,7,7,7,7,7,7},
        {7,7,4,2,3,5,6,3,2,4,7,7},
        {7,7,1,1,1,1,1,1,1,1,7,7},
        {7,7,0,0,0,0,0,0,0,0,7,7},
        {7,7,0,0,0,0,0,0,0,0,7,7},
        {7,7,0,0,0,0,0,0,0,0,7,7},
        {7,7,0,0,0,0,0,0,0,0,7,7},
        {7,7,-1,-1,-1,-1,-1,-1,-1,-1,7,7},
        {7,7,-4,-2,-3,-5,-6,-3,-2,-4,7,7},
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
