using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Tankontroller.World.Particles
{
    public class Particle
    {
        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float Radius { get; private set; }
        public float RadiusIncreaseRate { get; private set; }
        public Color Colour { get; private set; }
        public float TimeTillDead { get; private set; }
        /// <summary>
        /// Initiates all values.
        /// </summary>
        /// <param name="pPosition">Initial position of particle</param>
        /// <param name="pVelocity">Initial velocity of particle</param>
        /// <param name="pRadius">Initial radius of particle</param>
        /// <param name="pRadiusIncreaseRate">Rate of change of radius</param>
        /// <param name="pColour">Particle colour</param>
        /// <param name="pTimeTillDead">Time until particle will be recycled</param>
        public void Initiate(Vector2 pPosition, Vector2 pVelocity, float pRadius, float pRadiusIncreaseRate, Color pColour, float pTimeTillDead)
        {
            Position = pPosition;
            Velocity = pVelocity;
            Radius = pRadius;
            RadiusIncreaseRate = pRadiusIncreaseRate;
            Colour = pColour;
            TimeTillDead = pTimeTillDead;
        }

        /// <summary>
        /// Update particle
        /// </summary>
        /// <param name="pSeconds">delta time (seconds)</param>
        /// <returns>Particle still active</returns>
        public bool Update(float pSeconds)
        {
            Position = Position + Velocity * pSeconds;
            Radius = Radius + RadiusIncreaseRate * pSeconds;
            TimeTillDead = TimeTillDead - pSeconds;
            return TimeTillDead > 0;
        }

        /// <summary>
        /// Draws a circle. Used to draw the particle.
        /// </summary>
        /// <param name="pBatch">SpriteBatch to draw to</param>
        /// <param name="pTexture">Texture to draw</param>
        /// <param name="pRadius">Radius of circle</param>
        /// <param name="pPos">Position of circle centre</param>
        /// <param name="pColour">Colour of circle</param>
        static public void DrawCircle(SpriteBatch pBatch, Texture2D pTexture, int pRadius, Vector2 pPos, Color pColour)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = pRadius;
            rectangle.Height = pRadius;
            rectangle.X = (int)pPos.X - pRadius / 2;
            rectangle.Y = (int)pPos.Y - pRadius / 2;
            pBatch.Draw(pTexture, rectangle, pColour);
        }

        /// <summary>
        /// Draws the background of the particle.
        /// </summary>
        /// <param name="pBatch"></param>
        /// <param name="pTexture"></param>
        public void DrawBackground(SpriteBatch pBatch, Texture2D pTexture)
        {
            DrawCircle(pBatch, pTexture, (int)Radius + 2 * DGS.Instance.GetInt("PARTICLE_EDGE_THICKNESS"), Position, Color.Black);
        }

        /// <summary>
        /// Draws the foreground particle.
        /// </summary>
        /// <param name="pBatch"></param>
        /// <param name="pTexture"></param>
        public void DrawForeground(SpriteBatch pBatch, Texture2D pTexture)
        {
            DrawCircle(pBatch, pTexture, (int)Radius, Position, Colour);
        }
    }

    /// <summary>
    /// The ParticleManager singleton manages an array of particle objects.
    /// Particle systems are created by passing an InitialisationPolicy object to the InitialiseParticles method.
    /// ParticleManager also updates and draws the particles.
    /// </summary>
    public class ParticleManager
    {
        private Particle[] m_Particles = null;
        private int m_LastParticleIndex = 0;
        private Texture2D m_Texture;
        private Rectangle m_Rectangle;

        private static ParticleManager m_Instance = null;
        private ParticleManager()
        {
            m_Texture = Tankontroller.Instance().CM().Load<Texture2D>("circle");
            m_Rectangle = new Rectangle();
            m_Particles = new Particle[1000];
            for (int i = 0; i < m_Particles.Length; i++)
            {
                m_Particles[i] = new Particle();
            }
        }

        /// <summary>
        /// Singleton instanct of ParticleManager
        /// </summary>
        /// <returns>The instance of ParticleManager</returns>
        public static ParticleManager Instance()
        {
            if (m_Instance == null)
            {
                m_Instance = new ParticleManager();
            }
            return m_Instance;
        }

        /// <summary>
        /// Updates all active particles. Any particle that is no longer active is recycled
        /// </summary>
        /// <param name="pSeconds">delta time (seconds)</param>
        public void Update(float pSeconds)
        {
            for (int i = 0; i < m_LastParticleIndex; i++)
            {
                if (!m_Particles[i].Update(pSeconds))
                {
                    Particle temp = m_Particles[i];
                    m_Particles[i] = m_Particles[m_LastParticleIndex - 1];
                    m_Particles[m_LastParticleIndex - 1] = temp;
                    i = i - 1;
                    m_LastParticleIndex--;
                }
            }
        }

        /// <summary>
        /// Allocates number of particles to be initialised
        /// </summary>
        /// <param name="pNumberOfParticles">Requested number of particles to be allocated</param>
        /// <returns>Particles allocated</returns>
        private Particle[] GetParticles(int pNumberOfParticles)
        {
            Particle[] particles;

            int availableParticles = m_Particles.Length - m_LastParticleIndex;

            if (availableParticles > pNumberOfParticles)
            {
                particles = new Particle[pNumberOfParticles];
            }
            else
            {
                particles = new Particle[availableParticles];
            }

            int i = 0;
            for (; i < particles.Length; i++)
            {
                particles[i] = m_Particles[m_LastParticleIndex + i];
            }
            m_LastParticleIndex += i;
            return particles;
        }

        /// <summary>
        /// Requests number of particles and initialises according to the ParticleInitialisationPolicy parameter
        /// </summary>
        /// <param name="pInitialisationPolicy">Policy used to initialise particles</param>
        /// <param name="pNumberOfParticles">Requested number of particles</param>
        public void InitialiseParticles(IParticleInitialisationPolicy pInitialisationPolicy, int pNumberOfParticles)
        {
            pInitialisationPolicy.InitiateParticles(GetParticles(pNumberOfParticles));
        }

        /// <summary>
        /// Draws all particles with black border.
        /// </summary>
        /// <param name="pBatch">SpriteBatch to use to draw with.</param>
        public void Draw(SpriteBatch pBatch)
        {
            for (int i = 0; i < m_LastParticleIndex; i++)
            {
                m_Particles[i].DrawBackground(pBatch, m_Texture);
            }
            for (int i = 0; i < m_LastParticleIndex; i++)
            {
                m_Particles[i].DrawForeground(pBatch, m_Texture);
            }
        }
    }

    /// <summary>
    /// Interface describes how initial values of particle should be set
    /// </summary>
    public interface IParticleInitialisationPolicy
    {
        void InitiateParticles(Particle[] pParticles);
    }

    public class ExplosionInitialisationPolicy : IParticleInitialisationPolicy
    {
        private Vector2 m_Position;
        private Vector2 m_Normal;
        private Color m_Colour;
        private Random m_Rng = new Random();

        /// <summary>
        /// ExplosionInitialisationPolicy creates particles at the point of explision travelling in a 
        /// semicircle around the collision normal.
        /// </summary>
        /// <param name="pPosition">Central position of explosion.</param>
        /// <param name="pNormal">Normal provides "direction" for explosion.</param>
        /// <param name="pColour">Colour of all particles.</param>
        public ExplosionInitialisationPolicy(Vector2 pPosition, Vector2 pNormal, Color pColour)
        {
            m_Position = pPosition;
            m_Normal = pNormal;
            m_Colour = pColour;
        }

        public void InitiateParticles(Particle[] pParticles)
        {
            for (int i = 0; i < pParticles.Length; i++)
            {
                float lifetime = m_Rng.Next(1000, 2001) * 0.00015f;
                Vector2 velocity = Vector2.Transform(m_Normal, Matrix.CreateRotationZ((float)(m_Rng.NextDouble() * Math.PI - Math.PI / 2))) * m_Rng.Next(90, 121);
                pParticles[i].Initiate(m_Position, velocity, m_Rng.Next(3, 11), m_Rng.Next(25, 46), m_Colour, lifetime);
            }
        }
    }

    public class DustInitialisationPolicy : IParticleInitialisationPolicy
    {
        private Vector2 m_Point1;
        private Vector2 m_Point2;
        private Random m_Rng = new Random();

        /// <summary>
        /// DustInitialisationPolicy creates particles at a random point on a line between two points.
        /// </summary>
        /// <param name="pPoint1">Start point of line.</param>
        /// <param name="pPoint2">End point of line.</param>
        public DustInitialisationPolicy(Vector2 pPoint1, Vector2 pPoint2)
        {
            m_Point1 = pPoint1;
            m_Point2 = pPoint2;
        }

        public void InitiateParticles(Particle[] pParticles)
        {
            for (int i = 0; i < pParticles.Length; i++)
            {
                Vector2 position = m_Point1 + (m_Point2 - m_Point1) * (float)m_Rng.NextDouble();
                float lifetime = m_Rng.Next(250, 751) * 0.001f;
                pParticles[i].Initiate(position, Vector2.Zero, 1, m_Rng.Next(5, 15), DGS.Instance.GetColour("COLOUR_DUST"), lifetime);
            }
        }
    }

    public class BulletInitialisationPolicy : IParticleInitialisationPolicy
    {
        private Vector2 m_Position;
        private Color m_Colour;
        private Random m_Rng = new Random();

        public BulletInitialisationPolicy(Vector2 pPosition, Color pColour)
        {
            m_Position = pPosition;
            m_Colour = pColour;
        }

        public void InitiateParticles(Particle[] pParticles)
        {
            for (int i = 0; i < pParticles.Length; i++)
            {
                float lifetime = m_Rng.Next(1000, 2001) * 0.0001f;
                Vector2 position = m_Position;
                position.X += m_Rng.Next(-2, 2);
                position.Y += m_Rng.Next(-2, 2);
                pParticles[i].Initiate(position, Vector2.Zero, m_Rng.Next(2, 4), 0, m_Colour, lifetime);
            }
        }
    }
}
