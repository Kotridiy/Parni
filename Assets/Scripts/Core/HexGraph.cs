using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class HexGraph
    {
        public static HexGraph Graph { get; private set; }

        public int SizeN { get; private set; }
        public int SizeM { get; private set; }
        public float CellSize { get; private set; }

        readonly GraphPoint[,] points;
        readonly GraphEdge[,,] edges;

        public HexGraph(int N, int M, float cellSize = 1)
        {
            if (Graph == null) Graph = this;

            SizeN = N;
            SizeM = M;
            CellSize = cellSize;

            points = new GraphPoint[N, M];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    points[i, j] = new GraphPoint(i, j, N, M, cellSize);
                }
            }

            edges = new GraphEdge[N, M, 3];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        if (i < N - 1) edges[i, j, 0] = new GraphEdge(points[i, j], points[i + 1, j]);
                        if (j > 0) edges[i, j, 1] = edges[i, j - 1, 2];
                        if (j < M - 1) edges[i, j, 2] = new GraphEdge(points[i, j], points[i, j + 1]);
                    }
                    else
                    {
                        if (i > 0) edges[i, j, 0] = edges[i - 1, j, 0];
                        if (j > 0) edges[i, j, 1] = edges[i, j - 1, 2];
                        if (j < M - 1) edges[i, j, 2] = new GraphEdge(points[i, j], points[i, j + 1]);
                    }
                }
            }
        }

        public GraphEdge GetGraphEdge(int aX, int aY, int bX, int bY)
        {
            if (aX < 0 || aY < 0 || bX < 0 || bY < 0 ||
                aX >= SizeN || aY >= SizeM || bX >= SizeN || bY >= SizeM) return null;

            if (aY - bY == -1 && aX == bX) return edges[aX, aY, 2];
            if (aY - bY == 1 && aX == bX) return edges[aX, aY, 1];
            if (aX - bX == -1 && aY == bY && (aX + aY) % 2 == 0) return edges[aX, aY, 0];
            if (aX - bX == 1 && aY == bY && (aX + aY) % 2 == 1) return edges[aX, aY, 0];

            return null;
        }

        public GraphEdge GetGraphEdge(GraphPoint a, GraphPoint b) => (a != null && b != null) ? GetGraphEdge(a.X, a.Y, b.X, b.Y) : null;

        public GraphPoint GetGraphPoint(int x, int y)
        {
            if ((x < 0) || (y < 0) || (x >= SizeN) || (y >= SizeM)) return null;
            return points[x, y];
        }

        public GraphPoint[] GetNeighbours(GraphPoint point)
        {
            GraphPoint[] neighbours = new GraphPoint[4];
            neighbours[0] = GetGraphPoint(point.X - 1, point.Y);
            neighbours[1] = GetGraphPoint(point.X + 1, point.Y);
            neighbours[2] = GetGraphPoint(point.X, point.Y - 1);
            neighbours[3] = GetGraphPoint(point.X, point.Y + 1);

            for (int i = 0; i < 4; i++)
            {
                if (GetGraphEdge(point, neighbours[i]) == null) neighbours[i] = null;
            }
            return neighbours;
        }

        public GraphPoint FindClosestPoint(float posX, float posY)
        {
            int indexX = -1, indexY = -1;

            // Find y
            if (points[0, 0].PosY >= posY)
            {
                indexY = 0;
            }
            else if (points[0, SizeM - 1].PosY <= posY)
            {
                indexY = SizeM - 1;
            }
            else
            {
                int right = 0, left = SizeM - 1;
                while (left - right > 1)
                {
                    if (posY - points[0, right].PosY > points[0, left].PosY - posY)
                    {
                        right += (left - right) / 2;
                    }
                    else
                    {
                        left -= (left - right) / 2;
                    }
                }
                if (posY - points[0, right].PosY > points[0, left].PosY - posY)
                {
                    indexY = left;
                }
                else
                {
                    indexY = right;
                }
            }

            // Find x
            if (points[0, indexY].PosX >= posX)
            {
                indexX = 0;
            }
            else if (points[SizeN - 1, indexY].PosX <= posX)
            {
                indexX = SizeN - 1;
            }
            else
            {
                int right = 0, left = SizeN - 1;
                while (left - right > 1)
                {
                    if (posX - points[right, indexY].PosX > points[left, indexY].PosX - posX)
                    {
                        right += (left - right) / 2;
                    }
                    else
                    {
                        left -= (left - right) / 2;
                    }
                }
                if (posX - points[right, indexY].PosX > points[left, indexY].PosX - posX)
                {
                    indexX = left;
                }
                else
                {
                    indexX = right;
                }
            }

            return points[indexX, indexY];
        }

        #region Find Way
        public List<GraphPoint> FindShortWay(int fromX, int fromY, int toX, int toY) => FindShortWay((level) => 1, points[fromX, fromY], points[toX, toY]);
        public List<GraphPoint> FindShortWay(GraphPoint fromPoint, GraphPoint toPoint) => FindShortWay((level) => 1, fromPoint, toPoint);
        public List<GraphPoint> FindShortWay(Func<EdgeLevel, float> speedFunc, int fromX, int fromY, int toX, int toY) => FindShortWay(speedFunc, points[fromX, fromY], points[toX, toY]);
        public List<GraphPoint> FindShortWay(Func<EdgeLevel, float> speedFunc, GraphPoint fromPoint, GraphPoint toPoint) => FindWay(speedFunc, fromPoint, toPoint);

        public List<GraphPoint> FindSafeWay(int fromX, int fromY, int toX, int toY, Memory memory, float safeCoefficient) => FindSafeWay((level) => 1, points[fromX, fromY], points[toX, toY], memory, safeCoefficient);
        public List<GraphPoint> FindSafeWay(GraphPoint fromPoint, GraphPoint toPoint, Memory memory, float safeCoefficient) => FindSafeWay((level) => 1, fromPoint, toPoint, memory, safeCoefficient);
        public List<GraphPoint> FindSafeWay(Func<EdgeLevel, float> speedFunc, int fromX, int fromY, int toX, int toY, Memory memory, float safeCoefficient) => FindSafeWay(speedFunc, points[fromX, fromY], points[toX, toY], memory, safeCoefficient);
        public List<GraphPoint> FindSafeWay(Func<EdgeLevel, float> speedFunc, GraphPoint fromPoint, GraphPoint toPoint, Memory memory, float safeCoefficient) => FindWay(speedFunc, fromPoint, toPoint, memory, safeCoefficient);

        private List<GraphPoint> FindWay(Func<EdgeLevel, float> speedFunc, GraphPoint fromPoint, GraphPoint toPoint, Memory memory = null, float safeCoefficient = 0)
        {
            List<GraphPoint> visited = new List<GraphPoint>(); // V - коллекция посещённых точек
            List<GraphPoint> candidates = new List<GraphPoint>(); // Q - коллекция потенциальных кандидатов
            Dictionary<GraphPoint, float> passed = new Dictionary<GraphPoint, float>(); // g(p) - расстояние до начальной точки
            Dictionary<GraphPoint, float> full = new Dictionary<GraphPoint, float>(); // f(p) = g(p) + h(p) - оценка полного пути
            Dictionary<GraphPoint, GraphPoint> parents = new Dictionary<GraphPoint, GraphPoint>(); // parent(p) - для дальнейшего построения обратного маршрута

            candidates.Add(fromPoint);
            passed.Add(fromPoint, 0);
            full.Add(fromPoint, GetWayEstimate(fromPoint, toPoint));
            while (candidates.Count != 0)
            {
                var current = candidates.Aggregate(candidates[0], (min, p) => full[p] < full[min] ? p : min);
                if (current == toPoint)
                {
                    return GetWayFromParents(parents, fromPoint, toPoint);
                }
                candidates.Remove(current);
                visited.Add(current);

                GraphPoint[] neighbours = GetNeighbours(current);
                foreach (var point in neighbours)
                {
                    var edge = GetGraphEdge(point, current);
                    var tentativeScore = passed[current] + (edge == null ? 0 : speedFunc(edge.Level) + GetDangerScore(memory, safeCoefficient, point));

                    if (point == null || edge == null || visited.Contains(point) && tentativeScore >= passed[point]) continue;
                    if (!visited.Contains(point) || tentativeScore < passed[point])
                    {
                        parents[point] = current;
                        passed[point] = tentativeScore;
                        full[point] = tentativeScore + GetWayEstimate(point, toPoint);
                        if (!candidates.Contains(point)) candidates.Add(point);
                    }

                }
            }
            return null;
        }

        private float GetDangerScore(Memory memory, float safeCoefficient, GraphPoint point)
        {
            if (memory != null && memory.TryGetMemory(point.GetInfo(), out var memoryInfo))
            {
                return memoryInfo.DangerLevel * safeCoefficient;
            }
            else return 0;
            
        }

        private float GetWayEstimate(GraphPoint fromPoint, GraphPoint toPoint) // h(p) - евристика расстояния до цели (манхетенское расстояние)
        {
            return (Mathf.Abs(fromPoint.X - toPoint.X) + Mathf.Abs(fromPoint.Y - toPoint.Y)) * 0.5f;
        }

        private List<GraphPoint> GetWayFromParents(Dictionary<GraphPoint, GraphPoint> parents, GraphPoint from, GraphPoint to)
        {
            List<GraphPoint> way = new List<GraphPoint> { to };
            GraphPoint current = to;
            while (current != from)
            {
                current = parents[current];
                way.Add(current);
            }
            way.Reverse();
            return way;
        }
        #endregion

        public ScanInfo[] ScanPoint(GraphPoint point, int visualRange = 1)
        {
            var visitedPoints = new List<GraphPoint>();
            List<ScanInfo> infos = new List<ScanInfo>(ScanRecursion(point, ref visitedPoints, visualRange));

            foreach (var info in infos)
            {
                info.Entities = info.Entities.Where(e =>
                    {
                        if (e == null) return false;
                        if (e is GamePlace) return true;
                        var unit = e as GameUnit;
                        if (unit.OnRoad == null) return true;
                        return visitedPoints.Contains(unit.OnRoad.First)
                            && visitedPoints.Contains(unit.OnRoad.Second);
                    });
            }
            return infos.ToArray();
        }

        private IEnumerable<ScanInfo> ScanRecursion(GraphPoint point, ref List<GraphPoint> visitedPoints, int visualRange)
        {
            List<ScanInfo> infos = new List<ScanInfo>();

            var entities = new List<GameEntity>();
            entities.AddRange(point.GetUnits());
            if (point.GamePlace != null) entities.Add(point.GamePlace);

            infos.Add(new ScanInfo(point.GetInfo(), entities));
            visitedPoints.Add(point);

            if (visualRange > 0)
            {
                var neighbours = Graph.GetNeighbours(point);
                foreach (var neighbour in neighbours)
                {
                    if (neighbour == null || visitedPoints.Contains(neighbour)) continue;
                    infos.AddRange(ScanRecursion(neighbour, ref visitedPoints, visualRange - 1));
                }
            }

            return infos;
        }

        public bool IsInRange(Vector2 pointA, Vector2 pointB, float edgesDistance = 0.5f)
        {
            float distance = edgesDistance * GraphPoint.R * CellSize * 2;
            return Vector2.Distance(pointA, pointB) <= distance;
        }
        public bool IsNear(GameEntity entityA, GameEntity entityB)
        {
            return entityA.Point == entityB.Point && IsInRange(entityA.transform.position, entityB.transform.position, 0.1f);
        }
    }

    public class GraphEdge
    {
        public GraphPoint First { get; private set; }
        public GraphPoint Second { get; private set; }

        public event Action<EdgeLevel> LevelChanged;

        // Travelers

        public GraphEdge(GraphPoint first, GraphPoint second)
        {
            First = first;
            Second = second;
            exp = UnityEngine.Random.Range(0, 100);
        }

        public EdgeLevel Level
        {
            // Для прикола сделано, надо переделать через опыт
            get
            {
                if (exp > 90) return EdgeLevel.Highway;
                if (exp > 80) return EdgeLevel.Road;
                if (exp > 35) return EdgeLevel.Walkway;
                if (exp > 10) return EdgeLevel.Foorpath;

                return EdgeLevel.None;
            }
        }

        private int exp;

        public GraphPoint GetAnotherEnd(GraphPoint point)
        {
            return First == point ? Second : (Second == point ? First : null);
        }

        public override string ToString()
        {
            return string.Format($"_road from {First} to {Second}_");
        }
    }

    public class GraphPoint
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public Vector2 Pos { get => new Vector2 (PosX, PosY); }

        public static float R
        {
            get
            {
                if (r == 0) r = 1 / (float)Math.Sqrt(3);
                return r;
            }
        }
        static float r = 0;

        public GamePlace GamePlace
        {
            get => gamePlace;
            set
            {
                if (gamePlace == null) gamePlace = value;
                else throw new InvalidOperationException("Точка уже занята другим объектом - " + GamePlace.Name);
            }
        }

        List<GameUnit> units;
        private GamePlace gamePlace;

        public GraphPoint(int x, int y, int sizeX, int sizeY, float cellSize = 1)
        {
            X = x;
            Y = y;

            if ((x + y) % 2 == 0)
            {
                PosX = (x * 3 + 2 - sizeX * 1.5f) * R * cellSize;
            }
            else
            {
                PosX = (x * 3 + 1 - sizeX * 1.5f) * R * cellSize;
            }
            PosY = (y - sizeY / 2) * cellSize;
        }

        public void AddUnit(GameUnit unit)
        {
            if (units == null) units = new List<GameUnit>();
            if (units.Contains(unit)) throw new InvalidOperationException($"Точка {this} уже содержит в себе этот {unit.name}");
            units.Add(unit);
        }

        public void RemoveUnit(GameUnit unit)
        {
            if (!units.Contains(unit)) throw new InvalidOperationException($"Точка {this} не содержит в себе этот {unit.name}");
            units.Remove(unit);
            if (units.Count == 0) units = null;
        }

        public GameUnit GetUnit(int index = 0)
        {
            return (units != null) && (units.Count > index) ? units[index] : null;
        }

        public IEnumerable<GameUnit> GetUnits()
        {
            return units ?? new List<GameUnit>();
        }

        public GraphPointInfo GetInfo()
        {
            return new GraphPointInfo(X, Y, PosX, PosY);
        }

        public override bool Equals(object obj)
        {
            return (this is null && obj is null) ||
                   !(this is null) && (
                        (obj is GraphPointInfo pointInfo && X == pointInfo.X && Y == pointInfo.Y) ||
                        (obj is GraphPoint point && X == point.X && Y == point.Y)
                   );
        }

        public override int GetHashCode()
        {
            return 564852156 + X.GetHashCode() + Y.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format($"[{X}, {Y}]");
        }
    }

    public enum EdgeLevel
    {
        None = 0,
        Foorpath = 1,
        Walkway = 2,
        Road = 3,
        Highway = 4,
    }

}