using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication
{
    public class CrashException : Exception
    {
        public CrashException(string message) : base(message)
        {
        }
    }

    public enum PlayerDirection
    {
        UP, DOWN, LEFT, RIGHT, NONE
    }
    public class GameBoard
    {
        private Object _lock = new Object();

        public PlayerDirection CurrentDirection
        {
            get
            {
                return _CurrentDirection;
            }
            set
            {
                if (value == _CurrentDirection && this.PlayerSpeed > 15)
                {
                    //Speed Up
                    this.PlayerSpeed -= 15;
                }
                _CurrentDirection = value;
            }
        }
        private PlayerDirection _CurrentDirection = PlayerDirection.NONE;
        public List<Point> SnakeBody { get; set; } = new List<Point>();
        private Stack<Point> ErasePos { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int LasTimeMoved { get; private set; } = 0;

        public Point GetSnakeHead()
        {
            return SnakeBody.FirstOrDefault();
        }
        public Point GetSnakeTail()
        {
            return SnakeBody.LastOrDefault();
        }
        public GameBoard(int width, int height)
        {
            Console.CursorVisible = false;
            Width = width;
            Height = height;
            ErasePos = new Stack<Point>();
        }

        int PlayerSpeed;
        public void StartPlayer(int userSpeedMS = 250)
        {

            SnakeBody = new List<Point>();
            SnakeBody.Add(new Point() { X = 4, Y = 4 });
            SnakeBody.Add(new Point() { X = 4, Y = 4 });

            PlayerSpeed = userSpeedMS;
        }
        private void MoveSnake(PlayerDirection direction, bool grow = false)
        {
            lock (_lock)
            {
                var newPoint = GetSnakeHead().Clone();
                switch (direction)
                {
                    case PlayerDirection.DOWN:
                        newPoint = MovePoint(newPoint, 0, 1);
                        break;
                    case PlayerDirection.UP:
                        newPoint = MovePoint(newPoint, 0, -1);
                        break;
                    case PlayerDirection.LEFT:
                        newPoint = MovePoint(newPoint, -1, 0);
                        break;
                    case PlayerDirection.RIGHT:
                        newPoint = MovePoint(newPoint, 1, 0);
                        break;
                }
                if (newPoint != null)
                {
                    //Validate Collition
                    if (direction != PlayerDirection.NONE && SnakeBody.Where(x => x.X == newPoint.X && x.Y == newPoint.Y).FirstOrDefault() != null)
                    {
                        throw new CrashException("chocaste");
                    }
                    if (!grow)
                    {
                        var remove = GetSnakeTail();
                        SnakeBody.Remove(remove);
                        ErasePos.Push(remove);
                    }
                    SnakeBody.Insert(0, newPoint);
                }
            }
        }

        public void OnDraw()
        {
            lock (_lock)
            {
                if (SnakeBody.Count == 0) { return; }
                if ((Environment.TickCount - LasTimeMoved) > PlayerSpeed)
                {
                    MoveSnake(CurrentDirection, SnakeBody.Count < 50);
                    LasTimeMoved = Environment.TickCount;
                }

                while (ErasePos.Count > 0)
                {
                    ClearPos(ErasePos.Pop());
                }
                foreach (var dot in SnakeBody)
                {
                    Console.SetCursorPosition(dot.X, dot.Y);
                    Console.Write('*');
                }
            }
        }

        private void ClearPos(Point point)
        {
            Console.SetCursorPosition(point.X, point.Y);
            Console.Write(" ");
        }

        internal void Redraw()
        {
            OnDraw();
        }

        internal void OnKeyPress(ConsoleKeyInfo keyInfo)
        {
            lock (_lock)
            {
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow: { CurrentDirection = PlayerDirection.UP; } break;
                    case ConsoleKey.DownArrow: { CurrentDirection = PlayerDirection.DOWN; } break;
                    case ConsoleKey.LeftArrow: { CurrentDirection = PlayerDirection.LEFT; } break;
                    case ConsoleKey.RightArrow: { CurrentDirection = PlayerDirection.RIGHT; } break;
                }
            }
        }

        private Point MovePoint(Point point, int xOffset, int yOffset)
        {
            if ((point.X + xOffset) >= 0
                && (point.X + xOffset) < Width)
            {
                point.X += xOffset;
            }
            if ((point.Y + yOffset) >= 0
                && (point.Y + yOffset) < Height)
            {
                point.Y += yOffset;
            }
            return point;
        }
    }

    /*
        Launch ConsoleApplication. (Entry point)
     */
    public class Program
    {
        public static void Main(string[] args)
        {
            var gameBoad = new GameBoard(80, 24);
            var gameLoop = new GameLoop(gameBoad);
            gameLoop.Start();
            ConsoleKeyInfo keyInfo;
            //Game loop
            Console.Clear();            
            //Loop to handle Key inputs
            //while (gameLoop.Active)
            while (true)
            {
                keyInfo = Console.ReadKey(true);
                if (!gameLoop.Active)
                {
                    if( Char.ToUpper(keyInfo.KeyChar) =='Y' ){
                        gameBoad = new GameBoard(80, 24);
                        gameLoop = new GameLoop(gameBoad);
                        gameLoop.Start();                        
                    }else if( Char.ToUpper(keyInfo.KeyChar) == 'N' ){
                        //exit
                        break;
                    }
                }
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    gameLoop.Stop();
                    //exit
                    break;
                }
                if (gameLoop.Active){
                    gameBoad.OnKeyPress(keyInfo);
                }
            }
            Console.CursorVisible = true;
            Console.SetCursorPosition(0, 0);            
            Console.Clear();
        }
    }
}
