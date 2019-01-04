using System;

namespace UnityEngine.ProBuilder
{
	public class CachedValue<T>
	{
		bool m_IsDirty;
		T m_Value;
		Func<T> m_GetValue;

		public T value
		{
			get
			{
				if (m_IsDirty)
				{
					m_Value = m_GetValue();
					m_IsDirty = false;
				}
				
				return m_Value;
			}

			set
			{
				m_Value = value;
				m_IsDirty = false;
			}
		}

		public CachedValue(Func<T> getValue)
		{
			m_IsDirty = true;
			m_GetValue = getValue;
		}

		public void SetDirty()
		{
			m_IsDirty = true;
		}
	}
}
