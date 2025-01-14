using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tankontroller.World.Particles;


namespace Tankontroller.World
{
    public class Tank
    {

        private Vector2[] TANK_CORNERS = { new Vector2(DGS.Instance.GetInt("TANK_HEIGHT") / 2 - DGS.Instance.GetInt("TANK_FRONT_BUFFER"), -DGS.Instance.GetInt("TANK_WIDTH") / 2), new Vector2(-DGS.Instance.GetInt("TANK_HEIGHT") / 2, -DGS.Instance.GetInt("TANK_WIDTH") / 2), new Vector2(-DGS.Instance.GetInt("TANK_HEIGHT") / 2, DGS.Instance.GetInt("TANK_WIDTH") / 2), new Vector2(DGS.Instance.GetInt("TANK_HEIGHT") / 2 - DGS.Instance.GetInt("TANK_FRONT_BUFFER"), DGS.Instance.GetInt("TANK_WIDTH") / 2) };

        private float mRotation;
        private float mOldRotation;
        private Vector3 mPosition;
        private Vector3 mOldPosition;
        private float mCannonRotation;
        private float m_TimePrimed;
        private int m_Health;
        private int mFired;
        private Color mColour;
        private List<Bullet> m_Bullets;
        private int m_LeftTrackFrame;
        private int m_RightTrackFrame;
        private float m_Scale;

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
            ChargeDown = false;
            m_LeftTrackFrame = 1;
            m_RightTrackFrame = 1;
        }

        public List<Bullet> GetBulletList()
        {
            return m_Bullets;
        }

        private void LeftTrackFrameForwards()
        {
            m_LeftTrackFrame++;
            if (m_LeftTrackFrame > 14)
            {
                m_LeftTrackFrame = 1;
            }
            DustInitialisationPolicy dust = new DustInitialisationPolicy(GetLeftFrontCorner(), GetLeftBackCorner());
            ParticleManager.Instance().InitialiseParticles(dust, 4);
        }

        private void LeftTrackFrameBackwards()
        {
            m_LeftTrackFrame--;
            if (m_LeftTrackFrame < 1)
            {
                m_LeftTrackFrame = 14;
            }
            DustInitialisationPolicy dust = new DustInitialisationPolicy(GetLeftFrontCorner(), GetLeftBackCorner());
            ParticleManager.Instance().InitialiseParticles(dust, 4);
        }

        private void RightTrackFrameForwards()
        {
            m_RightTrackFrame++;
            if (m_RightTrackFrame > 14)
            {
                m_RightTrackFrame = 1;
            }
            DustInitialisationPolicy dust = new DustInitialisationPolicy(GetRightFrontCorner(), GetRightBackCorner());
            ParticleManager.Instance().InitialiseParticles(dust, 4);
        }

        private void RightTrackFrameBackwards()
        {
            m_RightTrackFrame--;
            if (m_RightTrackFrame < 1)
            {
                m_RightTrackFrame = 14;
            }
            DustInitialisationPolicy dust = new DustInitialisationPolicy(GetRightFrontCorner(), GetRightBackCorner());
            ParticleManager.Instance().InitialiseParticles(dust, 4);
        }

        public bool ChargeDown { get; private set; }

        public int Health()
        {
            return m_Health;
        }

