using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class StageInfo
    {
        public int StageNumber { get; set; }
        public string DisplayName { get; set; }
        public string BackgroundTexture { get; set; }
        public string LayoutName { get; set; }
        public int StartingFunds { get; set; }
        public float HealthMultiplier { get; set; }
        public float SpeedMultiplier { get; set; }
        public List<SpawnItem> Spawns { get; set; } = new List<SpawnItem>();
    }
}
