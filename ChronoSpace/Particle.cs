using System;
using Microsoft.Xna.Framework;

namespace ChronoSpace
{
    public class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Life;
        public float Scale;

        public Particle(Game1 game, Vector2 p, float scaleAll = 1f)
        {
            Position = p;
            float d = (float)(Math.PI * 2 * game.random.NextDouble());
            Velocity = new Vector2(
                (float)Math.Cos(d),
                (float)Math.Sin(d)
            ) * game.random.Next(100, 600);
            Color = new Color(
                (float)game.random.NextDouble(),
                (float)game.random.NextDouble(),
                (float)game.random.NextDouble(),
                1f
            );
            Life = (float)(game.random.NextDouble() * 0.5f + 0.25f);
            Scale = 2 + (float)(game.random.NextDouble()) * 3f;

            Velocity *= scaleAll;
            Life *= scaleAll;
            Scale *= scaleAll;

            game.Particles.Add(this);
        }

        public void Update(GameTime gameTime)
        {
            Life -=
                gameTime.ElapsedGameTime.Milliseconds
                * Game1.Slomo
                / 1000f;
            Position += Velocity
                * Game1.Slomo
                * gameTime.ElapsedGameTime.Milliseconds
                / 1000f;
        }
    }
}
