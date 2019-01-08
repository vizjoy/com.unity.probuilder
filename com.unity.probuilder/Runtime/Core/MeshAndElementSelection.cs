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
    class AttributeSelection
    {
        [SerializeField]
        List<VertexIndex> m_Vertices;

        [SerializeField]
        List<Edge> m_Edges;

        [SerializeField]
        List<Face> m_Faces;

        public static event Action<AttributeSelection> selectionWillChange;
        public static event Action<AttributeSelection> selectionDidChange;

        public IEnumerable<VertexIndex> vertices
        {
            get { return m_Vertices; }
            set
            {
                if (selectionWillChange != null)
                    selectionWillChange(this);
                m_Vertices = value.ToList();
                if (selectionDidChange != null)
                    selectionDidChange(this);
            }
        }

        public IEnumerable<Edge> edges
        {
            get { return m_Edges; }
            set
            {
                if (selectionWillChange != null)
                    selectionWillChange(this);
                m_Edges = value.ToList();
                if (selectionDidChange != null)
                    selectionDidChange(this);
            }
        }

        public IEnumerable<Face> faces
        {
            get { return m_Faces; }
            set
            {
                if (selectionWillChange != null)
                    selectionWillChange(this);
                m_Faces = value.ToList();
                if (selectionDidChange != null)
                    selectionDidChange(this);
            }
        }

        public IEnumerable<T> Get<T>() where T : ISelectable
        {
            if (typeof(T) == typeof(VertexIndex))
                return (IEnumerable<T>) vertices;
            if (typeof(T) == typeof(Edge))
                return (IEnumerable<T>) edges;
            if (typeof(T) == typeof(Face))
                return (IEnumerable<T>) faces;

            return new List<T>();
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
