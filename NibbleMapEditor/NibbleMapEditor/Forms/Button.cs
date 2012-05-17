using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace NibbleMapEditor.Forms
{
    class Button
    {
        public event OnClicked OnClickedEvent;
        public delegate void OnClicked();
        public Color bgColor = Color.Black * 0.5f;
        public Color borderColor;
        public Rectangle rect;
        public Texture2D pixel;
        public SpriteFont font;
        public float padding = 4;
        public float fontHeight;
        public string text;

        public Button(int x, int y, string text)
        {
            rect = new Rectangle(x, y, 0, 0);
            this.text = text;
        }

        public Button Load(ContentManager content, GraphicsDevice graphics)
        {
            pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            font = content.Load<SpriteFont>(@"fonts/font");
            fontHeight = font.MeasureString("HEIGHT").Y;

            rect.Width = (int)(font.MeasureString(text).X + padding * 2f);
            rect.Height = (int)(font.MeasureString(text).Y + padding * 2f);

            return this;
        }

        MouseState om, m;
        public void Update()
        {
            om = m;
            m = Mouse.GetState();

            if (rect.Contains(m.X, m.Y))
            {
                if (m.RightButton == ButtonState.Pressed)
                {
                    rect.X += (int)((m.X - om.X));
                    rect.Y += (int)((m.Y - om.Y));
                }
            }


            if (rect.Contains(m.X, m.Y))
            {
                if (om.LeftButton == ButtonState.Released && m.LeftButton == ButtonState.Pressed)
                {
                    if (OnClickedEvent != null)
                    {
                        OnClickedEvent();
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(pixel, rect, bgColor);
            sb.DrawOutline(pixel, rect, Color.Black);
            if (rect.Contains(m.X, m.Y))
            {
                sb.Draw(pixel, rect, Color.White * 0.5f);
            }
            sb.DrawString(font, text, new Vector2(rect.X + padding, rect.Y + padding), Color.White);
        }
    }
}
