using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Tankontroller.World;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Tankontroller.Controller
{
    public enum Control { LEFT_TRACK_FORWARDS = 0, LEFT_TRACK_BACKWARDS = 1, RIGHT_TRACK_FORWARDS = 2, RIGHT_TRACK_BACKWARDS = 3, FIRE = 4, RECHARGE = 5, NONE = 6, TURRET_LEFT = 7, TURRET_RIGHT = 8 };

    public interface IController
    {

        bool IsPressedWithCharge(Control pControl);
        void DepleteCharge(Control pControl, float amount);
        void AddCharge(Control pControl, float amount);
        float GetJackCharge(int pJackIndex);
        Control GetJackControl(int pJackIndex);
        bool IsPressed(Control pControl);
        void ResetJacks();
        void TurnOffLights();
        void TurnOnLights();

        void SetColour(Color pColour);

        void UpdateController();
    }

    public abstract class Controller : IController
    {
        protected class LEDArray
        {
            public int[] LED_IDS = new int[4];
            public LEDArray()
            {

            }
        }
        protected class Jack
        {
            public Control Control;
            public bool IsDown;
            public int[] LED_IDS = new int[4];
            public float charge;


            public Jack()
            {
                ResetCharge();
            }
            public void ResetCharge()
            {
                charge = DGS.Instance.GetFloat("STARTING_CHARGE");
            }
        };
        protected Color mColour;
        protected Jack[] mJacks;
        protected LEDArray mLeds;
        protected bool mLightsOn;

        protected Controller()
        {
            mJacks = new Jack[7] { new Jack(), new Jack(), new Jack(), new Jack(), new Jack(), new Jack(), new Jack() };
            mLeds = new LEDArray();
            mLightsOn = true;
        }

        public void SetColour(Color pColour)
        {
            mColour = pColour;
        }
        public void ResetJacks()
        {
            foreach (Jack j in mJacks)
            {
                j.ResetCharge();
            }
        }

        public void TurnOffLights()
        {
            mLightsOn = false;
            UpdateController();
        }
        public void TurnOnLights()
        {
            mLightsOn = true;
        }

        public Control GetJackControl(int pJackIndex)
        {
            return mJacks[pJackIndex].Control;
        }

        public float GetJackCharge(int pJackIndex)
        {
            return mJacks[pJackIndex].charge;
        }

        public abstract void UpdateController();

        public bool IsPressedWithCharge(Control pControl)
        {
            for (int i = 0; i < 7; ++i)
            {
                if (mJacks[i].Control == pControl)
                {
                    return mJacks[i].IsDown && (pControl == Control.FIRE && mJacks[i].charge >= DGS.Instance.GetFloat("BULLET_CHARGE_DEPLETION") || pControl != Control.FIRE && mJacks[i].charge > 0);
                }
            }
            return false;
        }
        public void DepleteCharge(Control pControl, float amount)
        {
            return;
            for (int i = 0; i < 7; ++i)
            {
                if (mJacks[i].Control == pControl)
                {
                    if (mJacks[i].charge > amount)
                    {

                        mJacks[i].charge -= amount;
                    }
                    else
                        mJacks[i].charge = 0;
                }
            }
        }

        public void AddCharge(Control pControl, float amount)
        {
            for (int i = 0; i < 7; ++i)
            {
                if (mJacks[i].Control == pControl)
                {
                    if (mJacks[i].charge < DGS.Instance.GetFloat("MAX_CHARGE") - amount)
                    {

                        mJacks[i].charge += amount;
                    }
                    else
                        mJacks[i].charge = DGS.Instance.GetFloat("MAX_CHARGE");
                }
            }
        }

        public bool IsPressed(Control pControl)
        {
            for (int i = 0; i < 7; ++i)
            {
                if (mJacks[i].Control == pControl)
                {
                    return mJacks[i].IsDown;
                }
            }
            return false;
        }


    }

    public class KeyboardController : Controller
    {
        private Dictionary<Keys, Control> m_KeyMap;
        private Dictionary<Keys, int> m_PortMap;

        public KeyboardController(Dictionary<Keys, Control> pKeyMap, Dictionary<Keys, int> pPortMap) : base()
        {
            m_KeyMap = pKeyMap;
            m_PortMap = pPortMap;

            mJacks[0].Control = Control.LEFT_TRACK_FORWARDS;
            mJacks[1].Control = Control.LEFT_TRACK_BACKWARDS;
            mJacks[2].Control = Control.RIGHT_TRACK_FORWARDS;
            mJacks[3].Control = Control.RIGHT_TRACK_BACKWARDS;
            mJacks[4].Control = Control.FIRE;
            mJacks[5].Control = Control.TURRET_LEFT;
            mJacks[6].Control = Control.TURRET_RIGHT;
        }

        public override void UpdateController()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            int rechargeJack = 0;

            for (int i = 0; i < mJacks.Length; i++)
            {
                if (mJacks[i].Control == Control.RECHARGE)
                {
                    rechargeJack = i;
                }
            }

            foreach (KeyValuePair<Keys, int> kvp in m_PortMap)
            {
                if (keyboardState.IsKeyDown(kvp.Key))
                {
                    mJacks[rechargeJack].Control = mJacks[kvp.Value].Control;
                    mJacks[kvp.Value].Control = Control.RECHARGE;
                    break;
                }
            }

            foreach (KeyValuePair<Keys, Control> kvp in m_KeyMap)
            {
                for (int i = 0; i < mJacks.Length; i++)
                {
                    if (mJacks[i].Control == kvp.Value)
                    {
                        mJacks[i].IsDown = keyboardState.IsKeyDown(kvp.Key);
                    }
                }
            }
        }
    }

    public class ModularController : Controller
    {
        private Hacktroller mHacktroller;

        public ModularController(Hacktroller pHackTroller) : base()
        {

            mJacks[5].LED_IDS = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
            mJacks[4].LED_IDS = new int[8] { 8, 9, 10, 11, 12, 13, 14, 15 };
            mJacks[3].LED_IDS = new int[8] { 16, 17, 18, 19, 20, 21, 22, 23 };
            mJacks[2].LED_IDS = new int[8] { 24, 25, 26, 27, 28, 29, 30, 31 };
            mJacks[1].LED_IDS = new int[8] { 32, 33, 34, 35, 36, 37, 38, 39 };
            mJacks[0].LED_IDS = new int[8] { 40, 41, 42, 43, 44, 45, 46, 47 };
            mJacks[6].LED_IDS = new int[8] { 48, 49, 50, 51, 52, 53, 54, 55 };
            mLeds.LED_IDS = new int[5] { 56, 57, 58, 59, 60 };
            mHacktroller = pHackTroller;

            PullDataThread();
        }

        public void PullDataThread()
        {
            Thread.Sleep(10);
            Thread UpdateController = new Thread(new ThreadStart(PullData));
            UpdateController.Start();
        }

        private async Task UpdateColors()
        {
            ControllerColor[] result = new ControllerColor[61];

            for (int i = 0; i < result.Length; i++)
            {
                result[i].R = 0;
                result[i].G = 0;
                result[i].B = 0;
            }
            if (mLightsOn)
            {
                foreach (Jack J in mJacks)
                {
                    // red empty orange 33% yellow 66% green 100% split into 4 chunks so we get 8.25% splits
                    // J.charge should be between 0 and DGS.MAX_CHARGE
                    int FullByte = 50;
                    float brightness = 0.2f;
                    float decimalCharge = J.charge / DGS.Instance.GetFloat("MAX_CHARGE");
                    float remainingCharge = 8 * decimalCharge;
                    foreach (int i in J.LED_IDS)
                    {
                        if (remainingCharge >= 1)
                        {
                            result[i].R = (byte)(mColour.R * brightness);
                            result[i].G = (byte)(mColour.G * brightness);
                            result[i].B = (byte)(mColour.B * brightness);
                        }
                        else
                        {
                            result[i].R = 0;
                            result[i].G = 0;
                            result[i].B = 0;
                        }
                        remainingCharge--;
                    }
                    foreach (int i in mLeds.LED_IDS)
                    {
                        result[i].R = 30;
                        result[i].G = 30;
                        result[i].B = 30;
                    }
                }
            }


            await mHacktroller.SetColor(result);
        }
        Task updateColourTask = null;
        public override void UpdateController()
        {
            if (mLightsOn && (updateColourTask == null || updateColourTask.IsCompleted))
            {
                updateColourTask = Task.Run(async () => await UpdateColors());
            }
        }

        private void PullData()
        {
            while (mLightsOn)
            {
                PortState[] ports = mHacktroller.GetPorts();

                if (ports == null)
                {
                    mLightsOn = false;
                    return;
                }
                for (int i = 0; i < ports.Length; ++i)
                {
                    mJacks[i].IsDown = ports[i].FirePressed;

                    if (ports[i].Controller == ControllerState.NOT_CONNECTED)
                    {
                        mJacks[i].Control = Control.NONE;
                    }


                    else if (ports[i].Controller == ControllerState.LEFT_TRACK_FORWARDS)
                    {
                        mJacks[i].Control = Control.LEFT_TRACK_FORWARDS;
                    }
                    else if (ports[i].Controller == ControllerState.LEFT_TRACK_FORWARDS_PRESSED)
                    {
                        mJacks[i].Control = Control.LEFT_TRACK_FORWARDS;
                        mJacks[i].IsDown = true;
                    }


                    else if (ports[i].Controller == ControllerState.LEFT_TRACK_BACKWARDS)
                    {
                        mJacks[i].Control = Control.LEFT_TRACK_BACKWARDS;
                    }
                    else if (ports[i].Controller == ControllerState.LEFT_TRACK_BACKWARDS_PRESSED)
                    {
                        mJacks[i].Control = Control.LEFT_TRACK_BACKWARDS;
                        mJacks[i].IsDown = true;
                    }


                    else if (ports[i].Controller == ControllerState.RIGHT_TRACK_FORWARDS)
                    {
                        mJacks[i].Control = Control.RIGHT_TRACK_FORWARDS;
                    }
                    else if (ports[i].Controller == ControllerState.RIGHT_TRACK_FORWARDS_PRESSED)
                    {
                        mJacks[i].Control = Control.RIGHT_TRACK_FORWARDS;
                        mJacks[i].IsDown = true;
                    }


                    else if (ports[i].Controller == ControllerState.RIGHT_TRACK_BACKWARDS)
                    {
                        mJacks[i].Control = Control.RIGHT_TRACK_BACKWARDS;
                    }
                    else if (ports[i].Controller == ControllerState.RIGHT_TRACK_BACKWARDS_PRESSED)
                    {
                        mJacks[i].Control = Control.RIGHT_TRACK_BACKWARDS;
                        mJacks[i].IsDown = true;
                    }


                    else if (ports[i].Controller == ControllerState.TURRET_LEFT)
                    {
                        mJacks[i].Control = Control.TURRET_LEFT;
                    }
                    else if (ports[i].Controller == ControllerState.TURRET_LEFT_PRESSED)
                    {
                        mJacks[i].Control = Control.TURRET_LEFT;
                        mJacks[i].IsDown = true;
                    }

                    else if (ports[i].Controller == ControllerState.TURRET_RIGHT)
                    {
                        mJacks[i].Control = Control.TURRET_RIGHT;
                    }
                    else if (ports[i].Controller == ControllerState.TURRET_RIGHT_PRESSED)
                    {
                        mJacks[i].Control = Control.TURRET_RIGHT;
                        mJacks[i].IsDown = true;
                    }


                    else if (ports[i].Controller == ControllerState.CHARGE)
                    {
                        mJacks[i].Control = Control.RECHARGE;
                    }
                    else if (ports[i].Controller == ControllerState.CHARGE_PRESSED)
                    {
                        mJacks[i].Control = Control.RECHARGE;
                        mJacks[i].IsDown = true;
                    }


                    else if (ports[i].Controller == ControllerState.FIRE)
                    {
                        mJacks[i].Control = Control.FIRE;
                    }
                    else if (ports[i].Controller == ControllerState.FIRE_PRESSED)
                    {
                        mJacks[i].Control = Control.FIRE;
                        mJacks[i].IsDown = true;
                    }


                    else if (ports[i].Controller == ControllerState.NO_MATCH)
                    {
                    }
                }
            }
        }
    }
}
