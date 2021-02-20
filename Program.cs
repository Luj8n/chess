using System;
using System.Threading;

namespace chess
{
  class Program
  {
    static void Main(string[] args)
    {
      Game game1 = new Game(false);

      int respone = 0;

      while (respone != 2 && respone != -2)
      {
        game1.DisplayBoard(game1.BOARD);
        string legalFrom = Console.ReadLine();
        string legalTo = Console.ReadLine();
        respone = game1.Move(game1.ConvertPos(legalFrom), game1.ConvertPos(legalTo), game1.BOARD);
        Console.WriteLine(respone);

        Thread.Sleep(1000);
      }

      // Position[][][] positions = game1.GetBlackMoves(game1.BOARD);

      // foreach (var item in positions)
      // {
      //   Console.WriteLine("------------");
      //   foreach (Position pos in item[0])
      //   {
      //     Console.WriteLine(game1.ConvertPos(pos));
      //   }

      //   Console.WriteLine("---");

      //   foreach (Position pos in item[1])
      //   {
      //     Console.WriteLine(game1.ConvertPos(pos));
      //   }
      // }
    }
  }
}
