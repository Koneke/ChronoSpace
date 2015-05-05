using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChronoSpace
{
    public class Sprite
    {
        public Texture2D Texture;
        public Vector2 Origin;

        public Sprite(Texture2D texture, bool center = false)
        {
            Texture = texture;

            if (center)
                CenterOrigin();
        }

        public void CenterOrigin()
        {
            Origin = new Vector2(
                Texture.Width / 2f,
                Texture.Height / 2f
            );
        }

        public void Draw(
            SpriteBatch spritebatch,
            Vector2 position
        ) {
            spritebatch.Draw(
                Texture,
                position,
                null,
                Color.White,
                0f,
                Origin,
                new Vector2(1, 1),
                SpriteEffects.None,
                0
            );
        }
    }

    public class BulletTemplate
    {
        public string Texture;
        public Vector2 Position;
        public float Speed;
        public float Radius;

        public BulletTemplate(
            string s,
            float sp,
            float r = 16
        ) {
            Texture = s;
            Speed = sp;
            Radius = r;
        }

        public Bullet Instantiate(
            Gun g
        ) {
            bool playerBullet = g.Owner == Game1.Player;

            return new Bullet(
                (Ship)g.Owner,
                Texture,
                g.Position,
                new Vector2(
                    (float)Math.Cos(g.Direction),
                    (float)Math.Sin(g.Direction)
                ) *
                (playerBullet
                    ? Speed * 4
                    : Speed),
                Radius
            );
        }
    }

    public class Bullet
    {
        public Ship Owner;
        public string Texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public float Radius;

        public Bullet(
            Ship owner,
            string s,
            Vector2 p,
            Vector2 v,
            float r = 16
        ) {
            Owner = owner;
            Texture = s;
            Position = p;
            Velocity = v;
            Radius = r;
        }

        private float particleSpawntimer = 0;

        public void Update(GameTime gameTime)
        {
            particleSpawntimer -=
                gameTime.ElapsedGameTime.Milliseconds
                * Game1.Slomo
                / 1000f;
            if (particleSpawntimer <= 0)
            {
                new Particle(Game1.Game, Position + new Vector2(8), 0.4f);
                particleSpawntimer = 1 / 180f;
            }

            Position +=
                Velocity
                * Game1.Slomo
                * gameTime.ElapsedGameTime.Milliseconds
                / 1000f
            ;
        }
    }
}
