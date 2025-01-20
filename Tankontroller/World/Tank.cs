using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Tankontroller.World.Particles;


namespace Tankontroller.World
{
    public class Tank
    {
        public static readonly int MAX_HEALTH = DGS.Instance.GetInt("MAX_TANK_HEALTH");
        public static readonly float TANK_SPEED = DGS.Instance.GetFloat("TANK_SPEED");
        public static readonly float BULLET_SPEED = DGS.Instance.GetFloat("BULLET_SPEED");
        public static readonly int BLAST_DELAY = DGS.Instance.GetInt("BLAST_DELAY");
        public static readonly int TANK_HEIGHT = DGS.Instance.GetInt("TANK_HEIGHT");
        public static readonly int TANK_WIDTH = DGS.Instance.GetInt("TANK_WIDTH");
        public static readonly int TANK_FRONT_BUFFER = DGS.Instance.GetInt("TANK_FRONT_BUFFER");
        public static readonly float BASE_TURRET_ROTATION_ANGLE = DGS.Instance.GetFloat("BASE_TURRET_ROTATION_ANGLE");
        public static readonly float BASE_TANK_ROTATION_ANGLE = DGS.Instance.GetFloat("BASE_TANK_ROTATION_ANGLE");
        public static readonly float TRACK_OFFSET = DGS.Instance.GetFloat("TRACK_OFFSET");


        static private Texture2D mBaseTexture;
        static private Texture2D mBrokenTexture;
        static private Texture2D mRightTrackTexture;
        static private Texture2D mLeftTrackTexture;
        static private Texture2D mCannonTexture;
        static private Texture2D mCannonFireTexture;

        public static void SetupStaticTextures(Texture2D pBase, Texture2D brokenTexture, Texture2D pRightTrack, Texture2D pLeftTrack, Texture2D pCannon, Texture2D pCannonFire)
        {
            mBaseTexture = pBase;
            mBrokenTexture = brokenTexture;
            mRightTrackTexture = pRightTrack;
            mLeftTrackTexture = pLeftTrack;
            mCannonTexture = pCannon;
            mCannonFireTexture = pCannonFire;
        }

        private Vector2[] TANK_CORNERS = { new Vector2(TANK_HEIGHT / 2 - TANK_FRONT_BUFFER, -TANK_WIDTH / 2), new Vector2(-TANK_HEIGHT / 2, -TANK_WIDTH / 2), new Vector2(-TANK_HEIGHT / 2, TANK_WIDTH / 2), new Vector2(TANK_HEIGHT / 2 - TANK_FRONT_BUFFER, TANK_WIDTH / 2) };

        private List<Bullet> m_Bullets;

        private int m_Health;

        private float mRotation;
        private float mOldRotation;
        private Vector3 mPosition;
        private Vector3 mOldPosition;

        private float mCannonRotation;
        private float m_TimePrimed; // How long the player held the shoot button
        private int mFired; // Number of frames since the player fired

        private Color mColour;
        private float m_Scale;
        private int m_LeftTrackFrame;
        private int m_RightTrackFrame;

        public Tank(float pXPosition, float pYPosition, float pRotation, Color pColour, List<Bullet> pBullets, float pScale)
        {
            m_Scale = pScale;
            m_Bullets = pBullets;
            mRotation = pRotation;
            mOldRotation = mRotation;
            int screenWidth = Tankontroller.Instance().GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = Tankontroller.Instance().GDM().GraphicsDevice.Viewport.Height;
            mPosition = new Vector3(pXPosition, pYPosition, 0);
            mOldPosition = mPosition;
            mCannonRotation = pRotation;
            m_Health = 5;
            mFired = 0;
            mColour = pColour;
            m_LeftTrackFrame = 1;
            m_RightTrackFrame = 1;
        }

        private void ChangeLeftTrackFrame(int pAmount)
        {
            m_LeftTrackFrame += pAmount;
            m_LeftTrackFrame = Math.Clamp(m_LeftTrackFrame, 1, 14);
            DustInitialisationPolicy dust = new DustInitialisationPolicy(GetLeftFrontCorner(), GetLeftBackCorner());
            ParticleManager.Instance().InitialiseParticles(dust, 4);
        }

