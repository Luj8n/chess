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

      Console.WriteLine("End");
    }
  }
}