        public int Fired()
        {
            return mFired;
        }
        public void DecFired()
        {
            mFired--;
        }
        public Color Colour()
        {
            return mColour;
        }
        public void SetFired(int pDelay)
        {
            mFired = pDelay;
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
            Vector3 v = LocalTransform.Translation;
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

        public void ChargePressed()
        {
            ChargeDown = true;
        }

        public void ChargeReleased()
        {
            ChargeDown = false;
        }

        public float GetCannonWorldRotation() { return mCannonRotation; }
        public Vector2 GetCannonWorldPosition()
        {
            return GetWorldPosition();
        }
        public void BothTracksForward()
        {
            Translate(DGS.Instance.GetFloat("TANK_SPEED"));
            RightTrackFrameForwards();
            LeftTrackFrameForwards();
        }
        public void BothTracksBackward()
        {
            Translate(-DGS.Instance.GetFloat("TANK_SPEED"));
            RightTrackFrameBackwards();
            LeftTrackFrameBackwards();
        }
        public void LeftTrackForward()
        {
            Rotate(DGS.Instance.GetFloat("BASE_TANK_ROTATION_ANGLE"));
            LeftTrackFrameForwards();
            AdvancedTrackRotation(DGS.Instance.GetFloat("BASE_TANK_ROTATION_ANGLE"), true);
        }

        private void AdvancedTrackRotation(float pAngle, bool pForwards)
        {
            float arcLength = (float)Math.Sqrt(2 * DGS.Instance.GetFloat("TRACK_OFFSET_SQRD") - 2 * DGS.Instance.GetFloat("TRACK_OFFSET_SQRD") * Math.Cos(pAngle));
            arcLength = pForwards ? arcLength : arcLength * -1;
            Vector3 translationVector = new Vector3(arcLength, 0, 0);
            translationVector = Vector3.Transform(translationVector, Matrix.CreateRotationZ(mRotation));
            mOldPosition = mPosition;
            mPosition += translationVector;
        }
        public void RightTrackForward()
        {
            Rotate(-DGS.Instance.GetFloat("BASE_TANK_ROTATION_ANGLE"));
            AdvancedTrackRotation(-DGS.Instance.GetFloat("BASE_TANK_ROTATION_ANGLE"), true);
            RightTrackFrameForwards();
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

        public void LeftTrackBackward()
        {
            Rotate(-DGS.Instance.GetFloat("BASE_TANK_ROTATION_ANGLE"));
            LeftTrackFrameBackwards();
            AdvancedTrackRotation(-DGS.Instance.GetFloat("BASE_TANK_ROTATION_ANGLE"), false);
        }
        public void RightTrackBackward()
        {
            Rotate(DGS.Instance.GetFloat("BASE_TANK_ROTATION_ANGLE"));
            RightTrackFrameBackwards();
            AdvancedTrackRotation(DGS.Instance.GetFloat("BASE_TANK_ROTATION_ANGLE"), false);
        }

        public void BothTracksOpposite(bool clockwise)
        {
            float angle = 2 * DGS.Instance.GetFloat("BASE_TANK_ROTATION_ANGLE");
            if (!clockwise)
            {
                angle *= -1;
            }
            Rotate(angle);

            if (clockwise)
            {
                LeftTrackFrameForwards();
                RightTrackFrameBackwards();
            }
            else
            {
                LeftTrackFrameBackwards();
                RightTrackFrameForwards();
            }
            AdvancedTrackRotation(DGS.Instance.GetFloat("BASE_TANK_ROTATION_ANGLE"), false);
        }
        public void CannonLeft() { mCannonRotation -= DGS.Instance.GetFloat("BASE_TURRET_ROTATION_ANGLE"); }
        public void CannonRight() { mCannonRotation += DGS.Instance.GetFloat("BASE_TURRET_ROTATION_ANGLE"); }

        public Matrix LocalTransform
        {
            get
            {
                return Matrix.CreateRotationZ(mRotation) * Matrix.CreateTranslation(mPosition);
            }
        }

        public int LeftTrackFrame()
        {
            return m_LeftTrackFrame;
        }

        public int RightTrackFrame()
        {
            return m_RightTrackFrame;
        }

        public void PrimingWeapon(float pSeconds)
        {
            m_TimePrimed += pSeconds;
        }

        public bool IsFirePrimed()
        {
            return m_TimePrimed > 0;
        }

        public void ResetPrime()
        {
            m_TimePrimed = 0;
        }

        public void Fire()
        {
            float cannonRotation = GetCannonWorldRotation();
            Vector2 cannonDirection = new Vector2((float)Math.Cos(cannonRotation), (float)Math.Sin(cannonRotation));
            Vector2 endOfCannon = GetCannonWorldPosition() + cannonDirection * 30;
            m_Bullets.Add(new Bullet(endOfCannon, cannonDirection * DGS.Instance.GetFloat("BULLET_SPEED"), Colour()));

            /*     if(m_TimePrimed > 0)
                 {
                     float cannonRotation = GetCannonWorldRotation();

                     float bulletSpeed = m_TimePrimed * DGS.BULLET_SPEED_SCALER;

                     Vector2 cannonDirection = new Vector2((float)Math.Cos(cannonRotation), (float)Math.Sin(cannonRotation));
                     Vector2 endOfCannon = GetCannonWorldPosition() + cannonDirection * 30;
                     m_Bullets.Add(new Bullet(endOfCannon, cannonDirection * bulletSpeed, m_TimePrimed));
                     m_TimePrimed = -1;
                     return true;
                 }
                 return false;
             */
        }

        public void PutBack()
        {
            mPosition = mOldPosition;
            mRotation = mOldRotation;
        }

        public bool Collide(Tank pTank)
        {
            /*
            Vector2 tankPos = pTank.GetWorldPosition();
            float tankRot = pTank.GetRotation();
            Vector2 tankPos2 = GetWorldPosition();

            if ((tankPos - tankPos2).Length() < 2 * DGS.TANK_RADIUS)
            {
                return true;
            }

            return false;
             * */
            Vector2[] thisTankCorners = new Vector2[4];
            Vector2[] otherTankCorners = new Vector2[4];
            GetCorners(thisTankCorners);
            pTank.GetCorners(otherTankCorners);

            for (int i = 0; i < 4; i++)
            {
                if (PointIsInTank(otherTankCorners[i]) || pTank.PointIsInTank(thisTankCorners[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public void CollideWithPlayArea(Rectangle pRectangle)
        {
            Vector2 tankPos = GetWorldPosition();
            if ((tankPos.X <= pRectangle.Left + DGS.Instance.GetFloat("TANK_RADIUS")) ||
                (tankPos.X >= pRectangle.Right - DGS.Instance.GetFloat("TANK_RADIUS")) ||
                (tankPos.Y >= pRectangle.Bottom - DGS.Instance.GetFloat("TANK_RADIUS")) ||
                (tankPos.Y <= pRectangle.Top + DGS.Instance.GetFloat("TANK_RADIUS")))
            {
                PutBack();
            }
        }

        public void TakeDamage()
        {
            m_Health--;
            if (m_Health < 0)
            {
                m_Health = 0;
            }
        }
    }
}
