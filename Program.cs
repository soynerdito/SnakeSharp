﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication
{
    public enum PlayerDirection
    {
        UP, DOWN, LEFT, RIGHT, NONE
    }
    public class GameBoard
    {
        private Object _lock = new Object();
        public PlayerDirection CurrentDirection { get; set; } = PlayerDirection.NONE;
        public int UserSpeed { get; set; }
        System.Threading.Timer Timer;
        //public Point DotPosition { get; set; } = new Point() { X = 0, Y = 0 };
        public List<Point> SnakeBody { get; set; } = new List<Point>();
        private Stack<Point> ErasePos { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

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

        public void StartPlayer(int userSpeedMS = 250)
        {
            if (Timer != null)
            {
                Timer.Dispose();
                Timer = null;
            }
            SnakeBody = new List<Point>();
            SnakeBody.Add(new Point() { X = 4, Y = 4 });
            SnakeBody.Add(new Point() { X = 5, Y = 4 });
            SnakeBody.Add(new Point() { X = 6, Y = 4 });
            Timer = new System.Threading.Timer(PlayerTimerCallback, null, 0, userSpeedMS);
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
        private void PlayerTimerCallback(object state)
        {
            MoveSnake(CurrentDirection, SnakeBody.Count < 50);
        }

        public void OnDraw()
        {
            lock (_lock)
            {
                while (ErasePos.Count > 0)
                {
                    ClearPos(ErasePos.Pop());
                }
                foreach (var dot in SnakeBody)
                {
                    Console.SetCursorPosition(dot.X, dot.Y);
                    Console.Write('x');
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
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow: { CurrentDirection = PlayerDirection.UP; } break;
                case ConsoleKey.DownArrow: { CurrentDirection = PlayerDirection.DOWN; } break;
                case ConsoleKey.LeftArrow: { CurrentDirection = PlayerDirection.LEFT; } break;
                case ConsoleKey.RightArrow: { CurrentDirection = PlayerDirection.RIGHT; } break;
            }
        }

        private Point MovePoint(Point point, int xOffset, int yOffset)
        {
            if ((point.X + xOffset) >= 0
                && (point.X + xOffset) < Width)
            {
                //this.ErasePos.Push(point.Clone());
                point.X += xOffset;
            }
            if ((point.Y + yOffset) >= 0
                && (point.Y + yOffset) < Height)
            {
                //this.ErasePos.Push(point.Clone());
                point.Y += yOffset;
            }
            return point;
        }
    }

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
            while (gameLoop.Active)
            {
                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    gameLoop.Stop();
                    break;
                }
                gameBoad.OnKeyPress(keyInfo);
            }
            Console.CursorVisible = true;
            Console.SetCursorPosition(0, 0);
            Console.Clear();
        }
    }
}