using ChessBoard;
using System;

using System.Collections.Generic;
using ChessBoard.Excepetions;

namespace Chess
{
    class ChessMatch
    {
        public Board Board { get; private set; }
        public int Turn { get; private set; }
        public Color CurrentPlayer { get; private set; }
        public bool Finished { get; private set; }

        private HashSet<Piece> Pieces;
        private HashSet<Piece> Captured;

        public bool Check { get; private set; }
        public Piece VulnerableEnPassant { get; private set; }

        public ChessMatch()
        {
            Board = new Board(8, 8);
            Turn = 1;
            CurrentPlayer = Color.White;
            Finished = false;
            Check = false;
            VulnerableEnPassant = null;

            Pieces = new HashSet<Piece>();
            Captured = new HashSet<Piece>();

            PlacePieces();
        }

        public Piece Move(Position origin, Position destination)
        {
            Piece p = Board.RemovePiece(origin);
            p.AddMoveCount();

            Piece pieceCaptured = Board.RemovePiece(destination);
            Board.InsertPiece(p, destination);

            if (pieceCaptured != null)
            {
                Captured.Add(pieceCaptured);
            }

            // #SpecialMove Castling short
            if (p is King && destination.Column == origin.Column + 2)
            {
                Position RookOrigin = new Position(origin.Line, origin.Column + 3);
                Position RookDestination = new Position(origin.Line, origin.Column + 1);
                Piece R = Board.RemovePiece(RookOrigin);
                R.AddMoveCount();
                Board.InsertPiece(R, RookDestination);
            }

            // #SpecialMove Castling long
            if (p is King && destination.Column == origin.Column - 2)
            {
                Position RookOrigin = new Position(origin.Line, origin.Column - 4);
                Position RookDestination = new Position(origin.Line, origin.Column - 1);
                Piece R = Board.RemovePiece(RookOrigin);
                R.AddMoveCount();
                Board.InsertPiece(R, RookDestination);
            }

            // #SpecialMove En Passant
            if (p is Pawn)
            {
                if (origin.Column != destination.Column && pieceCaptured == null)
                {
                    Position posP;
                    if (p.Color == Color.White)
                    {
                        posP = new Position(destination.Line + 1, destination.Column);
                    }
                    else
                    {
                        posP = new Position(destination.Line - 1, destination.Column);
                    }

                    pieceCaptured = Board.RemovePiece(posP);
                    Captured.Add(pieceCaptured);
                }
            }

            return pieceCaptured;
        }

        public void UndoMove(Position origin, Position destination, Piece pieceCaptured)
        {
            Piece p = Board.RemovePiece(destination);
            p.SubtractMoveCount();
            if (pieceCaptured != null)
            {
                Board.InsertPiece(pieceCaptured, destination);
                Captured.Remove(pieceCaptured);
            }

            Board.InsertPiece(p, origin);

            // #SpecialMove Castling short
            if (p is King && destination.Column == origin.Column + 2)
            {
                Position RookOrigin = new Position(origin.Line, origin.Column + 3);
                Position RookDestination = new Position(origin.Line, origin.Column + 1);
                Piece R = Board.RemovePiece(RookDestination);
                R.SubtractMoveCount();
                Board.InsertPiece(R, RookOrigin);
            }

            // #SpecialMove Castling long
            if (p is King && destination.Column == origin.Column - 2)
            {
                Position RookOrigin = new Position(origin.Line, origin.Column - 4);
                Position RookDestination = new Position(origin.Line, origin.Column - 1);
                Piece R = Board.RemovePiece(RookDestination);
                R.SubtractMoveCount();
                Board.InsertPiece(R, RookOrigin);
            }

            // #SpecialMove En Passant
            if (p is Pawn)
            {
                if (origin.Column != destination.Column && pieceCaptured == VulnerableEnPassant)
                {
                    Piece pawn = Board.RemovePiece(destination);
                    Position posP;
                    if (p.Color == Color.White)
                    {
                        posP = new Position(3, destination.Column);
                    }
                    else
                    {
                        posP = new Position(4, destination.Column);
                    }

                    Board.InsertPiece(pawn, posP);
                }
            }
        }


        public void PlayTurn(Position origin, Position destination)
        {
            Piece pieceCaptured = Move(origin, destination);

            if (IsInCheck(CurrentPlayer))
            {

                UndoMove(origin, destination, pieceCaptured);
                throw new BoardException("You can't put yourself in check!");
;           }

            Piece p = Board.Piece(destination);
            
            // #SpecialMove Promotion
            if (p is Pawn)
            {
                if ((p.Color == Color.White && destination.Line == 0) || (p.Color == Color.Black && destination.Line == 7))
                {
                    p = Board.RemovePiece(destination);
                    Pieces.Remove(p);
                    Piece queen = new Queen(Board, p.Color);
                    Board.InsertPiece(queen, destination);
                    Pieces.Add(queen);
                }
            }

            if (IsInCheck(Adversary(CurrentPlayer)))
            {
                

                Check = true;
            }
            else
            {
                Console.WriteLine("hi");

                Check = false;
            }

            if (IsInCheckmate(Adversary(CurrentPlayer)))
            {
                Finished = true;
            }
            else
            {
                Turn++;
                ChangePlayer();
            }


            // #SpecialMove En Passant
            if (p is Pawn && (destination.Line == origin.Line - 2 || destination.Line == origin.Line + 2))
            {
                VulnerableEnPassant = p;
            }
            else
            {
                VulnerableEnPassant = null;
            }


        }

