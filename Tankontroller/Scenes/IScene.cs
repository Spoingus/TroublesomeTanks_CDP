namespace Tankontroller.Scenes
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
    }
}
