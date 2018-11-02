using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace LineTest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            initialiseVertices();
            sortVertices();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }
        private List<Vector2> vertices;
        private void initialiseVertices()
        {
            vertices = new List<Vector2>();
            Vector2 vertex = new Vector2(100, 100);
            vertices.Add(vertex);
            vertex = new Vector2(200, 100);
            vertices.Add(vertex);
            vertex = new Vector2(200, 200);
            vertices.Add(vertex);
            vertex = new Vector2(300, 200);
            vertices.Add(vertex);
            vertex = new Vector2(300, 300);
            vertices.Add(vertex);
            vertex = new Vector2(400, 300);
            vertices.Add(vertex);
            vertex = new Vector2(400, 400);
            vertices.Add(vertex);
            vertex = new Vector2(200, 400);
            vertices.Add(vertex);
            vertex = new Vector2(200, 300);
            vertices.Add(vertex);
            vertex = new Vector2(100, 300);
            vertices.Add(vertex);
        }
        private List<int> indices;
        private double dotProduct(Vector2 a, Vector2 b)
        {
            double result = a.X * b.X + a.Y * b.Y;
            return result;
        }
        private double crossProduct(Vector2 a, Vector2 b)
        {
            double cross = a.X * b.Y - a.Y * b.X;
            return cross;
        }
            private bool inTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 v0 = c - a;
            Vector2 v1 = b - a;
            Vector2 v2 = point - a;

            double dot00 = dotProduct(v0, v0);
            double dot01 = dotProduct(v0, v1);
            double dot02 = dotProduct(v0, v2);
            double dot11 = dotProduct(v1, v1);
            double dot12 = dotProduct(v1, v2);
            // Compute barycentric coordinates
            double invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            double u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            double v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // Check if point is in triangle
            return (u >= 0) && (v >= 0) && (u + v < 1);
        }
        private void sortVertices()
        {
            indices = new List<int>();
            List<Vector2> verticesCopy = new List<Vector2>(vertices);
            List<int> indicesCopy = new List<int>();
            for(int i = 0; i < vertices.Count;i++)
            {
                indicesCopy.Add(i);
            }
            while (true) { 
                bool removed = false;
                for (int i = 1; i < verticesCopy.Count; i++)
                {
                    Vector2 previousVertex = verticesCopy[i-1];

                    Vector2 currentVertex = verticesCopy[i];
                    int index = i + 1;
                    if (index == verticesCopy.Count)
                    {
                        index = 0;
                    }
                    Vector2 nextVertex = verticesCopy[index];

                    Vector2 previousVector = previousVertex - currentVertex;
                    Vector2 nextVector = nextVertex - currentVertex;
                    double cross = crossProduct(previousVector, nextVector);
                    if (cross >= 0)
                    {
                        continue;
                    }
                    bool vertexInTriangle = false;
                    for (int j = 0; j < verticesCopy.Count; j++)
                    {
                        if (j == i || j == i-1 || j == index)
                        {
                            continue;
                        }
                        Vector2 vertex = verticesCopy[j];
                        if (inTriangle(vertex, previousVertex, currentVertex, nextVertex))
                        {
                            vertexInTriangle = true;
                            break;
                        }
                    }
                    if (vertexInTriangle)
                    {
                        continue;
                    }
                    indices.Add(indicesCopy[i - 1]);
                    indices.Add(indicesCopy[i]);
                    indices.Add(indicesCopy[index]);
                    verticesCopy.RemoveAt(i);
                    indicesCopy.RemoveAt(i);
                    removed = true;
                }
                if (!removed || verticesCopy.Count < 3)
                {
                    break;
                }
            }
            

        }
        private Random rand = new Random();
        private Texture2D _texture;
        private Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if (_texture == null)
            {
                _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                _texture.SetData(new[] { Color.White });
            }

            return _texture;
        }

        public void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            
            Matrix world = Matrix.CreateTranslation(0, 0, 0);
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 3), Vector3.Forward, Vector3.Up);
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);


            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);
          //  basicEffect.World = world;
          //  basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.VertexColorEnabled = true;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            VertexPositionColor[] vert = new VertexPositionColor[vertices.Count];
            
            for(int j = 0; j < vertices.Count; j++)
            {
                Vector3 vector = new Vector3(vertices[j], 0);
                vert[j].Position = vector;
                vert[j].Color = Color.Red;
            }
            
            

            int[] ind = indices.ToArray();
            

            foreach (EffectPass effectPass in basicEffect.CurrentTechnique.Passes)
            {

                effectPass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vert, 0, vert.Length, ind, 0, ind.Length / 3);

            }

            Vector2 startVector;
            Vector2 endVector;
            spriteBatch.Begin();
            for (int i = 0; i < vertices.Count; i++)
            {
                int index = i - 1;
                if (index < 0)
                {
                    index = vertices.Count - 1;
                }
                startVector = vertices[index];
                endVector = vertices[i];
                DrawLine(spriteBatch, startVector, endVector, Color.White, 1f);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
