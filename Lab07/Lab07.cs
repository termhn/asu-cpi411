﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Lab07
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab07 : Game
    {
        GraphicsDeviceManager graphics;

        Vector2 angle = new Vector2(0, 0);

        MouseState prevMouse;
        KeyboardState prevKey;

        Vector3 cameraPos = new Vector3(0, 0, -10);
        Vector3 ambientColor = new Vector3(0.2f, 0.2f, 0.2f);
        float ambientIntensity = 0.25f;
        float specularIntensity = 0.8f;
        float diffuseIntensity = 0.8f;
        float shininess = 20.0f;

        int technique = 0;

        Vector3 lightPos = new Vector3(0, 17, 0);

        Model plane;
        Texture2D normalMap;

        Matrix view;
        Matrix projection;

        Effect effect;

        public Lab07()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
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

            effect = Content.Load<Effect>("Lighting");
            plane = Content.Load<Model>("Plane");
            normalMap = Content.Load<Texture2D>("Round");

            projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(90),
                    GraphicsDevice.Viewport.AspectRatio,
                    0.1f, 100);

            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["NormalMap"].SetValue(normalMap);


            prevMouse = Mouse.GetState();


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

            KeyboardState currKey = Keyboard.GetState();
            MouseState currMouse = Mouse.GetState();

            if (currMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Pressed)
            {
                angle.X -= (currMouse.X - prevMouse.X) * 0.01f;
                angle.Y -= (currMouse.Y - prevMouse.Y) * 0.01f;
            }

            cameraPos = Vector3.Transform(
                    new Vector3(0, 0, 5),
                    Matrix.CreateRotationX(angle.Y) * Matrix.CreateRotationY(angle.X)
                );

            view = Matrix.CreateLookAt(
                cameraPos,
                new Vector3(0, 0, 0),
                Vector3.Transform(
                    new Vector3(0, 1, 0),
                    Matrix.CreateRotationX(angle.Y) * Matrix.CreateRotationY(angle.X)
                )
            );


            prevMouse = currMouse;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                lightPos.X -= 0.5f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                lightPos.X += 0.5f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                lightPos.Y -= 0.5f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                lightPos.Y += 0.5f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                lightPos.Z -= 0.5f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                lightPos.Z += 0.5f;
            }

            if ((currKey.IsKeyDown(Keys.PageUp) && !prevKey.IsKeyDown(Keys.PageUp)) || (currKey.IsKeyDown(Keys.PageDown) && !prevKey.IsKeyDown(Keys.PageDown)))
            {
                technique += 1;
            }

            technique = technique % 3;

            prevKey = currKey;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;


            effect.CurrentTechnique = effect.Techniques[technique];
            foreach (var pass in effect.CurrentTechnique.Passes)
            {

                foreach (ModelMesh mesh in plane.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        Matrix model = Matrix.CreateScale(0.25f) * mesh.ParentBone.Transform;
                        effect.Parameters["Model"].SetValue(model);
                        effect.Parameters["AmbientColor"].SetValue(ambientColor);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);
                        effect.Parameters["SpecularIntensity"].SetValue(specularIntensity);
                        effect.Parameters["Shininess"].SetValue(shininess);
                        effect.Parameters["LightPosition"].SetValue(lightPos);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["CameraPosition"].SetValue(cameraPos);

                        pass.Apply();

                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            part.VertexOffset,
                            part.StartIndex,
                            part.PrimitiveCount);

                    }
                }
            }

            base.Draw(gameTime);
        }
    }
}
