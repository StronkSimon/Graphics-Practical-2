using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Rasterization
{
    public class Camera
    {
        public Vector3 position;
        public Vector3 front;
        public Vector3 up;
        public Vector3 right;
        public Vector3 worldUp;
        public float yaw;
        public float pitch;
        public float zoom;
        public float speed;
        public float sensitivity;

        // Constructor to initialize the camera with given parameters
        public Camera(Vector3 position, Vector3 up, float yaw, float pitch, float speed = 2.5f, float sensitivity = 0.1f, float zoom = 45.0f)
        {
            this.position = position;
            this.worldUp = up;
            this.yaw = yaw;
            this.pitch = pitch;
            this.speed = speed;
            this.sensitivity = sensitivity;
            this.zoom = zoom;
            UpdateCameraVectors(); // Update the initial camera vectors
        }

        // Returns the view matrix calculated using the LookAt method
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, position + front, up);
        }

        // Returns the zoom level in radians
        public float GetZoom()
        {
            return MathHelper.DegreesToRadians(zoom);
        }

        // Processes keyboard input and moves the camera accordingly
        public void ProcessKeyboard(Keys key, float deltaTime)
        {
            float velocity = speed * deltaTime;
            if (key == Keys.W)
                position += front * velocity;
            if (key == Keys.S)
                position -= front * velocity;
            if (key == Keys.A)
                position -= right * velocity;
            if (key == Keys.D)
                position += right * velocity;
            if (key == Keys.Space)
                position += worldUp * velocity;
            if (key == Keys.LeftControl)
                position -= worldUp * velocity;
        }

        // Processes mouse movement to adjust camera orientation
        public void ProcessMouseMovement(float xOffset, float yOffset)
        {
            xOffset *= sensitivity;
            yOffset *= sensitivity;

            yaw += xOffset;
            pitch += yOffset;

            if (pitch > 89.0f)
                pitch = 89.0f;
            if (pitch < -89.0f)
                pitch = -89.0f;

            UpdateCameraVectors();
        }

        // Processes mouse scroll to adjust zoom level
        public void ProcessMouseScroll(float yOffset)
        {
            zoom -= yOffset;
            if (zoom < 1.0f)
                zoom = 1.0f;
            if (zoom > 45.0f)
                zoom = 45.0f;
        }

        // Updates the camera vectors based on the current yaw and pitch values
        private void UpdateCameraVectors()
        {
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
            this.front = Vector3.Normalize(front);
            right = Vector3.Normalize(Vector3.Cross(this.front, worldUp));
            up = Vector3.Normalize(Vector3.Cross(right, this.front));
        }
    }
}