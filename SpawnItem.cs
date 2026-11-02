using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class SpawnItem
    {
        public EnemyKind EnemyKind { get; set; }
        public float Delay { get; set; }

        public SpawnItem(EnemyKind enemyKind, float delay)
        {
            EnemyKind = enemyKind;
            Delay = delay;
        }
    }
}
