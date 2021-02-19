using System;
using System.Collections.Generic;

// https://www.chessprogramming.org/Getting_Started

namespace chess
{
  struct Position
  {
    public int x;
    public int y;
    public Position Copy()
    {
      Position newPos = new Position();
      newPos.x = this.x;
      newPos.y = this.y;
      return newPos;
    }
  }
  class Game
  {
    // https://en.wikipedia.org/wiki/Board_representation_(computer_chess)#Square_list
    // but it's flipped: white is top, black is bottom (easier this way)
    public int[,] BOARD;

    private bool whitesTurn; // if true, it is white's turn, else - black's

    private int[,] knightRelativeMoves = new int[8, 2] { { -2, 1 }, { -1, 2 }, { 1, 2 }, { 2, 1 }, { 2, -1 }, { 1, -2 }, { -1, -2 }, { -2, -1 } };

    private int InCheck = 0; // -1 - black. 1 - white. 0 - no one


    public Game(bool withAi)
    {
      if (withAi) throw new Exception("Ai is not supported yet");

      BOARD = NewBoard();
      whitesTurn = true;
    }

    public bool TryMove(Position from, Position to, int[,] board)
    {
      int fromPiece = board[from.y, from.x];
      int toPiece = board[to.y, to.x];

      int fromPieceSide = fromPiece / Math.Abs(fromPiece); // -1 - black. 1 - white
      if (toPiece == 0) goto someLabel;

      // avoid this if toPiece is 0
      int toPieceSide = toPiece / Math.Abs(toPiece); // -1 - black. 1 - white
      if (toPieceSide != fromPieceSide) goto someLabel;
      return false; // cant attack your own piece
    someLabel:;
      // copy board
      int[,] newBoard = board.Clone() as int[,];
      newBoard[to.y, to.x] = fromPiece;
      newBoard[from.y, from.x] = 0;
      if (IsCheck(newBoard) == fromPieceSide) return false; // cant make yourself check
      return true;
    }

    public Position[] AvailableMoves(Position pos, int[,] board)
    {
      // gets available moves for a piece that is in pos
      int oldPiece = board[pos.y, pos.x];
      int oldSide = oldPiece / Math.Abs(oldPiece); // -1 - black. 1 - white
      List<Position> moves = new List<Position>();
      switch (board[pos.y, pos.x])
      {
        case 2: // knight
        case -2:
          for (int i = 0; i < 8; i++)
          {
            Position newPos = new Position();
            newPos.x = pos.x + knightRelativeMoves[i, 1];
            newPos.y = pos.y + knightRelativeMoves[i, 0];
            if (TryMove(pos, newPos, board)) moves.Add(newPos);
          }
          break;
        case 3: // bishop
        case -3:
          // check to top/right
          {
            int someFunc(Position inputPos)
            {
              int curPiece = board[inputPos.y, inputPos.x];
              if (TryMove(pos, inputPos, board)) return 1;
              if (curPiece != 0) return -1;
              return 0;
            }
            Position[] newMoves = Diagonally(pos, someFunc);
            moves.AddRange(newMoves);
            break;
          }
        case 4:
        case -4:
          // check to the top
          for (int y = pos.y - 1; y > 1; y--)
          {
            Position newPos = new Position();
            newPos.y = y;
            newPos.x = pos.x;
            int piece = board[newPos.y, newPos.x];
            if (piece != 0)
            {
              if (TryMove(pos, newPos, board)) moves.Add(newPos);
              break;
            }
            if (TryMove(pos, newPos, board)) moves.Add(newPos);
          }
          // check to the bottom
          for (int y = pos.y + 1; y < 10; y++)
          {
            Position newPos = new Position();
            newPos.y = y;
            newPos.x = pos.x;
            int piece = board[newPos.y, newPos.x];
            if (piece != 0)
            {
              if (TryMove(pos, newPos, board)) moves.Add(newPos);
              break;
            }
            if (TryMove(pos, newPos, board)) moves.Add(newPos);
          }
          // check to the left
          for (int x = pos.x - 1; x > 1; x--)
          {
            Position newPos = new Position();
            newPos.y = pos.y;
            newPos.x = x;
            int piece = board[newPos.y, newPos.x];
            if (piece != 0)
            {
              if (TryMove(pos, newPos, board)) moves.Add(newPos);
              break;
            }
            if (TryMove(pos, newPos, board)) moves.Add(newPos);
          }
          // check to the right
          for (int x = pos.x + 1; x < 10; x++)
          {
            Position newPos = new Position();
            newPos.y = pos.y;
            newPos.x = x;
            int piece = board[newPos.y, newPos.x];
            if (piece != 0)
            {
              if (TryMove(pos, newPos, board)) moves.Add(newPos);
              break;
            }
            if (TryMove(pos, newPos, board)) moves.Add(newPos);
          }
          break;
        // case 1:
        //   {
        //     // check if is on the first line
        //     Position newPos = new Position();
        //     newPos.y = pos.y + 1;
        //     newPos.x = pos.x;
        //     if (TryMove(pos, newPos, board)) moves.Add(newPos);
        //     if (pos.y == 3)
        //     {
        //       newPos.y = pos.y + 1;
        //       if (TryMove(pos, newPos, board)) moves.Add(newPos);

        //     }
        //     break;
        //   }
        default:
          return new Position[0];
      }
      return moves.ToArray();
    }

