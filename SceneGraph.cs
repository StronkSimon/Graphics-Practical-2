using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Rasterization
{
    public class SceneNode
    {
        public Mesh? Mesh { get; set; }
        public Matrix4 LocalTransform { get; set; } = Matrix4.Identity;
        public List<SceneNode> Children { get; } = new List<SceneNode>();

        // Constructor to initialize the node with an optional mesh
        public SceneNode(Mesh? mesh)
        {
            Mesh = mesh;
        }

        // Adds a child node to this node
        public void AddChild(SceneNode child)
        {
            Children.Add(child);
        }

        // Sets the local transformation matrix of this node
        public void SetTransform(Matrix4 transform)
        {
            LocalTransform = transform;
        }
    }

    public class SceneGraph
    {
        public SceneNode Root { get; } = new SceneNode(null);

        // Renders the entire scene graph recursively
        public void Render(Shader shader, Matrix4 viewProjectionMatrix, Matrix4 parentTransform, Texture texture, List<Light> lights, Vector3 cameraPosition)
        {
            RenderNode(Root, shader, viewProjectionMatrix, parentTransform, texture, lights, cameraPosition);
        }

        // Renders a single node and its children recursively
        private void RenderNode(SceneNode node, Shader shader, Matrix4 viewProjectionMatrix, Matrix4 parentTransform, Texture texture, List<Light> lights, Vector3 cameraPosition)
        {
            Matrix4 globalTransform = node.LocalTransform * parentTransform;
            if (node.Mesh != null)
            {
                Matrix4 modelViewProjection = globalTransform * viewProjectionMatrix;
                node.Mesh.Render(shader, modelViewProjection, globalTransform, texture, lights, cameraPosition);
            }
            foreach (var child in node.Children)
            {
                RenderNode(child, shader, viewProjectionMatrix, globalTransform, texture, lights, cameraPosition);
            }
        }
    }
}