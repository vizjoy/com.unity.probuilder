using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder
{
    /// <summary>
    /// Specifies a method for comparing vertex indices.
    /// </summary>
    public enum VertexComparison
    {
        /// <summary>
        /// The actual index of the vertex is used.
        /// </summary>
        Discrete,

        /// <summary>
        /// The common index of the vertex is used. This is useful for testing vertices that are discrete, but share a common position.
        /// </summary>
        Common
    }

    /// <summary>
    /// Represents the state of a ProBuilderMesh and it's selected elements.
    /// </summary>
    [Serializable]
    class MeshAndElementSelection
    {
        [SerializeField]
        ProBuilderMesh m_Mesh;

        [SerializeField]
        List<VertexIndex> m_Vertices;

        [SerializeField]
        List<Edge> m_Edges;

        [SerializeField]
        List<Face> m_Faces;

        public ProBuilderMesh mesh
        {
            get { return m_Mesh; }
        }

        public IEnumerable<VertexIndex> vertices
        {
            get { return m_Vertices; }
            set { m_Vertices = value.ToList(); }
        }

        public IEnumerable<Edge> edges
        {
            get { return m_Edges; }
            set { m_Edges = value.ToList(); }
        }

        public IEnumerable<Face> faces
        {
            get { return m_Faces; }
            set { m_Faces = value.ToList(); }
        }

        // Cached information
        HashSet<VertexIndex> m_CommonVertices;

        // todo CachedValue
        public int vertexCount
        {
            get { return m_Vertices.Count; }
        }

        // todo CachedValue
        public int edgeCount
        {
            get { return m_Edges.Count; }
        }

        // todo CachedValue
        public int faceCount
        {
            get { return m_Faces.Count; }
        }
    }
}
