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

    public Game(bool withAi)
    {
      if (withAi) throw new Exception("Ai is not supported yet");

      BOARD = NewBoard();
    }

    public int Move(Position from, Position to, int[,] board)
    {
      // returns 0 if nothing happens
      // returns 1 if white is being checked
      // returns -1 if black is being checked
      // returns 2 if white is being checkmated
      // returns -2 if black is being checkmated

      // trusting the input to be a legal move
      board[to.y, to.x] = board[from.y, from.x];
      board[from.y, from.x] = 0;
      int inCheck = IsCheck(board);  // -1 - black. 1 - white. 0 - no one

      if (inCheck == 1)
      {
        if (GetWhiteMoves(board).Length == 0) return 2;
        return 1;
      }

      if (inCheck == -1)
      {
        if (GetBlackMoves(board).Length == 0) return -2;
        return -1;
      }

      return 0;
    }

    public Position[][][] GetBlackMoves(int[,] board)
    {
      List<Position[][]> allMoves = new List<Position[][]>();

      for (int y = 2; y < 10; y++)
      {
        for (int x = 2; x < 10; x++)
        {
          if (board[y, x] >= 0) continue;
          Position newPos = new Position();
          newPos.x = x; newPos.y = y;
          Position[][] newMoves = Moves(newPos, board);

          if (newMoves.Length != 0) allMoves.Add(newMoves);
        }
      }

      return allMoves.ToArray();
    }

    public Position[][][] GetWhiteMoves(int[,] board)
    {
      List<Position[][]> allMoves = new List<Position[][]>();

      for (int y = 2; y < 10; y++)
      {
        for (int x = 2; x < 10; x++)
        {
          if (board[y, x] <= 0) continue;

          Position newPos = new Position();
          newPos.x = x; newPos.y = y;
          Position[][] newMoves = Moves(newPos, board);

          if (newMoves.Length != 0) allMoves.Add(newMoves);
        }
      }

      return allMoves.ToArray();
    }

    public Position[][][] GetAllMoves(int[,] board)
    {
      List<Position[][]> allMoves = new List<Position[][]>();

      for (int y = 2; y < 10; y++)
      {
        for (int x = 2; x < 10; x++)
        {
          if (board[y, x] == 0) continue;

          Position newPos = new Position();
          newPos.x = x; newPos.y = y;
          Position[][] newMoves = Moves(newPos, board);

          allMoves.Add(newMoves);
        }
      }

      return allMoves.ToArray();
    }

    private int TryMove(Position from, Position to, int[,] board)
    {
      // returns 0 if it's not possible (tried attacking your own piece)
      // returns 1 if that square is empty and possible to go to
      // returns 2 if it will be a possible capture
      // returns 3 if it will be an impossible capture (will be check)
      // returns 4 if that square is empty and impossible to go to (will be check)
      // returns 5 if that square is not on the board
      int fromPiece = board[from.y, from.x];
      int toPiece = board[to.y, to.x];

      if (toPiece == 7) return 5;

      int fromPieceSide = fromPiece / Math.Abs(fromPiece); // -1 - black. 1 - white

      // copy board and move the piece to the square
      int[,] newBoard = board.Clone() as int[,];
      newBoard[to.y, to.x] = fromPiece;
      newBoard[from.y, from.x] = 0;

      if (toPiece != 0)
      {
        int toPieceSide = toPiece / Math.Abs(toPiece); // -1 - black. 1 - white
        if (toPieceSide == fromPieceSide) return 0; // cant attack your own piece
        // check if it will be check next move. YOU CANT MAKE YOURSELF CHECK!
        if (IsCheck(newBoard) == fromPieceSide) return 3; // cant make yourself check
        return 2;
      }
      // check if it will be check next move. YOU CANT MAKE YOURSELF CHECK!
      if (IsCheck(newBoard) == fromPieceSide) return 3; // cant make yourself check
      return 1; // everything is fine, so return true
    }

    public Position[][] Moves(Position initialPos, int[,] board)
    {
      // return 2 arrays in an array. first one is possible moves on empty squares, second is possible captures

      int piece = board[initialPos.y, initialPos.x];
      switch (piece)
      {
        case 1:
        case -1:
          return PawnMoves(initialPos, board);
        case 6:
        case -6:
          return KingMoves(initialPos, board);
        case 5:
        case -5:
          return QueenMoves(initialPos, board);
        case 4:
        case -4:
          return RookMoves(initialPos, board);
        case 3:
        case -3:
          return BishopMoves(initialPos, board);
        case 2:
        case -2:
          return KnightMoves(initialPos, board);
        default: // empty square
          return new Position[][] { };
      }
    }

    private Position[][] PawnMoves(Position initialPos, int[,] board)
    {
      // return 2 arrays in an array. first one is possible moves on empty squares, second is possible captures

      List<Position> moves = new List<Position>();
      List<Position> captures = new List<Position>();
      Position pos = initialPos.Copy(); // making a copy to not alter the input pos

      int piece = board[initialPos.y, initialPos.x];
      int side = piece / Math.Abs(piece); // -1 - black. 1 - white

      int justMove(Position inputPos)
      {
        int result = TryMove(initialPos, inputPos, board);
        if (result == 1) moves.Add(inputPos);
        if (result == 3 || result == 2 || result == 0) return 0; // stop checking
        return 1; // dont stop checking
      }

      void justCapture(Position inputPos)
      {
        int result = TryMove(initialPos, inputPos, board);
        if (result == 2) captures.Add(inputPos);
      }

      // check one square to the up/down (depending on the side)
      if (side == 1)
      {
        pos.y = initialPos.y + 1;
        if (justMove(pos) == 1 && pos.y == 4) // second white row
        {
          pos.y++;
          justMove(pos);
          pos.y--;
        }
      }
      else
      {
        pos.y = initialPos.y - 1;
        justMove(pos);
        if (justMove(pos) == 1 && pos.y == 7) // second black row
        {
          pos.y--;
          justMove(pos);
          pos.y++;
        }
      }

      // check diagonally (left)
      pos.x = initialPos.x - 1;
      justCapture(pos);

      // check diagonally (right)
      pos.x = initialPos.x + 1;
      justCapture(pos);

      return new Position[][] { moves.ToArray(), captures.ToArray() };
    }

    private Position[][] KingMoves(Position initialPos, int[,] board)
    {
      // return 2 arrays in an array. first one is possible moves on empty squares, second is possible captures

      List<Position> moves = new List<Position>();
      List<Position> captures = new List<Position>();
      Position pos = initialPos.Copy(); // making a copy to not alter the input pos

      void process(Position inputPos)
      {
        int result = TryMove(initialPos, inputPos, board);
        if (result == 2) captures.Add(inputPos);
        if (result == 1) moves.Add(inputPos);
      }

      // check three squares above
      pos.y = initialPos.y - 1;
      pos.x = initialPos.x - 1;
      for (int i = 0; i < 3; i++) { process(pos); pos.x++; };
      pos = initialPos.Copy(); // reseting the position to the initial position

      // check one square to the right
      pos.x++;
      process(pos);
      pos = initialPos.Copy(); // reseting the position to the initial position

      // check three squares below
      pos.y = initialPos.y + 1;
      pos.x = initialPos.x - 1;
      for (int i = 0; i < 3; i++) { process(pos); pos.x++; };
      pos = initialPos.Copy(); // reseting the position to the initial position

      // check one square to the left
      pos.x--;
      process(pos);

      return new Position[][] { moves.ToArray(), captures.ToArray() };
    }

    private Position[][] QueenMoves(Position initialPos, int[,] board)
    {
      // return 2 arrays in an array. first one is possible moves on empty squares, second is possible captures

      List<Position> moves = new List<Position>();
      List<Position> captures = new List<Position>();

      Position[][] bishopLike = BishopMoves(initialPos, board);
      Position[][] rookLike = RookMoves(initialPos, board);

      moves.AddRange(bishopLike[0]);
      moves.AddRange(rookLike[0]);

      captures.AddRange(bishopLike[1]);
      captures.AddRange(rookLike[1]);

      return new Position[][] { moves.ToArray(), captures.ToArray() };
    }

    private Position[][] RookMoves(Position initialPos, int[,] board)
    {
      // return 2 arrays in an array. first one is possible moves on empty squares, second is possible captures
      List<Position> moves = new List<Position>();
      List<Position> captures = new List<Position>();
      Position pos = initialPos.Copy(); // making a copy to not alter the input pos

      int process(Position inputPos)
      {
        int result = TryMove(initialPos, inputPos, board);
        if (result == 2) captures.Add(inputPos);
        if (result == 1) moves.Add(inputPos);
        if (result == 3 || result == 2 || result == 0) return 0; // stop checking
        return 1; // dont stop checking
      }

      // check to top
      while (pos.y > 1) { pos.y--; if (process(pos) == 0) break; }
      pos = initialPos.Copy(); // reseting the position to the initial position

      // check to right
      while (pos.x < 10) { pos.x++; if (process(pos) == 0) break; }
      pos = initialPos.Copy(); // reseting the position to the initial position

      // check to bottom
      while (pos.y < 10) { pos.y++; if (process(pos) == 0) break; }
      pos = initialPos.Copy(); // reseting the position to the initial position

      // check to left
      while (pos.x > 1) { pos.x--; if (process(pos) == 0) break; }

      return new Position[][] { moves.ToArray(), captures.ToArray() };
    }

    private Position[][] BishopMoves(Position initialPos, int[,] board)
    {
      // return 2 arrays in an array. first one is possible moves on empty squares, second is possible captures
      List<Position> moves = new List<Position>();
      List<Position> captures = new List<Position>();
      Position pos = initialPos.Copy(); // making a copy to not alter the input pos

      int process(Position inputPos)
      {
        int result = TryMove(initialPos, inputPos, board);
        if (result == 2) captures.Add(inputPos);
        if (result == 1) moves.Add(inputPos);
        if (result == 3 || result == 2 || result == 0) return 0; // stop checking
        return 1; // dont stop checking
      }

      // check to top/right
      while (pos.y > 1 && pos.x < 10) { pos.y--; pos.x++; if (process(pos) == 0) break; }
      pos = initialPos.Copy(); // reseting the position to the initial position

      // check to bottom/right
      while (pos.y < 10 && pos.x < 10) { pos.y++; pos.x++; if (process(pos) == 0) break; }
      pos = initialPos.Copy(); // reseting the position to the initial position

      // check to bottom/left
      while (pos.y < 10 && pos.x > 1) { pos.y++; pos.x--; if (process(pos) == 0) break; }
      pos = initialPos.Copy(); // reseting the position to the initial position

      // check to top/left
      while (pos.y > 1 && pos.x > 1) { pos.y--; pos.x--; if (process(pos) == 0) break; }

      return new Position[][] { moves.ToArray(), captures.ToArray() };
    }

    private Position[][] KnightMoves(Position initialPos, int[,] board)
    {
      // return 2 arrays in an array. first one is possible moves on empty squares, second is possible captures
      List<Position> moves = new List<Position>();
      List<Position> captures = new List<Position>();
      Position pos = initialPos.Copy(); // making a copy to not alter the input pos

      int[,] relativeMoves = new int[8, 2] { { -2, 1 }, { -1, 2 }, { 1, 2 }, { 2, 1 }, { 2, -1 }, { 1, -2 }, { -1, -2 }, { -2, -1 } };

      for (int i = 0; i < 8; i++)
      {
        pos.y = initialPos.y + relativeMoves[i, 0];
        pos.x = initialPos.x + relativeMoves[i, 1];
        int result = TryMove(initialPos, pos, board);
        if (result == 2) captures.Add(pos);
        if (result == 1) moves.Add(pos);
      }

      return new Position[][] { moves.ToArray(), captures.ToArray() };
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
      // for example: move is a1. it will return Position with x = 2, y = 2
      Position pos = new Position();
      pos.x = (char.ToUpper(move[0])) - 63; // converts character to its alphabet integer position
      pos.y = int.Parse(move[1].ToString()) + 1;
      return pos;
    }

    public string ConvertPos(Position pos)
    {
      // for example: Position's x = 2, y = 2. it will return a string "a1"
      string move = "";
      move += (char)(pos.x + 63);
      move += pos.y - 1;
      return move.ToLower();
    }

    private int[,] NewBoard()
    {
      int[,] board = new int[12, 12] {
        {7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
        {7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
        {7, 7, 4, 2, 3, 5, 6, 3, 2, 4, 7, 7},
        {7, 7, 1, 1, 1, 1, 1, 1, 1, 1, 7, 7},
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
      Console.WriteLine("------------------------");
      for (int y = 2; y < 10; y++)
      {
        for (int x = 2; x < 10; x++)
        {
          if (board[y, x] >= 0) Console.Write(" ");
          Console.Write(board[y, x]);
          if (x < 9) Console.Write("|");
        }
        Console.WriteLine("");
        Console.WriteLine("------------------------");
      }
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
      // side: -1 is black, 1 is white
      // first one is y, second is x
      // check for knight

      int[,] knightRelativeMoves = new int[8, 2] { { -2, 1 }, { -1, 2 }, { 1, 2 }, { 2, 1 }, { 2, -1 }, { 1, -2 }, { -1, -2 }, { -2, -1 } };

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
  }
}
