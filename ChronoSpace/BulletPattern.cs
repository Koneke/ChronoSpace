using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ChronoSpace
{
    //eg "rotate cannon", "fire"
    public abstract class PatternAction
    {
        protected Game1 game;
        protected Gun gun;

        protected PatternAction(
            Game1 game
        ) {
            this.game = game;
        }

        public PatternAction SetGun(Gun g) { gun = g; return this; }

        public abstract void Start();
        public abstract void Update(GameTime gameTime);
        public abstract bool IsFinished();
    }

    class Delay : PatternAction
    {
        private float timer;
        private float timerLength;

        public Delay(Game1 game, float length) : base(game)
        {
            timerLength = length;
        }

        public override void Start()
        {
            timer = timerLength;
        }

        public override void Update(GameTime gameTime)
        {
            if (timer > 0)
                timer -=
                    gameTime.ElapsedGameTime.Milliseconds
                    * Game1.Slomo
                    / 1000f;
        }

        public override bool IsFinished()
        {
            return timer <= 0;
        }
    }

    public class Fire : PatternAction
    {
        //private BulletTemplate template;
        private string template;
        private bool hasFired;

        public Fire(
            Game1 game,
            //BulletTemplate template
            string templ
        ) : base(game) {
            //this.template = template;
            template = templ;
        }

        public override void Start()
        {
            BulletTemplate bt =
                template == "random"
                ? game.GetRandom()
                : game.GetTemplate(template);

            game.SpawnBullet(bt.Instantiate(gun));
            hasFired = true;
            float p =
                ((float)game.random.NextDouble() - 0.5f) * 0.1f;

            float pa =
                ((float)game.random.NextDouble() - 0.5f) * 0.1f;

            Game1.engine.Get("pew")
                .Play(
                    Game1.Volume,
                    p * Game1.Slomo,
                    pa
                );
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override bool IsFinished()
        {
            return hasFired;
        }
    }

    public class Rotate : PatternAction
    {
        private float amount;
        private bool rotated;

        public Rotate(Game1 game, float amt) : base(game)
        {
            amount = amt;
        }

        public override void Start()
        {
            rotated = false;
        }

        public override void Update(GameTime gameTime)
        {
            gun.Direction += amount * 2f * (float)Math.PI;
            rotated = true;
        }

        public override bool IsFinished()
        {
            return rotated;
        }
    }

    public class Repeat : PatternAction
    {
        private int current;
        private int count;
        private int index;
        private List<PatternAction> Block; 

        public Repeat(Game1 game, int count) : base(game)
        {
            this.count = count;
            Block = new List<PatternAction>();
        }

        public Repeat Add(PatternAction pa)
        {
            pa.SetGun(gun);
            Block.Add(pa);
            return this;
        }

        public override void Start()
        {
            index = 0;
            current = 0;
            Block[index].Start();
        }

        public override void Update(GameTime gameTime)
        {
            if (current == count) return;

            while (Block[index].IsFinished())
            {
                if (index == Block.Count - 1)
                    current++;

                index = (index + 1 + Block.Count) % Block.Count;
                Block[index].Start();
            }

            Block[index].Update(gameTime);
        }

        public override bool IsFinished()
        {
            return current == count;
        }
    }
}
