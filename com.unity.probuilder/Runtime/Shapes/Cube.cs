﻿using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace UnityEngine.ProBuilder.Shapes
{
    public class Cube : Shape
    {
        static readonly Vector3[] k_CubeVertices = new Vector3[]
        {
            // bottom 4 verts
            new Vector3(-.5f, -.5f, .5f),
            new Vector3(.5f, -.5f, .5f),
            new Vector3(.5f, -.5f, -.5f),
            new Vector3(-.5f, -.5f, -.5f),

            // top 4 verts
            new Vector3(-.5f, .5f, .5f),
            new Vector3(.5f, .5f, .5f),
            new Vector3(.5f, .5f, -.5f),
            new Vector3(-.5f, .5f, -.5f)
        };

        static readonly int[] k_CubeTriangles = new int[]
        {
            0, 1, 4, 5,
            1, 2, 5, 6,
            2, 3, 6, 7,
            3, 0, 7, 4,
            4, 5, 7, 6,
            3, 2, 0, 1
        };

        static readonly Face[] k_CubeFaces = new Face[]
        {
            new Face(new[] {  0,  1,  2,  1,  3,  2 }),
            new Face(new[] {  4,  5,  6,  5,  7,  6 }),
            new Face(new[] {  8,  9, 10,  9, 11, 10 }),
            new Face(new[] { 12, 13, 14, 13, 15, 14 }),
            new Face(new[] { 16, 17, 18, 17, 19, 18 }),
            new Face(new[] { 20, 21, 22, 21, 23, 22 })
        };

        ProBuilderMesh m_Mesh;

        public override GameObject gameObject
        {
            get { return m_Mesh != null ? m_Mesh.gameObject : null; }
        }

        public override void DoSettings()
        {
        }

        public override void Initialize()
        {
            Vector3[] points = new Vector3[k_CubeTriangles.Length];

            for (var i = 0; i < k_CubeTriangles.Length; i++)
                points[i] = Vector3.Scale(k_CubeVertices[k_CubeTriangles[i]], Vector3.one);

            m_Mesh = ProBuilderMesh.Create(points, k_CubeFaces);
        }

        public override void Rebuild(Vector3 bounds)
        {
            var positions = m_Mesh.positionsInternal;

            for (var i = 0; i < k_CubeTriangles.Length; i++)
                positions[i] = Vector3.Scale(k_CubeVertices[k_CubeTriangles[i]], bounds);

            m_Mesh.ToMesh();
            m_Mesh.Refresh();
        }

        public override void Complete()
        {

        }
    }
}
