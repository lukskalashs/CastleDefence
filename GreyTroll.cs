using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class GreyTroll : Enemy
    {
        public GreyTroll(
            Texture2D walkTex, int walkFrames, float walkFrameTime,
            Texture2D attackTex, int attackFrames, float attackFrameTime,
            Texture2D hurtTex, int hurtFrames, float hurtFrameTime,
            Texture2D dieTex, int dieFrames, float dieFrameTime,
            List<Vector2> waypoints,
            float maxHealth, float speed, float damage, float attackRate,
            int reward, float scale, float meleeEngageDist,
            float feetOffsetY, float aimOffsetY, float deathHoldDuration,
            float flipDeadzone, float walkFlipDeadzone,
            int barWidth, int barHeight, int barBorder, float barOffsetY,
            Color barBackColor, Color barFillColor)
            : base(
                waypoints, maxHealth, speed, damage, attackRate,
                reward, scale, meleeEngageDist, aimOffsetY, deathHoldDuration,
                flipDeadzone, walkFlipDeadzone,
                barWidth, barHeight, barBorder, barOffsetY, barBackColor, barFillColor)
        {
            _walkAnim = new Animation(walkTex, walkFrames, walkFrameTime, true, feetOffsetY);
            _attackAnim = new Animation(attackTex, attackFrames, attackFrameTime, true, feetOffsetY);
            _hurtAnim = new Animation(hurtTex, hurtFrames, hurtFrameTime, false, feetOffsetY);
            _dieAnim = new Animation(dieTex, dieFrames, dieFrameTime, false, feetOffsetY);

            _currentAnim = _walkAnim;
        }
    }

}
