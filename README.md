# Troublesome Tanks!

Troublesome tanks is a top down tank shooter where up to four teams of three compete against each other in the hopes of becoming the most troubling tank. However, its never that simple. With the custom controller created by Spooky Elephant, each team has to manage the power of their tank by plugging/unplugging each part of the tank and using their charger to stay topped up on power. Can you maintain strong team work and leverage the powerful pickups to your advantage or will your team become a pile of scrap metal!

## How to Setup and Play

Download the latest release version of Troublesome Tanksfrom the release tab on the main repository and extract the files. The two main items of interest is the Tankontroller application file and the DGS.txt file in the Content folder. The DGS file allows for changes to be made to certain areas of the game, such as:

- Change the resolution
- Adjust the tank properties such as speed and health
- Change if pickups will spawn as well as what type of pickups can spawn
- What controllers can be used on the main menu and if a keyboard controller is needed

The Tankontroller application is wht needs to be run to launch the game.

## The Controls

### Tank Controller

### Keyboard




# Troublesome Tanks - Commercial Development Practice

## Description:

Troublesome Tanks is a project created by the Spooky Elephants team, managed by David Parker. This repository is for the CDP version of the project (Commercial Development Practice), the version consists of additions and changes to the code base at the requst of David as the teams client.
The repository is a fork of the 3DP version (3D printed), which is the original repository for the project using the 3D printed controllers. The aim of this project is to create a more feature rich and complete game experience than what was offered before, updating and reworking the exisiting code to allow for new features.

## New Features:
The CDP version of the game has implemented a number of new features, here is a brief list of those features:

- Map Loading From Json Files
- Power Up Abilities
- Map Based Pickup System
- Map Select Screen (Storing Thumbnails of the maps as images!)

It should be of note that not all of these new features are in the main branch as of yet.

## Refactoring:
The project has undergone numerous refactoring efforts over the course of the CDP version, this is a brief look into what has been refactored within this repository:

- Controller Code: Adding ID support to the controllers for easier reconnection and preventing bugs with team controllers getting mixed swapped.
- Map Management: Maps now have a dedicated manager to handle the parsing and loading of the Json map files.
- Manager Classes: Manager classes have been implemented for Sound, Collision and other similar aspects of the game to prevent code duplication while improving readability.
- Game Scene / The World class changes.

It should be of note that not all of these refactoring efforts are in the main branch as of yet.

## Solution Diagrams:
### Scenes - Class Diagram
```mermaid
classDiagram
    IScene <|-- FlashScreenScene
    IScene <|-- GameScene
    IScene <|-- GameOverScene
    IScene <|-- PlayerSelectionScene
    IScene <|-- StartScene
    IScene <|-- TransitionScene
    IScene <|-- LevelSelectionScene
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
        +Reset()
        +RemainingTeams()
    }
    class GameOverScene{
        +TheWorld mWorld
        +List<Player> mTeams
        +Texture2D mBackGroundTexture
        +RepositionGUIs() void
    }
    class PlayerSelectionScene{
        +PrepareAvatarPickers() void
        +UpdateAvatarPickers() void
    }
    class StartScene{
        +string defaultMapFile
        +SetDefaultMapFile(string mapFile)
        +SelectLevel()
        +ExitGame() void
        +StartGame() void
    }
    class TransitionScene{
        +RenderTarget2D GenerateSceneTexture(IScene pscene)
    }
    class LevelSelectionScene{
        +List<Texture2D> mThumbnailTextures
        +List<string> mMapFiles
        +SelectMap(string mapFile)
        +MakeThumbnailFromMapFile(string pMapFile)
        +DrawOutline(Rectangle rect, string textureName)
    }
```
### GUIs - Class Diagram
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
        +Texture2D mHeartColour
        +PrepareRectangles()
        +Draw()
    }
    Player o-- TeamGUI
    Player *-- Tank
    Player *-- Avatar
    Player *-- IController
    Player o-- Bullet
    class Player{
        +TeamGUI GUI
        +Tank Tank
        +IController Controller
        +Color Colour
        +Avatar Avatar
        List~Bullet~ Bullets
        +DoTankControls() bool
    }
    Tank o-- Bullet
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
