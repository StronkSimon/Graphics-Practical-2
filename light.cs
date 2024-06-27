using System;
using OpenTK.Mathematics;

namespace Rasterization
{
    public class Light
    {
        // Properties for position and color of the light
        public Vector3 Position { get; set; }
        public Vector3 Color { get; set; }

        // Constructor to initialize the light with given position and color
        public Light(Vector3 position, Vector3 color)
        {
            Position = position;
            Color = color;
        }
    }
}