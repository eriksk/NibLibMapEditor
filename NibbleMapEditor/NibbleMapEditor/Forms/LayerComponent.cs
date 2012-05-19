using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NibLib.Maps;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using NibbleMapEditor.Utils;

namespace NibbleMapEditor.Forms
{
    class LayerComponent
    {
        public Texture2D pixel;
        public Color bgColor = Color.Black * 0.5f;
        public SpriteFont font;
        public float fontHeight;
        private MapLayer layer;
        private int selectedPart = -1;


        public LayerComponent Load(ContentManager content, GraphicsDevice graphics)
        {
            pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            font = content.Load<SpriteFont>(@"fonts/font");
            fontHeight = font.MeasureString("HEIGHT").Y;

            return this;
        }

        public void SetLayer(MapLayer layer)
        {
            this.layer = layer;
            if (layer != null)
            {
                if (layer.parts.Count == 0)
                {
                    layer.parts.Add(new MapPart(Vector2.Zero, 0f, 1f) { Source = new Rectangle(0, 0, 64, 64) });
                }
            }
            else
            {
                selectedPart = -1;
            }
        }

        public void SetSource(Rectangle source)
        {
            if (selectedPart != -1)
            {
                layer.parts[selectedPart].Source = source;
            }
        }

        MouseState om, m;
        KeyboardState key, oldkey;
        public void Update(float dt, Game1 game)
        {
            om = m;
            m = Mouse.GetState();
            oldkey = key;
            key = Keyboard.GetState();


            if (layer != null && key.IsKeyUp(Keys.Tab))
            {
                if (selectedPart != -1 && key.IsKeyDown(Keys.LeftShift))
                {
                    if (m.LeftButton == ButtonState.Pressed)
                    {
                        if (key.IsKeyDown(Keys.LeftControl))
                        {
                            // move all
                            for (int i = 0; i < layer.parts.Count; i++)
                            {
                                layer.parts[i].position += new Vector2(m.X - om.X, m.Y - om.Y);
                            }
                        }
                        else
                        {
                            // move
                            layer.parts[selectedPart].position += new Vector2(m.X - om.X, m.Y - om.Y);
                        }
                    }
                    if(m.RightButton == ButtonState.Pressed)
                    {
                        // rotate
                        layer.parts[selectedPart].rotation += (m.Y - om.Y) * 0.005f;                    
                    }
                    if (m.MiddleButton == ButtonState.Pressed)
                    {
                        // scale
                        layer.parts[selectedPart].scale += (m.Y - om.Y) * 0.005f;
                    }
                    if (key.IsKeyDown(Keys.LeftControl))
                    {
                        if (key.IsKeyDown(Keys.NumPad1))
                        {
                            layer.parts[selectedPart].scale = 1f;
                        }
                        if (key.IsKeyDown(Keys.NumPad0))
                        {
                            layer.parts[selectedPart].rotation = 0f;
                        }
                        if (key.IsKeyDown(Keys.Delete))
                        {
                            layer.parts.RemoveAt(selectedPart);
                            selectedPart = -1;
                        }
                        if (key.IsKeyDown(Keys.A) && oldkey.IsKeyUp(Keys.A))
                        {
                            layer.parts.Add(new MapPart(Vector2.Zero, 0f, 1f));
                            selectedPart = layer.parts.Count - 1;
                        }
                        if (key.IsKeyDown(Keys.D) && oldkey.IsKeyUp(Keys.D))
                        {
                            if (selectedPart != -1)
                            {
                                layer.parts.Add(layer.parts[selectedPart].Clone());
                                selectedPart = layer.parts.Count - 1;
                            }
                        }
                    }
                    if (key.IsKeyDown(Keys.Escape))
                    {
                        selectedPart = -1;
                    }
                }
                else // not selected
                {
                    if (m.LeftButton == ButtonState.Pressed)
                    {
                        for (int i = layer.parts.Count - 1; i >= 0; i--)
                        {
                            MapPart fp = layer.parts[i];
                            if (Util.RotateRectangle(fp.origin, fp.position, fp.scale, fp.rotation).Contains(game.MPos.X, game.MPos.Y))
                            {
                                selectedPart = i;
                                break;
                            }
                        }
                    }
                }


                // always
                if (m.ScrollWheelValue != om.ScrollWheelValue)
                {
                    if (key.IsKeyDown(Keys.LeftControl))
                    {
                        if (layer.parts.Count > 0)
                        {
                            int target = selectedPart + (m.ScrollWheelValue - om.ScrollWheelValue) / 100;
                            if (target > layer.parts.Count - 1)
                            {
                                target = 0;
                            }
                            else if (target < 0)
                            {
                                target = layer.parts.Count - 1;
                            }

                            // swap
                            MapPart temp = layer.parts[selectedPart];
                            layer.parts[selectedPart] = layer.parts[target];
                            layer.parts[target] = temp;

                            selectedPart = target;
                        }
                    }
                    else
                    {
                        if (layer.parts.Count > 0)
                        {
                            selectedPart += (m.ScrollWheelValue - om.ScrollWheelValue) / 100;
                            if (selectedPart > layer.parts.Count - 1)
                            {
                                selectedPart = 0;
                            }
                            else if (selectedPart < 0)
                            {
                                selectedPart = layer.parts.Count - 1;
                            }
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb, Game1 game)
        {
            if (layer != null)
            {
                for (int i = 0; i < layer.parts.Count; i++)
                {
                    if (selectedPart == i)
                    {
                        MapPart fp = layer.parts[selectedPart];
                        sb.DrawOutline(pixel, Util.RotateRectangle(fp.origin, fp.position, fp.scale, fp.rotation), Color.White * 0.5f);
                    }
                }
                if (selectedPart == -1)
                {
                    for (int i = layer.parts.Count - 1; i >= 0; i--)
                    {
                        MapPart fp = layer.parts[i];
                        if (Util.RotateRectangle(fp.origin, fp.position, fp.scale, fp.rotation).Contains(game.MPos.X, game.MPos.Y))
                        {
                            sb.Draw(pixel, Util.RotateRectangle(fp.origin, fp.position, fp.scale, fp.rotation), Color.Red * 0.1f);
                            sb.DrawOutline(pixel, Util.RotateRectangle(fp.origin, fp.position, fp.scale, fp.rotation), Color.Red * 0.8f);
                            break;
                        }
                    }
                }
            }
        }
    }
}