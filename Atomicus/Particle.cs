using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Atomicus
{
    public class Particle
    {
        internal Vector2 location;
        internal Vector2 direction;
        internal bool isRemoved;
        internal bool inUniverse;
        internal bool movedToUniverse;
        internal ParticleType type;
        internal float rotation;
        internal float orbit;
        internal Vector2 centrePoint;
        internal float scale;
        internal Texture2D texture;
        internal Rectangle universe;
        internal Vector2 universeCentre;
        internal Vector2 destination;
        internal float angle;
        internal Rectangle destinationZone;
        internal bool movedFromReceptical;
        float radius;
        bool init;
        float orbitIncrement;

        internal bool mouseOver = false, beingdragged = false;

        internal Particle(Rectangle universe_, Texture2D texture_, ParticleType type_, float x, float y)
        {
            texture = texture_;
            type = type_;
            location = new Vector2(x, y);
            scale = 1f;
            centrePoint = Func.getCentrePoint(texture, scale);
            isRemoved = false;
            inUniverse = false;
            movedToUniverse = false;
            rotation = 0f;
            direction = new Vector2(0, 0);
            universe = universe_;
            universeCentre = new Vector2(universe.X + (universe.Width / 2), universe.Y + (universe.Height / 2));
            movedFromReceptical = false;
            init = true;
        }

        internal void update(MouseState mouseState)
        {
            if (new Rectangle((int)location.X - (int)centrePoint.X, (int)location.Y - (int)centrePoint.Y, (int)(texture.Width * scale), (int)(texture.Height * scale)).Intersects(new Rectangle(mouseState.X, mouseState.Y, 1, 1)))
            {
                mouseOver = true;
            }
            else mouseOver = false;

            checkParticleInsideUniverse();

            if (inUniverse && (type == ParticleType.Proton || type == ParticleType.Neutron))
            {

                if (Game1.protonCount + Game1.neutronCount == 1)
                {
                    destination = universeCentre;
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
                    orbit = (float)Math.Atan2(universeCentre.Y - location.Y, universeCentre.X - location.X) + (float)Math.PI;
                    orbitIncrement = 0.035f;
                }
                
                radius = 80f;
                orbitCentre();
            }
        }

        internal void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, location, null, Color.White, rotation, centrePoint, scale, SpriteEffects.None, 0);
        }

        internal void checkParticleInsideUniverse()
        {
            inUniverse = universe.Intersects(new Rectangle((int)location.X, (int)location.Y, 0, 0));
        }

        internal void moveLinear()
        {
            if (destinationZone.Intersects(new Rectangle((int)location.X, (int)location.Y, 0, 0))) location = universeCentre;
            else
            {
                angle = (float)Math.Atan2(destination.Y - location.Y, destination.X - location.X);
                direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                location += direction * 15;
            }
        }

        internal void orbitCentre()
        {
            orbit += orbitIncrement;
            orbitIncrement += (orbitIncrement / 200);
            location.X = (float)(universeCentre.X + (radius * Math.Cos(orbit))); 
            location.Y = (float)(universeCentre.Y + (radius * Math.Sin(orbit)));
        }
    }
}
