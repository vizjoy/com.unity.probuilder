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

        public Vector3[] localCorners
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

        internal bool IntersectRay(Vector3 origin, Vector3 direction, out float distance)
        {
            float dist = Mathf.Infinity, best = dist;
            direction.Normalize();

            Vector3 up = (rotation * Vector3.up); up.Normalize();
            Vector3 down = (rotation * Vector3.down); down.Normalize();
            Vector3 right = (rotation * Vector3.right); right.Normalize();
            Vector3 left = (rotation * Vector3.left); left.Normalize();
            Vector3 forward = (rotation * Vector3.forward); forward.Normalize();
            Vector3 back = (rotation * Vector3.back); back.Normalize();

            if (Math.RayIntersectsPlane(origin, direction, center + (up * extents.y), up, out dist))
                best = Mathf.Min(best, dist);

            if (Math.RayIntersectsPlane(origin, direction, center + (down * extents.y), down, out dist))
                best = Mathf.Min(best, dist);

            if (Math.RayIntersectsPlane(origin, direction, center + (right * extents.x), right, out dist))
                best = Mathf.Min(best, dist);

            if (Math.RayIntersectsPlane(origin, direction, center + (left * extents.x), left, out dist))
                best = Mathf.Min(best, dist);

            if (Math.RayIntersectsPlane(origin, direction, center + (forward * extents.z), forward, out dist))
                best = Mathf.Min(best, dist);

            if (Math.RayIntersectsPlane(origin, direction, center + (back * extents.z), back, out dist))
                best = Mathf.Min(best, dist);

            if (dist < Mathf.Infinity)
            {
                distance = best;
                return true;
            }

            distance = 0f;
            return false;
        }
    }
}
