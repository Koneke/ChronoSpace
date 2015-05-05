using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChronoSpace
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;

        public static fbEngine engine;
        private List<Bullet> bullets;
        private List<Ship> enemies; 

        public Dictionary<string, Sprite> BulletSprites;

        private fbRectangle gameArea;

        public static Ship Player;

        public static float Slomo = 1f;
        private bool doSlomo;
        private float slomoDebt;
        private float bombTimer;
        private const float bombCost = 2500f; //s
        private const float bombLength = 5000f; //s

        public static float Volume = 0.2f;

        public Game1()
        {
            //this is completely and utterly illegal, but fffffffffukc you
            Game = this;

            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            engine = new fbEngine(this);
        }

        private Dictionary<string, BulletTemplate> templates;

        public List<Particle> Particles;
        public static Game1 Game;

        public void AddTemplate(string name, BulletTemplate bt)
        {
            templates.Add(name.ToLower(), bt);
        }

        public BulletTemplate GetTemplate(string name)
        {
            return templates[name.ToLower()];
        }

        public BulletTemplate GetRandom()
        {
            return templates[templates.Keys.ToList()[
                random.Next(0, templates.Count)
            ]];
        }

        private void setupBullets()
        {
            templates = new Dictionary<string, BulletTemplate>();

            AddTemplate(
                "red",
                new BulletTemplate("bullet-red", 250)
            );

            AddTemplate(
                "blu",
                new BulletTemplate("bullet-blue", 200)
            );

            AddTemplate(
                "yel",
                new BulletTemplate("bullet-yellow", 150)
            );
        }

        protected void Reset()
        {
            bullets = new List<Bullet>();
            enemies = new List<Ship>();
            random = new Random();
            Particles = new List<Particle>();

            spawnLength = 1.5f;

            Player.Dead = false;
            Player.Position =
                new Vector2(
                    engine.GetSize().X / 2,
                    engine.GetSize().Y - 40
                );

            slomoDebt = 0f;
            bombTimer = 0f;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Graphics.PreferredBackBufferWidth = 360;
            Graphics.PreferredBackBufferHeight = 500;
            Graphics.ApplyChanges();

            setupBullets();
            Enemy.Setup();

            Player = new Ship(
                "player",
                new Vector2(
                    engine.GetSize().X / 2,
                    engine.GetSize().Y - 40
                )
            );

            Gun g = new Gun();
            g.Direction = (float)(-Math.PI / 2f);
            Player.AddGun(g);

            g
                .Add(new Fire(this, "random"))
                .Add(new Delay(this, 0.02f))
            ;

            Reset();

            Player.StartGuns();

            gameArea = new fbRectangle(
                new Vector2(-30),
                engine.GetSize() + new Vector2(30)
            );
        }

        protected override void UnloadContent()
        {
            foreach (string s in engine.Afx.Keys.ToList())
                engine.Afx[s].Dispose();

            foreach (string s in engine.Textures.Keys.ToList())
                engine.Textures[s].Dispose();

            base.UnloadContent();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (string f in Directory.GetFiles("gfx/"))
            {
                string fname = f
                    .Substring(4, f.Length - 4)
                    .Substring(0, f.Length - 8);
                engine.LoadTexture(fname, f);
            }

            engine.LoadSound("pew", "afx/pew.wav");
            engine.LoadSound("pow", "afx/pow.wav");

            engine.DefaultFont = new Font();
            engine.DefaultFont.FontSheet = engine.GetTexture("font");
            engine.DefaultFont.CharSize = new Vector2(8);
        }

        private float spawnTimer;
        private float spawnLength;
        public Random random;

        protected override void Update(GameTime gameTime)
        {
            flash = false;
            engine.Update();

            if (engine.KeyPressed(Keys.R))
                Reset();

            Slomo = 1f;

            if (
                (engine.KeyPressed(Keys.LeftShift) ||
                engine.KeyPressed(Keys.Z))
                && slomoDebt < 500
            )
                doSlomo = true;
            if (
                (engine.KeyReleased(Keys.LeftShift) ||
                engine.KeyReleased(Keys.Z)))
                doSlomo = false;

            if(doSlomo && slomoDebt < 1500)
            {
                slomoDebt += gameTime.ElapsedGameTime.Milliseconds * 1.5f;
                Slomo = 0.5f;
            }
            else if (slomoDebt > 0)
            {
                doSlomo = false;
                Slomo = 1.5f;
                slomoDebt -= gameTime.ElapsedGameTime.Milliseconds;
                if (slomoDebt < 0) slomoDebt = 0;
            }

            if (
                (
                engine.KeyPressed(Keys.Space) ||
                engine.KeyPressed(Keys.X)
                )&&
                slomoDebt == 0 && bombTimer == 0
            ) {
                foreach (Ship s in enemies)
                    s.Die(this);
                slomoDebt = bombCost;
                bombTimer = bombLength;

                flash = true;

                float p =
                    ((float)random.NextDouble() - 0.5f) * 0.1f;

                float pa =
                    ((float)random.NextDouble() - 0.5f) * 0.1f;

                engine.Get("pow")
                    .Play(
                        Volume,
                        p * Slomo,
                        pa
                    );

            }

            bombTimer -= gameTime.ElapsedGameTime.Milliseconds;
            if (bombTimer < 0) bombTimer = 0;

            if (!Player.Dead)
            {
                Player.Velocity = new Vector2(0);

                float speed = 340;
                if (
                    engine.KeyDown(Keys.Left) ||
                    engine.KeyDown(Keys.A) ||
                    engine.KeyDown(Keys.J)
                )
                    Player.Velocity += new Vector2(-1, 0);
                if (
                    engine.KeyDown(Keys.Right) ||
                    engine.KeyDown(Keys.D) ||
                    engine.KeyDown(Keys.L)
                )
                    Player.Velocity += new Vector2(1, 0);
                if (
                    engine.KeyDown(Keys.Up) ||
                    engine.KeyDown(Keys.W) ||
                    engine.KeyDown(Keys.I)
                )
                    Player.Velocity += new Vector2(0, -1);
                if (
                    engine.KeyDown(Keys.Down) ||
                    engine.KeyDown(Keys.S) ||
                    engine.KeyDown(Keys.K)
                )
                    Player.Velocity += new Vector2(0, 1);

                Player.Velocity *= speed;
            }

            if (!Player.Dead)
                Player.Update(gameTime);

            Player.Position.X =
                Player.Position.X.Clamp(0, Game1.engine.GetSize().X - 16);
            Player.Position.Y =
                Player.Position.Y.Clamp(0, Game1.engine.GetSize().Y - 16);


            if (bullets
                .Any(
                    b =>
                        b.Owner != Player &&
                        Vector2.Distance(b.Position, Player.Position) < 16f)
                && !Player.Dead)
            {
                Player.Die(this);
            }

            foreach (Ship s in enemies)
            {
                s.Update(gameTime);
                if (bullets
                    .Any(
                        b =>
                            b.Owner == Player &&
                            Vector2.Distance(b.Position, s.Position) < 16f)
                    )
                {
                    s.Die(this);
                }
            }

            enemies =
                enemies
                    .Where(e => gameArea.Contains(e.Position))
                    .Where(e => !e.Dead)
                .ToList();

            foreach (Bullet b in bullets)
                b.Update(gameTime);

            bullets =
                bullets.Where(b => gameArea.Contains(b.Position))
                .ToList();

            foreach (Particle p in Particles)
                p.Update(gameTime);

            Particles =
                Particles.Where(p => p.Life > 0).ToList();

            if (spawnTimer > 0)
                spawnTimer -=
                    gameTime.ElapsedGameTime.Milliseconds
                    * Slomo
                    / 1000f;

            if (spawnTimer <= 0)
            {
                enemies.Add(
                    Enemy.GetRandom().Spawn(
                        this,
                        new Vector2(
                            random.Next(30, (int)(engine.GetSize().X - 30)),
                            -30
                        )
                    )
                );

                spawnTimer += spawnLength;
                if(!Player.Dead)
                    spawnLength -= 0.01f;
            }

            if (engine.KeyDown(Keys.Escape))
                engine.Exit();

            base.Update(gameTime);
        }

        public bool flash;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(
                flash
                    ? Color.White
                    : Color.Black
            );

            if (!Player.Dead)
                new DrawCall(
                    engine.GetTexture(Player.Texture),
                    new fbRectangle(Player.Position, new Vector2(-1))
                        .Center(),
                    -1
                ).Draw(engine);

            foreach (Ship s in enemies)
                new DrawCall(
                    engine.GetTexture(s.Texture),
                    new fbRectangle(s.Position, new Vector2(-1))
                        .Center(),
                    -1
                ).Draw(engine);

            foreach (Bullet b in bullets)
                new DrawCall(
                    engine.GetTexture(b.Texture),
                    new fbRectangle(b.Position, new Vector2(-1))
                        .Center()
                ).Draw(engine);

            foreach (Particle p in Particles)
                new DrawCall(
                    engine.GetTexture("particle"),
                    new fbRectangle(p.Position, new Vector2(-1))
                        .Scale(p.Scale)
                        .Center(),
                    1,
                    p.Color
                ).Draw(engine);

            new TextCall(
                "ease: " + Math.Round(spawnLength, 2),
                engine.DefaultFont,
                new Vector2(0, 0)
            ).Draw(engine);

            string slomoString =
                "debt: " + slomoDebt;
            if (slomoDebt < 500)
                slomoString += " SLOMO READY!";

            new TextCall(
                slomoString,
                engine.DefaultFont,
                new Vector2(0, 10)
            ).Draw(engine);

            string bombString =
                "bomb in: " + bombTimer;
            if (bombTimer == 0 && slomoDebt == 0)
                bombString = "BOMB READY!";

            new TextCall(
                bombString,
                engine.DefaultFont,
                new Vector2(0, 20)
            ).Draw(engine);

            new TextCall(
                "@lhkoneke",
                engine.DefaultFont,
                engine.GetSize() - new Vector2(0, 10)
            ).RightAlign().Draw(engine);

            /*new TextCall(
                enemies.Count.ToString(),
                engine.DefaultFont,
                new Vector2(engine.GetSize().X, 0)
            ).RightAlign().Draw(engine);

            new TextCall(
                bullets.Count.ToString(),
                engine.DefaultFont,
                new Vector2(engine.GetSize().X, 10)
            ).RightAlign().Draw(engine);

            new TextCall(
                Particles.Count.ToString(),
                engine.DefaultFont,
                new Vector2(engine.GetSize().X, 20)
            ).RightAlign().Draw(engine);*/

            engine.Render();

            base.Draw(gameTime);
        }

        public void SpawnBullet(Bullet b)
        {
            bullets.Add(b);
        }
    }
}
