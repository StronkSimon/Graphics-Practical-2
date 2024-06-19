using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

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
        Shader? shader;
        Shader? postproc;
        Texture? wood;
        RenderTarget? target;
        ScreenQuad? quad;
        readonly bool useRenderTarget = true;
        private Vector2 lastMousePosition;
        private bool firstMouseMovement = true;

        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
            sceneGraph = new SceneGraph();
            camera = new Camera(new Vector3(0, 0, 3), Vector3.UnitY, -90.0f, 0.0f);
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

            // load a texture
            wood = new Texture("../../../assets/wood.jpg");

            // create the render target
            if (useRenderTarget) target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();

            // setup scene graph
            SceneNode root = sceneGraph.Root;
            SceneNode teapotNode = new SceneNode(teapot);
            SceneNode floorNode = new SceneNode(floor);
            root.AddChild(teapotNode);
            root.AddChild(floorNode);

            // initial transformations
            teapotNode.LocalTransform = Matrix4.CreateScale(0.5f) * Matrix4.CreateRotationY(a);
            floorNode.LocalTransform = Matrix4.CreateScale(4.0f) * Matrix4.CreateRotationY(a);
        }

        // tick for background surface
        public void Tick()
        {
            screen.Clear(0);
            screen.Print("hello world", 2, 2, 0xffff00);
        }

        // tick for OpenGL rendering code
        public void RenderGL()
        {
            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();

            // update rotation
            a += 0.001f * frameDuration;
            if (a > 2 * MathF.PI) a -= 2 * MathF.PI;

            // update scene graph transforms
            SceneNode teapotNode = sceneGraph.Root.Children[0];
            SceneNode floorNode = sceneGraph.Root.Children[1];
            teapotNode.LocalTransform = Matrix4.CreateScale(0.5f) * Matrix4.CreateRotationY(a);
            floorNode.LocalTransform = Matrix4.CreateScale(4.0f) * Matrix4.CreateRotationY(a);

            // prepare matrices
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(camera.GetZoom(), (float)screen.width / screen.height, 0.1f, 1000f);
            Matrix4 viewProjection = view * projection;

            if (useRenderTarget && target != null && quad != null)
            {
                target.Bind();

                if (shader != null && wood != null)
                {
                    sceneGraph.Render(shader, viewProjection, Matrix4.Identity, wood);
                }

                target.Unbind();
                if (postproc != null)
                    quad.Render(postproc, target.GetTextureID());
            }
            else
            {
                if (shader != null && wood != null)
                {
                    sceneGraph.Render(shader, viewProjection, Matrix4.Identity, wood);
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