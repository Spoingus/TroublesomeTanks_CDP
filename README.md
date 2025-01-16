Scenes - Class Diagram
```mermaid
classDiagram
    IScene <|-- FlashScreenScene
    IScene <|-- GameScene
    IScene <|-- GameOverScene
    IScene <|-- PlayerSelectionScene
    IScene <|-- StartScene
    IScene <|-- TransitionScene
    SceneManager <-- IScene
    <<Interface>> IScene

    class IScene{
    +Spritebatch mSpriteBatch
    +Draw(float pSeconds) void
    +Update(float pSeconds) void
    +Escape() void
    }
    class FlashScreenScene{
        +Texture2D mLogotexture
        +Rectangle mRectangle
    }
    class GameScene{
        +Setup4Player() void
        +SetupNot4Player() void
        +SetupPlayers() void
    }
    class GameOverScene{
        +RepositionGUIs() void
    }
    class PlayerSelectionScene{
        +PrepareAvatarPickers() void
        +UpdateAvatarPickers() void
    }
    class StartScene{
        +ExitGame() void
        +StartGame() void
    }
    class TransitionScene{
        +GeneratePreviousTexture() void
        +GenerateNextTexture() void
    }
```
GUIs - Class Diagram
```mermaid
classDiagram
    ButtonList *-- Button
    class ButtonList{
        List~Button~ mButtons
        +SelectNextButton() void
        +SelectPreviousButton() void
    }
    class Button{
        +Texture2D Texture
        +Texture2D TexturePressed
        +Color SelectedColour
        +Press
    }

    TeamGUI *-- Avatar
    TeamGUI *-- HealthBar
    TeamGUI *-- Player
    TeamGUI *-- Tank
    Tank *-- HealthBar
    class TeamGUI{
        +Avatar m_Avatar
        +HealthBar m_HealthBar
        +Player m_Player
        +Tank m_Tank
        +PrepareAvatar(Avatar)
        +DrawHealthBar(SpriteBatch)
        +DrawAvatar(SpriteBatch)
    }
    class Avatar{
        -Color m_Colour
        -string m_Name
        -PrepareDrawrectangles()
        +Draw()
    }
    class HealthBar{
        +Tank m_Tank
        +Texture2D mHeartColour
        +PrepareRectangles()
        +Draw()
    }
    Player *-- TeamGUI
    Player *-- Tank
    Player *-- Avatar
    Player *-- IController
    Player *-- Bullet
    class Player{
        +TeamGUI GUI
        +Tank Tank
        +IController Controller
        +Color Colour
        +Avatar Avatar
        List~Bullet~ Bullets
        +DoTankControls() bool
    }
    Tank *-- Bullet
    class Tank{
        List~Bullet~ m_Bullets
        -Texture2D TankTextures
        +Fire() void
        +TankMovements() void
        +TankTurrentMovements() vois
        +Collide(Tank) void
        +CollideWithPlayArea(Rectangle) void
    }
    class Bullet{
        +Vector2 Position
        +Vector2 Velocity
        +Color Colour
        +Collide(Rectangle, out Vector2)
        +Collide(Tank, out Vector2)
    }
   
```
