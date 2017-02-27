using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Atomicus
{
    public class Particle
    {
        internal Rectangle rect;
        internal Vector2 location;
        internal Vector2 direction;
        internal bool isRemoved;
        internal bool inUniverse;
        internal bool movedToUniverse;
        internal ParticleType type;
        internal float rotation;
        internal float orbit;
        internal float initialScale;
        internal float scale;
        internal Texture2D texture;
        internal Vector2 destination;
        internal float angle;
        internal Rectangle destinationZone;
        internal bool movedFromReceptical;
        float radius;
        bool init;
        float orbitIncrement;

        internal bool mouseOver = false, beingdragged = false;

        internal Particle(Texture2D texture_, ParticleType type_)
        {
            texture = texture_;
            type = type_;

            initScale();
            scale = initialScale;

            location = new Vector2(0, 0);
            rect = new Rectangle((int)location.X, (int)location.Y, (int)(texture.Width * scale), (int)(texture.Height * scale));
            initLocation();

            inUniverse = false;
            isRemoved = false;
            rotation = 0f;
            direction = new Vector2(0, 0);
            movedFromReceptical = false;
            movedToUniverse = false;

            initialise();
        }

        internal void initialise()
        {
            scale = initialScale;
            init = true;
        }
       

        internal void update()
        {
            if (rect.Intersects(new Rectangle(Game1.mouse.X, Game1.mouse.Y, 0, 0)))
            {
                mouseOver = true;
            }
            else mouseOver = false;

            bool lastUniverseState = inUniverse;
            checkParticleInsideUniverse();
            if (lastUniverseState && !inUniverse) initialise();


                if (inUniverse && (type == ParticleType.Proton || type == ParticleType.Neutron))
            {

                if (Game1.protonCount + Game1.neutronCount == 1)
                {
                    destination = Game1.universeCentre;
                    destinationZone = new Rectangle((int)destination.X - 10, (int)destination.Y - 10, 20, 20);
                    moveLinear();
                }
                else if (Game1.protonCount + Game1.neutronCount > 1)
                {
                    if (init)
                    {
                        init = false;
                        scale /= Game1.protonCount + Game1.neutronCount * 0.5f;
                        orbit += (float)Math.PI / 2;

                        orbitIncrement = -0.018f;
                    }
                    radius = texture.Width * scale / 2;
                    orbitCentre();
                }
            }

            if (inUniverse && type == ParticleType.Electron && (Game1.protonCount > 0 || Game1.neutronCount > 0))
            {
                if (init)
                {
                    init = false;
                    orbit = (float)Math.Atan2(Game1.universeCentre.Y - location.Y, Game1.universeCentre.X - location.X) + (float)Math.PI;
                    orbitIncrement = 0.035f;
                    radius = 130f;
                }
                
                orbitCentre();
            }
            
            rect = new Rectangle((int)location.X, (int)location.Y, (int)(texture.Width * scale), (int)(texture.Height * scale));
        }

        internal void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, location, null, Color.White, (float)Math.PI * 2, new Vector2(0, 0), scale, SpriteEffects.None, 0);
        }

        internal void checkParticleInsideUniverse()
        {
            inUniverse = Game1.universe.Intersects(new Rectangle((int)getLocation().X, (int)getLocation().Y, 0, 0));
        }

        internal void moveLinear()
        {
            if (destinationZone.Intersects(new Rectangle((int)getLocation().X, (int)getLocation().Y, 0, 0))) setLocation(Game1.universeCentre);
            else
            {
                angle = (float)Math.Atan2(destination.Y - getLocation().Y, destination.X - getLocation().X);
                direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                location += direction * 15;
            }
        }

        internal void orbitCentre()
        {
            orbit += orbitIncrement;
            setLocation(new Vector2((float)(Game1.universeCentre.X + (radius * Math.Cos(orbit))), (float)(Game1.universeCentre.Y + (radius * Math.Sin(orbit)))));
        }

        internal void setLocation(Vector2 newLocation, bool shift = true)
        {
            if (shift) location = newLocation - new Vector2(rect.Width / 2, rect.Height / 2);
            else location = newLocation;
        }

        internal Vector2 getLocation()
        {
            return location + new Vector2(rect.Width / 2, rect.Height / 2);
        }

        internal void initLocation()
        {
            if (type == ParticleType.Proton) setLocation(Game1.receptical1 + Game1.recepticalSize / 2);
            else if (type == ParticleType.Neutron) setLocation(Game1.receptical2 + Game1.recepticalSize / 2);
            else if (type == ParticleType.Electron) setLocation(Game1.receptical3 + Game1.recepticalSize / 2);
        }

        internal void initScale()
        {
            if (type == ParticleType.Proton) initialScale = 0.351f;
            else if (type == ParticleType.Neutron) initialScale = 0.351f;
            else if (type == ParticleType.Electron) initialScale = 0.14f;
        }
    }
}