    private Position[] Diagonally(Position start, Func<Position, int> function)
    {
      List<Position> moves = new List<Position>();
      Position copy = start.Copy(); // making a copy to not alter the input "start"

      // check to top/right
      while (copy.y > 1 && copy.x < 10)
      {
        copy.y--;
        copy.x++;
        int reply = function(copy);
        if (reply == 1) moves.Add(copy);
        else if (reply == -1) break;
      }
      copy = start.Copy();

      // check to bottom/right
      while (copy.y < 10 && copy.x < 10)
      {
        copy.y++;
        copy.x++;
        int reply = function(copy);
        if (reply == 1) moves.Add(copy);
        else if (reply == -1) break;
      }
      copy = start.Copy();

      // check to bottom/left
      while (copy.y < 10 && copy.x > 1)
      {
        copy.y++;
        copy.x--;
        int reply = function(copy);
        if (reply == 1) moves.Add(copy);
        else if (reply == -1) break;
      }
      copy = start.Copy();

      // check to top/left
      while (copy.y > 1 && copy.x > 1)
      {
        copy.y--;
        copy.x--;
        int reply = function(copy);
        if (reply == 1) moves.Add(copy);
        else if (reply == -1) break;
      }
      copy = start.Copy();


      return moves.ToArray();
    }

    public int IsCheck(int[,] board)
    {
      // returns -1 if black is being checked,
      // returns 1 if white is being checked,
      // returns 0 if no one is being checked,
      Position whiteK = FindPieces(6, board)[0];
      Position blackK = FindPieces(-6, board)[0];
      if (OneSideCheck(whiteK, 1, board)) return 1;
      if (OneSideCheck(blackK, -1, board)) return -1;
      return 0;
    }

