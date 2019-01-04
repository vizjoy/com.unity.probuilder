using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder
{
	static class SelectionUtility
	{
        internal static void CalculateAAB(IEnumerable<MeshAndElementSelection> selection)
        {
            var bounds = new Bounds();
            var boundsInitialized = false;

            foreach(var entry in selection)
            {
                var mesh = entry.mesh;

                // Undo causes this state
                if (mesh == null)
                    return;

                if (mesh.selectedVertexCount > 0)
                {
                    if (!boundsInitialized)
                    {
                        boundsInitialized = true;
                        bounds = new Bounds(
                            mesh.transform.TransformPoint(mesh.positionsInternal[entry.vertices.First()]),
                            Vector3.zero);
                    }

                    var shared = mesh.sharedVerticesInternal;

                    foreach (var sharedVertex in mesh.selectedSharedVertices)
                        bounds.Encapsulate(mesh.transform.TransformPoint(mesh.positionsInternal[shared[sharedVertex][0]]));
                }
            }
        }
	}
}
