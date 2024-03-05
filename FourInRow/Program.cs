using System.Runtime.InteropServices;

namespace FourInRow
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose game mode:");
            Console.WriteLine("1 - Two players");
            Console.WriteLine("2 - Against Computer");
            Console.WriteLine("3 - Solve task");
            string gameModeInput = Console.ReadLine();
            GameMode gameMode = GameMode.TwoPlayers;
            if (gameModeInput == "1")
                gameMode = GameMode.TwoPlayers;
            if (gameModeInput == "2")
                gameMode = GameMode.AgainstComputer;
            if (gameModeInput == "3")
                gameMode = GameMode.SolveTask;
            bool humanIsPlayingAsWhite = true;
            if (gameMode == GameMode.AgainstComputer||gameMode==GameMode.SolveTask)
            {
                Console.WriteLine("Choose your color to play:");
                Console.WriteLine("1 - White");
                Console.WriteLine("-1 - Black");
                string colorInput = Console.ReadLine();
                if (colorInput == "-1")
                    humanIsPlayingAsWhite = false;
            }

            Board board = new Board(gameMode, humanIsPlayingAsWhite);
            Computer computer = new Computer();
            if (gameMode == GameMode.SolveTask)
            {
                StreamReader sr = new StreamReader("input.txt");
                int[,] position = new int[7, 7];
                int row = 6;
                for(int i = 0; i < 7; i++)
                {
                    string[] line = sr.ReadLine().Split(' ');
                    for (int j = 0; j < 7; j++)
                    {
                        int cell = 0;
                        if (line[j] == "I")
                            cell = 1;
                        if (line[j] == "X")
                            cell = -1;
                        position[row, j] = cell;
                    }
                    row--;
                }
                sr.Close();
                board.LoadTask(position, !humanIsPlayingAsWhite);
            }

            string inputMove;
            while (board.gameStatus==GameStatus.InProgress)
            {

                if (board.whiteTurn)
                    Console.WriteLine("White to turn");
                else
                    Console.WriteLine("Black to turn");
                if (((board.whiteTurn && !humanIsPlayingAsWhite) || (!board.whiteTurn && humanIsPlayingAsWhite)) && (gameMode == GameMode.AgainstComputer||gameMode==GameMode.SolveTask))
                {
                    //Computers move
                    int computersMove = computer.FindTheBestMoveForPosition(board, board.whiteTurn);
                    //int f = board.board[computersMove.start.x, computersMove.start.y];
                    Console.WriteLine("Computer move: " + computersMove);
                    board.MakeATurn(computersMove);
                  
                }
                else
                {
                    //Human Move
                    inputMove = Console.ReadLine();
                    
                    if (inputMove == "l")
                    {
                        board.Quit();
                        //chessComputer.Quit();
                        computer.Quit();
                        break;
                    }

                    int move = Convert.ToInt32(inputMove);
                    board.MakeATurn(move);
                }
                board.OutputBoard();
            }
            if (board.gameStatus== GameStatus.Draw)
                Console.WriteLine("Draw");
            else if (board.gameStatus == GameStatus.WhiteWon)
                Console.WriteLine("White won!");
            else if (board.gameStatus == GameStatus.BlackWon)
                Console.WriteLine("Black won!");
            string input = Console.ReadLine();
            if (input == "l")
            {
                board.Quit();
                computer.Quit();
                //chessComputer.Quit();
            }
            Console.ReadLine();
        }
    }
    public enum GameMode 
    {
        TwoPlayers,
        AgainstComputer,
        SolveTask,
    }

}