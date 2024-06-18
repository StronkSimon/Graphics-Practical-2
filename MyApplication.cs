using System.Diagnostics;
using OpenTK.Mathematics;

namespace Rasterization
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        SceneGraph sceneGraph;
        float a = 0;
        readonly Stopwatch timer = new();
        Shader? shader;
        Shader? postproc;
        Texture? wood;
        RenderTarget? target;
        ScreenQuad? quad;
        readonly bool useRenderTarget = true;

        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
            sceneGraph = new SceneGraph();
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
            float angle90degrees = MathF.PI / 2;
            Matrix4 worldToCamera = Matrix4.CreateTranslation(new Vector3(0, -14.5f, 0)) * Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), angle90degrees);
            Matrix4 cameraToScreen = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), (float)screen.width / screen.height, .1f, 1000);

            if (useRenderTarget && target != null && quad != null)
            {
                target.Bind();

                if (shader != null && wood != null)
                {
                    sceneGraph.Render(shader, cameraToScreen, worldToCamera, wood);
                }

                target.Unbind();
                if (postproc != null)
                    quad.Render(postproc, target.GetTextureID());
            }
            else
            {
                if (shader != null && wood != null)
                {
                    sceneGraph.Render(shader, cameraToScreen, worldToCamera, wood);
                }
            }
        }
    }
}