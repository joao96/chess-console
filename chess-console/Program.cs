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
                ChessMatch match = new ChessMatch();

                while(!match.Finished)
                {
                    Console.Clear();
                    Screen.PrintBoard(match.Board);

                    Console.WriteLine();
                    Console.Write("Origin: ");
                    Position origin = Screen.ReadChessPosition().ToPosition();
                    Console.Write("Destination: ");
                    Position destination = Screen.ReadChessPosition().ToPosition();
                    
                    match.Move(origin, destination);
                }

            }
            catch (BoardException e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
