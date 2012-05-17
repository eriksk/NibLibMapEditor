using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NibbleMapEditor
{
    static class SBExtensions
    {
        public static void DrawOutline(this SpriteBatch sb, Texture2D pixel, Rectangle rect, Color color)
        {
            sb.Draw(pixel, new Rectangle(rect.Left, rect.Top, rect.Width, 1), color);
            sb.Draw(pixel, new Rectangle(rect.Right, rect.Top, 1, rect.Height), color);
            sb.Draw(pixel, new Rectangle(rect.Left, rect.Bottom, rect.Width, 1), color);
            sb.Draw(pixel, new Rectangle(rect.Left, rect.Top, 1, rect.Height), color);
        }
    }
}