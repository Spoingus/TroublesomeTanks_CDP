```mermaid
classDiagram
    IScene <|-- FlashScreenScene
    IScene <|-- GameScene
    IScene <|-- GameOverScene
    IScene <|-- PlayerSelectionScene
    IScene <|-- StartScene
    IScene <|-- TransitionScene
    <<Interface>> IScene

    IScene : +float mSecondsLeft
    IScene : +Spritebatch mSpriteBatch
    IScene : +Draw(float pSeconds) void
    IScene : +Update(float pSeconds) void
    IScene : +Escape() void

    class FlashScreenScene{
        +Texture2D mLogotexture
        +Rectangle mRectangle
    }
```
