using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class EnergyBall : Projectile
    {
        public EnergyBall(
            Texture2D texture, Vector2 startPos, Enemy target,
            float speed, float damage, float scale, bool homing,
            float hitRadius, int mapWidth, int mapHeight, float offscreenMargin)
            : base(texture, startPos, target, speed, damage, scale, homing, hitRadius, mapWidth, mapHeight, offscreenMargin)
        {
        }

        protected override float GetDrawRotation()
        {
            return _angle;
        }
    }
}
