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
    enum EditMode
    {
        Grid,
        Ledge
    }

    class CollisionComponent
    {
        public Texture2D pixel;
        public Color bgColor = Color.Black * 0.5f;
        public SpriteFont font;
        public float fontHeight;
        public List<Ledge> ledges;
        public int[,] grid;
        private EditMode mode = EditMode.Ledge;
        private int selectedLedge = -1;


        public CollisionComponent Load(ContentManager content, GraphicsDevice graphics)
        {
            pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            font = content.Load<SpriteFont>(@"fonts/font");
            fontHeight = font.MeasureString("HEIGHT").Y;

            return this;
        }

        public CollisionComponent Set(List<Ledge> ledges, int[,] grid)
        {
            this.ledges = ledges;
            this.grid = grid;
            return this;
        }

        MouseState om, m;
        KeyboardState key, oldkey;
        public void Update(float dt, Game1 game)
        {
            om = m;
            m = Mouse.GetState();
            oldkey = key;
            key = Keyboard.GetState();

            switch (mode)
            {
                case EditMode.Grid:
                    break;
                case EditMode.Ledge:
                    if (selectedLedge != -1)
                    {
                        if (om.LeftButton == ButtonState.Released && m.LeftButton == ButtonState.Pressed)
                        {
                            ledges[selectedLedge].points.Add(new Vector2(game.MPos.X, game.MPos.Y));
                        }
                        if (om.RightButton == ButtonState.Released && m.RightButton == ButtonState.Pressed)
                        {
                            if (ledges[selectedLedge].points.Count > 0)
                            {
                                ledges[selectedLedge].points.RemoveAt(ledges[selectedLedge].points.Count - 1);
                                if (ledges[selectedLedge].points.Count == 0)
                                {
                                    ledges.RemoveAt(selectedLedge);
                                    selectedLedge = -1;
                                }
                            }
                        }
                        if (oldkey.IsKeyUp(Keys.Space) && key.IsKeyDown(Keys.Space))
                        {
                            ledges[selectedLedge].solid = !ledges[selectedLedge].solid;
                        }

                        if (oldkey.IsKeyUp(Keys.A) && key.IsKeyDown(Keys.A))
                        {
                            selectedLedge--;
                            if (selectedLedge < 0)
                            {
                                selectedLedge = ledges.Count - 1;
                            }
                        }
                        if (oldkey.IsKeyUp(Keys.D) && key.IsKeyDown(Keys.D))
                        {
                            selectedLedge++;
                            if (selectedLedge > ledges.Count - 1)
                            {
                                selectedLedge = 0;
                            }
                        }

                        if (key.IsKeyDown(Keys.Escape))
                        {
                            selectedLedge = -1;
                        }
                    }
                    else
                    {

                        if (key.IsKeyDown(Keys.LeftControl))
                        {
                            if (oldkey.IsKeyUp(Keys.A) && key.IsKeyDown(Keys.A))
                            {
                                ledges.Add(new Ledge());
                                selectedLedge = ledges.Count - 1;
                            }
                        }
                        else if (key.IsKeyDown(Keys.A) || key.IsKeyDown(Keys.D))
                        {
                            if (ledges.Count > 0)
                            {
                                selectedLedge = 0;
                            }
                        }
                    }
                    break;
            }
            
        }

        public void Draw(SpriteBatch sb, Game1 game)
        {
            for (int i = 0; i < ledges.Count; i++)
            {
                ledges[i].Draw(sb, pixel, selectedLedge == i);
            }

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] != 0)
                    {
                        sb.Draw(pixel, new Rectangle(i * Map.CELL_SIZE, j * Map.CELL_SIZE, Map.CELL_SIZE, Map.CELL_SIZE), Color.Red * 0.2f);
                    }
                    else
                    {
                        sb.DrawOutline(pixel, new Rectangle(i * Map.CELL_SIZE, j * Map.CELL_SIZE, Map.CELL_SIZE, Map.CELL_SIZE), Color.Red * 0.2f);
                    }
                }
            }
        }
    }
}