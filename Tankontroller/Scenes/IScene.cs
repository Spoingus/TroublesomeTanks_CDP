using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

﻿namespace Tankontroller.Scenes
{
    //-------------------------------------------------------------------------------------------------
    // IScene
    //
    // This interface is used to define the methods that a scene must implement. The interface contains
    // methods to draw and update the scene.
    //-------------------------------------------------------------------------------------------------
    public interface IScene
    {
        void Draw(float pSeconds);
        void Update(float pSeconds);
        void Escape();

        SpriteBatch spriteBatch { get; set;  }
    }
}
