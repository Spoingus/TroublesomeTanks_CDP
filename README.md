```mermaid
classDiagram
    IScene <|-- FlashScreenScene
    IScene <|-- GameScene
    IScene <|-- GameOverScene
    IScene <|-- PlayerSelectionScene
    IScene <|-- StartScene
    IScene <|-- TransitionScene
    <<Interface>> IScene
    IScene : +Draw(float pSeconds) void
    IScene : +Update(float pSeconds) void
    IScene : +Escape() void

    Class FlashScreenScene {
    +Texture2D mLogotexture
    +Spritebatch mSpriteBatch
    +Rectangle mRectangle
    +float mSecondsLeft
    }
```
