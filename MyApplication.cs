using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

namespace Rasterization
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        SceneGraph sceneGraph;
        Camera camera;
        float a = 0;
        readonly Stopwatch timer = new();
        Texture? wood;
        ScreenQuad? quad;
        readonly bool useRenderTarget = true;
        private Vector2 lastMousePosition;
        private bool firstMouseMovement = true;

        Vector3 CameraPosition = new Vector3(0, -10f, 0);
        Vector3 CameraAngle = new Vector3(1, 0, 0);

        List<Light> lights = new List<Light>();

        // Add new shaders and render targets
        Shader? shader;
        Shader? postproc;
        Shader? extractBrightShader;
        Shader? blurShaderH;
        Shader? blurShaderV;
        Shader? combineShader;
        Shader? vignetteChromaticShader;
        Shader? colorGradingShader;
        Shader? toneMappingShader;

        RenderTarget? target;
        RenderTarget? hdrRenderTarget;
        RenderTarget? blurRenderTarget1;
        RenderTarget? blurRenderTarget2;
        RenderTarget? intermediateTarget;

        int lutTexture;
        const int lutSize = 16; // LUT size

        // Position variables for teapots
        Vector3 teapot1Position = new Vector3(0, 0, 0);
        Vector3 teapot2Position = new Vector3(0, 5, 0);
        Vector3 teapot3Position = new Vector3(-10, 0, 5);
        Vector3 teapot4Position = new Vector3(-10, 5, 5);

        // Post-processing toggles
        private bool enableVignetteChromatic = true;
        private bool enableColorGrading = true;
        private bool enableBlur = true;
        private bool enableHDRGlow = true;

        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
            sceneGraph = new SceneGraph();
            camera = new Camera(new Vector3(0, 0, 10), Vector3.UnitY, -90.0f, 0.0f);
        }

        // initialize
        public void Init()
        {
            // load meshes
            Mesh teapot = new Mesh("../../../assets/teapot.obj");
            Mesh floor = new Mesh("../../../assets/floor.obj");

            // create shaders
            shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
            postproc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
            extractBrightShader = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_extract_bright.glsl");
            blurShaderH = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_blur_horizontal.glsl");
            blurShaderV = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_blur_vertical.glsl");
            combineShader = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_combine.glsl");
            vignetteChromaticShader = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_vignette_chromatic.glsl");
            colorGradingShader = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_color_grading.glsl");
            toneMappingShader = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_tone_mapping.glsl");

            // load a texture
            wood = new Texture("../../../assets/wood.jpg");

            // create the render target
            if (useRenderTarget) target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();
            hdrRenderTarget = new RenderTarget(screen.width, screen.height);
            blurRenderTarget1 = new RenderTarget(screen.width, screen.height);
            blurRenderTarget2 = new RenderTarget(screen.width, screen.height);
            intermediateTarget = new RenderTarget(screen.width, screen.height);

            // Generate and load the identity LUT
            byte[] lutData = LUTUtility.GenerateIdentityLUT(lutSize);
            lutTexture = LUTUtility.LoadLUTTexture(lutData, lutSize);

            // setup scene graph
            SceneNode root = sceneGraph.Root;
            SceneNode teapotNode = new SceneNode(teapot);
            SceneNode floorNode = new SceneNode(floor);
            root.AddChild(teapotNode);
            root.AddChild(floorNode);

            // create additional nodes
            SceneNode teapotNode2 = new SceneNode(teapot);
            teapotNode2.SetTransform(Matrix4.CreateTranslation(teapot2Position));
            root.AddChild(teapotNode2);

            SceneNode teapotNode3 = new SceneNode(teapot);
            teapotNode3.SetTransform(Matrix4.CreateTranslation(teapot3Position));
            root.AddChild(teapotNode3);

            SceneNode teapotNode4 = new SceneNode(teapot);
            teapotNode4.SetTransform(Matrix4.CreateTranslation(teapot4Position));
            root.AddChild(teapotNode4);

            // initial transformations
            teapotNode.LocalTransform = Matrix4.CreateScale(0.5f);
            floorNode.LocalTransform = Matrix4.CreateScale(4.0f);

            // Add multiple lights with different properties
            lights.Add(new Light(new Vector3(10f, 5f, 2.0f), new Vector3(1.0f, 0.5f, 0.5f)));
            lights.Add(new Light(new Vector3(-10f, 5f, 2.0f), new Vector3(0.5f, 1.0f, 0.5f)));
            lights.Add(new Light(new Vector3(0f, 5f, 10.0f), new Vector3(0.5f, 0.5f, 1.0f)));
            lights.Add(new Light(new Vector3(0f, 10f, -10.0f), new Vector3(1.0f, 1.0f, 0.5f)));
        }

        // tick for background surface
        public void Tick()
        {
            screen.Clear(0);
            screen.Print("3D Engine Demo", 2, 2, 0xffff00);
        }

        // tick for OpenGL rendering code
        public void RenderGL()
        {
            // Measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();

            // Update rotation
            a += 0.001f * frameDuration;
            if (a > 2 * MathF.PI) a -= 2 * MathF.PI;

            // Update scene graph transforms
            SceneNode teapotNode = sceneGraph.Root.Children[0];
            SceneNode floorNode = sceneGraph.Root.Children[1];
            teapotNode.LocalTransform = Matrix4.CreateScale(0.5f) * Matrix4.CreateRotationY(a) * Matrix4.CreateTranslation(teapot1Position);
            floorNode.LocalTransform = Matrix4.CreateScale(4.0f) * Matrix4.CreateRotationY(a);

            SceneNode teapotNode2 = sceneGraph.Root.Children[2];
            teapotNode2.LocalTransform = Matrix4.CreateScale(0.5f) * Matrix4.CreateRotationY(a) * Matrix4.CreateTranslation(teapot2Position);

            SceneNode teapotNode3 = sceneGraph.Root.Children[3];
            teapotNode3.LocalTransform = Matrix4.CreateScale(0.5f) * Matrix4.CreateRotationY(-a) * Matrix4.CreateTranslation(teapot3Position);

            SceneNode teapotNode4 = sceneGraph.Root.Children[4];
            teapotNode4.LocalTransform = Matrix4.CreateScale(0.5f) * Matrix4.CreateRotationY(-a) * Matrix4.CreateTranslation(teapot4Position);

            // Prepare matrices
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(camera.GetZoom(), (float)screen.width / screen.height, 0.1f, 1000f);
            Matrix4 viewProjection = view * projection;

            if (hdrRenderTarget != null && blurRenderTarget1 != null && blurRenderTarget2 != null && quad != null)
            {
                // 1. Render the scene to the HDR render target
                hdrRenderTarget.Bind();
                if (shader != null && wood != null)
                {
                    sceneGraph.Render(shader, viewProjection, Matrix4.Identity, wood, lights, camera.position);
                }
                hdrRenderTarget.Unbind();

                // 2. Extract bright areas
                blurRenderTarget1.Bind();
                if (extractBrightShader != null)
                {
                    quad.Render(extractBrightShader, hdrRenderTarget.GetTextureID());
                }
                blurRenderTarget1.Unbind();

                if (enableBlur)
                {
                    // 3. Apply horizontal blur
                    blurRenderTarget2.Bind();
                    if (blurShaderH != null)
                    {
                        quad.Render(blurShaderH, blurRenderTarget1.GetTextureID());
                    }
                    blurRenderTarget2.Unbind();

                    // 4. Apply vertical blur
                    blurRenderTarget1.Bind();
                    if (blurShaderV != null)
                    {
                        quad.Render(blurShaderV, blurRenderTarget2.GetTextureID());
                    }
                    blurRenderTarget1.Unbind();
                }

                // 5. Combine the original image with the blurred image
                intermediateTarget.Bind();
                if (combineShader != null)
                {
                    quad.Render(combineShader, hdrRenderTarget.GetTextureID(), blurRenderTarget1.GetTextureID());
                }
                intermediateTarget.Unbind();

                // 6. Apply vignetting and chromatic aberration
                if (enableVignetteChromatic && vignetteChromaticShader != null)
                {
                    vignetteChromaticShader.Use();
                    vignetteChromaticShader.SetUniform("screenSize", new Vector2(screen.width, screen.height));
                    quad.Render(vignetteChromaticShader, intermediateTarget.GetTextureID());
                }

                // 7. Apply color grading
                if (enableColorGrading && colorGradingShader != null)
                {
                    colorGradingShader.Use();
                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture3D, lutTexture);
                    colorGradingShader.SetUniform("colorLUT", 1);
                    colorGradingShader.SetUniform("lutSize", (float)lutSize);

                    quad.Render(colorGradingShader, intermediateTarget.GetTextureID());
                }

                // 8. Apply tone-mapping
                if (toneMappingShader != null)
                {
                    toneMappingShader.Use();
                    quad.Render(toneMappingShader, intermediateTarget.GetTextureID());
                }
            }
            else
            {
                if (shader != null && wood != null)
                {
                    sceneGraph.Render(shader, viewProjection, Matrix4.Identity, wood, lights, camera.position);
                }
            }
        }

        public void ProcessInput(KeyboardState keyboardState, MouseState mouseState, float deltaTime)
        {
            if (keyboardState.IsKeyDown(Keys.W))
                camera.ProcessKeyboard(Keys.W, deltaTime);
            if (keyboardState.IsKeyDown(Keys.S))
                camera.ProcessKeyboard(Keys.S, deltaTime);
            if (keyboardState.IsKeyDown(Keys.A))
                camera.ProcessKeyboard(Keys.A, deltaTime);
            if (keyboardState.IsKeyDown(Keys.D))
                camera.ProcessKeyboard(Keys.D, deltaTime);
            if (keyboardState.IsKeyDown(Keys.Space))
                camera.ProcessKeyboard(Keys.Space, deltaTime);
            if (keyboardState.IsKeyDown(Keys.LeftControl))
                camera.ProcessKeyboard(Keys.LeftControl, deltaTime);

            if (firstMouseMovement)
            {
                lastMousePosition = new Vector2(mouseState.X, mouseState.Y);
                firstMouseMovement = false;
            }

            float xOffset = mouseState.X - lastMousePosition.X;
            float yOffset = lastMousePosition.Y - mouseState.Y;
            lastMousePosition = new Vector2(mouseState.X, mouseState.Y);

            camera.ProcessMouseMovement(xOffset, yOffset);

            camera.ProcessMouseScroll(mouseState.Scroll.Y);
        }
    }
}