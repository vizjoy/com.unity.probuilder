using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.ProBuilder
{
    [System.Serializable]
    public struct OrientedBoundingBox
    {
        [SerializeField]
        Vector3 m_Center;

        [SerializeField]
        Quaternion m_Rotation;

        [SerializeField]
        Vector3 m_Extents;

        public Vector3 center
        {
            get { return m_Center; }
            set { m_Center = value; }
        }

        public Quaternion rotation
        {
            get { return m_Rotation; }
            set { m_Rotation = value; }
        }

        public Vector3 extents
        {
            get { return m_Extents; }
            set { m_Extents = value; }
        }

        public Vector3 size
        {
            get { return m_Extents * 2f; }
            set { m_Extents = value * .5f; }
        }

        public Vector3[] corners
        {
            get
            {
                var e = m_Extents;

                return new Vector3[8]
                {
                    new Vector3(-e.x, -e.y, -e.z),
                    new Vector3( e.x, -e.y, -e.z),
                    new Vector3(-e.x, -e.y,  e.z),
                    new Vector3( e.x, -e.y,  e.z),

                    new Vector3(-e.x,  e.y, -e.z),
                    new Vector3( e.x,  e.y, -e.z),
                    new Vector3(-e.x,  e.y,  e.z),
                    new Vector3( e.x,  e.y,  e.z)
                };
            }
        }

    }
}
