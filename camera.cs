using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Rasterization
{
    public class Camera
    {
        private Vector3 position;
        private Vector3 front;
        private Vector3 up;
        private Vector3 right;
        private Vector3 worldUp;
        private float yaw;
        private float pitch;
        private float speed;
        private float sensitivity;

        public Camera(Vector3 position, Vector3 up, float yaw, float pitch, float speed = 2.5f, float sensitivity = 0.1f)
        {
            this.position = position;
            this.worldUp = up;
            this.yaw = yaw;
            this.pitch = pitch;
            this.speed = speed;
            this.sensitivity = sensitivity;
            UpdateCameraVectors();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, position + front, up);
        }

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
        }

        public void ProcessMouseMovement(float xOffset, float yOffset)
        {
            xOffset *= sensitivity;
            yOffset *= sensitivity;

            yaw += xOffset;
            pitch -= yOffset;

            if (pitch > 89.0f)
                pitch = 89.0f;
            if (pitch < -89.0f)
                pitch = -89.0f;

            UpdateCameraVectors();
        }

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