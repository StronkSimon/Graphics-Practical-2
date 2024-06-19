using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Rasterization
{
    public class SceneNode
    {
        public Mesh? Mesh { get; set; }
        public Matrix4 LocalTransform { get; set; } = Matrix4.Identity;
        public List<SceneNode> Children { get; } = new List<SceneNode>();

        public SceneNode(Mesh? mesh)
        {
            Mesh = mesh;
        }

        public void AddChild(SceneNode child)
        {
            Children.Add(child);
        }

        public void SetTransform(Matrix4 transform)
        {
            LocalTransform = transform;
        }
    }

    public class SceneGraph
    {
        public SceneNode Root { get; } = new SceneNode(null);

        public void Render(Shader shader, Matrix4 viewProjectionMatrix, Matrix4 parentTransform, Texture texture)
        {
            RenderNode(Root, shader, viewProjectionMatrix, parentTransform, texture);
        }

        private void RenderNode(SceneNode node, Shader shader, Matrix4 viewProjectionMatrix, Matrix4 parentTransform, Texture texture)
        {
            Matrix4 globalTransform = node.LocalTransform * parentTransform;
            if (node.Mesh != null)
            {
                Matrix4 modelViewProjection = globalTransform * viewProjectionMatrix;
                node.Mesh.Render(shader, modelViewProjection, globalTransform, texture);
            }
            foreach (var child in node.Children)
            {
                RenderNode(child, shader, viewProjectionMatrix, globalTransform, texture);
            }
        }
    }
}