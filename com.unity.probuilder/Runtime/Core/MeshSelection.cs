using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder
{
	[Serializable]
	class MeshSelection
	{
		[SerializeField]
		List<MeshAndElementSelection> m_Selection = new List<MeshAndElementSelection>();


		public IEnumerable<MeshAndElementSelection> selection
		{
			get { return m_Selection; }
		}

		internal static void SyncUnitySelection(GameObject[] gameObjects)
		{

		}
	}
}
