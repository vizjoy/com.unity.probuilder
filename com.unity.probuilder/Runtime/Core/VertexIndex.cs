using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder
{
	[Serializable]
	struct VertexIndex : ISelectable, IEquatable<VertexIndex>
	{
		[SerializeField]
		int m_Value;

		public int value
		{
			get { return m_Value; }
			set { m_Value = value; }
		}

		public static implicit operator int(VertexIndex value)
		{
			return value.m_Value;
		}

		public static implicit operator VertexIndex(int value)
		{
			return new VertexIndex(value);
		}

		public VertexIndex(int value) : this()
		{
			m_Value = value;
		}

		public IEnumerable<T> Convert<T>() where T : ISelectable
		{
			if (typeof(T) == typeof(VertexIndex))
				return new T[] { (T) (ISelectable) this };

			return new T[0];
		}

		public void AppendIndices(List<int> indices)
		{
			indices.Add(m_Value);
		}

		public bool Equals(VertexIndex other)
		{
			return m_Value == other.m_Value;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is VertexIndex && Equals((VertexIndex) obj);
		}

		public override int GetHashCode()
		{
			return m_Value.GetHashCode();
		}
	}
}
