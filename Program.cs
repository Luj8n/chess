using System;

namespace chess
{
  class Program
  {
    static void Main(string[] args)
    {
      Game game1 = new Game(false);

      // game1.DisplayBoard(game1.BOARD);

      Position[] positions = game1.AvailableMoves(game1.ConvertPos("c1"), game1.BOARD);

      foreach (Position pos in positions)
      {
        Console.Write(pos.x);
        Console.Write(" ");
        Console.Write(pos.y);
        Console.WriteLine("");
      }

      // Console.WriteLine(game1.IsCheck(game1.BOARD));
    }
  }
}
