using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ChronoSpace
{
    public abstract class Entity
    {
        public string Texture;
        public Vector2 Position;

        protected Entity(string s, Vector2 p)
        {
            Texture = s;
            Position = p;
        }

        public abstract void Update(GameTime gameTime);
    }

    public class Ship : Entity
    {
        public Vector2 Velocity;
        public List<Gun> Guns; 

        public Ship(string s, Vector2 p) : base(s, p)
        {
            Guns = new List<Gun>();
            Dead = false;
        }

        public bool Dead;

        public void AddGun(Gun g)
        {
            Guns.Add(g);
            g.Owner = this;
        }

        public override void Update(GameTime gameTime)
        {
            Position += Velocity
                * Game1.Slomo
                * gameTime.ElapsedGameTime.Milliseconds
                / 1000f;

            foreach (Gun g in Guns)
                g.Update(gameTime);
        }

        public void StartGuns()
        {
            foreach (Gun g in Guns)
                g.Start();
        }

        public void Die(Game1 game)
        {
            Dead = true;
            float p =
                ((float)game.random.NextDouble() - 0.5f) * 0.1f;

            float pa =
                ((float)game.random.NextDouble() - 0.5f) * 0.1f;

            for (int i = 0; i < 50; i++)
                new Particle(game, Position);

            Game1.engine.Get("pow")
                .Play(
                    Game1.Volume,
                    p * Game1.Slomo,
                    pa
                );

            game.flash = true;
        }
    }
}
