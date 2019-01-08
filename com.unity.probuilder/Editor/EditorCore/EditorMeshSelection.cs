using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace UnityEditor.ProBuilder
{
	[Serializable]
	class MeshSelectionObject : ScriptableSingleton<MeshSelectionObject>
	{
		public MeshSelection meshSelection = new MeshSelection();
	}

	/// <summary>
	/// This keeps the runtime MeshSelection static class in sync with the Editor selection.
	/// </summary>
	[InitializeOnLoad]
	static class EditorMeshSelection
	{
		public static event Action meshSelectionWillChange;
		public static event Action meshSelectionDidChange;

		static EditorMeshSelection()
		{
			Selection.selectionChanged += UnitySelectionChanged;
		}
		
		static MeshSelection selection
		{
			get { return MeshSelectionObject.instance.meshSelection; }
		}

		static IEnumerable<ProBuilderMesh> top
		{
			get { return selection.meshes; }
		}

		static void UnitySelectionChanged()
		{
			selection.SyncUnitySelection(Selection.gameObjects);
		}
	}

}
