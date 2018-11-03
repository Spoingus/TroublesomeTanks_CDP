using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankontroller.World;

namespace Tankontroller
{
    public class Player
    {
        public TeamGUI GUI { get; private set; }
        public Tank Tank { get; private set; }
        public IController Controller { get; private set; }
        public Color Colour { get; private set; }

        public List<Bullet> Bullets { get; private set; }

        public Player(Color pColour, IController pController, 
            float pTankXPosition, float pTankYPosition, float pTankRotation, float pTankScale,
            Texture2D pHealthBar1, Texture2D pHealthBar2, Texture2D pHealthBar3, Texture2D pHealthBar4, Texture2D pHealthBar5, Texture2D pHealthBar6, 
            Rectangle pRectangle, bool pIsOnLeft)
        {
            Colour = pColour;
            Controller = pController;
            Controller.SetColour(pColour);
            Bullets = new List<Bullet>();
            Tank = new Tank(pTankXPosition, pTankYPosition, pTankRotation, Colour, Bullets, pTankScale);
            GUI = new TeamGUI(pHealthBar1, pHealthBar2, pHealthBar3, pHealthBar4, pHealthBar5, pHealthBar6, pRectangle, Tank, pIsOnLeft, Controller, Colour);
        }
        public void Reset()
        {
            Controller.ResetJacks();
            
        }
        public bool DoTankControls(float pSeconds)
        {
            bool tankMoved = false;
            if (Tank.Health() > 0)
            {
                
                if (Controller.IsPressedWithCharge(Control.LEFT_TRACK_FORWARDS))
                {
                    tankMoved = true;
                    if (Controller.IsPressedWithCharge(Control.RIGHT_TRACK_FORWARDS))
                    {
                        Tank.BothTracksForward();
                        Controller.DepleteCharge(Control.RIGHT_TRACK_FORWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                        Controller.DepleteCharge(Control.LEFT_TRACK_FORWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                    }
                    else if (Controller.IsPressedWithCharge(Control.RIGHT_TRACK_BACKWARDS))
                    {
                        Tank.LeftTrackForward();
                        Tank.RightTrackBackward();
                        Controller.DepleteCharge(Control.RIGHT_TRACK_BACKWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                        Controller.DepleteCharge(Control.LEFT_TRACK_FORWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                    }
                    else
                    {
                        Tank.LeftTrackForward();
                        Controller.DepleteCharge(Control.LEFT_TRACK_FORWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                    }
                }
                else if (Controller.IsPressedWithCharge(Control.LEFT_TRACK_BACKWARDS))
                {
                    tankMoved = true;
                    if (Controller.IsPressedWithCharge(Control.RIGHT_TRACK_BACKWARDS))
                    {
                        Tank.BothTracksBackward();
                        Controller.DepleteCharge(Control.LEFT_TRACK_BACKWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                        Controller.DepleteCharge(Control.RIGHT_TRACK_BACKWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                    }
                    else if (Controller.IsPressedWithCharge(Control.RIGHT_TRACK_FORWARDS))
                    {
                        Tank.LeftTrackBackward();
                        Tank.RightTrackForward();
                        Controller.DepleteCharge(Control.RIGHT_TRACK_FORWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                        Controller.DepleteCharge(Control.LEFT_TRACK_BACKWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                    }
                    else
                    {
                        Tank.LeftTrackBackward();
                        Controller.DepleteCharge(Control.LEFT_TRACK_BACKWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                    }
                }
                else if (Controller.IsPressedWithCharge(Control.RIGHT_TRACK_FORWARDS))
                {
                    tankMoved = true;
                    if (Controller.IsPressedWithCharge(Control.LEFT_TRACK_FORWARDS))
                    {
                        Tank.BothTracksForward();
                        Controller.DepleteCharge(Control.RIGHT_TRACK_FORWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                        Controller.DepleteCharge(Control.LEFT_TRACK_FORWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                    }
                    else if (Controller.IsPressedWithCharge(Control.LEFT_TRACK_BACKWARDS))
                    {
                        Tank.LeftTrackBackward();
                        Tank.RightTrackForward();
                        Controller.DepleteCharge(Control.RIGHT_TRACK_FORWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                        Controller.DepleteCharge(Control.LEFT_TRACK_BACKWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                    }
                    else
                    {
                        Tank.RightTrackForward();
                        Controller.DepleteCharge(Control.RIGHT_TRACK_FORWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                    }
                }
                else if (Controller.IsPressedWithCharge(Control.RIGHT_TRACK_BACKWARDS))
                {
                    tankMoved = true;
                    if (Controller.IsPressedWithCharge(Control.LEFT_TRACK_BACKWARDS))
                    {
                        Tank.BothTracksBackward();
                        Controller.DepleteCharge(Control.LEFT_TRACK_BACKWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                        Controller.DepleteCharge(Control.RIGHT_TRACK_BACKWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                    }
                    else if (Controller.IsPressedWithCharge(Control.LEFT_TRACK_FORWARDS))
                    {
                        Tank.LeftTrackForward();
                        Tank.RightTrackBackward();
                        Controller.DepleteCharge(Control.RIGHT_TRACK_BACKWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                        Controller.DepleteCharge(Control.LEFT_TRACK_FORWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                    }
                    else
                    {
                        Tank.RightTrackBackward();
                        Controller.DepleteCharge(Control.RIGHT_TRACK_BACKWARDS, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                    }
                }

                if (Controller.IsPressedWithCharge(Control.TURRET_LEFT))
                {
                    Tank.CannonLeft();
                    Controller.DepleteCharge(Control.TURRET_LEFT, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);
                }
                else if (Controller.IsPressedWithCharge(Control.TURRET_RIGHT))
                {
                    Tank.CannonRight();
                    Controller.DepleteCharge(Control.TURRET_RIGHT, DGS.Instance.GetFloat("TRACK_DEPLETION_RATE") * pSeconds);

                }

                if (Controller.IsPressedWithCharge(Control.FIRE))
                {
                    Tank.PrimingWeapon(pSeconds);
                }
                else
                {
                    if (Tank.FireIfPrimed())
                    {
                        SoundEffectInstance bulletShot = Tankontroller.Instance().GetSoundManager().GetSoundEffectInstance("Sounds/Tank_Gun");
                        bulletShot.Play();
                        Controller.DepleteCharge(Control.FIRE, DGS.Instance.GetFloat("BULLET_CHARGE_DEPLETION")); // BULLET CHARGE HERE
                        Tank.SetFired(DGS.Instance.GetInt("BLAST_DELAY"));
                    }
                }

                if (Controller.IsPressed(Control.RECHARGE))
                {
                    if (!Tank.ChargeDown)
                    {
                        Tank.ChargePressed();
                        Controller.AddCharge(Control.RECHARGE, DGS.Instance.GetFloat("CHARGE_AMOUNT"));
                    }
                }
                else
                {
                    Tank.ChargeReleased();
                }

                

               
            }
            if (Tank.Fired() > 0)
            {
                Tank.DecFired();
            }
            return tankMoved;
        }
    }
}
