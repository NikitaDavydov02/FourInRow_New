using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FourInRow
{
    [Serializable]
    internal class Board
    {
        int[,] board;
        public bool whiteTurn;
        public GameStatus gameStatus;
        private List<Vector> minTranlations;
        public Board(GameMode gameMode, bool himanIsPlayingAsWhite)
        {
            board = new int[7, 7];
            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 7; j++)
                    board[i, j] = 0;
            whiteTurn = true;
            gameStatus = GameStatus.InProgress;
            minTranlations = new List<Vector>();
            minTranlations.Add(new Vector(0, 1));
            //minTranlations.Add(new Vector(0, -1));
            minTranlations.Add(new Vector(1, 0));
            //minTranlations.Add(new Vector(-1, 0));

            minTranlations.Add(new Vector(1, 1));
            minTranlations.Add(new Vector(1, -1));

            minTranlations.Add(new Vector(-1, 1));
            minTranlations.Add(new Vector(-1, -1));
            minTranlations.Add(new Vector(0, -1));
            minTranlations.Add(new Vector(-1, 0));
        }
        public void Init()
        {

        }
        public void Quit()
        {

        }
        public bool MakeATurn(int turn)
        {
            if (gameStatus != GameStatus.InProgress)
                return false;
            //Is Move Possible
            if (!IsMovePossible(whiteTurn, turn))
                return false;
            //Make a move physicaly
            MakeATurnPhysicaly(whiteTurn, turn);
            CheckIfTheGameIsFinished();
            whiteTurn = !whiteTurn;
            return true;
        }
        private void MakeATurnPhysicaly(bool white, int turn)
        {
            int coin = 1;
            if (!white)
                coin = -1;
            int row = 6;
            while (row >= 0 && board[row, turn] == 0)
                row--;
            board[row + 1, turn] = coin;

        }
        private bool IsMovePossible(bool white, int turn)
        {
            if (board[6, turn] == 0)
                return true;
            return false;
        }
        public void ReverseMove(int move)
        {
            int row = 0;
            while (row < 7 && board[row, move] != 0)
                row++;
            board[row - 1, move] = 0;
            whiteTurn = !whiteTurn;
            gameStatus = GameStatus.InProgress;
        }
        public void LoadTask(int[,] position, bool whiteStart)
        {
            board = position;
            whiteTurn = whiteStart;
            CheckIfTheGameIsFinished();

        }
        public double AdvancedPositionEstimation()
        {
            double total = 0;
            for (int i = 0; i < 4; i++)
            {
                Vector translation = minTranlations[i];
                total += CountRowsColumnsDiagonals(translation);
            }
            total /= 300;
            if (total >= 100 || total <= -100)
                ;
            return total;
        }
        private int IfRowCanBeCompleted(Vector start, Vector end, Vector translation, int length)
        {
            Vector checkEnd = end;
            int freeFieldsAfterEnd = 0;
            do
            {
                checkEnd = VectorMath.Sum(checkEnd, translation);
                if (PositionIsOnTheField(checkEnd))
                {
                    int checkColor = board[checkEnd.x, checkEnd.y];
                    if (checkColor != 0)
                        break;
                    freeFieldsAfterEnd++;
                }
                else
                    break;
            }
            while (freeFieldsAfterEnd + length<4);
            //Before start
            int freeFieldsBeforeStart = 0;
            checkEnd = start;
            do
            {
                checkEnd = VectorMath.Substract(checkEnd, translation);
                if (PositionIsOnTheField(checkEnd))
                {
                    int checkColor = board[checkEnd.x, checkEnd.y];
                    if (checkColor != 0)
                        break;
                    freeFieldsBeforeStart++;
                }
                else
                    break;
            }
            while (freeFieldsAfterEnd + length + freeFieldsBeforeStart < 4);

            if (freeFieldsAfterEnd + length + freeFieldsBeforeStart < 4)
                return 0;
            else
            {
                if (freeFieldsAfterEnd > 0 && freeFieldsBeforeStart > 0)
                    return 2;
                else
                    return 1;
            }
        }
        private double CountRowsColumnsDiagonals(Vector translation)
        {
            if (translation.x == 1 && translation.y == -1)
                ;
            double output = 0;
            List<Vector> initialFields = InitialFieldsForCheckStart(translation);
            foreach(Vector initialField in initialFields)
            {
                int currentColor = board[initialField.x,initialField.y];
                int numberOfCellsWithSameColor = 1;
                Vector checkEnd = initialField;
                Vector startOfLastColor = checkEnd;
                do
                {
                    checkEnd = VectorMath.Sum(checkEnd, translation);
                    int checkFieldColor = 0;
                    if (PositionIsOnTheField(checkEnd))
                        checkFieldColor = board[checkEnd.x, checkEnd.y];

                    if (checkFieldColor * currentColor > 0)
                    {
                        numberOfCellsWithSameColor++;
                    }
                    else
                    {
                        Vector endOfLastColor = VectorMath.Substract(checkEnd, translation);
                        if (currentColor > 0)
                        {
                            int openDegree = IfRowCanBeCompleted(startOfLastColor, endOfLastColor, translation, numberOfCellsWithSameColor);
                            if (openDegree == 0)
                                output += 0;
                            if (openDegree == 1)
                                output += Math.Pow(3, numberOfCellsWithSameColor - 1);
                            if (openDegree == 2)
                                output += Math.Pow(3, numberOfCellsWithSameColor);

                        }
                        else if (currentColor < 0)
                        {
                            int openDegree = IfRowCanBeCompleted(startOfLastColor, endOfLastColor, translation, numberOfCellsWithSameColor);
                            if (openDegree == 0)
                                output -= 0;
                            if (openDegree == 1)
                                output -= Math.Pow(3, numberOfCellsWithSameColor - 1);
                            if (openDegree == 2)
                                output -= Math.Pow(3, numberOfCellsWithSameColor);
                        }
                        else if (currentColor == 0)
                            output += 0;
                        currentColor = checkFieldColor;
                        //???
                        startOfLastColor = checkEnd;
                        numberOfCellsWithSameColor = 1;
                    }
                }
                while (PositionIsOnTheField(checkEnd));

                /*while (PositionIsOnTheField(checkEnd))
                {
                    int checkFieldColor = board[checkEnd.x, checkEnd.y];
                    
                    if (checkFieldColor * currentColor > 0)
                    {
                        numberOfCellsWithSameColor++;
                    }
                    else
                    {
                        if (currentColor > 0)
                            output += Math.Pow(2, numberOfCellsWithSameColor);
                        else if (currentColor < 0)
                            output -= Math.Pow(2, numberOfCellsWithSameColor);
                        else if (currentColor == 0)
                            output += 0;
                        currentColor = checkFieldColor;
                        numberOfCellsWithSameColor = 1;
                    }
                    checkEnd = VectorMath.Sum(checkEnd, translation);
                }*/
            }
            return output;
        }
        private bool PositionIsOnTheField(Vector position)
        {
            if (position.x < 7 && position.x >= 0 && position.y < 7 && position.y >= 0)
                return true;
            return false;
        }
        
        public double EstimatePosition()
        {
            return AdvancedPositionEstimation();
            double total = 0;
            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 7; j++)
                {
                    foreach (Vector translation in minTranlations)
                    {
                        int sum = 0;
                        Vector checkPosition = new Vector(i, j);
                        for (int k = 0; k < 4 && checkPosition.y >= 0 && checkPosition.x >= 0 && checkPosition.y < 7 && checkPosition.x < 7; k++)
                        {
                            sum += board[checkPosition.y, checkPosition.x];
                            checkPosition = VectorMath.Sum(checkPosition, translation);
                        }
                        total += sum;
                    }
                }
            return total/20;
        }
        private bool CheckIfTheGameIsFinished()
        {
            bool noFreeColumns = true;
            for (int i = 0; i < 7; i++)
                if (board[6, i] == 0)
                {
                    noFreeColumns = false;
                    break;
                }
            if (noFreeColumns)
            {
                gameStatus = GameStatus.Draw;
                return true;
            }
            for (int i=0;i<7;i++)
                for(int j = 0; j < 7; j++)
                {
                    foreach(Vector translation in minTranlations)
                    {
                        int sum = 0;
                        Vector checkPosition = new Vector(i, j);
                        for(int k = 0; k < 4 && checkPosition.y>=0 && checkPosition.x>=0 && checkPosition.y<7 && checkPosition.x<7; k++)
                        {
                            sum += board[checkPosition.y, checkPosition.x];
                            checkPosition = VectorMath.Sum(checkPosition, translation);
                        }
                        if (sum == 4)
                        {
                            gameStatus = GameStatus.WhiteWon;
                            return true;
                        }
                        if (sum == -4)
                        {
                            gameStatus = GameStatus.BlackWon;
                            return true;
                        }
                    }
                }
            gameStatus = GameStatus.InProgress;
            return false;
        }
        public void OutputBoard()
        {
            Console.WriteLine();
            Console.WriteLine("0 1 2 3 4 5 6");
            Console.WriteLine("-------------");
            for (int i = 6; i >=0; i--) 
            {
                string line = "";
                for (int j = 0; j < 7; j++)
                {
                    if(board[i, j]==-1)
                        line += "X" + " ";
                    if (board[i, j] == 1)
                        line += "I" + " ";
                    if (board[i, j] == 0)
                        line += " " + " ";
                }
                Console.WriteLine(line);
            }
        }
        public List<int> FindPossibleMoves()
        {
            List<int> output = new List<int>();
            for(int i = 0; i < 7; i++)
            {
                if (board[6, i] == 0)
                    output.Add(i);
            }
            return output;
        }
        private List<Vector> InitialFieldsForCheckStart(Vector translation)
        {
            List<Vector> output = new List<Vector>();
            

            if (translation.x > 0)//Up
                for (int i = 1; i < 6; i++)
                    output.Add(new Vector(0, i));
            if (translation.x < 0)//Down
                for (int i = 1; i < 6; i++)
                    output.Add(new Vector(6, i));
            if (translation.y < 0)//Left
                for (int i = 1; i < 6; i++)
                    output.Add(new Vector(i, 6));
            if (translation.y > 0)//Right
                for (int i = 1; i < 6; i++)
                    output.Add(new Vector(i, 0));

            if (translation.x > 0)
            {
                output.Add(new Vector(0, 6));
                output.Add(new Vector(0, 0));
                if (translation.y > 0)
                {
                    output.Add(new Vector(6, 0));
                }
                if (translation.y < 0)
                {
                    output.Add(new Vector(6, 6));
                }
            }
            if (translation.x < 0)
            {
                output.Add(new Vector(6, 6));
                output.Add(new Vector(6, 0));
                if (translation.y > 0)
                {
                    output.Add(new Vector(0, 0));
                }
                if (translation.y < 0)
                {
                    output.Add(new Vector(0, 6));
                }
            }
            if (translation.x == 0)
            {
                if (translation.y >0)
                {
                    output.Add(new Vector(6, 0));
                    output.Add(new Vector(0, 0));
                }
                if (translation.y < 0)
                {
                    output.Add(new Vector(6, 6));
                    output.Add(new Vector(0, 6));
                }
                if (translation.y ==0) { throw new Exception(); }
            }

            return output;
        }
    }
    public enum GameStatus
    {
        InProgress,
        WhiteWon,
        BlackWon,
        Draw,
    }
}
