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

namespace NibbleMapEditor
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Map map;
        List<MapLayer> layers = new List<MapLayer>();
        MapLayer selectedLayer;
        List<Ledge> ledges = new List<Ledge>();
        MapSettings settings = new MapSettings();
        int[,] grid = new int[64, 64];

        ListComponent<MapLayer> layerList;
        List<Button> buttons;

        Texture2D mapTexture;
        Texture2D pixel;

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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            map = new Map(layers, settings, ledges, grid);
            mapTexture = Content.Load<Texture2D>(@"gfx/map");
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });


            layerList = new ListComponent<MapLayer>(16, 46, "Layers", layers).Load(Content, GraphicsDevice);
            layerList.OnClickedEvent += new ListComponent<MapLayer>.OnClicked(layerList_OnClickedEvent);

            buttons = new List<Button>();

            Button btnNewLayer = new Button(16, 16, "New layer").Load(Content, GraphicsDevice);
            btnNewLayer.OnClickedEvent += new Button.OnClicked(btnNewLayer_OnClickedEvent);
            buttons.Add(btnNewLayer);

            Button btnRmLayer = new Button(100, 16, "Delete layer").Load(Content, GraphicsDevice);
            btnRmLayer.OnClickedEvent += new Button.OnClicked(btnRmLayer_OnClickedEvent);
            buttons.Add(btnRmLayer);

        }

        void btnRmLayer_OnClickedEvent()
        {
            layerList.DeleteSelected();
        }

        void btnNewLayer_OnClickedEvent()
        {
            layerList.AddNew(new MapLayer() { parallax = 1.0f, Name = "Layer" });
        }

        void layerList_OnClickedEvent(MapLayer item)
        {
            selectedLayer = item;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            map.Update(dt);
            layerList.Update(dt);
            foreach (Button b in buttons)
            {
                b.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            for (int i = 0; i < map.layers.Count; i++)
			{
                map.DrawLayer(spriteBatch, i);			 
			}

            foreach (Button b in buttons)
            {
                b.Draw(spriteBatch);
            }
            layerList.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
