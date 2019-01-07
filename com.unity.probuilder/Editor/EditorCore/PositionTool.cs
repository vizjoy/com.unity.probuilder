//#define DEBUG_HANDLES

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace UnityEditor.ProBuilder
{
    abstract class PositionTool : VertexManipulationTool
    {
        const bool k_CollectCoincidentVertices = true;

#if APPLY_POSITION_TO_SPACE_GIZMO
        Matrix4x4 m_CurrentDelta = Matrix4x4.identity;
#endif

        Dictionary<ProBuilderMesh, Vector3[]> m_PositionOrigins = new Dictionary<ProBuilderMesh, Vector3[]>();

        protected Vector3[] GetPositionOrigins(ProBuilderMesh mesh)
        {
            return m_PositionOrigins[mesh];
        }

        internal Matrix4x4 GetPostApplyMatrix(ElementGroup group)
        {
            switch (pivotPoint)
            {
                case PivotPoint.Center:
                    return Matrix4x4.TRS(handlePositionOrigin, handleRotationOrigin, Vector3.one);

                case PivotPoint.ActiveElement:
                    return Matrix4x4.TRS(handlePositionOrigin, handleRotationOrigin, Vector3.one);

                case PivotPoint.IndividualOrigins:
                    return Matrix4x4.TRS(group.position, group.rotation, Vector3.one);

                default:
                    return Matrix4x4.identity;
            }
        }

        protected override void OnToolEngaged()
        {
            m_PositionOrigins.Clear();

            foreach (var sel in elementSelection.value)
            {
                var mesh = sel.mesh;
                var positions = mesh.positions.ToArray();
                var l2w = mesh.transform.localToWorldMatrix;

                for (int i = 0, c = positions.Length; i < c; i++)
                    positions[i] = l2w.MultiplyPoint3x4(positions[i]);

                m_PositionOrigins.Add(mesh, positions);
            }
        }

        protected override void DoTool(Vector3 handlePosition, Quaternion handleRotation)
        {
            if (isEditing && currentEvent.type == EventType.Repaint)
            {
                foreach (var key in elementSelection.value)
                {
                    foreach (var group in key.elementGroups)
                    {
#if DEBUG_HANDLES
                        var positions = ((MeshAndPositions)key).positions;
                        var postApplyMatrix = GetPostApplyMatrix(group);
                        var preApplyMatrix = postApplyMatrix.inverse;

                        using (var faceDrawer = new EditorMeshHandles.TriangleDrawingScope(Color.cyan,
                                       UnityEngine.Rendering.CompareFunction.Always))
                        {
                            foreach (var face in key.mesh.GetSelectedFaces())
                            {
                                var indices = face.indexesInternal;

                                for (int i = 0, c = indices.Length; i < c; i += 3)
                                {
                                    faceDrawer.Draw(
                                        preApplyMatrix.MultiplyPoint3x4(positions[indices[i]]),
                                        preApplyMatrix.MultiplyPoint3x4(positions[indices[i + 1]]),
                                        preApplyMatrix.MultiplyPoint3x4(positions[indices[i + 2]])
                                        );
                                }
                            }
                        }
#endif
                    }
                }
            }
        }

        protected void Apply(Matrix4x4 delta)
        {
#if APPLY_POSITION_TO_SPACE_GIZMO
            m_CurrentDelta.SetColumn(3, delta.GetColumn(3));
#endif

            foreach (var selection in elementSelection.value)
            {
                var mesh = selection.mesh;
                var worldToLocal = mesh.transform.worldToLocalMatrix;
                var origins = m_PositionOrigins[mesh];
                var positions = mesh.positionsInternal;

                foreach (var group in selection.elementGroups)
                {
                    var postApplyMatrix = GetPostApplyMatrix(group);
                    var preApplyMatrix = postApplyMatrix.inverse;

                    foreach (var index in group.indices)
                    {
                        positions[index] = worldToLocal.MultiplyPoint3x4(
                                postApplyMatrix.MultiplyPoint3x4(
                                    delta.MultiplyPoint3x4(preApplyMatrix.MultiplyPoint3x4(origins[index]))));
                    }
                }

                mesh.mesh.vertices = positions;
                mesh.RefreshUV(MeshSelectionOld.selectedFacesInEditZone[mesh]);
                mesh.Refresh(RefreshMask.Normals);
            }

            ProBuilderEditor.UpdateMeshHandles(false);
        }
    }
}
