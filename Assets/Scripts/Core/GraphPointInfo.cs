﻿using UnityEngine;

namespace Assets.Scripts.Core
{
    public class GraphPointInfo
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public Vector2 Pos { get => new Vector2(PosX, PosY); } 

        public GraphPointInfo(int x, int y, float posX, float posY)
        {
            X = x;
            Y = y;
            PosX = posX;
            PosY = posY;
        }

        public static bool operator ==(GraphPointInfo a, GraphPointInfo b)
        {
            return ReferenceEquals(a, b) || !(a is null) && !(b is null) && (a.X == b.X && a.Y == b.Y);
        }

        public static bool operator !=(GraphPointInfo a, GraphPointInfo b)
        {
            return !(ReferenceEquals(a, b) || !(a is null) && !(b is null) && (a.X == b.X && a.Y == b.Y));
        }

        public override bool Equals(object obj)
        {
            return obj is GraphPointInfo point &&
                   X == point.X && Y == point.Y;
        }

        public override int GetHashCode()
        {
            return 846285656 + X.GetHashCode() + Y.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format($"[{X}, {Y}]");
        }
    }
}