using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder
{
	[Serializable]
	class MeshSelection : ISerializationCallbackReceiver
	{
		Dictionary<ProBuilderMesh, AttributeSelection> m_Selection = new Dictionary<ProBuilderMesh, AttributeSelection>();

		[SerializeField]
		ProBuilderMesh[] m_SelectionKeys;

		[SerializeField]
		AttributeSelection[] m_SelectionValue;

		public static event Action meshSelectionWillChange;
		public static event Action meshSelectionDidChange;
		public static event Action elementSelectionWillChange;
		public static event Action elementSelectionDidChange;

		public MeshSelection()
		{
			AttributeSelection.selectionWillChange += (x) =>
			{
				if (elementSelectionWillChange != null)
					elementSelectionWillChange();
			};

			AttributeSelection.selectionDidChange += (x) =>
			{
				if (elementSelectionDidChange != null)
					elementSelectionDidChange();
			};
		}

		public void OnBeforeSerialize()
		{
			m_SelectionKeys = m_Selection.Keys.ToArray();
			m_SelectionValue = m_Selection.Values.ToArray();
		}

		public void OnAfterDeserialize()
		{
			for (int i = 0, c = m_SelectionKeys.Length; i < c; i++)
			{
				m_Selection.Add(m_SelectionKeys[i], m_SelectionValue[i]);
			}
		}

		public IEnumerable<ProBuilderMesh> meshes
		{
			get { return m_Selection.Keys; }
		}

		public IEnumerable<T> GetSelectedElements<T>(ProBuilderMesh mesh) where T : ISelectable
		{
			AttributeSelection selection;

			if (m_Selection.TryGetValue(mesh, out selection))
				return selection.Get<T>();

			return new T[0];
		}

		internal void SyncUnitySelection(GameObject[] gameObjects)
		{
			var meshes = gameObjects
				.Select(x => x.GetComponent<ProBuilderMesh>())
					.Where(x => x != null);

			var selection = new Dictionary<ProBuilderMesh, AttributeSelection>();

			foreach (var mesh in meshes)
			{
				if (m_Selection.ContainsKey(mesh))
					selection.Add(mesh, m_Selection[mesh]);
				else
					selection.Add(mesh, new AttributeSelection());
			}

			m_Selection = selection;
		}
	}
}
