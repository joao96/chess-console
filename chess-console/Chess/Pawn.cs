using ChessBoard;

namespace Chess
{
    class Pawn : Piece
    {
        private ChessMatch Match;
        public Pawn(Board board, Color color, ChessMatch match) : base(board, color)
        {
            Match = match;
        }

        public override string ToString()
        {
            return "P";
        }

        private bool HasEnemy(Position pos)
        {
            Piece p = Board.Piece(pos);
            return p != null && p.Color != Color;
        }

        private bool IsFree(Position pos)
        {
            return Board.Piece(pos) == null;
        }

        public override bool[,] PossibleMoves()
        {

            bool[,] mat = new bool[Board.Lines, Board.Columns];

            Position pos = new Position(0, 0);

            if (Color == Color.White)
            {
                pos.DefineValues(Position.Line - 1, Position.Column);
                if (Board.IsValidPosition(pos) && IsFree(pos))
                {
                    mat[pos.Line, pos.Column] = true;
                }

                pos.DefineValues(Position.Line - 2, Position.Column);
                Position pos2 = new Position(Position.Line - 1, Position.Column);
                if (Board.IsValidPosition(pos2) && IsFree(pos2) && Board.IsValidPosition(pos) && IsFree(pos) && MoveCount == 0)
                {
                    mat[pos.Line, pos.Column] = true;
                }

                pos.DefineValues(Position.Line - 1, Position.Column - 1);
                if (Board.IsValidPosition(pos) && HasEnemy(pos))
                {
                    mat[pos.Line, pos.Column] = true;
                }

                pos.DefineValues(Position.Line - 1, Position.Column + 1);
                if (Board.IsValidPosition(pos) && HasEnemy(pos))
                {
                    mat[pos.Line, pos.Column] = true;
                }

                //#SpecialMove En Passant
                if (Position.Line == 3)
                {
                    Position left = new Position(Position.Line, Position.Column - 1);
                    if (Board.IsValidPosition(left) && HasEnemy(left) && Board.Piece(left) == Match.VulnerableEnPassant)
                    {
                        mat[left.Line - 1, left.Column] = true;
                    }

                    Position right = new Position(Position.Line, Position.Column + 1);
                    if (Board.IsValidPosition(right) && HasEnemy(right) && Board.Piece(right) == Match.VulnerableEnPassant)
                    {
                        mat[right.Line - 1, right.Column] = true;
                    }
                }
            }
            else
            {
                pos.DefineValues(Position.Line + 1, Position.Column);
                if (Board.IsValidPosition(pos) && IsFree(pos))
                {
                    mat[pos.Line, pos.Column] = true;
                }

                pos.DefineValues(Position.Line + 2, Position.Column);
                Position pos2 = new Position(Position.Line + 1, Position.Column);

                if (Board.IsValidPosition(pos2) && IsFree(pos2) && Board.IsValidPosition(pos) && IsFree(pos) && MoveCount == 0)

                {
                    mat[pos.Line, pos.Column] = true;
                }

                pos.DefineValues(Position.Line + 1, Position.Column - 1);
                if (Board.IsValidPosition(pos) && HasEnemy(pos))
                {
                    mat[pos.Line, pos.Column] = true;
                }

                pos.DefineValues(Position.Line + 1, Position.Column + 1);
                if (Board.IsValidPosition(pos) && HasEnemy(pos))
                {
                    mat[pos.Line, pos.Column] = true;
                }

                //#SpecialMove En Passant
                if (Position.Line == 4)
                {
                    Position left = new Position(Position.Line, Position.Column - 1);
                    if (Board.IsValidPosition(left) && HasEnemy(left) && Board.Piece(left) == Match.VulnerableEnPassant)
                    {
                        mat[left.Line + 1, left.Column] = true;
                    }

                    Position right = new Position(Position.Line, Position.Column + 1);
                    if (Board.IsValidPosition(right) && HasEnemy(right) && Board.Piece(right) == Match.VulnerableEnPassant)
                    {
                        mat[right.Line + 1, right.Column] = true;
                    }
                }
            }

            return mat;
        }
    }
}
