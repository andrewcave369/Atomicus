using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Atomicus
{
    enum ParticleType { Proton, Neutron, Electron };
    
    public class Game1 : Game
    {
        static internal int protonCount, neutronCount, electronCount;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D protonTexture, neutronTexture, electronTexture, dottedRectangleTexture, squareBorderTexture;
        private MouseState mouseState;
        private Vector2 mouseLocation;
        private Vector2 universeCentre;
        private Particle[] particles;
        private SpriteFont arial12;
        private bool mouseDragging;
        private Rectangle universe;
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

            universe = new Rectangle(360, 30, dottedRectangleTexture.Width, dottedRectangleTexture.Height);
            universeCentre = new Vector2(360 , 30) + Func.getCentrePoint(dottedRectangleTexture, 1);

            particles = new Particle[3];
            particles[0] = new Particle(universe, protonTexture, ParticleType.Proton, 60, 60);
            particles[1] = new Particle(universe, neutronTexture, ParticleType.Neutron, 60, 164);
            particles[2] = new Particle(universe, electronTexture, ParticleType.Electron, 60, 268);

            description = "";
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            protonTexture = Content.Load<Texture2D>("particles/proton");
            neutronTexture = Content.Load<Texture2D>("particles/neutron");
            electronTexture = Content.Load<Texture2D>("particles/electron");
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

            mouseState = Mouse.GetState();
            mouseLocation = new Vector2(mouseState.X, mouseState.Y);

            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].beingdragged = false;
            }
            mouseDragging = false;

            for (int i = particles.Length - 1; i >= 0; i--)
            {
                if (particles[i].mouseOver && mouseState.LeftButton == ButtonState.Pressed)
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
                if (!particles[i].beingdragged) particles[i].update(mouseState);
            }
            
            if (mouseDragging) particles[particles.Length - 1].location = mouseLocation;

            
            
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

            spriteBatch.DrawString(arial12, description, new Vector2(universeCentre.X - (int)(arial12.MeasureString(description).X / 2), 34), Color.DarkRed, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);

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
            }

            if (protonCount == electronCount && protonCount > 0) charge = "";
            else if (protonCount > electronCount) charge = "(ion +" + (protonCount - electronCount).ToString() + ")";
            else if (protonCount < electronCount) charge = "(ion -" + (electronCount - protonCount).ToString() + ")";

            if (element != "") description = element + "-" + isotope + " " + charge;
        }

        private void drawBackground()
        {
            spriteBatch.Draw(dottedRectangleTexture, universe, Color.White);

            spriteBatch.DrawString(arial12, "PROTON", new Vector2(28, 12), Color.Purple);
            spriteBatch.DrawString(arial12, "NEUTRON", new Vector2(26, 116), Color.Purple);
            spriteBatch.DrawString(arial12, "ELECTRON", new Vector2(20, 220), Color.Purple);

            spriteBatch.Draw(squareBorderTexture, new Vector2(30, 30), Color.White);
            spriteBatch.Draw(squareBorderTexture, new Vector2(30, 134), Color.White);
            spriteBatch.Draw(squareBorderTexture, new Vector2(30, 238), Color.White);
        }

        private void spawnNewParticle(ParticleType type)
        {
            Array.Resize(ref particles, particles.Length + 1);

            for (int i = particles.Length - 1; i > 0; i--)
            {
                particles[i] = particles[i - 1];
            }

            if (type == ParticleType.Proton) particles[0] = new Particle(universe, protonTexture, ParticleType.Proton, 60, 60);
            else if (type == ParticleType.Neutron) particles[0] = new Particle(universe, neutronTexture, ParticleType.Neutron, 60, 164);
            else if (type == ParticleType.Electron) particles[0] = new Particle(universe, electronTexture, ParticleType.Electron, 60, 268);
        }
    }
}
