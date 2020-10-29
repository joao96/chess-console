using System.Reflection.PortableExecutable;
using ChessBoard;
using ChessBoard.Excepetions;

namespace Chess
{
    class ChessMatch
    {
        public Board Board { get; private set; }
        public int Turn { get; private set; }
        public Color CurrentPlayer { get; private set; }
        public bool Finished { get; private set; }

        public ChessMatch()
        {
            Board = new Board(8, 8);
            Turn = 1;
            CurrentPlayer = Color.White;
            Finished = false;
            PlacePieces();
        }

        public void Move(Position origin, Position destination)
        {
            Piece p = Board.RemovePiece(origin);
            p.AddMoveCount();
            Piece pieceCaptured = Board.RemovePiece(destination);
            Board.InsertPiece(p, destination);
        }

        public void PlayTurn(Position origin, Position destination)
        {
            Move(origin, destination);
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

        private void PlacePieces()
        {
            Board.InsertPiece(new Rook(Board, Color.White), new ChessPosition('c', 1).ToPosition());
            Board.InsertPiece(new Rook(Board, Color.White), new ChessPosition('c', 2).ToPosition());
            Board.InsertPiece(new Rook(Board, Color.White), new ChessPosition('d', 2).ToPosition());
            Board.InsertPiece(new Rook(Board, Color.White), new ChessPosition('e', 2).ToPosition());
            Board.InsertPiece(new Rook(Board, Color.White), new ChessPosition('e', 1).ToPosition());

            Board.InsertPiece(new King(Board, Color.White), new ChessPosition('d', 1).ToPosition());

            Board.InsertPiece(new Rook(Board, Color.Black), new ChessPosition('c', 7).ToPosition());
            Board.InsertPiece(new Rook(Board, Color.Black), new ChessPosition('c', 8).ToPosition());
            Board.InsertPiece(new Rook(Board, Color.Black), new ChessPosition('d', 7).ToPosition());
            Board.InsertPiece(new Rook(Board, Color.Black), new ChessPosition('e', 7).ToPosition());
            Board.InsertPiece(new Rook(Board, Color.Black), new ChessPosition('e', 8).ToPosition());

            Board.InsertPiece(new King(Board, Color.Black), new ChessPosition('d', 8).ToPosition());

        }
    }
}
