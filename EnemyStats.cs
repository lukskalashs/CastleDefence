using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class EnemyStats
    {
        public EnemyKind Kind { get; set; }
        public string DisplayName { get; set; }
        public float MaxHealth { get; set; }
        public float Speed { get; set; }
        public float Damage { get; set; }
        public float AttackRate { get; set; }
        public int Reward { get; set; }
        public float Scale { get; set; }
        public float MeleeEngageDistance { get; set; }
        public float HealthBarOffsetY { get; set; }
        public float TargetAimOffsetY { get; set; }

        public string WalkTexture { get; set; }
        public int WalkFrames { get; set; }
        public float WalkFrameTime { get; set; }

        public string AttackTexture { get; set; }
        public int AttackFrames { get; set; }
        public float AttackFrameTime { get; set; }

        public string HurtTexture { get; set; }
        public int HurtFrames { get; set; }
        public float HurtFrameTime { get; set; }

        public string DieTexture { get; set; }
        public int DieFrames { get; set; }
        public float DieFrameTime { get; set; }
    }
}
