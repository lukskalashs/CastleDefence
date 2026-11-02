using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleDefence
{
    public class ShopButton
    {
        private Texture2D _iconTexture;
        private Rectangle _iconCrop;
        private Rectangle _iconOffset;
        private Vector2 _titleOffset;
        private Vector2 _costOffset;
        private int _borderThickness;

        private Color _normalBgColor;
        private Color _hoverBgColor;
        private Color _selectedBgColor;
        private Color _disabledBgColor;
        private Color _borderColor;
        private Color _disabledIconTint;
        private Color _disabledCostColor;

        public int DefenceId { get; private set; }
        public string DisplayName { get; private set; }
        public int Cost { get; private set; }
        public Rectangle Bounds { get; private set; }

        public ShopButton(
            int defenceId, string displayName, int cost,
            Texture2D iconTexture, Rectangle iconCrop, Rectangle bounds,
            Rectangle iconOffset, Vector2 titleOffset, Vector2 costOffset, int borderThickness,
            Color normalBgColor, Color hoverBgColor, Color selectedBgColor, Color disabledBgColor,
            Color borderColor, Color disabledIconTint, Color disabledCostColor)
        {
            DefenceId = defenceId;
            DisplayName = displayName;
            Cost = cost;
            _iconTexture = iconTexture;
            _iconCrop = iconCrop;
            Bounds = bounds;
            _iconOffset = iconOffset;
            _titleOffset = titleOffset;
            _costOffset = costOffset;
            _borderThickness = borderThickness;

            _normalBgColor = normalBgColor;
            _hoverBgColor = hoverBgColor;
            _selectedBgColor = selectedBgColor;
            _disabledBgColor = disabledBgColor;
            _borderColor = borderColor;
            _disabledIconTint = disabledIconTint;
            _disabledCostColor = disabledCostColor;
        }

        public bool Contains(Point mousePos)
        {
            return Bounds.Contains(mousePos);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel, SpriteFont font, bool isSelected, bool canAfford, bool isHovered)
        {
            Color bgColor = _normalBgColor;
            if (!canAfford)
            {
                bgColor = _disabledBgColor;
            }
            else if (isSelected)
            {
                bgColor = _selectedBgColor;
            }
            else if (isHovered)
            {
                bgColor = _hoverBgColor;
            }

            Color borderCol = isSelected ? Color.Gold : _borderColor;
            int b = _borderThickness;

            spriteBatch.Draw(pixel, Bounds, bgColor);
            spriteBatch.Draw(pixel, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, b), borderCol);
            spriteBatch.Draw(pixel, new Rectangle(Bounds.X, Bounds.Bottom - b, Bounds.Width, b), borderCol);
            spriteBatch.Draw(pixel, new Rectangle(Bounds.X, Bounds.Y, b, Bounds.Height), borderCol);
            spriteBatch.Draw(pixel, new Rectangle(Bounds.Right - b, Bounds.Y, b, Bounds.Height), borderCol);

            Rectangle iconDest = new Rectangle(Bounds.X + _iconOffset.X, Bounds.Y + _iconOffset.Y, _iconOffset.Width, _iconOffset.Height);
            Color tint = canAfford ? Color.White : _disabledIconTint;
            spriteBatch.Draw(_iconTexture, iconDest, _iconCrop, tint);

            if (font != null)
            {
                Color textColor = canAfford ? Color.White : Color.Gray;
                Color costColor = canAfford ? Color.Gold : _disabledCostColor;
                Vector2 boxPos = new Vector2(Bounds.X, Bounds.Y);

                spriteBatch.DrawString(font, DisplayName, boxPos + _titleOffset, textColor);
                spriteBatch.DrawString(font, $"Cost: {Cost}", boxPos + _costOffset, costColor);
            }
        }
    }
}

