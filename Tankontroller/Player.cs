using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Tankontroller.Controller;
using Tankontroller.GUI;
using Tankontroller.World;

namespace Tankontroller
{
    public class Player
    {
        public TeamGUI GUI { get; private set; }
        public Tank Tank { get; private set; }
        public IController Controller { get; private set; }
        public Color Colour { get; private set; }
        public Avatar Avatar { get; private set; }

        public List<Bullet> Bullets { get; private set; }

        public bool ColourSet { get; private set; }
        public bool AvatarSet { get; private set; }
        public int SelectionIndex { get; set; }
        public Player(IController pController)
        {
            Controller = pController;
            ColourSet = false;
            AvatarSet = false;
            SelectionIndex = 0;
        }

        public void AddColour(Color pColour)
        {
            Colour = pColour;
        }
        public void SetColour()
        {
            ColourSet = true;
        }
        public void RemoveColour()
        {
            ColourSet = false;
            Colour = Color.White;
        }
        public void AddAvatar(Avatar pAvatar)
        {
            Avatar = pAvatar;
        }
        public void SetAvatar()
        {
            AvatarSet = true;
        }
        public void RemoveAvatar()
        {
            AvatarSet = false;
            Avatar = null;
        }
        public void SetController(IController pController)
        {
            Controller = pController;
        }
        public Player(Color pColour, IController pController,
            float pTankXPosition, float pTankYPosition, float pTankRotation, float pTankScale,
            Texture2D pWhitePixel,
            Texture2D pHealthBarBlackAndWhiteLayer,
            Texture2D pHealthBarColourLayer, Texture2D pAvatarBlackAndWhiteLayer,
            Texture2D pAvatarColourLayer,
            Rectangle pRectangle)
        {
            Colour = pColour;
            Controller = pController;
            Controller.SetColour(pColour);
            Bullets = new List<Bullet>();
            Tank = new Tank(pTankXPosition, pTankYPosition, pTankRotation, Colour, Bullets, pTankScale);
            GUI = new TeamGUI(pWhitePixel, pHealthBarBlackAndWhiteLayer, pHealthBarColourLayer, pAvatarBlackAndWhiteLayer,
                pAvatarColourLayer, pRectangle, Tank, this, Colour);
        }
        public void GamePreparation(
            float pTankXPosition, float pTankYPosition, float pTankRotation, float pTankScale,
            Texture2D pHealthBarBlackAndWhiteLayer,
            Texture2D pHealthBarColourLayer,
            Rectangle pRectangle)
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            Controller.SetColour(Colour);
            Bullets = new List<Bullet>();
            Tank = new Tank(pTankXPosition, pTankYPosition, pTankRotation, Colour, Bullets, pTankScale);
            Texture2D whitePixel = game.CM().Load<Texture2D>("white_pixel");
            GUI = new TeamGUI(whitePixel, pHealthBarBlackAndWhiteLayer, pHealthBarColourLayer, Avatar, pRectangle, Tank, this, Colour);
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
                        //Tank.LeftTrackForward();
                        //Tank.RightTrackBackward();
                        Tank.BothTracksOpposite(true);
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
                        //Tank.LeftTrackBackward();
                        //Tank.RightTrackForward();
                        Tank.BothTracksOpposite(false);
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
                        //Tank.LeftTrackBackward();
                        //Tank.RightTrackForward();
                        Tank.BothTracksOpposite(false);
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
                        //Tank.LeftTrackForward();
                        //Tank.RightTrackBackward();
                        Tank.BothTracksOpposite(true);
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
