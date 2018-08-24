using UnityEngine;

namespace UnityEngine.ProBuilder
{
	public abstract class Shape
	{
		public abstract GameObject gameObject { get; }
		public abstract void Initialize();
		public abstract void Rebuild(Vector3 size);
		public abstract void Complete();
		public virtual void DoSettings() { }

		public virtual void Cancelled()
		{
			if(gameObject != null)
				Object.DestroyImmediate(gameObject);
		}

		public Transform transform
		{
			get { return gameObject.transform; }
		}
	}
}
