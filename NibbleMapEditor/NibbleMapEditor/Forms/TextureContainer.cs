using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NibbleMapEditor.Forms
{

    class TextureContainer
    {
        private Rectangle rect;
        public Texture2D texture;
        private Texture2D pixel;
        private string asset;
        public event OnNewSourceDelegate OnNewSource;
        public delegate void OnNewSourceDelegate(Rectangle source);
        private Rectangle source;

        public TextureContainer(int x, int y, string asset)
        {
            this.asset = asset;
            rect = new Rectangle(x, y, 0, 0);
        }

        public TextureContainer Load(ContentManager content, GraphicsDevice graphics)
        {
            texture = content.Load<Texture2D>(asset);
            rect.Width = texture.Width;
            rect.Height = texture.Height;

            pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            return this;
        }

        MouseState om, m;
        KeyboardState k;
        public void Update(float dt)
        {
            om = m;
            m = Mouse.GetState();
            k = Keyboard.GetState();

            if (k.IsKeyDown(Keys.Tab))
            {
                if (rect.Contains(m.X, m.Y))
                {
                    if (m.RightButton == ButtonState.Pressed)
                    {
                        rect.X += (int)((m.X - om.X));
                        rect.Y += (int)((m.Y - om.Y));
                    }
                    if (m.LeftButton == ButtonState.Pressed || om.LeftButton == ButtonState.Pressed)
                    {
                        if (m.LeftButton == ButtonState.Pressed && om.LeftButton == ButtonState.Released)
                        {
                            // start
                            source.X = m.X - rect.X;
                            source.Y = m.Y - rect.Y;
                        }
                        else if (m.LeftButton == ButtonState.Pressed && om.LeftButton == ButtonState.Pressed)
                        {
                            source.Width = (m.X - rect.X) - source.X;
                            source.Height = (m.Y - rect.Y) - source.Y;
                        }
                        else if (m.LeftButton == ButtonState.Released && om.LeftButton == ButtonState.Pressed)
                        {
                            if (OnNewSource != null)
                            {
                                OnNewSource(source);
                            }
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (k.IsKeyDown(Keys.Tab))
            {
                sb.Draw(pixel, rect, Color.Black * 0.2f);
                sb.DrawOutline(pixel, rect, Color.Black);
                sb.Draw(texture, rect, Color.White * 0.8f);
                if (rect.Contains(m.X, m.Y))
                {
                    if (m.LeftButton == ButtonState.Pressed || om.LeftButton == ButtonState.Pressed)
                    {
                        sb.DrawOutline(pixel, new Rectangle(rect.X + source.X, rect.Y + source.Y, source.Width, source.Height), Color.Red);
                    }
                }
            }
        }
    }
}