using OpenTK.Graphics.OpenGL;

namespace Rasterization
{
    public static class LUTUtility
    {
        public static byte[] GenerateIdentityLUT(int size)
        {
            byte[] data = new byte[size * size * size * 3];
            int index = 0;
            for (int z = 0; z < size; z++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        data[index++] = (byte)(x * 255 / (size - 1));
                        data[index++] = (byte)(y * 255 / (size - 1));
                        data[index++] = (byte)(z * 255 / (size - 1));
                    }
                }
            }
            return data;
        }

        public static int LoadLUTTexture(byte[] lutData, int size)
        {
            int textureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture3D, textureID);

            GL.TexImage3D(TextureTarget.Texture3D, 0, PixelInternalFormat.Rgb8, size, size, size, 0, PixelFormat.Rgb, PixelType.UnsignedByte, lutData);

            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            GL.BindTexture(TextureTarget.Texture3D, 0);

            return textureID;
        }
    }
}