        private void ChangeRightTrackFrame(int pAmount)
        {
            m_RightTrackFrame += pAmount;
            m_RightTrackFrame = Math.Clamp(m_RightTrackFrame, 1, 14);
            DustInitialisationPolicy dust = new DustInitialisationPolicy(GetRightFrontCorner(), GetRightBackCorner());
            ParticleManager.Instance().InitialiseParticles(dust, 4);
        }

        public int Health()
        {
            return m_Health;
        }
        public Color Colour()
        {
            return mColour;
        }

        public void Rotate(float pRotate)
        {
            mOldRotation = mRotation;
            mRotation += pRotate;
        }

        public float GetRotation()
        {
            return mRotation;
        }

        public Vector2 GetWorldPosition()
        {
            Matrix localTransformation = Matrix.CreateRotationZ(mRotation) * Matrix.CreateTranslation(mPosition);
            Vector3 v = localTransformation.Translation;
            return new Vector2(v.X, v.Y);
        }

        public void Translate(float distance)
        {
            Vector3 translationVector = new Vector3(distance, 0, 0);
            translationVector = Vector3.Transform(translationVector, Matrix.CreateRotationZ(mRotation));
            mOldPosition = mPosition;
            mPosition += translationVector;
        }

        public Vector2 GetIndexedCorner(int pIndex)
        {
            Vector3 temp = Vector3.Zero;
            temp.X = TANK_CORNERS[pIndex].X;
            temp.Y = TANK_CORNERS[pIndex].Y;
            temp = Vector3.Transform(temp, Matrix.CreateRotationZ(mRotation));
            temp = temp + mPosition;
            return new Vector2(temp.X, temp.Y);
        }

        public Vector2 GetRightFrontCorner()
        {
            return GetIndexedCorner(3);
        }

        public Vector2 GetLeftTrackMidpoint()
        {
            return GetLeftBackCorner();
        }

        public Vector2 GetRightTrackMidpoint()
        {
            return GetRightBackCorner();
        }

        public Vector2 GetRightBackCorner()
        {
            return GetIndexedCorner(2);
        }

        public Vector2 GetLeftFrontCorner()
        {
            return GetIndexedCorner(0);
        }

        public Vector2 GetLeftBackCorner()
        {
            return GetIndexedCorner(1);
        }

        public void GetCorners(Vector2[] pCorners)
        {
            if (pCorners.Length == 4)
            {
                Vector3 temp = Vector3.Zero;
                for (int i = 0; i < 4; i++)
                {
                    temp.X = TANK_CORNERS[i].X * m_Scale;
                    temp.Y = TANK_CORNERS[i].Y * m_Scale;
                    temp = Vector3.Transform(temp, Matrix.CreateRotationZ(mRotation));
                    temp = temp + mPosition;
                    pCorners[i].X = temp.X;
                    pCorners[i].Y = temp.Y;
                }
            }
        }

        public float GetCannonWorldRotation() { return mCannonRotation; }
        public Vector2 GetCannonWorldPosition() { return GetWorldPosition(); }

        public void CannonLeft() { mCannonRotation -= BASE_TURRET_ROTATION_ANGLE; }
        public void CannonRight() { mCannonRotation += BASE_TURRET_ROTATION_ANGLE; }


        public void LeftTrackForward()
        {
            Rotate(BASE_TANK_ROTATION_ANGLE);
            ChangeLeftTrackFrame(1);
            AdvancedTrackRotation(BASE_TANK_ROTATION_ANGLE, true);
        }
        public void RightTrackForward()
        {
            Rotate(-BASE_TANK_ROTATION_ANGLE);
            AdvancedTrackRotation(-BASE_TANK_ROTATION_ANGLE, true);
            ChangeRightTrackFrame(1);
        }
        public void LeftTrackBackward()
        {
            Rotate(-BASE_TANK_ROTATION_ANGLE);
            ChangeLeftTrackFrame(-1);
            AdvancedTrackRotation(-BASE_TANK_ROTATION_ANGLE, false);
        }
        public void RightTrackBackward()
        {
            Rotate(BASE_TANK_ROTATION_ANGLE);
            ChangeRightTrackFrame(-1);
            AdvancedTrackRotation(BASE_TANK_ROTATION_ANGLE, false);
        }
        public void BothTracksForward()
        {
            Translate(TANK_SPEED);
            ChangeLeftTrackFrame(1);
            ChangeRightTrackFrame(1);
        }
        public void BothTracksBackward()
        {
            Translate(-TANK_SPEED);
            ChangeLeftTrackFrame(-1);
            ChangeRightTrackFrame(-1);
        }
        public void BothTracksOpposite(bool clockwise)
        {
            float angle = 2 * BASE_TANK_ROTATION_ANGLE;
            angle = clockwise ? angle : -angle;
            Rotate(angle);

            ChangeLeftTrackFrame(clockwise ? 1 : -1);
            ChangeRightTrackFrame(clockwise ? -1 : 1);
            AdvancedTrackRotation(BASE_TANK_ROTATION_ANGLE, false);
        }

