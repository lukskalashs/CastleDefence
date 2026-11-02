using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class Arrow : Projectile
    {
        private float _rotationOffset;

        public Arrow(
            Texture2D texture, Vector2 startPos, Enemy target,
            float speed, float damage, float scale, bool homing,
            float rotationOffset, float hitRadius, int mapWidth, int mapHeight, float offscreenMargin)
            : base(texture, startPos, target, speed, damage, scale, homing, hitRadius, mapWidth, mapHeight, offscreenMargin)
        {
            _rotationOffset = rotationOffset;
        }

        protected override float GetDrawRotation()
        {
            // Arrow sprite points UP in the png so add PI/2 offset
            return _angle + _rotationOffset;
        }
    }

}
