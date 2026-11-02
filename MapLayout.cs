using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class MapLayout
    {
        public string Name { get; set; }
        public List<Vector2> Waypoints { get; set; } = new List<Vector2>();
        public List<Point> KnightLaneCells { get; set; } = new List<Point>();
        public List<Point> BlockedRoadCells { get; set; } = new List<Point>();
        public List<Point> BlockedSceneryCells { get; set; } = new List<Point>();
    }
}