        private void AdvancedTrackRotation(float pAngle, bool pForwards)
        {
            float offsetSqrd = TRACK_OFFSET * TRACK_OFFSET;
            float arcLength = (float)Math.Sqrt(2 * offsetSqrd - 2 * offsetSqrd * Math.Cos(pAngle));
            arcLength = pForwards ? arcLength : arcLength * -1;
            Vector3 translationVector = new Vector3(arcLength, 0, 0);
            translationVector = Vector3.Transform(translationVector, Matrix.CreateRotationZ(mRotation));
            mOldPosition = mPosition;
            mPosition += translationVector;
        }

        public bool PointIsInTank(Vector2 pPoint)
        {
            Vector2[] corners = new Vector2[4];
            GetCorners(corners);
            int i;
            int j;
            bool result = false;
            for (i = 0, j = corners.Length - 1; i < corners.Length; j = i++)
            {
                if ((corners[i].Y > pPoint.Y) != (corners[j].Y > pPoint.Y) &&
                    (pPoint.X < (corners[j].X - corners[i].X) * (pPoint.Y - corners[i].Y) / (corners[j].Y - corners[i].Y) + corners[i].X))
                {
                    result = !result;
                }
            }
            return result;
        }


        public void PrimingWeapon(float pSeconds)
        {
            m_TimePrimed += pSeconds;
        }

        public bool IsFirePrimed()
        {
            return m_TimePrimed > 0;
        }

        public void Fire()
        {
            m_TimePrimed = 0;
            mFired = BLAST_DELAY;
            float cannonRotation = GetCannonWorldRotation();
            Vector2 cannonDirection = new Vector2((float)Math.Cos(cannonRotation), (float)Math.Sin(cannonRotation));
            Vector2 endOfCannon = GetCannonWorldPosition() + cannonDirection * 30;
            m_Bullets.Add(new Bullet(endOfCannon, cannonDirection * BULLET_SPEED, Colour()));
        }

        public void PutBack()
        {
            mPosition = mOldPosition;
            mRotation = mOldRotation;
        }

        public bool Collide(Tank pTank)
        {
            Vector2[] thisTankCorners = new Vector2[4];
            Vector2[] otherTankCorners = new Vector2[4];
            GetCorners(thisTankCorners);
            pTank.GetCorners(otherTankCorners);
            for (int i = 0; i < 4; i++)
            {
                if (PointIsInTank(otherTankCorners[i]) || pTank.PointIsInTank(thisTankCorners[i]))
                {
                    PutBack();
                    pTank.PutBack();
                    return true;
                }
            }
            return false;
        }
        public bool Collide(RectWall pWall)
        {
            Rectangle rectangle = pWall.Rectangle;
            Vector2[] tankCorners = new Vector2[4];
            GetCorners(tankCorners);

            foreach (Vector2 corner in tankCorners)
            {
                if (rectangle.Contains(corner))
                {
                    PutBack();
                    return true;
                }
            }
            // Check if any of the corners of the wall are within the tank
            if (PointIsInTank(new Vector2(rectangle.Left, rectangle.Top)) ||
               PointIsInTank(new Vector2(rectangle.Right, rectangle.Top)) ||
               PointIsInTank(new Vector2(rectangle.Left, rectangle.Bottom)) ||
               PointIsInTank(new Vector2(rectangle.Right, rectangle.Bottom)))
            {
                PutBack();
                return true;
            }
            return false;
        }
        public bool Collide(Bullet pBullet)
        {
            if (pBullet.Collide(this))
            {
                TakeDamage();
                Random rand = new Random();
                int clang = rand.Next(1, 4);
                string tankClang = "Sounds/Tank_Clang" + clang;
                SoundEffectInstance tankClangSound = Tankontroller.Instance().GetSoundManager().GetSoundEffectInstance(tankClang);
                tankClangSound.Play();
                return true;
            }
            return false;
        }

