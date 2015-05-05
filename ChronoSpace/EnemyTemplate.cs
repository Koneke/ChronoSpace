using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ChronoSpace
{
    public abstract class Enemy
    {
        private static Dictionary<string, Enemy> templates
            = new Dictionary<string, Enemy>();

        public static void AddTemplate(string name, Enemy e)
        {
            templates.Add(name.ToLower(), e);
        }

        public static Enemy Get(string name)
        {
            return templates[name.ToLower()];
        }

        public static Enemy GetRandom()
        {
            Random random = new Random();
            return templates[templates.Keys.ToList()[
                random.Next(0, templates.Count)
            ]];
        }

        public static void Setup()
        {
            AddTemplate("jonah", new Jonah());
            AddTemplate("axel", new Axel());
            AddTemplate("ronald", new Ronald());
        }

        public abstract Ship Spawn(Game1 G, Vector2 p);
    }

    public class Jonah : Enemy
    {
        public override Ship Spawn(Game1 G, Vector2 p)
        {
            Ship s = new Ship(
                "jonah",
                p
            );

            s.Velocity = new Vector2(0, 150);

            Gun g = new Gun();
            g.Owner = s;

            g
                .Add(new Fire(G, "red"))
                .Add(new Rotate(G, 0.05f))
                .Add(new Delay(G, 0.05f))
            ;

            s.AddGun(g);

            g = new Gun();

            g
                .Add(new Fire(G, "yel"))
                .Add(new Rotate(G, -0.04f))
                .Add(new Delay(G, 0.05f))
            ;

            s.AddGun(g);

            g = new Gun();
            //g.Direction = (float)Math.PI;

            g
                .Add(new Fire(G, "blu"))
                .Add(new Rotate(G, -0.05f))
                .Add(new Delay(G, 0.05f))
            ;

            s.AddGun(g);

            s.StartGuns();

            return s;
        }
    }

    public class Axel : Enemy
    {
        public override Ship Spawn(Game1 G, Vector2 p)
        {
            Ship s = new Ship("axel", p);
            s.Velocity = new Vector2(0, 100);

            Gun g;

            g = new Gun();
            s.AddGun(g);

            g
                .Add(
                    ((Repeat)new Repeat(G, 4).SetGun(g))
                        .Add(new Fire(G, "red"))
                        .Add(new Rotate(G, 0.25f))
                )
                .Add(new Rotate(G, 0.125f))
                .Add(new Delay(G, 0.07f))
                .Add(
                    ((Repeat)new Repeat(G, 4).SetGun(g))
                        .Add(new Fire(G, "yel"))
                        .Add(new Rotate(G, 0.25f))
                )
                .Add(new Rotate(G, 0.125f))
                .Add(new Delay(G, 0.07f))
                .Add(new Rotate(G, 1 / 5f))
            ;

            s.StartGuns();
            return s;
        }
    }

    public class Ronald : Enemy
    {
        public override Ship Spawn(Game1 G, Vector2 p)
        {
            Ship s = new Ship("ronald", p);
            s.Velocity = new Vector2(0, 80);

            Gun g;

            g = new Gun();
            s.AddGun(g);
            g.Direction += (float)Math.PI/4;

            g
                .Add(new Fire(G, "random"))
                .Add(new Rotate(G, 1 / 8f))
                .Add(new Fire(G, "random"))
                .Add(new Rotate(G, 1 / 8f))
                .Add(new Fire(G, "random"))
                .Add(new Rotate(G, -1 / 4f))
                .Add(new Delay(G, 0.05f))
                .Add(new Rotate(G, 1 / 5f))
            ;

            s.StartGuns();
            return s;
        }
    }
}
