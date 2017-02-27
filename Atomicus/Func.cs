using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Atomicus
{
    internal static class Func
    {
        internal static Vector2 getCentrePoint(Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        internal static string arrayToString(ParticleType[] input)
        {
            string output = "";

            for (int i = 0; i < input.Length; i++)
            {
                output += input[i].ToString() + " ";
            }
            
            return output;
        }

        internal static void countParticles(ParticleType[] input, ref int protons, ref int neutrons, ref int electrons)
        {
            protons = 0;
            neutrons = 0;
            electrons = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == ParticleType.Proton) protons++;
                if (input[i] == ParticleType.Neutron) neutrons++;
                if (input[i] == ParticleType.Electron) electrons++;
            }
        }
    }
}
