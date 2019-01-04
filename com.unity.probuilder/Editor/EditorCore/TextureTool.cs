using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace UnityEditor.ProBuilder
{
    abstract class TextureTool : VertexManipulationTool
    {
        protected const int k_TextureChannel = 0;

        internal class TextureModificationData
        {
            public List<Vector4> origins;
            public List<Vector4> textures;
            public List<TextureModificationElementGroup> elementGroups;
        }

        internal class TextureModificationElementGroup
        {
            public IEnumerable<int> indices;
            public Matrix4x4 preApplyMatrix;
            public Matrix4x4 postApplyMatrix;
        }

        Dictionary<ProBuilderMesh, TextureModificationData> m_TextureSelection = new Dictionary<ProBuilderMesh, TextureModificationData>();

        internal TextureModificationData GetCachedData(ProBuilderMesh mesh)
        {
            return m_TextureSelection[mesh];
        }

        const string k_UnityMoveSnapX = "MoveSnapX";
        const string k_UnityMoveSnapY = "MoveSnapY";
        const string k_UnityMoveSnapZ = "MoveSnapZ";
        const string k_UnityScaleSnap = "ScaleSnap";
        const string k_UnityRotateSnap = "RotationSnap";

        protected static float relativeSnapX
        {
            get { return EditorPrefs.GetFloat(k_UnityMoveSnapX, 1f); }
        }

        protected static float relativeSnapY
        {
            get { return EditorPrefs.GetFloat(k_UnityMoveSnapY, 1f); }
        }

        protected static float relativeSnapZ
        {
            get { return EditorPrefs.GetFloat(k_UnityMoveSnapZ, 1f); }
        }

        protected static float relativeSnapScale
        {
            get { return EditorPrefs.GetFloat(k_UnityScaleSnap, .1f); }
        }

        protected static float relativeSnapRotation
        {
            get { return EditorPrefs.GetFloat(k_UnityRotateSnap, 15f); }
        }

        protected override void OnToolEngaged()
        {
            base.OnToolEngaged();

            m_TextureSelection.Clear();

            foreach (var selection in elementSelection.value)
            {
                var data = new TextureModificationData();
                data.textures = new List<Vector4>();
                selection.mesh.GetUVs(k_TextureChannel, data.textures);
                data.origins = new List<Vector4>(data.textures);

                var groups = new List<TextureModificationElementGroup>();

                foreach (var group in selection.elementGroups)
                {
                    var grp = new TextureModificationElementGroup();
                    grp.indices = group.indices;
                    grp.preApplyMatrix = Matrix4x4.Translate(-Bounds2D.Center(data.origins, grp.indices));
                    grp.postApplyMatrix = grp.preApplyMatrix.inverse;
                    groups.Add(grp);
                }

                data.elementGroups = groups;

                m_TextureSelection.Add(selection.mesh, data);
            }
        }

        // Texture tools do not update the ProBuilderMesh in realtime, but rather the MeshFilter.sharedMesh.
        // After the modification is finished, apply the changes back to ProBuilderMesh.
        protected override void OnToolDisengaged()
        {
            var isFaceMode = ProBuilderEditor.selectMode.ContainsFlag(SelectMode.TextureFace | SelectMode.Face);

            foreach (var selection in elementSelection.value)
            {
                var data = GetCachedData(selection.mesh);

                if (isFaceMode)
                {
                    foreach (var face in selection.mesh.selectedFacesInternal)
                        face.manualUV = true;
                }
                else
                {
                    var indices = new HashSet<int>(data.elementGroups.SelectMany(x => x.indices));

                    foreach (var face in selection.mesh.facesInternal)
                    {
                        foreach (var index in face.distinctIndexesInternal)
                        {
                            if (indices.Contains(index))
                            {
                                face.manualUV = true;
                                break;
                            }
                        }
                    }
                }

                var textures = GetCachedData(selection.mesh).textures;
                selection.mesh.SetUVs(k_TextureChannel, textures);
            }
        }
    }
}
