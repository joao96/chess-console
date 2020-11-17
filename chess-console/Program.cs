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
                    try
                    {
                        Console.Clear();
                        Screen.PrintMatch(match);

                        Console.WriteLine();
                        Console.Write("Origin: ");
                        Position origin = Screen.ReadChessPosition().ToPosition();

                        match.ValidateOriginPosition(origin);

                        bool[,] possiblePositions = match.Board.Piece(origin).PossibleMoves();
                    
                        Console.Clear();
                        Screen.PrintBoard(match.Board, possiblePositions);
                        Console.WriteLine();

                        Console.Write("Destination: ");
                        Position destination = Screen.ReadChessPosition().ToPosition();

                        match.ValidateDestinationPosition(origin, destination);

                        match.PlayTurn(origin, destination);
                    }
                    catch (BoardException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }
                }
                Console.Clear();
                Screen.PrintMatch(match);


            }
            catch (BoardException e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
