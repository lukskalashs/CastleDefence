using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class Knight : Defence
    {
        private Animation _idleAnim;
        private Animation _attackAnim;
        private Animation _hurtAnim;
        private Animation _dieAnim;
        private float _deathHoldDuration;
        private float _deathTimer = 0f;
        private bool _isAttacking = false;

        public Knight(
            Point cell, Vector2 position,
            Texture2D attackTex, Texture2D hurtTex, Texture2D dieTex,
            Rectangle idleCrop,
            int attackFrames, float attackFrameTime,
            int hurtFrames, float hurtFrameTime,
            int dieFrames, float dieFrameTime,
            float maxHealth, float damage, float range, float fireRate,
            float scale, float feetOffsetY, float deathHoldDuration, float flipDeadzone,
            int barWidth, int barHeight, int barBorder, float barOffsetY,
            Color barBackColor, Color barFillColor)
            : base(
                cell, position, maxHealth, damage, range, fireRate,
                scale, 0f, flipDeadzone, 0f,
                barWidth, barHeight, barBorder, barOffsetY, barBackColor, barFillColor)
        {
            _deathHoldDuration = deathHoldDuration;

            // Idle uses first frame of attack_knight1.png
            _idleAnim = new Animation(attackTex, 1, attackFrameTime, true, feetOffsetY, idleCrop);
            _attackAnim = new Animation(attackTex, attackFrames, attackFrameTime, false, feetOffsetY);
            _hurtAnim = new Animation(hurtTex, hurtFrames, hurtFrameTime, false, feetOffsetY);
            _dieAnim = new Animation(dieTex, dieFrames, dieFrameTime, false, feetOffsetY);

            _currentAnim = _idleAnim;
        }

        public override void TakeDamage(float amount)
        {
            if (IsDying || IsDead) return;

            base.TakeDamage(amount);
            if (CurrentHealth <= 0f)
            {
                IsDying = true;
                _dieAnim.Reset();
                _currentAnim = _dieAnim;
            }
            else if (!_isAttacking)
            {
                _hurtAnim.Reset();
                _currentAnim = _hurtAnim;
            }
        }

        public override void Update(GameTime gameTime, List<Enemy> enemies, List<Projectile> projectiles)
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

            if (_cooldownTimer > 0f) _cooldownTimer -= dt;

            Enemy target = FindNearestEnemy(enemies);
            if (target != null)
            {
                AimAtTarget(target.Position);
                if (_cooldownTimer <= 0f)
                {
                    _isAttacking = true;
                    _attackAnim.Reset();
                    _currentAnim = _attackAnim;
                    target.TakeDamage(Damage);
                    _cooldownTimer = 1f / FireRate;
                }
            }

            _currentAnim.Update(gameTime);

            if (_currentAnim == _attackAnim && _attackAnim.Finished)
            {
                _isAttacking = false;
                _currentAnim = _idleAnim;
            }
            else if (_currentAnim == _hurtAnim && _hurtAnim.Finished)
            {
                _currentAnim = _idleAnim;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            base.Draw(spriteBatch, pixel);
            if (!IsDying && !IsDead)
            {
                DrawHealthBar(spriteBatch, pixel);
            }
        }
    }
}

