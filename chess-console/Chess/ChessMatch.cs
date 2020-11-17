using ChessBoard;
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

        public ChessMatch()
        {
            Board = new Board(8, 8);
            Turn = 1;
            CurrentPlayer = Color.White;
            Finished = false;
            Check = false;

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
        }


        public void PlayTurn(Position origin, Position destination)
        {
            Piece pieceCaptured = Move(origin, destination);

            if (IsInCheck(CurrentPlayer))
            {
                UndoMove(origin, destination, pieceCaptured);
                throw new BoardException("You can't put yourself in check!");
;           }

            if (IsInCheck(Adversary(CurrentPlayer)))
            {
                Check = true;
            }
            else
            {
                Check = false;
            }


            Turn++;
            ChangePlayer();
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
            if (!Board.Piece(origin).CanMoveTo(destination))
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

        public void PlaceNewPiece(char column, int line, Piece piece)
        {
            Board.InsertPiece(piece, new ChessPosition(column, line).ToPosition());
            Pieces.Add(piece);
        }

        private void PlacePieces()
        {
            PlaceNewPiece('c', 1, new Rook(Board, Color.White));
            PlaceNewPiece('c', 2, new Rook(Board, Color.White));
            PlaceNewPiece('d', 2, new Rook(Board, Color.White));
            PlaceNewPiece('e', 2, new Rook(Board, Color.White));
            PlaceNewPiece('e', 1, new Rook(Board, Color.White));
            PlaceNewPiece('d', 1, new King(Board, Color.White));

            PlaceNewPiece('c', 7, new Rook(Board, Color.Black));
            PlaceNewPiece('c', 8, new Rook(Board, Color.Black));
            PlaceNewPiece('d', 7, new Rook(Board, Color.Black));
            PlaceNewPiece('e', 7, new Rook(Board, Color.Black));
            PlaceNewPiece('e', 8, new Rook(Board, Color.Black));
            PlaceNewPiece('d', 8, new King(Board, Color.Black));
        }
    }
}
