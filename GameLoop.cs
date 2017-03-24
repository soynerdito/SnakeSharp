
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
                GameBoard.Redraw();
                Thread.Sleep(RedrawDelay);
            }
            Active = false;
        } 
    }
}