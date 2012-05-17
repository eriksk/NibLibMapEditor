using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NibLib.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace NibbleMapEditor.Forms
{
    class ListComponent<T> where T : INameable
    {
        public event OnClicked OnClickedEvent;
        public delegate void OnClicked(T item);
        public event OnClicked OnDoubleClickedEvent;
        public delegate void OnDoubleClicked(T item);
        public Rectangle rect;
        public Texture2D pixel;
        public Color bgColor = Color.Black * 0.5f;
        public List<T> items;
        public SpriteFont font;
        public float padding = 8;
        public float fontHeight;
        public int selected;
        public string name;

        public ListComponent(int x, int y, string name, List<T> items)
        {
            rect = new Rectangle(x, y, 256, 512);
            this.name = name;
            this.items = items;
            selected = -1;
        }

        public ListComponent<T> Load(ContentManager content, GraphicsDevice graphics)
        {
            pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            font = content.Load<SpriteFont>(@"fonts/font");
            fontHeight = font.MeasureString("HEIGHT").Y;

            return this;
        }

        public T GetSelected()
        {
            return items[selected];
        }

        public void DeleteSelected()
        {
            if (selected != -1)
            {
                items.RemoveAt(selected);
                selected = items.Count > 0 ? 0 : -1;
            }
        }

        public void AddNew(T item)
        {
            items.Add(item);
            selected = items.Count - 1;
            if (OnClickedEvent != null)
            {
                OnClickedEvent(item);
            }
        }

        MouseState om, m;
        public void Update(float dt)
        {
            om = m;
            m = Mouse.GetState();
            rect.Height = (int)((items.Count * fontHeight) + padding * 4f);

            if (rect.Contains(m.X, m.Y))
            {
                if (m.RightButton == ButtonState.Pressed)
                {
                    rect.X += (int)((m.X - om.X));
                    rect.Y += (int)((m.Y - om.Y));
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                Vector2 vec = new Vector2(rect.X + padding, rect.Y + padding + (i + 1) * fontHeight);
                Rectangle area = new Rectangle((int)vec.X, (int)vec.Y, (int)(rect.Width - (padding * 2f)), (int)fontHeight);
                if (area.Contains(m.X, m.Y))
                {
                    if (om.LeftButton == ButtonState.Released && m.LeftButton == ButtonState.Pressed)
                    {
                        if (selected == i)
                        {
                            if (OnDoubleClickedEvent != null)
                            {
                                OnDoubleClickedEvent(items[i]);
                            }
                        }
                        else if (OnClickedEvent != null)
                        {
                            OnClickedEvent(items[i]);
                        }
                        selected = i;
                        break;
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(pixel, rect, bgColor);
            sb.DrawOutline(pixel, rect, Color.Black);
            sb.DrawString(font, name, new Vector2(rect.X + padding, rect.Y + padding), Color.White);
            for (int i = 0; i < items.Count; i++)
            {
                Vector2 vec = new Vector2(
                    rect.X + padding,
                    rect.Y + padding + (i + 1) * fontHeight);
                Rectangle area = new Rectangle(
                    (int)vec.X,
                    (int)vec.Y,
                    (int)(rect.Width - (padding * 2f)),
                    (int)fontHeight);
                if (area.Contains(m.X, m.Y) || selected == i)
                {
                    sb.Draw(pixel, area, Color.White * 0.5f);
                }
                sb.DrawString(
                    font,
                    1 + i + ". " + items[i].Name,
                    vec,
                    Color.White);
            }
        }
    }
}
