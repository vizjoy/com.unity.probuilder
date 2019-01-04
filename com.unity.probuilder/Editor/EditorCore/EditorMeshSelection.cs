using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace UnityEditor.ProBuilder
{
	/// <summary>
	/// This keeps the runtime MeshSelection static class in sync with the Editor selection.
	/// </summary>
	[InitializeOnLoad]
	static class EditorMeshSelection
	{
		static EditorMeshSelection()
		{
			Selection.selectionChanged += UnitySelectionChanged;
//			EditorMeshUtility.meshOptimized += (x, y) => { s_TotalElementCountCacheIsDirty = true; };
		}

		static ProBuilderMesh[] selection
		{
			get
			{
				return ProBuilderEditor.instance != null
					? ProBuilderEditor.instance.selection
					: InternalUtility.GetComponents<ProBuilderMesh>(Selection.transforms);
			}
		}

		static void UnitySelectionChanged()
		{

		}
	}

}