        public void ValidateOriginPosition(Position pos)
        {
            if (Board.Piece(pos) == null)
            {
                throw new BoardException("No piece in given position.");
            }
            if (CurrentPlayer != Board.Piece(pos).Color)
            {
                throw new BoardException("Given piece is not yours.");
            }
            if (!Board.Piece(pos).HasPossibleMoves())
            {
                throw new BoardException("No possible moves for given piece.");
            }
        }

        public void ValidateDestinationPosition(Position origin, Position destination)
        {
            if (!Board.Piece(origin).PossibleMove(destination))
            {
                throw new BoardException("Destination is invalid.");
            }
        }

        public void ChangePlayer()
        {
            if (CurrentPlayer == Color.White)
            {
                CurrentPlayer = Color.Black;
            }
            else
            {
                CurrentPlayer = Color.White;
            }
        }

        public HashSet<Piece> CapturedPieces(Color color)
        {
            HashSet<Piece> aux = new HashSet<Piece>();

            foreach (Piece x in Captured)
            {
                if (x.Color == color)
                {
                    aux.Add(x);
                }
            }
            // return all captured pieces of given color
            return aux;
        }

        public HashSet<Piece> InGamePieces(Color color)
        {
            HashSet<Piece> aux = new HashSet<Piece>();

            foreach (Piece x in Pieces)
            {
                if (x.Color == color)
                {
                    aux.Add(x);
                }
            }

            aux.ExceptWith(CapturedPieces(color));
            // return all in game pieces of given color
            return aux;
        }

        private Color Adversary(Color color)
        {
            if (color == Color.White)
            {
                return Color.Black;
            }
            else
            {
                return Color.White;
            }
        }

        private Piece King(Color color)
        {
            foreach(Piece x in InGamePieces(color))
            {
                if (x is King)
                {
                    return x;
                }
            }

            return null;
        }

        public bool IsInCheck(Color color)
        {
            Piece K = King(color);

            if (K == null)
            {
                throw new BoardException("No king for " + color + "on the board!");
            }

            foreach(Piece x in InGamePieces(Adversary(color)))
            {
                bool[,] mat = x.PossibleMoves();
                if (mat[K.Position.Line, K.Position.Column])
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsInCheckmate(Color color)
        {
            if (!IsInCheck(color))
            {
                return false;
            }

            foreach(Piece x in InGamePieces(color))
            {
                bool[,] mat = x.PossibleMoves();
                /* plays ALL possible moves of all the pieces, 
                 * accordingly to the given color,
                 * and tests to see
                 * if any move removes the check
                 */
                for (int i = 0; i < Board.Lines; i++)
                {
                    for (int j = 0; j < Board.Columns; j++)
                    {
                        if (mat[i,j])
                        {
                            Position origin = x.Position;
                            Position destination = new Position(i, j);
                            Piece capturedPiece = Move(origin, destination);
                            bool testCheck = IsInCheck(color);
                            UndoMove(origin, destination, capturedPiece);

                            if (!testCheck)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void PlaceNewPiece(char column, int line, Piece piece)
        {
            Board.InsertPiece(piece, new ChessPosition(column, line).ToPosition());
            Pieces.Add(piece);
        }

        private void PlacePieces()
        {
            PlaceNewPiece('a', 1, new Rook(Board, Color.White));
            PlaceNewPiece('b', 1, new Knight(Board, Color.White));
            PlaceNewPiece('c', 1, new Bishop(Board, Color.White));
            PlaceNewPiece('d', 1, new Queen(Board, Color.White));
            PlaceNewPiece('e', 1, new King(Board, Color.White, this));
            PlaceNewPiece('f', 1, new Bishop(Board, Color.White));
            PlaceNewPiece('g', 1, new Knight(Board, Color.White));
            PlaceNewPiece('h', 1, new Rook(Board, Color.White));
            PlaceNewPiece('a', 2, new Pawn(Board, Color.White, this));
            PlaceNewPiece('b', 2, new Pawn(Board, Color.White, this));
            PlaceNewPiece('c', 2, new Pawn(Board, Color.White, this));
            PlaceNewPiece('d', 2, new Pawn(Board, Color.White, this));
            PlaceNewPiece('e', 2, new Pawn(Board, Color.White, this));
            PlaceNewPiece('f', 2, new Pawn(Board, Color.White, this));
            PlaceNewPiece('g', 2, new Pawn(Board, Color.White, this));
            PlaceNewPiece('h', 2, new Pawn(Board, Color.White, this));

            PlaceNewPiece('a', 8, new Rook(Board, Color.Black));
            PlaceNewPiece('b', 8, new Knight(Board, Color.Black));
            PlaceNewPiece('c', 8, new Bishop(Board, Color.Black));
            PlaceNewPiece('d', 8, new Queen(Board, Color.Black));
            PlaceNewPiece('e', 8, new King(Board, Color.Black, this));
            PlaceNewPiece('f', 8, new Bishop(Board, Color.Black));
            PlaceNewPiece('g', 8, new Knight(Board, Color.Black));
            PlaceNewPiece('h', 8, new Rook(Board, Color.Black));
            PlaceNewPiece('a', 7, new Pawn(Board, Color.Black, this));
            PlaceNewPiece('b', 7, new Pawn(Board, Color.Black, this));
            PlaceNewPiece('c', 7, new Pawn(Board, Color.Black, this));
            PlaceNewPiece('d', 7, new Pawn(Board, Color.Black, this));
            PlaceNewPiece('e', 7, new Pawn(Board, Color.Black, this));
            PlaceNewPiece('f', 7, new Pawn(Board, Color.Black, this));
            PlaceNewPiece('g', 7, new Pawn(Board, Color.Black, this));
            PlaceNewPiece('h', 7, new Pawn(Board, Color.Black, this));
        }
    }
}
