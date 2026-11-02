using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public abstract class Defence
    {
        protected Animation _currentAnim;
        protected float _cooldownTimer = 0f;
        protected float _rotation = 0f;
        protected bool _flipHorizontal = false;

        private float _flipDeadzone;
        private float _tiltFactor;
        private int _barWidth;
        private int _barHeight;
        private int _barBorder;
        private float _barOffsetY;
        private Color _barBackColor;
        protected Color _barFillColor;

        public Point Cell { get; set; }
        public Vector2 Position { get;  set; }
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
        public float Damage { get; set; }
        public float Range { get; set; }
        public float FireRate { get; set; }
        public float Scale { get; set; }
        public float MaxTilt { get; set; }

        public bool IsDying { get; set; }
        public bool IsDead { get; set; }

        public Defence(
            Point cell, Vector2 position,
            float maxHealth, float damage, float range, float fireRate,
            float scale, float maxTilt, float flipDeadzone, float tiltFactor,
            int barWidth, int barHeight, int barBorder, float barOffsetY,
            Color barBackColor, Color barFillColor)
        {
            Cell = cell;
            Position = position;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Damage = damage;
            Range = range;
            FireRate = fireRate;
            Scale = scale;
            MaxTilt = maxTilt;
            _flipDeadzone = flipDeadzone;
            _tiltFactor = tiltFactor;
            _barWidth = barWidth;
            _barHeight = barHeight;
            _barBorder = barBorder;
            _barOffsetY = barOffsetY;
            _barBackColor = barBackColor;
            _barFillColor = barFillColor;
        }

        public abstract void Update(GameTime gameTime, List<Enemy> enemies, List<Projectile> projectiles);

        public virtual void TakeDamage(float amount)
        {
            if (IsDying || IsDead) return;
            CurrentHealth = Math.Max(0f, CurrentHealth - amount);
        }

        protected Enemy FindNearestEnemy(List<Enemy> enemies)
        {
            Enemy nearest = null;
            float bestDistSq = Range * Range;

            foreach (var enemy in enemies)
            {
                if (enemy.IsDying || enemy.IsDead) continue;

                float distSq = Vector2.DistanceSquared(Position, enemy.Position);
                if (distSq <= bestDistSq)
                {
                    bestDistSq = distSq;
                    nearest = enemy;
                }
            }
            return nearest;
        }

        protected void AimAtTarget(Vector2 targetPos)
        {
            Vector2 diff = targetPos - Position;
            _flipHorizontal = diff.X < -_flipDeadzone;

            if (MaxTilt > 0f)
            {
                float rawAngle = (float)Math.Atan2(diff.Y, Math.Abs(diff.X));
                float tilt = MathHelper.Clamp(rawAngle * _tiltFactor, -MaxTilt, MaxTilt);
                _rotation = _flipHorizontal ? -tilt : tilt;
            }
            else
            {
                _rotation = 0f;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            if (IsDead || _currentAnim == null) return;
            _currentAnim.Draw(spriteBatch, Position, Scale, _rotation, _flipHorizontal, Color.White);
        }

        protected void DrawHealthBar(SpriteBatch spriteBatch, Texture2D pixel)
        {
            float ratio = MathHelper.Clamp(CurrentHealth / MaxHealth, 0f, 1f);
            int x = (int)(Position.X - _barWidth / 2f);
            int y = (int)(Position.Y + _barOffsetY);

            spriteBatch.Draw(pixel, new Rectangle(x - _barBorder, y - _barBorder, _barWidth + _barBorder * 2, _barHeight + _barBorder * 2), Color.Black);
            spriteBatch.Draw(pixel, new Rectangle(x, y, _barWidth, _barHeight), _barBackColor);
            spriteBatch.Draw(pixel, new Rectangle(x, y, (int)(_barWidth * ratio), _barHeight), _barFillColor);
        }
    }
}
