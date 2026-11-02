using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class FloatingText
    {
        private string _text;
        private Vector2 _position;
        private float _speed;
        private float _duration;
        private Vector2 _shadowOffset;
        private float _timer = 0f;

        public bool IsExpired => _timer >= _duration;

        public FloatingText(string text, Vector2 startPos, float speed, float duration, Vector2 shadowOffset)
        {
            _text = text;
            _position = startPos;
            _speed = speed;
            _duration = duration;
            _shadowOffset = shadowOffset;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timer += dt;
            _position.Y -= _speed * dt;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            if (font == null) return;
            float alpha = MathHelper.Clamp(1f - (_timer / _duration), 0f, 1f);
            spriteBatch.DrawString(font, _text, _position + _shadowOffset, Color.Black * alpha);
            spriteBatch.DrawString(font, _text, _position, Color.Gold * alpha);
        }
    }
}
