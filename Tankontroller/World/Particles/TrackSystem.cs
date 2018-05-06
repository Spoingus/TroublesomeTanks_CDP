using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tankontroller.World.Particles
{
    public class Track
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Color Colour { get; set; }

        public Track(Vector2 pPosition, float pRotation, Color pColour)
        {
            Position = pPosition;
            Rotation = pRotation;
            Colour = pColour;
        }
    }
    public class TrackSystem
    {
        private static Texture2D m_Texture;
        private static Rectangle m_Rectangle;

        private static TrackSystem m_Instance = null;

        private Track[] m_Tracks;
        private const int MAX_TRACKS = 250;
        private int m_NextTrack;

        public static void SetupStaticMembers(Texture2D pTexture)
        {
            int trackWidth = 20;
            int trackHeight = 10;
            m_Texture = pTexture;
            m_Rectangle = new Rectangle(0, 0, trackWidth, trackHeight);
            m_Instance = new TrackSystem();
        }

        public static TrackSystem GetInstance()
        {
            return m_Instance;
        }
        private TrackSystem()
        {
            m_Tracks = new Track[MAX_TRACKS];
            m_NextTrack = 0;
            for(int i = 0; i < MAX_TRACKS; i++)
            {
                m_Tracks[i] = new Track(Vector2.Zero, 0f, Color.Black/*DGS.COLOUR_GROUND*/);
            }
        }

        public void AddTrack(Vector2 pPosition, float pRotation, Color pColour)
        {
            m_Tracks[m_NextTrack].Colour = pColour;
            m_Tracks[m_NextTrack].Position = pPosition;
            m_Tracks[m_NextTrack].Rotation = pRotation;
            m_NextTrack++;
            if(m_NextTrack == MAX_TRACKS)
            {
                m_NextTrack = 0;
            }
        }

        public void Draw(SpriteBatch pBatch)
        {
            for(int i = 0; i < MAX_TRACKS; i++)
            {
                pBatch.Draw(m_Texture, m_Tracks[i].Position, null, m_Tracks[i].Colour, m_Tracks[i].Rotation, new Vector2(m_Texture.Width / 2, m_Texture.Height / 2), 1f, SpriteEffects.None, 0.0f);
            }
        }
    }
}
