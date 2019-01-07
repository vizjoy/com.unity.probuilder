using System.Collections.Generic;

namespace UnityEngine.ProBuilder
{
	public interface ISelectable
	{
		IEnumerable<T> Convert<T>() where T : ISelectable;

		void AppendIndices(List<int> indices);
	}
}
