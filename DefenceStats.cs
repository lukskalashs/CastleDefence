using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class DefenceStats
    {
        public DefenceKind Kind { get; set; }
        public string DisplayName { get; set; }
        public int Cost { get; set; }
        public float MaxHealth { get; set; }
        public float Damage { get; set; }
        public float range { get; set; }
        public float FireRate { get; set; }
        public float ProjectileSpeed { get; set; }
        public bool HomingProjectile { get; set; }
        public PlacementType PlacementRule { get; set; }
        public float Scale { get; set; }
        public float ProjectileScale { get; set; }
        public float MaxTiltAngle { get; set; }
        public float TiltMultiplier { get; set; }
        public Vector2 ProjectileSpawnOffset { get; set; }
        public float HealthBarOffsetY { get; set; }
        public string IconTexture { get; set; }
        public Rectangle IconCrop { get; set; }
        public string IdleTexture { get; set; }
        public int IdleFrames { get; set; }
        public float IdleFrameTime { get; set; }
        public Rectangle? IdleCrop { get; set; }

        public string AttackTexture { get; set; }
        public int AttackFrames { get; set; }
        public float AttackFrameTime { get; set; }

        public string HurtTexture { get; set; }
        public int HurtFrames { get; set; }
        public float HurtFrameTime { get; set; }

        public string DieTexture { get; set; }
        public int DieFrames { get; set; }
        public float DieFrameTime { get; set; }

        public string ProjectileTexture { get; set; }
    }
}
