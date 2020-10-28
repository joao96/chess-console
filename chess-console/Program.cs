using System;
using chess_console;
using Chess;
using ChessBoard.Excepetions;

namespace ChessBoard
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Board board = new Board(8, 8);

                board.InsertPiece(new Rook(board, Color.Black), new Position(0, 0));
                board.InsertPiece(new Rook(board, Color.Black), new Position(1, 3));
                board.InsertPiece(new King(board, Color.Black), new Position(0, 2));

                Screen.PrintBoard(board);
            }
            catch (BoardException e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
