using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Atomicus
{
    enum ParticleType { Proton, Neutron, Electron, Photon };
    
    public class Game1 : Game
    {
        static internal int protonCount, neutronCount, electronCount;
        static internal Vector2 recepticalSize, receptical1, receptical2, receptical3;
        static internal Rectangle universe;
        static internal Vector2 universeCentre;
        static internal MouseState mouse;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D protonTexture, neutronTexture, electronTexture, dottedRectangleTexture, squareBorderTexture, photonTexture;
        private Vector2 mouseLocation;
        private Particle[] particles;
        private SpriteFont arial12;
        private bool mouseDragging;
        private ParticleType[] particlesInUniverse;
        private string description;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            base.Initialize();

            universe = new Rectangle(260, 30, dottedRectangleTexture.Width + 100, dottedRectangleTexture.Height + 100);
            universeCentre = Func.getCentrePoint(universe);

            recepticalSize = new Vector2(squareBorderTexture.Width, squareBorderTexture.Height);
            receptical1 = new Vector2(30, 40);
            receptical2 = new Vector2(30, 144);
            receptical3 = new Vector2(30, 248);

            particles = new Particle[3];
            particles[0] = new Particle(protonTexture, ParticleType.Proton);
            particles[1] = new Particle(neutronTexture, ParticleType.Neutron);
            particles[2] = new Particle(electronTexture, ParticleType.Electron);
            //particles[3] = new Particle(photonTexture, ParticleType.Photon);

            description = "";
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            protonTexture = Content.Load<Texture2D>("particles/proton");
            neutronTexture = Content.Load<Texture2D>("particles/neutron");
            electronTexture = Content.Load<Texture2D>("particles/electron");
            photonTexture = Content.Load<Texture2D>("particles/photon");
            dottedRectangleTexture = Content.Load<Texture2D>("dottedRectangle");
            squareBorderTexture = Content.Load<Texture2D>("squareBorder");
            arial12 = Content.Load<SpriteFont>("arial12");
        }
        
        protected override void UnloadContent()
        {

        }
        
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            mouse = Mouse.GetState();
            mouseLocation = new Vector2(mouse.X, mouse.Y);

            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].beingdragged = false;
            }
            mouseDragging = false;

            for (int i = particles.Length - 1; i >= 0; i--)
            {
                if (particles[i].mouseOver && mouse.LeftButton == ButtonState.Pressed)
                {
                    if (!particles[i].beingdragged) bringToFront(ref particles, i);
                    particles[i].beingdragged = true;
                    mouseDragging = true;
                    if (!particles[i].movedFromReceptical)
                    {
                        particles[i].movedFromReceptical = true;
                        spawnNewParticle(particles[i].type);
                        if (!particles[i + 1].beingdragged) bringToFront(ref particles, i + 1);
                    }
                    break;
                }
            }
            
            for (int i = 0; i < particles.Length; i++)
            {
                if (!particles[i].beingdragged) particles[i].update();
            }

            if (mouseDragging) particles[particles.Length - 1].setLocation(new Vector2(mouse.X, mouse.Y));
            
            checkParticlesInUniverse();
            Func.countParticles(particlesInUniverse, ref protonCount, ref neutronCount, ref electronCount);
            contructDescription();
        }

        protected override void Draw(GameTime gameTime)
        {
            IsMouseVisible = true;

            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
            
            spriteBatch.Begin();
            
            spriteBatch.DrawString(arial12, "protons: " + protonCount, new Vector2(5, 430), Color.White);
            spriteBatch.DrawString(arial12, "neutrons: " + neutronCount, new Vector2(5, 445), Color.White);
            spriteBatch.DrawString(arial12, "electrons: " + electronCount, new Vector2(5, 460), Color.White);

            spriteBatch.DrawString(arial12, description, new Vector2(universeCentre.X - (int)(arial12.MeasureString(description).X / 2), universe.Y + 8), Color.DarkRed, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);

            drawBackground();

            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].draw(spriteBatch);
            }

            spriteBatch.End();
        }

        private void bringToFront(ref Particle[] particles, int swapIndex)
        {
            Particle temp;
            
            temp = particles[swapIndex];

            for (int i = swapIndex; i < particles.Length - 1; i++)
            {
                particles[i] = particles[i + 1];
            }

            particles[particles.Length - 1] = temp;
        }

        private void checkParticlesInUniverse()
        {
            particlesInUniverse = new ParticleType[0];
            
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i].inUniverse)
                {
                    Array.Resize(ref particlesInUniverse, particlesInUniverse.Length + 1);
                    particlesInUniverse[particlesInUniverse.Length - 1] = particles[i].type;
                }
            }
        }

        private void contructDescription()
        {
            description = "";
            string element = "";
            string charge = "";
            string isotope = (neutronCount + protonCount).ToString();

            switch (protonCount)
            {
                case 1: element = "HYDROGEN"; break;
                case 2: element = "HELIUM"; break;
                case 3: element = "LITHIUM"; break;
                case 4: element = "BERYLIUM"; break;
                case 5: element = "BORON"; break;
                case 6: element = "CARBON"; break;
                case 7: element = "NITROGEN"; break;
                case 8: element = "OXYGEN"; break;
                case 9: element = "FLUORINE"; break;
                case 10: element = "NEON"; break;
                case 11: element = "SODIUM"; break;
                case 12: element = "MAGNESIUM"; break;
                case 13: element = "ALUMINIUM"; break;
                case 14: element = "SILICON"; break;
                case 15: element = "PHOSPHORUS"; break;
                case 16: element = "SULFUR"; break;
                case 17: element = "CHLORINE"; break;
                case 18: element = "ARGON"; break;
                case 19: element = "POTASSIUM"; break;
                case 20: element = "CALCIUM"; break;
            }

            if (protonCount == electronCount && protonCount > 0) charge = "";
            else if (protonCount > electronCount) charge = "(ion +" + (protonCount - electronCount).ToString() + ")";
            else if (protonCount < electronCount) charge = "(ion -" + (electronCount - protonCount).ToString() + ")";

            if (element != "") description = element + "-" + isotope + " " + charge;
        }

        private void drawBackground()
        {
            spriteBatch.Draw(dottedRectangleTexture, universe, Color.White);

            spriteBatch.DrawString(arial12, "PROTON", new Vector2(28, receptical1.Y - 18), Color.Purple);
            spriteBatch.DrawString(arial12, "NEUTRON", new Vector2(26, receptical2.Y - 18), Color.Purple);
            spriteBatch.DrawString(arial12, "ELECTRON", new Vector2(20, receptical3.Y - 18), Color.Purple);

            spriteBatch.Draw(squareBorderTexture, receptical1, Color.White);
            spriteBatch.Draw(squareBorderTexture, receptical2, Color.White);
            spriteBatch.Draw(squareBorderTexture, receptical3, Color.White);
        }

        private void spawnNewParticle(ParticleType type)
        {
            Array.Resize(ref particles, particles.Length + 1);

            for (int i = particles.Length - 1; i > 0; i--)
            {
                particles[i] = particles[i - 1];
            }

            if (type == ParticleType.Proton) particles[0] = new Particle(protonTexture, ParticleType.Proton);
            else if (type == ParticleType.Neutron) particles[0] = new Particle(neutronTexture, ParticleType.Neutron);
            else if (type == ParticleType.Electron) particles[0] = new Particle(electronTexture, ParticleType.Electron);
        }

        private void updateParticlesBelow()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].checkParticlesBelow();
            }
        }
    }
}
