using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Rasterization
{
    public class Shader
    {
        // data members
        public int programID, vsID, fsID;
        public int in_vertexPositionObject;
        public int in_vertexNormalObject;
        public int in_vertexUV;
        public int uniform_objectToScreen;
        public int uniform_objectToWorld;
        public int uniform_lightPosition;
        public int uniform_lightColor;
        public int uniform_ambientLightColor;
        public int uniform_cameraPosition;


        // constructor
        public Shader(string vertexShader, string fragmentShader)
        {
            // compile shaders
            programID = GL.CreateProgram();
            if (!OpenTKApp.isMac) GL.ObjectLabel(ObjectLabelIdentifier.Program, programID, -1, vertexShader + " + " + fragmentShader);
            Load(vertexShader, ShaderType.VertexShader, programID, out vsID);
            Load(fragmentShader, ShaderType.FragmentShader, programID, out fsID);
            GL.LinkProgram(programID);
            string infoLog = GL.GetProgramInfoLog(programID);
            if (infoLog.Length != 0) Console.WriteLine(infoLog);

            // get locations of shader parameters
            in_vertexPositionObject = GL.GetAttribLocation(programID, "vertexPositionObject");
            in_vertexNormalObject = GL.GetAttribLocation(programID, "vertexNormalObject");
            in_vertexUV = GL.GetAttribLocation(programID, "vertexUV");
            uniform_objectToScreen = GL.GetUniformLocation(programID, "objectToScreen");
            uniform_objectToWorld = GL.GetUniformLocation(programID, "objectToWorld");
            uniform_lightPosition = GL.GetUniformLocation(programID, "lightPosition");
            uniform_lightColor = GL.GetUniformLocation(programID, "lightColor");
            uniform_ambientLightColor = GL.GetUniformLocation(programID, "ambientLightColor");
            uniform_cameraPosition = GL.GetUniformLocation(programID, "cameraPosition");
        }

        // method to use the shader program
        public void Use()
        {
            GL.UseProgram(programID);
        }

        // method to set uniform variables
        public void SetUniform(string name, int value)
        {
            int location = GL.GetUniformLocation(programID, name);
            if (location != -1)
            {
                GL.Uniform1(location, value);
            }
        }

        public void SetUniform(string name, float value)
        {
            int location = GL.GetUniformLocation(programID, name);
            if (location != -1)
            {
                GL.Uniform1(location, value);
            }
        }

        public void SetUniform(string name, Vector2 value)
        {
            int location = GL.GetUniformLocation(programID, name);
            if (location != -1)
            {
                GL.Uniform2(location, value);
            }
        }

        public void SetUniform(string name, Vector3 value)
        {
            int location = GL.GetUniformLocation(programID, name);
            if (location != -1)
            {
                GL.Uniform3(location, value);
            }
        }

        public void SetUniform(string name, Vector4 value)
        {
            int location = GL.GetUniformLocation(programID, name);
            if (location != -1)
            {
                GL.Uniform4(location, value);
            }
        }

        public void SetUniform(string name, Matrix4 value)
        {
            int location = GL.GetUniformLocation(programID, name);
            if (location != -1)
            {
                GL.UniformMatrix4(location, false, ref value);
            }
        }

        // loading shaders
        void Load(String filename, ShaderType type, int program, out int ID)
        {
            // source: http://neokabuto.blogspot.nl/2013/03/opentk-tutorial-2-drawing-triangle.html
            ID = GL.CreateShader(type);
            if (!OpenTKApp.isMac) GL.ObjectLabel(ObjectLabelIdentifier.Shader, ID, -1, filename);
            using (StreamReader sr = new StreamReader(filename)) GL.ShaderSource(ID, sr.ReadToEnd());
            GL.CompileShader(ID);
            GL.AttachShader(program, ID);
            string infoLog = GL.GetShaderInfoLog(ID);
            if (infoLog.Length != 0) Console.WriteLine(infoLog);
        }
    }
}