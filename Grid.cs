using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class Grid
    {
        private int _cellSize;
        private int _cols;
        private int _rows;
        private int _mapWidth;
        private int _mapHeight;

        private int _knightId;
        private int _elfId;
        private int _towerId;


        private List<Point> _laneCells = new List<Point>();
        private List<Point> _blockedRoadCells = new List<Point>();
        private List<Point> _blockedSceneryCells = new List<Point>();
        private List<Point> _occupiedCells = new List<Point>();

        public Grid(
            int cellSize, int cols, int rows, int mapWidth, int mapHeight,
            int knightId, int elfId, int towerId,
            Color laneDebugColor, Color roadDebugColor, Color sceneryDebugColor, Color lineDebugColor,
            int nodeSize, float lineThickness, Vector2 textOffset, float textScale)
        {
            _cellSize = cellSize;
            _cols = cols;
            _rows = rows;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;

            _knightId = knightId;
            _elfId = elfId;
            _towerId = towerId;

          
        }

        public void LoadStageCells(List<Point> laneCells, List<Point> blockedRoad, List<Point> blockedScenery)
        {
            _laneCells.Clear();
            _blockedRoadCells.Clear();
            _blockedSceneryCells.Clear();
            _occupiedCells.Clear();

            foreach (var p in laneCells) _laneCells.Add(p);
            foreach (var p in blockedRoad) _blockedRoadCells.Add(p);
            foreach (var p in blockedScenery) _blockedSceneryCells.Add(p);
        }

        public bool IsMouseInMap(Point mousePos)
        {
            return mousePos.X >= 0 && mousePos.X < _mapWidth &&
                   mousePos.Y >= 0 && mousePos.Y < _mapHeight;
        }

        public Point ToCell(Point screenPixel)
        {
            return new Point(screenPixel.X / _cellSize, screenPixel.Y / _cellSize);
        }

        public Vector2 CellCentre(Point cell)
        {
            return new Vector2(
                (cell.X * _cellSize) + (_cellSize / 2f),
                (cell.Y * _cellSize) + (_cellSize / 2f)
            );
        }

        public Rectangle GetCellRect(Point cell)
        {
            return new Rectangle(cell.X * _cellSize, cell.Y * _cellSize, _cellSize, _cellSize);
        }

        public bool InBounds(Point cell)
        {
            return cell.X >= 0 && cell.X < _cols && cell.Y >= 0 && cell.Y < _rows;
        }

        public bool IsLane(Point cell) => _laneCells.Contains(cell);
        public bool IsBlockedRoad(Point cell) => _blockedRoadCells.Contains(cell);
        public bool IsBlockedScenery(Point cell) => _blockedSceneryCells.Contains(cell);
        public bool IsBlocked(Point cell) => IsBlockedRoad(cell) || IsBlockedScenery(cell);
        public bool IsOccupied(Point cell) => _occupiedCells.Contains(cell);

        public void SetOccupied(Point cell, bool occupied)
        {
            if (occupied)
            {
                if (!_occupiedCells.Contains(cell)) _occupiedCells.Add(cell);
            }
            else
            {
                _occupiedCells.Remove(cell);
            }
        }

        public bool NearLane(Point cell)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    Point neighbour = new Point(cell.X + dx, cell.Y + dy);
                    if (InBounds(neighbour) && _laneCells.Contains(neighbour))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanPlace(int defenceId, Point cell)
        {
            if (!InBounds(cell) || IsOccupied(cell)) return false;

            if (defenceId == _knightId)
            {
                return IsLane(cell);
            }
            else if (defenceId == _elfId)
            {
                return !IsLane(cell) && !IsBlocked(cell) && NearLane(cell);
            }
            else if (defenceId == _towerId)
            {
                return !IsLane(cell) && !IsBlocked(cell);
            }

            return false;
        }
       

        
        
    }
}