        public bool CheckBulletCollisions(Tank pTank)
        {
            for (int i = 0; i < m_Bullets.Count; ++i)
            {
                if (pTank.Collide(m_Bullets[i]))
                {
                    m_Bullets.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        public bool CheckBulletCollisions(RectWall pWall)
        {
            for (int i = 0; i < m_Bullets.Count; ++i)
            {
                if (m_Bullets[i].Collide(pWall))
                {
                    m_Bullets.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        public bool CheckBulletCollisionsWithPlayArea(Rectangle pRect)
        {
            for (int i = 0; i < m_Bullets.Count; ++i)
            {
                if (m_Bullets[i].CollideWithPlayArea(pRect))
                {
                    m_Bullets.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool CollideWithPlayArea(Rectangle pRectangle)
        {
            Vector2[] tankCorners = new Vector2[4];
            GetCorners(tankCorners);
            foreach (Vector2 corner in tankCorners)
            {
                if (!pRectangle.Contains(corner))
                {
                    PutBack();
                    return true;
                }
            }
            return false;
        }

        public void TakeDamage()
        {
            m_Health--;
            if (m_Health < 0)
            {
                m_Health = 0;
            }
        }

        public void Update(float pSeconds)
        {
            if (mFired > 0)
            {
                mFired--;
            }
            foreach (Bullet bullet in m_Bullets)
            {
                bullet.Update(pSeconds);
            }
        }

        public void Draw(SpriteBatch pSpriteBatch, float pScale)
        {
            Rectangle trackRect = new Rectangle(0, 0, mLeftTrackTexture.Width, mLeftTrackTexture.Height / 15);

            if (m_Health > 0)
            {
                trackRect.Y = m_LeftTrackFrame * mLeftTrackTexture.Height / 15;
                pSpriteBatch.Draw(mLeftTrackTexture, GetWorldPosition(), trackRect, mColour, mRotation, new Vector2(mBaseTexture.Width / 2, mBaseTexture.Height / 2), pScale, SpriteEffects.None, 0.0f);
                trackRect.Y = m_RightTrackFrame * mLeftTrackTexture.Height / 15;
                pSpriteBatch.Draw(mRightTrackTexture, GetWorldPosition(), trackRect, mColour, mRotation, new Vector2(mBaseTexture.Width / 2, mBaseTexture.Height / 2), pScale, SpriteEffects.None, 0.0f);
                pSpriteBatch.Draw(mBaseTexture, GetWorldPosition(), null, mColour, mRotation, new Vector2(mBaseTexture.Width / 2, mBaseTexture.Height / 2), pScale, SpriteEffects.None, 0.0f);
                if (mFired == 0)
                {
                    pSpriteBatch.Draw(mCannonTexture, GetCannonWorldPosition(), null, mColour, mCannonRotation, new Vector2(mCannonTexture.Width / 2, mCannonTexture.Height / 2), pScale, SpriteEffects.None, 0.0f);
                }
                else
                {
                    pSpriteBatch.Draw(mCannonFireTexture, GetCannonWorldPosition(), null, mColour, mCannonRotation, new Vector2(mCannonTexture.Width / 2, mCannonTexture.Height / 2), pScale, SpriteEffects.None, 0.0f);
                }
            }
            else //If a tank has no health, its drawn as a destroyed tank
            {
                pSpriteBatch.Draw(mBrokenTexture, GetWorldPosition(), null, Colour(), GetRotation(), new Vector2(mBrokenTexture.Width / 2, mBrokenTexture.Height / 2), pScale, SpriteEffects.None, 0.0f);
            }
        }

        public void DrawBullets(SpriteBatch pSpriteBatch, Texture2D pTexture)
        {
            foreach (Bullet bullet in m_Bullets)
            {
                bullet.Draw(pSpriteBatch, pTexture);
            }
        }
    }
}
