using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ChronoSpace
{
    public class Gun
    {
        public Entity Owner;
        public List<PatternAction> Pattern;
        private int index;

        public float Direction;

        public Gun()
        {
            Pattern = new List<PatternAction>();
            index = 0;
        }

        public Vector2 Position {
            get { return Owner.Position; }
        }

        public Gun Add(PatternAction pa)
        {
            pa.SetGun(this);
            Pattern.Add(pa);
            return this;
        }

        public void Update(GameTime gameTime)
        {
            while (Pattern[index].IsFinished())
            {
                index = (index + 1 + Pattern.Count) % Pattern.Count;
                Pattern[index].Start();
            }

            Pattern[index].Update(gameTime);
        }

        public void Start()
        {
            Pattern[index].Start();
        }
    }
}