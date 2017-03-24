
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
        public void CreateThread()
        {
            _Thread = new Thread(OnRedraw);
        }
        public void Start()
        {
            Active = true;
            CreateThread();
            _Thread.Start();
            GameBoard.StartPlayer();
        }
        public void Stop()
        {
            _RequestExit = true;
        }
        public bool Active { get; private set; } = false;

        private void OnRedraw(object obj)
        {
            while (!_RequestExit)
            {
                GameBoard.Redraw();
                Thread.Sleep(RedrawDelay);
            }
            Active = false;
        }

        public void OnDraw()
        {

        }
    }
}