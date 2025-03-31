using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankontroller.Controller;

namespace Tankontroller.Scenes
{
    public class PickupAndBulletScene : IScene
    {
        private Tankontroller mGameInstance;
        private MainMenuScene mStartScene;
        public PickupAndBulletScene(MainMenuScene startScene)
        {
            mStartScene = startScene;
            mGameInstance = (Tankontroller)Tankontroller.Instance();
        }

        public override void Draw(float pSeconds)
        {
            
        }

        public override void Update(float pSeconds)
        {
            Escape();
            mGameInstance.GetControllerManager().DetectControllers();

            foreach (IController controller in mGameInstance.GetControllerManager().GetControllers())
            {
                controller.UpdateController();
                if (controller.IsPressed(Control.FIRE))
                {
                    IGame game = Tankontroller.Instance();
                    game.GetControllerManager().SetAllTheLEDsWhite();
                    game.SM().Transition(null);
                }
            }
        }
        public override void Escape()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                mGameInstance.SM().Transition(mStartScene, true);
            }
        }


    }
}