    private bool OneSideCheck(Position King, int side, int[,] board)
    {
      // K - king
      // side: -1 is black, 1 is white
      // first one is y, second is x
      // check for knight
      for (int i = 0; i < 8; i++)
      {
        if (board[King.y + knightRelativeMoves[i, 0], King.x + knightRelativeMoves[i, 1]] == -2 * side) return true;
      }
      // check for rook/queen/king
      // check to the top
      for (int y = King.y - 1; y > 1; y--)
      {
        if (y == King.y - 1 && board[y, King.x] == -6 * side) return true;
        if (board[y, King.x] == -5 * side || board[y, King.x] == -4 * side) return true;
        if (board[y, King.x] != 0) break;
      }
      // check to the bottom
      for (int y = King.y + 1; y < 10; y++)
      {
        if (y == King.y + 1 && board[y, King.x] == -6 * side) return true;
        if (board[y, King.x] == -5 * side || board[y, King.x] == -4 * side) return true;
        if (board[y, King.x] != 0) break;
      }
      // check to the left
      for (int x = King.x - 1; x > 1; x--)
      {
        if (x == King.x - 1 && board[King.y, x] == -6 * side) return true;
        if (board[King.y, x] == -5 * side || board[King.y, x] == -4 * side) return true;
        if (board[King.y, x] != 0) break;
      }
      // check to the right
      for (int x = King.x + 1; x < 10; x++)
      {
        if (x == King.x + 1 && board[King.y, x] == -6 * side) return true;
        if (board[King.y, x] == -5 * side || board[King.y, x] == -4 * side) return true;
        if (board[King.y, x] != 0) break;
      }
      // check for bishop/queen/pawn/king
      // check to top/right
      {
        int y = King.y - 1;
        int x = King.x + 1;
        bool firstCheck = true;
        while (y > 1 && x < 10)
        {
          if (firstCheck && (board[y, x] == -6 * side || board[y, x] == 1)) return true;
          if (board[y, x] == -5 * side || board[y, x] == -3 * side) return true;
          if (board[y, x] != 0) break;
          y--;
          x++;
          firstCheck = false;
        }
      }
      // check to bottom/right
      {
        int y = King.y + 1;
        int x = King.x + 1;
        bool firstCheck = true;
        while (y < 10 && x < 10)
        {
          if (firstCheck && (board[y, x] == -6 * side || board[y, x] == -1)) return true;
          if (board[y, x] == -5 * side || board[y, x] == -3 * side) return true;
          if (board[y, x] != 0) break;
          y++;
          x++;
          firstCheck = false;
        }
      }
      // check to bottom/left
      {
        int y = King.y + 1;
        int x = King.x - 1;
        bool firstCheck = true;
        while (y < 10 && x > 1)
        {
          if (firstCheck && (board[y, x] == -6 * side || board[y, x] == -1)) return true;
          if (board[y, x] == -5 * side || board[y, x] == -3 * side) return true;
          if (board[y, x] != 0) break;
          y++;
          x--;
          firstCheck = false;
        }
      }
      // check to top/left
      {
        int y = King.y - 1;
        int x = King.x - 1;
        bool firstCheck = true;
        while (y > 1 && x > 1)
        {
          if (firstCheck && (board[y, x] == -6 * side || board[y, x] == 1)) return true;
          if (board[y, x] == -5 * side || board[y, x] == -3 * side) return true;
          if (board[y, x] != 0) break;
          y--;
          x--;
          firstCheck = false;
        }
      }
      // if not check, return false
      return false;
    }

    public Position[] FindPieces(int piece, int[,] board)
    {
      // finds the positions of the pieces to find
      List<Position> found = new List<Position>();
      for (int y = 2; y < 10; y++)
      {
        for (int x = 2; x < 10; x++)
        {
          if (board[y, x] == piece)
          {
            Position pos = new Position();
            pos.x = x;
            pos.y = y;
            found.Add(pos);
          }
        }
      }
      // return an array
      return found.ToArray();
    }

    public Position ConvertPos(string move)
    {
      // move - a2
      Position pos = new Position();
      pos.x = (char.ToUpper(move[0])) - 63; // converts character to its alphabet integer position
      pos.y = int.Parse(move[1].ToString()) + 1;
      return pos;
    }

    private int[,] NewBoard()
    {
      int[,] board = new int[12, 12] {
        {7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
        {7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
        {7, 7, 4, 2, 3, 5, 6, 3, 2, 4, 7, 7},
        {7, 7, 1, 0, 1, 1, 1, 1, 1, 1, 7, 7},
        {7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7},
        {7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7},
        {7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7},
        {7, 7, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7},
        {7, 7,-1,-1,-1,-1,-1,-1,-1,-1, 7, 7},
        {7, 7,-4,-2,-3,-5,-6,-3,-2,-4, 7, 7},
        {7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
        {7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7}
      };
      return board;
    }

    public void DisplayBoard(int[,] board)
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
