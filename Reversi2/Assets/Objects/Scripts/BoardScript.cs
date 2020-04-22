using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BoardScript : MonoBehaviour
{

    public GameObject GamePiece;
    public enum PieceColor { NONE, BLACK, WHITE };
    float timer;
    bool AITurn;

    MiniMaxClass miniMax;
    Board board;
    // Start is called before the first frame update
    void Start()
    {
        board  = new Board(GamePiece);
        miniMax = new MiniMaxClass();
        AITurn = false;
        timer = 1.5f;
        print(board.ToString());
        //g = GameObject.Instantiate<GameObject>(GamePiece);
        //g.GetComponent<Rigidbody>().position = new Vector3(4, 10, 4);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0 && AITurn)
        {
            if (!board.IsTerminal(PieceColor.BLACK))
            {
                //TODO add depth selector
                Move tempMove = miniMax.MiniMax(board, PieceColor.BLACK, 7, 0, int.MinValue, int.MaxValue).Item2;
                board.MakeMoveReal(tempMove);
                AITurn = false;
            }
        } else
        {
            timer -= Time.deltaTime;
        }
    }

    private void OnMouseDown()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit h;
        Physics.Raycast(castPoint, out h);
        int row = (int)(h.point.z);
        int y = (int)(h.point.x);
        int x = 7 - row;
        print(x + " " + y);
        //TODO Add results
        if (!board.IsTerminal())
        {
            if (!board.IsTerminal(PieceColor.WHITE) && board.IsMoveValid(new Move(x,y,PieceColor.WHITE)))
            {
                
                board.MakeMoveReal(new Move(x, y, PieceColor.WHITE));
                timer = 1.5f;
                //new WaitForSeconds(3);
                if (board.IsTerminal())
                {
                    //TODO game end logic
                } else
                {
                    AITurn = true;
                }
            }
        }

        

        //g.GetComponent<PieceScript>().Flip();
    }

    void waiter(int x,int y)
    {
        board.MakeMoveReal(new Move(x, y, PieceColor.WHITE));
    }

    private class Board
    {
        PieceColor[,] board;
        public PieceColor currentPlayer;
        GameObject[,] board_pieces; 
        public GameObject GamePiece;
        int height;

        /// <summary>
        /// Constructor to generate basic board according to conventions
        /// </summary>
        public Board()
        {
            currentPlayer = PieceColor.WHITE;
            board = new PieceColor[8, 8];
            board_pieces = new GameObject[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    this.board[i, j] = PieceColor.NONE;
                }
            }
            board[3, 3] = PieceColor.WHITE;
            board[4, 4] = PieceColor.WHITE;
            board[3, 4] = PieceColor.BLACK;
            board[4, 3] = PieceColor.BLACK;
        }
        public Board(GameObject piece)
        {
            currentPlayer = PieceColor.WHITE;
            board = new PieceColor[8, 8];
            board_pieces = new GameObject[8, 8];
            GamePiece = piece;
            //height to drop piece from
            height = 2;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    this.board[i, j] = PieceColor.NONE;
                }
            }
            board[3, 3] = PieceColor.WHITE;
            board[4, 4] = PieceColor.WHITE;
            board[3, 4] = PieceColor.BLACK;
            board[4, 3] = PieceColor.BLACK;
            //Add actual objects 3,3
            GameObject temp = GameObject.Instantiate<GameObject>(GamePiece);
            temp.name = "piece33";
            temp.transform.position = new Vector3(3 + 0.5f, height, 7 - 3 + 0.5f);
            PieceScript abc = temp.GetComponent<PieceScript>();
            abc.setWhite();
            temp.GetComponent<PieceScript>().setWhite();
            board_pieces[3, 3] = temp;
            //4,4
            temp = GameObject.Instantiate<GameObject>(GamePiece);
            temp.name = "piece44";
            temp.transform.position = new Vector3(4 + 0.5f, height, 7 - 4 + 0.5f);
            temp.GetComponent<PieceScript>().setWhite();
            board_pieces[4, 4] = temp;
            //3,4
            temp = GameObject.Instantiate<GameObject>(GamePiece);
            temp.name = "piece34";
            temp.transform.position = new Vector3(4 + 0.5f, height, 7 - 3 + 0.5f);
            temp.transform.Rotate(180f, 0.0f, 0.0f, Space.Self);
            temp.GetComponent<PieceScript>().setBlack();
            board_pieces[3, 4] = temp;
            //4,3
            temp = GameObject.Instantiate<GameObject>(GamePiece);
            temp.name = "piece43";
            temp.transform.position = new Vector3(3 + 0.5f, height, 7 - 4 + 0.5f);
            temp.transform.Rotate(180f, 0.0f, 0.0f, Space.Self);
            temp.GetComponent<PieceScript>().setBlack();
            board_pieces[4, 3] = temp;


            print(board_pieces[3, 3].GetComponent<PieceScript>().Color);
        }

        /// <summary>
        /// Constructor to generate basic board with a custom first one to move
        /// </summary>
        /// <param name="playerSymbol"> Symbol (color) that moves first </param>
        public Board(PieceColor playerSymbol) : this()
        {
            currentPlayer = playerSymbol;
        }

        /// <summary>
        /// Constructor with a custom board layout
        /// </summary>
        /// <param name="board"> Starting board </param>
        public Board(PieceColor[,] board) : this()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    this.board[i, j] = board[i, j];
                }
            }
        }

        /// <summary>
        /// Copy constructor (from board object)
        /// </summary>
        /// <param name="oldBoard"> Board to copy </param>
        public Board(Board oldBoard) : this()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    this.board[i, j] = oldBoard.GetBoardAsArray()[i, j];
                    //this.board_pieces[i, j] = GameObject.Instantiate(oldBoard.GetBoardPiecesAsArray()[i, j]);
                }
            }
            this.currentPlayer = oldBoard.currentPlayer;
        }

        /// <summary>
        /// Get the game board as array
        /// </summary>
        /// <returns> Current playing board with char array</returns>
        public PieceColor[,] GetBoardAsArray()
        {
            return board;
        }

        /// <summary>
        /// Get the game board pieces reference as array
        /// </summary>
        /// <returns> Current playing board with char array</returns>
        public GameObject[,] GetBoardPiecesAsArray()
        {
            return board_pieces;
        }

        /// <summary>
        /// Override of the ToString method to print out the class (print the board)
        /// </summary>
        /// <returns> Board string representation</returns>
        public override string ToString()
        {
            string s = "x\\y1 2 3 4 5 6 7 8 \n";
            for (int i = 0; i < 7; i++)
            {
                s += (i + 1) + " |";
                for (int j = 0; j < 8; j++)
                {
                    s += board[i, j];
                    s += "|";
                }
                s += "\n";
            }
            s += "8 |";
            for (int j = 0; j < 8; j++)
            {
                s += board[7, j];
                s += "|";
            }

            return s;
        }

        /// <summary>
        /// Method to make a move on the current board
        /// </summary>
        /// <param name="move"> Move to be made </param>
        public void MakeMoveReal(Move move)
        {
            if (IsMoveValid(move)) // Test if this is a valid move first
            {
                board[move.x, move.y] = move.symbol;
                GameObject temp = GameObject.Instantiate<GameObject>(GamePiece);
                if (move.symbol==PieceColor.BLACK)
                {
                    temp.transform.Rotate(180f, 0.0f, 0.0f, Space.Self);
                    temp.GetComponent<PieceScript>().setBlack();
                } else
                {
                    temp.GetComponent<PieceScript>().setWhite();
                }
                temp.transform.position = new Vector3(move.y + 0.5f, height, 7-move.x + 0.5f);
                temp.name = "piece" + move.x + move.y;
                print(temp.name + " is created with color");
                board_pieces[move.x, move.y] = temp;


                int x = move.x;
                int y = move.y;

                /* Following logic is present in all further loops, just different direction:
                 * Start from the point in the move
                 * Move while you encouter an empty space or a friendly symbol
                 * Empty space means there is no your second symbol capturing enemies so no flip, break
                 * Once you encouter the friendly color - go back to start flipping everything (only enemy colors will be there)
                 */

                // Check top vertical from the desired move
                while (x > 0)
                {
                    x--;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        x++;
                        while (x != move.x)
                        {
                            board[x, y] = move.symbol;
                            board_pieces[x, y].GetComponent<PieceScript>().Flip();
                            x++;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check top left diagonal from the desired move
                while (y > 0 && x > 0)
                {
                    y--;
                    x--;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        y++;
                        x++;
                        while (x != move.x && y != move.y)
                        {
                            board[x, y] = move.symbol;
                            board_pieces[x, y].GetComponent<PieceScript>().Flip();
                            x++;
                            y++;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check top right diagonal from the desired move
                while (y < 7 && x > 0)
                {
                    x--;
                    y++;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        x++;
                        y--;
                        while (x != move.x && y != move.y)
                        {
                            board[x, y] = move.symbol;
                            board_pieces[x, y].GetComponent<PieceScript>().Flip();
                            x++;
                            y--;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check right horizontal from the desired move
                while (y < 7)
                {
                    y++;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        y--;
                        while (y != move.y)
                        {
                            board[x, y] = move.symbol;
                            board_pieces[x, y].GetComponent<PieceScript>().Flip();
                            y--;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check bottom right diagonal from the desired move
                while (y < 7 && x < 7)
                {
                    y++;
                    x++;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        x--;
                        y--;
                        while (x != move.x && y != move.y)
                        {
                            board[x, y] = move.symbol;
                            board_pieces[x, y].GetComponent<PieceScript>().Flip();
                            x--;
                            y--;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check bottom vertical from the desired move
                while (x < 7)
                {
                    x++;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        x--;
                        while (x != move.x)
                        {
                            board[x, y] = move.symbol;
                            board_pieces[x, y].GetComponent<PieceScript>().Flip();
                            x--;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check bottom left diagonal from the desired move
                while (x < 7 && y > 0)
                {
                    x++;
                    y--;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        x--;
                        y++;
                        while (x != move.x && y != move.y)
                        {
                            board[x, y] = move.symbol;
                            board_pieces[x, y].GetComponent<PieceScript>().Flip();
                            x--;
                            y++;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check left horizontal from the desired move
                while (y > 0)
                {
                    y--;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        y++;
                        while (y != move.y)
                        {
                            board[x, y] = move.symbol;
                            board_pieces[x, y].GetComponent<PieceScript>().Flip();
                            y++;
                        }
                        break;
                    }
                }
                if (currentPlayer == PieceColor.BLACK)
                {
                    currentPlayer = PieceColor.WHITE;
                }
                else currentPlayer = PieceColor.BLACK;
            }
            else
            {
                System.Console.WriteLine("Invalid move!");
            }
        }

        /// <summary>
        /// Method to make a move on the current board
        /// </summary>
        /// <param name="move"> Move to be made </param>
        public void MakeMove(Move move)
        {
            if (IsMoveValid(move)) // Test if this is a valid move first
            {
                board[move.x, move.y] = move.symbol;
                int x = move.x;
                int y = move.y;

                /* Following logic is present in all further loops, just different direction:
                 * Start from the point in the move
                 * Move while you encouter an empty space or a friendly symbol
                 * Empty space means there is no your second symbol capturing enemies so no flip, break
                 * Once you encouter the friendly color - go back to start flipping everything (only enemy colors will be there)
                 */

                // Check top vertical from the desired move
                while (x > 0)
                {
                    x--;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        while (x != move.x)
                        {
                            board[x, y] = move.symbol;
                            x++;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check top left diagonal from the desired move
                while (y > 0 && x > 0)
                {
                    y--;
                    x--;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        while (x != move.x && y != move.y)
                        {
                            board[x, y] = move.symbol;
                            x++;
                            y++;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check top right diagonal from the desired move
                while (y < 7 && x > 0)
                {
                    x--;
                    y++;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        while (x != move.x && y != move.y)
                        {
                            board[x, y] = move.symbol;
                            x++;
                            y--;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check right horizontal from the desired move
                while (y < 7)
                {
                    y++;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        while (y != move.y)
                        {
                            board[x, y] = move.symbol;
                            y--;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check bottom right diagonal from the desired move
                while (y < 7 && x < 7)
                {
                    y++;
                    x++;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        while (x != move.x && y != move.y)
                        {
                            board[x, y] = move.symbol;
                            x--;
                            y--;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check bottom vertical from the desired move
                while (x < 7)
                {
                    x++;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        while (x != move.x)
                        {
                            board[x, y] = move.symbol;
                            x--;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check bottom left diagonal from the desired move
                while (x < 7 && y > 0)
                {
                    x++;
                    y--;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        while (x != move.x && y != move.y)
                        {
                            board[x, y] = move.symbol;
                            x--;
                            y++;
                        }
                        break;
                    }
                }
                x = move.x; y = move.y;
                // Check left horizontal from the desired move
                while (y > 0)
                {
                    y--;
                    if (board[x, y] == PieceColor.NONE) break;
                    if (board[x, y] == move.symbol)
                    {
                        while (y != move.y)
                        {
                            board[x, y] = move.symbol;
                            y++;
                        }
                        break;
                    }
                }
                if (currentPlayer == PieceColor.BLACK)
                {
                    currentPlayer = PieceColor.WHITE;
                }
                else currentPlayer = PieceColor.BLACK;
            }
            else
            {
                System.Console.WriteLine("Invalid move!");
            }
        }

        /// <summary>
        /// Method to check if the move is valid
        /// </summary>
        /// <param name="move"> Move to check </param>
        /// <returns> True or false </returns>
        public bool IsMoveValid(Move move)
        {
            if (move.x < 0 || move.x > 7 || move.y < 0 || move.y > 7) return false;

            int x = move.x;
            int y = move.y;
            bool enemyInLine = false;
            // Check if space is already ocupied
            if (board[x, y] != PieceColor.NONE)
            {
                return false;
            }
            // Identify enemy symbol (color)
            PieceColor enemy = PieceColor.WHITE;
            if (move.symbol == PieceColor.WHITE)
            {
                enemy = PieceColor.BLACK;
            }

            // Check top vertical from the desired move
            while (x > 0)
            {
                x--;
                if (board[x, y] == PieceColor.NONE) break;
                if (board[x, y] == enemy)
                {
                    enemyInLine = true;
                }
                else if (enemyInLine && board[x, y] == move.symbol)
                {
                    return true;
                }
                else break;
            }
            //if (enemyInLine && allyInLine) return true;
            enemyInLine = false; x = move.x; y = move.y;
            // Check top left diagonal from the desired move
            while (y > 0 && x > 0)
            {
                y--;
                x--;
                if (board[x, y] == PieceColor.NONE) break;
                if (board[x, y] == enemy)
                {
                    enemyInLine = true;
                }
                else if (enemyInLine && board[x, y] == move.symbol)
                {
                    return true;
                }
                else break;
            }
            //if (enemyInLine && allyInLine) return true;
            enemyInLine = false; x = move.x; y = move.y;
            // Check top right diagonal from the desired move
            while (y < 7 && x > 0)
            {
                y++;
                x--;
                if (board[x, y] == PieceColor.NONE) break;
                if (board[x, y] == enemy)
                {
                    enemyInLine = true;
                }
                else if (enemyInLine && board[x, y] == move.symbol)
                {
                    return true;
                }
                else break;
            }
            //if (enemyInLine && allyInLine) return true;
            enemyInLine = false; x = move.x; y = move.y;
            // Check right horizontal from the desired move
            while (y < 7)
            {
                y++;
                if (board[x, y] == PieceColor.NONE) break;
                if (board[x, y] == enemy)
                {
                    enemyInLine = true;
                }
                else if (enemyInLine && board[x, y] == move.symbol)
                {
                    return true;
                }
                else break;
            }
            //if (enemyInLine && allyInLine) return true;
            enemyInLine = false; x = move.x; y = move.y;
            // Check bottom right diagonal from the desired move
            while (y < 7 && x < 7)
            {
                y++;
                x++;
                if (board[x, y] == PieceColor.NONE) break;
                if (board[x, y] == enemy)
                {
                    enemyInLine = true;
                }
                else if (enemyInLine && board[x, y] == move.symbol)
                {
                    return true;
                }
                else break;
            }
            //if (enemyInLine && allyInLine) return true;
            enemyInLine = false; x = move.x; y = move.y;
            // Check bottom vertical from the desired move
            while (x < 7)
            {
                x++;
                if (board[x, y] == PieceColor.NONE) break;
                if (board[x, y] == enemy)
                {
                    enemyInLine = true;
                }
                else if (enemyInLine && board[x, y] == move.symbol)
                {
                    return true;
                }
                else break;
            }
            //if (enemyInLine && allyInLine) return true;
            enemyInLine = false; x = move.x; y = move.y;
            // Check bottom left diagonal from the desired move
            while (x < 7 && y > 0)
            {
                x++;
                y--;
                if (board[x, y] == PieceColor.NONE) break;
                if (board[x, y] == enemy)
                {
                    enemyInLine = true;
                }
                else if (enemyInLine && board[x, y] == move.symbol)
                {
                    return true;
                }
                else break;
            }
            //if (enemyInLine && allyInLine) return true;
            enemyInLine = false; x = move.x; y = move.y;
            // Check left horizontal from the desired move
            while (y > 0)
            {
                y--;
                if (board[x, y] == PieceColor.NONE) break;
                if (board[x, y] == enemy)
                {
                    enemyInLine = true;
                }
                else if (enemyInLine && board[x, y] == move.symbol)
                {
                    return true;
                }
                else break;
            }
            //if (enemyInLine && allyInLine) return true;

            return false;
        }

        /// <summary>
        /// Check if the board is terminal, i.e. if there are at least one move to make
        /// </summary>
        /// <returns> True or false </returns>
        public bool IsTerminal()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == PieceColor.NONE)
                    {
                        if (IsMoveValid(new Move(i, j, PieceColor.BLACK)) || IsMoveValid(new Move(i, j, PieceColor.WHITE)))
                        {
                            return false;
                        }

                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool IsTerminal(PieceColor symbol)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == PieceColor.NONE)
                    {
                        if (IsMoveValid(new Move(i, j, symbol)))
                        {
                            return false;
                        }

                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Get the score for the designated player on the current board
        /// </summary>
        /// <param name="playerSymbol"> Symbol of player for whom to count</param>
        /// <returns> Score value </returns>
        public int GetScore(PieceColor playerSymbol)
        {
            int score = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == playerSymbol)
                    {
                        score++;
                    }
                }
            }
            return score;
        }

        /// <summary>
        /// Get all possible moves on the current board
        /// </summary>
        /// <returns> List of possible moves </returns>
        public List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Move move = new Move(i, j, currentPlayer);
                    if (IsMoveValid(move))
                    {
                        moves.Add(move);
                    }
                }
            }
            return moves;
        }


    }
    private class Move
    {
        public int x, y;
        public PieceColor symbol;

        public Move()
        {
            x = 0;
            y = 0;
            symbol = PieceColor.NONE;
        }

        public Move(int x, int y, PieceColor symbol)
        {
            this.x = x;
            this.y = y;
            this.symbol = symbol;
        }

    }

    private class MiniMaxClass
    {
        public Tuple<int, Move> MiniMax(Board board, PieceColor player, int maxDepth, int currentDepth, int alpha, int beta)
        {
            int bestScore;
            Move bestMove = new Move();
            // Check if the bottom of the recursion is reached
            if (board.IsTerminal() || currentDepth == maxDepth)
            {
                return new Tuple<int, Move>(board.GetScore(player), null);
            }
            // Check if the algorithm "plays" for player or for AI
            if (board.currentPlayer == player)
            {
                bestScore = int.MinValue;
            }
            else
            {
                bestScore = int.MaxValue;
            }
            // Test all moves available at a given board
            foreach (Move move in board.GetMoves())
            {
                Board newBoard = new Board(board);
                newBoard.MakeMove(move);
                // Run recursively
                Tuple<int, Move> temp = MiniMax(newBoard, player, maxDepth, currentDepth + 1, alpha, beta);
                if (board.currentPlayer == player)
                {
                    // If this move gives better score - save it ("our" turn, maximizing)
                    if (temp.Item1 > bestScore)
                    {
                        bestScore = temp.Item1;
                        bestMove = move;
                    }
                    alpha = Math.Max(alpha, bestScore);
                    if (beta <= alpha) break;
                }
                else
                {
                    // If this move gives worse score - save it ("enemy" turn, minimizing)
                    if (temp.Item1 < bestScore)
                    {
                        bestScore = temp.Item1;
                        bestMove = move;
                    }
                    beta = Math.Min(beta, bestScore);
                    if (beta <= alpha) break;
                }
            }
            return new Tuple<int, Move>(bestScore, bestMove);
        }

    }
}
