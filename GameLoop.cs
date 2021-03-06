
using System;
using System.Threading;

namespace ConsoleApplication
{
    public class GameLoop
    {
        private Thread _Thread;
        private bool _RequestExit = false;
        //Delay in milliseconds
        public int RedrawDelay { get; set; } = 17;
        public GameBoard GameBoard { get; private set; }
        public GameLoop(GameBoard gameBoard)
        {
            this.GameBoard = gameBoard;
        }
        public Thread CreateThread()
        {
            return new Thread(OnDrawThread);
        }
        public void Start()
        {
            Console.Clear();
            Active = true;
            GameBoard.StartPlayer();
            _Thread = CreateThread();
            _Thread.Start();
        }
        public void Stop()
        {
            _RequestExit = true;
        }
        public bool Active { get; private set; } = false;

        private void OnDrawThread(object obj)
        {
            while (!_RequestExit)
            {
                try{
                    GameBoard.Redraw();
                    Thread.Sleep(RedrawDelay);
                }catch(CrashException){
                    _RequestExit = true;
                    Console.Clear();
                    int yPos = 5;
                    Console.SetCursorPosition(5,yPos++);
                    Console.WriteLine(" _____________________");
                    Console.SetCursorPosition(5,yPos++);
                    Console.WriteLine("/                     \\");
                    Console.SetCursorPosition(5,yPos++);
                    Console.WriteLine("| Yo loose, try again |");
                    Console.SetCursorPosition(5,yPos++);
                    Console.WriteLine("\\_____________________/");
                    Console.SetCursorPosition(5,yPos++);
                    Console.WriteLine("\n\n Retry [Y/N]?");
                }
            }
            Active = false;
        } 
    }
}