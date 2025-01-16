using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tankontroller.Managers;
using Tankontroller.World;
using static Tankontroller.Scenes.GameScene;

namespace Tankontroller
{
    public class MapManager
    {
        public List<RectWall> Walls { get; private set; }
        public List<PlayerData> Tanks { get; private set; }

        static MapManager mInstance = new MapManager()
        {
            Walls = new List<RectWall>(),
            Tanks = new List<PlayerData>()
        };

        static MapManager() { }
        private MapManager() { }

        public static MapManager Instance
        {
            get { return mInstance; }
        }

        public void LoadMap(string filePath)
        {
            Walls.Clear();
            Tanks.Clear();

            string[] lines = File.ReadAllLines(filePath);
            ParseLines(lines);
        }

        private void ParseLines(string[] lines)
        {
            string texture = null;
            Vector2 position = Vector2.Zero;
            Vector2 size = Vector2.Zero;
            float rotation = 0f;
            bool isWall = false;
            bool isTank = false;

            foreach (string line in lines)
            {
                if (!isWall && !isTank)
                {
                    texture = null;
                    position = Vector2.Zero;
                    size = Vector2.Zero;
                    rotation = 0f;
                }

                if (line.StartsWith("wall"))
                {
                    isWall = true;
                    continue;
                }
                else if (line.StartsWith("tank"))
                {
                    isTank = true;
                    continue;
                }
                else if (line.Contains("texture"))
                {
                    texture = line.Split('=')[1].Trim().Trim('"');
                }
                else if (line.Contains("position"))
                {
                    string[] components = line.Split('=')[1].Trim().Split(',');
                    position = new Vector2(float.Parse(components[0]), float.Parse(components[1]));
                }
                else if (line.Contains("size"))
                {
                    string[] components = line.Split('=')[1].Trim().Split(',');
                    size = new Vector2(float.Parse(components[0]), float.Parse(components[1]));
                }
                else if (line.Contains("rotation"))
                {
                    rotation = float.Parse(line.Split('=')[1].Trim());
                }

                //check if the current object is a wall or a tank
                if (isWall)
                {
                    RectWall currentWall = new RectWall(
                        Tankontroller.Instance().CM().Load<Texture2D>(texture),
                        new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y));
                    Walls.Add(currentWall);
                    isWall = false;
                }
                else if (isTank)
                {
                    PlayerData currentTank = new();
                    currentTank.position = position;
                    currentTank.rotation = rotation;
                    Tanks.Add(currentTank);
                    isTank = false;
                }
            }
        }

        internal List<RectWall> GetWalls()
        {
            return Walls;
        }

        internal List<PlayerData> GetPlayerData()
        {
            return Tanks;
        }

        public class PlayerData
        {
            public Vector2 position;
            public float rotation;
        }
    }
}
