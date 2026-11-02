using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public abstract class Projectile
    {
        protected Texture2D _texture;
        protected float _speed;
        protected float _damage;
        protected float _scale;
        protected bool _homing;
        protected float _hitRadius;
        protected int _mapWidth;
        protected int _mapHeight;
        protected float _offscreenMargin;
        protected Enemy _target;
        protected Vector2 _direction;
        protected float _angle;

        public Vector2 Position { get; protected set; }
        public bool IsDead { get; protected set; }

        public Projectile(
            Texture2D texture, Vector2 startPos, Enemy target,
            float speed, float damage, float scale, bool homing,
            float hitRadius, int mapWidth, int mapHeight, float offscreenMargin)
        {
            _texture = texture;
            Position = startPos;
            _target = target;
            _speed = speed;
            _damage = damage;
            _scale = scale;
            _homing = homing;
            _hitRadius = hitRadius;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _offscreenMargin = offscreenMargin;

            Vector2 aimPos = target != null ? target.TargetCenter : startPos + Vector2.UnitX;
            Vector2 diff = aimPos - startPos;
            _direction = diff != Vector2.Zero ? Vector2.Normalize(diff) : Vector2.UnitX;
            _angle = (float)Math.Atan2(_direction.Y, _direction.X);
        }

        protected abstract float GetDrawRotation();

        public void Update(GameTime gameTime, List<Enemy> enemies)
        {
            if (IsDead) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_homing && _target != null && !_target.IsDying && !_target.IsDead)
            {
                Vector2 diff = _target.TargetCenter - Position;
                if (diff != Vector2.Zero)
                {
                    _direction = Vector2.Normalize(diff);
                    _angle = (float)Math.Atan2(_direction.Y, _direction.X);
                }
            }

            Position += _direction * (_speed * dt);

            if (Position.X < -_offscreenMargin || Position.X > _mapWidth + _offscreenMargin ||
                Position.Y < -_offscreenMargin || Position.Y > _mapHeight + _offscreenMargin)
            {
                IsDead = true;
                return;
            }

            float hitRadiusSq = _hitRadius * _hitRadius;
            foreach (var enemy in enemies)
            {
                if (enemy.IsDying || enemy.IsDead) continue;

                if (Vector2.DistanceSquared(Position, enemy.TargetCenter) <= hitRadiusSq)
                {
                    enemy.TakeDamage(_damage);
                    IsDead = true;
                    return;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsDead || _texture == null) return;
            Vector2 origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
            spriteBatch.Draw(_texture, Position, null, Color.White, GetDrawRotation(), origin, _scale, SpriteEffects.None, 0f);
        }
    }

}
