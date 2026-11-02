using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class Enemy
    {
        protected List<Vector2> _waypoints;
        protected Animation _walkAnim;
        protected Animation _attackAnim;
        protected Animation _hurtAnim;
        protected Animation _dieAnim;
        protected Animation _currentAnim;

        private float _speed;
        private float _damage;
        private float _attackRate;
        private float _scale;
        private float _meleeEngageDist;
        private float _aimOffsetY;
        private float _deathHoldDuration;
        private float _flipDeadzone;
        private float _walkFlipDeadzone;

        private int _barWidth;
        private int _barHeight;
        private int _barBorder;
        private float _barOffsetY;
        private Color _barBackColor;
        private Color _barFillColor;

        private int _waypointIndex;
        private float _attackCooldown = 0f;
        private float _deathTimer = 0f;
        private bool _flipHorizontal = false;
        private Knight _engagedKnight;

        public Vector2 Position { get; private set; }
        public Vector2 TargetCenter => new Vector2(Position.X, Position.Y + _aimOffsetY);
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }
        public int Reward { get; private set; }
        public bool IsDying { get; private set; }
        public bool IsDead { get; private set; }
        public bool ReachedExit { get; private set; }
        public bool RewardGranted { get; set; }

        public Enemy(
            List<Vector2> waypoints,
            float maxHealth, float speed, float damage, float attackRate,
            int reward, float scale, float meleeEngageDist,
            float aimOffsetY, float deathHoldDuration,
            float flipDeadzone, float walkFlipDeadzone,
            int barWidth, int barHeight, int barBorder, float barOffsetY,
            Color barBackColor, Color barFillColor)
        {
            _waypoints = waypoints;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            _speed = speed;
            _damage = damage;
            _attackRate = attackRate;
            Reward = reward;
            _scale = scale;
            _meleeEngageDist = meleeEngageDist;
            _aimOffsetY = aimOffsetY;
            _deathHoldDuration = deathHoldDuration;
            _flipDeadzone = flipDeadzone;
            _walkFlipDeadzone = walkFlipDeadzone;

            _barWidth = barWidth;
            _barHeight = barHeight;
            _barBorder = barBorder;
            _barOffsetY = barOffsetY;
            _barBackColor = barBackColor;
            _barFillColor = barFillColor;

            Position = waypoints[0];
            _waypointIndex = waypoints.Count > 1 ? 1 : 0;
        }

        public void TakeDamage(float amount)
        {
            if (IsDying || IsDead || ReachedExit) return;

            CurrentHealth = Math.Max(0f, CurrentHealth - amount);
            if (CurrentHealth <= 0f)
            {
                IsDying = true;
                _dieAnim.Reset();
                _currentAnim = _dieAnim;
            }
            else if (_engagedKnight == null)
            {
                _hurtAnim.Reset();
                _currentAnim = _hurtAnim;
            }
        }

        public void Update(GameTime gameTime, List<Defence> defences)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsDying)
            {
                _currentAnim.Update(gameTime);
                if (_currentAnim.Finished)
                {
                    _deathTimer += dt;
                    if (_deathTimer >= _deathHoldDuration)
                    {
                        IsDead = true;
                    }
                }
                return;
            }

            if (_attackCooldown > 0f) _attackCooldown -= dt;

            // Resume walking if the knight we were fighting died
            if (_engagedKnight != null && (_engagedKnight.IsDying || _engagedKnight.IsDead))
            {
                _engagedKnight = null;
                _currentAnim = _walkAnim;
            }

            if (_engagedKnight == null)
            {
                _engagedKnight = FindBlockingKnight(defences);
                if (_engagedKnight != null)
                {
                    _attackAnim.Reset();
                    _currentAnim = _attackAnim;
                }
            }

            if (_engagedKnight != null)
            {
                _flipHorizontal = (_engagedKnight.Position.X - Position.X) < -_flipDeadzone;
                if (_attackCooldown <= 0f)
                {
                    _engagedKnight.TakeDamage(_damage);
                    _attackCooldown = 1f / _attackRate;
                }
                _currentAnim.Update(gameTime);
                return;
            }

            if (_currentAnim == _hurtAnim && _hurtAnim.Finished)
            {
                _currentAnim = _walkAnim;
            }

            MoveAlongWaypoints(dt);
            _currentAnim.Update(gameTime);
        }

        private Knight FindBlockingKnight(List<Defence> defences)
        {
            float engageDistSq = _meleeEngageDist * _meleeEngageDist;
            foreach (var def in defences)
            {
                if (def is Knight knight && !knight.IsDying && !knight.IsDead)
                {
                    if (Vector2.DistanceSquared(Position, knight.Position) <= engageDistSq)
                    {
                        return knight;
                    }
                }
            }
            return null;
        }

        private void MoveAlongWaypoints(float dt)
        {
            float step = _speed * dt;
            while (step > 0f && _waypointIndex < _waypoints.Count)
            {
                Vector2 target = _waypoints[_waypointIndex];
                Vector2 diff = target - Position;
                float dist = diff.Length();

                if (Math.Abs(diff.X) > _walkFlipDeadzone)
                {
                    _flipHorizontal = diff.X < 0f;
                }

                if (dist <= step)
                {
                    Position = target;
                    step -= dist;
                    _waypointIndex++;
                }
                else
                {
                    Position += (diff / dist) * step;
                    step = 0f;
                }
            }

            if (_waypointIndex >= _waypoints.Count)
            {
                ReachedExit = true;
                IsDead = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            if (IsDead || _currentAnim == null) return;

            _currentAnim.Draw(spriteBatch, Position, _scale, 0f, _flipHorizontal, Color.White);

            if (!IsDying)
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
}

