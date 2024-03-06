using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FourInRow
{
    internal class Computer
    {
        public Computer()
        {
            //testBoard = new Board(GameMode.TwoPlayers);
            random = new Random();
            streamWriter = new StreamWriter(File.Create("computations.txt"));
        }
        Random random;
        static StreamWriter streamWriter;
        Board testBoard;
        int positionCounter = 0;
        public void Quit()
        {
            streamWriter.Close();
        }
        public int FindTheBestMoveForPosition(Board board, bool forWhite)
        {
            /*List<Move> possibleMoves = ChessLibrary.FindAllPosibleMoves(forWhite, position, castlingPossibilityHistory);
            int index = random.Next(possibleMoves.Count);
            return possibleMoves[index];*/
            //testBoard = Board.DeepClone<Board>(board);
            //testBoard = ExtensionMethods.DeepClone<Board>(board);
            int bestMove;
            streamWriter.WriteLine("------------------------------------------------------");
            streamWriter.WriteLine("------------------------------------------------------");
            streamWriter.WriteLine("------------------------------------------------------");
            //EstimatePosition(position, 0, forWhite, castlingPossibilityHistory, out bestMove);
            double limit;
            if (forWhite)
                limit = 100;
            else
                limit = -100;
            DateTime start = DateTime.Now;
            positionCounter = 0;
            double estimation = FindMaxOrMinMove(board, 0, forWhite, limit, out bestMove);
            DateTime end = DateTime.Now;
            TimeSpan dif = end - start;
            Console.WriteLine(dif);
            Console.WriteLine("Positions: " + positionCounter);
            Console.WriteLine("Estimation: " + estimation);
            return bestMove;

        }
        private double FindMaxOrMinMove(Board board, int depth, bool max, double limitEstimation, out int bestMove)
        {
            depth++;
            positionCounter++;
            string indent = "";
            for (int i = 0; i < depth; i++)
                indent += "    ";

            List<int> possibleMoves;
            if (max)
                possibleMoves = board.FindPossibleMoves();
            else
                possibleMoves = board.FindPossibleMoves();
            if (possibleMoves.Count == 0)
            {
                bestMove = -1;
                return 0;
            }
            bestMove = possibleMoves[0];
            if (depth == 10)
            {
                //Final estimation
                return EstimateFinalPosition(board);
            }
            string s = " max";
            if (!max)
                s = " min";
            streamWriter.WriteLine(indent + "Limit for move " + limitEstimation.ToString() + " find " + s);
            //List<Move> possibleMocves = ChessLibrary.FindAllPosibleMoves(forWhite, position, _castlingPosibilityFromHistory);

            //double totalEstimation = 0;
            double maxEstimation = -100;
            double minEstimation = 100;
            //preliminary estimation
            //Dictionary<Board, double> preliminaryEstimations = new Dictionary<Board, double>();

            //-------------------------------------------PRELIMINARY ESTIMATIONS---------------------------------------------//
            List<KeyValuePair<int, double>> preliminaryEstimations = new List<KeyValuePair<int, double>>();
            //Dictionary<Board, int> boardVSMoves = new Dictionary<Board, int>();
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                //Board newBoard = ExtensionMethods.DeepClone<Board>(board);
                board.MakeATurn(possibleMoves[i]);
                GameStatus GameResult = board.gameStatus;
                double preliminaryEstimation = 0;
                if (GameResult != GameStatus.InProgress)
                {
                    if (GameResult == GameStatus.Draw)
                    {
                        preliminaryEstimation = 0;
                    }
                    else if (GameResult == GameStatus.WhiteWon)
                    {
                        preliminaryEstimation = 100;
                    }
                    else if (GameResult == GameStatus.BlackWon)
                    {
                        preliminaryEstimation = -100;
                    }

                    //OutputBoard(board);
                }
                else
                {
                    preliminaryEstimation = EstimateFinalPosition(board);
                }
                preliminaryEstimations.Add(new KeyValuePair<int, double>(possibleMoves[i], preliminaryEstimation));
                //preliminaryEstimations.Add(newBoard, preliminaryEstimation);
                //boardVSMoves.Add(newBoard, possibleMoves[i]);
                board.ReverseMove(possibleMoves[i]);
            }
            if (!max)
                preliminaryEstimations.Sort((x, y) => x.Value.CompareTo(y.Value));
            else
                preliminaryEstimations.Sort((x, y) => y.Value.CompareTo(x.Value));


            //-------------------------------------------FULL ESTIMATIONS---------------------------------------------//
            
            foreach (KeyValuePair<int, double> keyValuePair in preliminaryEstimations)
            {
                if (depth == 1)
                    ;
                //int figure = board.board[possibleMoves[i].start.x, possibleMoves[i].start.y];
                ////Board newBoard = Board.DeepClone<Board>(board);
                //Board newBoard = ExtensionMethods.DeepClone<Board>(board);
                //newBoard.InputMove(possibleMoves[i]);
                //Result? GameResult = newBoard.GameResult;
                int move = keyValuePair.Key;
                board.MakeATurn(move);
                double moveEstimation = 0;
                if (board.gameStatus!=GameStatus.InProgress)
                {
                    if (board.gameStatus==GameStatus.Draw)
                    {
                        moveEstimation = 0;
                    }
                    else if (board.gameStatus==GameStatus.WhiteWon)
                    {
                        moveEstimation = 100;
                    }
                    else if (board.gameStatus == GameStatus.BlackWon)
                    {
                        moveEstimation = -100;
                    }

                    //OutputBoard(board);
                }
                else
                {
                    int bestContinuation;

                    double limitForNextStep;
                    if (max)
                        limitForNextStep = maxEstimation;
                    else
                        limitForNextStep = minEstimation;
                    if (depth == 1)
                        ;
                    moveEstimation = FindMaxOrMinMove(board, depth, !max, limitForNextStep, out bestContinuation);

                }
                //----------------------MIN-MAX-------------------------//
                if (moveEstimation > maxEstimation)
                {
                    maxEstimation = moveEstimation;
                    if (max)
                    {
                        bestMove = move;
                        if (moveEstimation > limitEstimation || moveEstimation == 100)
                        {
                            //string logString2 = possibleMoves[i].ToString();
                            streamWriter.WriteLine(indent + move + "(" + moveEstimation.ToString() + ")*");
                            board.ReverseMove(move);
                            return moveEstimation;

                        }
                    }
                }
                if (moveEstimation < minEstimation)
                {
                    minEstimation = moveEstimation;
                    if (!max)
                    {
                        bestMove = move;
                        if (moveEstimation < limitEstimation || moveEstimation == -100)
                        {
                            //string logString2 = ChessLibrary.OutputHumanMove(possibleMoves[i], figure);
                            streamWriter.WriteLine(indent + move + "(" + moveEstimation.ToString() + ")*");
                            board.ReverseMove(move);
                            return moveEstimation;
                        }

                    }
                }
                board.ReverseMove(move);
                //totalEstimation += moveEstimation;
                //string logString = ChessLibrary.OutputHumanMove(possibleMoves[i], figure);
                streamWriter.WriteLine(indent + move + "(" + moveEstimation.ToString() + ")");
            }


            if (max)
                return maxEstimation;
            else
                return minEstimation;
        }
        private double EstimateFinalPosition(Board board)
        {
            return board.EstimatePosition();
            //return (MaterialEstimation(board.board) + EstimateAreaSuperior(board));
            //return (MaterialEstimation(board.board));
        }
    }
}
