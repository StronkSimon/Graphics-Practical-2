using System.Diagnostics;
using OpenTK.Mathematics;

namespace Rasterization
{
    class MyApplication
    {
        // member variables
        public Surface screen;                  // background surface for printing etc.
        Mesh? teapot, floor;                    // meshes to draw using OpenGL
        float a = 0;                            // teapot rotation angle
        readonly Stopwatch timer = new();       // timer for measuring frame duration
        Shader? shader;                         // shader to use for rendering
        Shader? postproc;                       // shader to use for post processing
        Texture? wood;                          // texture to use for rendering
        RenderTarget? target;                   // intermediate render target
        ScreenQuad? quad;                       // screen filling quad for post processing
        readonly bool useRenderTarget = true;   // required for post processing

        Vector3 CameraPosition = new Vector3(0, -10f, 0);
        Vector3 CameraAngle = new Vector3(1, 0, 0);

        List<Light> lights = new List<Light>();


        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
        }
        // initialize
        public void Init()
        {
            // load teapot
            teapot = new Mesh("../../../assets/teapot.obj");
            floor = new Mesh("../../../assets/floor.obj");
            // initialize stopwatch
            timer.Reset();
            timer.Start();
            // create shaders
            shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
            postproc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
            // load a texture
            wood = new Texture("../../../assets/wood.jpg");
            // create the render target
            if (useRenderTarget) target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();

            lights.Add(new Light(new Vector3(10f, 5f, 2.0f), new Vector3(1.0f, 0.5f, 0.5f)));
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

            // prepare matrix for vertex shader
            float angle90degrees = MathF.PI / 2;
            Matrix4 teapotObjectToWorld = Matrix4.CreateScale(0.5f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
            Matrix4 floorObjectToWorld = Matrix4.CreateScale(4.0f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
            Matrix4 worldToCamera = Matrix4.CreateTranslation(new Vector3(0, -10f, 0)) * Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0),angle90degrees);
            Matrix4 cameraToScreen = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), (float)screen.width/screen.height, .1f, 1000);

            // update rotation
            a += 0.001f * frameDuration;
            if (a > 2 * MathF.PI) a -= 2 * MathF.PI;

            if (useRenderTarget && target != null && quad != null)
            {
                // enable render target
                target.Bind();

                // render scene to render target
                if (shader != null && wood != null)
                {
                    teapot?.Render(shader, teapotObjectToWorld * worldToCamera * cameraToScreen, teapotObjectToWorld, wood,lights,CameraPosition);
                    floor?.Render(shader, floorObjectToWorld * worldToCamera * cameraToScreen, floorObjectToWorld, wood,lights,CameraPosition);
                }

                // render quad
                target.Unbind();
                if (postproc != null)
                    quad.Render(postproc, target.GetTextureID());
            }
            else
            {
                // render scene directly to the screen
                if (shader != null && wood != null)
                {
                    teapot?.Render(shader, teapotObjectToWorld * worldToCamera * cameraToScreen, teapotObjectToWorld, wood,lights, CameraPosition);
                    floor?.Render(shader, floorObjectToWorld * worldToCamera * cameraToScreen, floorObjectToWorld, wood,lights, CameraPosition);
                }
            }
        }
    }
}