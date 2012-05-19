using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NibLib.Maps;
using NibbleMapEditor.Forms;
using NibLib.IO;
using NibLib.Cameras;

namespace NibbleMapEditor
{
    enum EditMode
    {
        Parts,
        Collision
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Map map;

        ListComponent<MapLayer> layerList;
        List<Button> buttons;

        Texture2D pixel;
        TextureContainer texContainer;
        LayerComponent layerCompo;
        CollisionComponent collisionCompo;

        EditMode mode = EditMode.Parts;

        Camera2D cam;
        SpriteFont font;

        string path = @"C:\Users\Erik\Desktop\testmap.map";

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            map = new Map(new List<MapLayer>(), new MapSettings(), new List<Ledge>(), new int[64,64]);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            font = Content.Load<SpriteFont>(@"fonts/font");

            cam = new Camera2D(new Vector2(1280, 720) / 2f);
            cam.speed = 0.2f;

            map.Load(Content);
            
            layerList = new ListComponent<MapLayer>(16, 46, "Layers", map.layers).Load(Content, GraphicsDevice);
            layerList.OnClickedEvent += new ListComponent<MapLayer>.OnClicked(layerList_OnClickedEvent);

            texContainer = new TextureContainer(500, 200, @"gfx/map").Load(Content, GraphicsDevice);
            texContainer.OnNewSource += new TextureContainer.OnNewSourceDelegate(texContainer_OnNewSource);

            layerCompo = new LayerComponent().Load(Content, GraphicsDevice);

            collisionCompo = new CollisionComponent().Load(Content, GraphicsDevice);
            collisionCompo.Set(map.ledges, map.grid);

            buttons = new List<Button>();

            Button btnNewLayer = new Button(16, 16, "New layer").Load(Content, GraphicsDevice);
            btnNewLayer.OnClickedEvent += new Button.OnClicked(btnNewLayer_OnClickedEvent);
            buttons.Add(btnNewLayer);

            Button btnRmLayer = new Button(100, 16, "Delete layer").Load(Content, GraphicsDevice);
            btnRmLayer.OnClickedEvent += new Button.OnClicked(btnRmLayer_OnClickedEvent);
            buttons.Add(btnRmLayer);

            Button btnSave = new Button(1280 - 300, 16, "Save").Load(Content, GraphicsDevice);
            btnSave.OnClickedEvent += new Button.OnClicked(btnSave_OnClickedEvent);
            buttons.Add(btnSave);

            Button btnLoad = new Button(1280 - 150, 16, "Load").Load(Content, GraphicsDevice);
            btnLoad.OnClickedEvent += new Button.OnClicked(btnLoad_OnClickedEvent);
            buttons.Add(btnLoad);

        }

        void btnLoad_OnClickedEvent()
        {
            map = MapIO.Load(path);
            LoadContent();
        }

        void btnSave_OnClickedEvent()
        {
            MapIO.Save(map, path);
        }

        void texContainer_OnNewSource(Rectangle source)
        {
            layerCompo.SetSource(source);
        }

        void btnRmLayer_OnClickedEvent()
        {
            layerList.DeleteSelected();
            layerCompo.SetLayer(null);
        }

        void btnNewLayer_OnClickedEvent()
        {
            MapLayer ml = new MapLayer(new List<MapPart>(), 1f) { parallax = 1.0f, Name = "Layer" };
            ml.Load(Content);
            layerList.AddNew(ml);
        }

        void layerList_OnClickedEvent(MapLayer item)
        {
            layerCompo.SetLayer(item);
        }

        public Point MPos
        {
            get 
            { 
                return new Point(
                    (int)(m.X + cam.position.X - cam.origin.X),
                    (int)(m.Y + cam.position.Y - cam.origin.Y));
            }
        }

        MouseState m, om;
        KeyboardState key;
        protected override void Update(GameTime gameTime)
        {
            om = m;
            m = Mouse.GetState();
            key = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (mode == EditMode.Parts && key.IsKeyDown(Keys.D1))
            {
                mode = EditMode.Collision;
            }
            if (mode == EditMode.Collision && key.IsKeyDown(Keys.D2))
            {
                mode = EditMode.Parts;
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            map.Update(dt);

            if (mode == EditMode.Parts)
            {
                layerList.Update(dt);
                texContainer.Update(dt);
                layerCompo.Update(dt, this);
            }
            else if (mode == EditMode.Collision)
            {
                collisionCompo.Update(dt, this);
                foreach (Button b in buttons)
                {
                    b.Update();
                }
            }

            if (m.MiddleButton == ButtonState.Pressed && key.IsKeyUp(Keys.LeftShift))
            {
                cam.Move(-new Vector2(m.X - om.X, m.Y - om.Y));
            }
            cam.Update(dt);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, cam.Matrix);
            for (int i = 0; i < map.layers.Count; i++)
			{
                map.DrawLayer(spriteBatch, i);			 
			}

            collisionCompo.Draw(spriteBatch, this);
            layerCompo.Draw(spriteBatch, this); 
            spriteBatch.Draw(pixel, new Rectangle(MPos.X, MPos.Y, 8, 8), Color.Red);
            spriteBatch.End();
            
            // GUI
            
            spriteBatch.Begin();

            foreach (Button b in buttons)
            {
                b.Draw(spriteBatch);
            }
            layerList.Draw(spriteBatch);
            texContainer.Draw(spriteBatch);

            spriteBatch.DrawString(font, mode.ToString(), new Vector2(5, 5), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
