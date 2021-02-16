using System;

namespace chess
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Start");

      Game game1 = new Game(false);

      game1.DisplayBoard();

      Position[] pos = game1.ConvertMove(new string[2] { "a2", "a4" });

      Console.Write(pos[0].x);
      Console.Write(" ");
      Console.Write(pos[0].y);

      Console.WriteLine();

      Console.Write(pos[1].x);
      Console.Write(" ");
      Console.Write(pos[1].y);

      Console.WriteLine("End");
    }
  }
}
