using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace UnityEngine.ProBuilder.RuntimeTests.Type
{
	[TestFixture]
	public class TestSelection
	{
		// This and SelectableConversionCase work around TestCaseData not supporting generics
		public interface ISelectableConversionTestCase
		{
			void DoConvertAndCompare();
		}

		public class SelectableConversionCase<T, K> : ISelectableConversionTestCase
			where T : ISelectable
			where K : ISelectable
		{
			public T input;
			public K[] expected;

			public SelectableConversionCase(T input, K[] expected)
			{
				this.input = input;
				this.expected = expected;
			}

			public void DoConvertAndCompare()
			{
				CollectionAssert.AreEquivalent(expected, input.Convert<K>());
			}
		}

		public static IEnumerable TestConvertSelectableCases
		{
			get
			{
				var face = new Face(new int[] { 0, 1, 2, 1, 3, 2 });

				yield return new TestCaseData(
					new SelectableConversionCase<VertexIndex, VertexIndex>(
						new VertexIndex(42),
						new VertexIndex[] { 42 })).SetName("VertexIndex to VertexIndex");

				yield return new TestCaseData(
					new SelectableConversionCase<VertexIndex, Edge>(
						new VertexIndex(42),
						new Edge[0])).SetName("VertexIndex to Edge");

				yield return new TestCaseData(
					new SelectableConversionCase<VertexIndex, Face>(
						new VertexIndex(42),
						new Face[0])).SetName("VertexIndex to Face");

				yield return new TestCaseData(new SelectableConversionCase<Edge, VertexIndex>(
					new Edge(0, 1),
					new VertexIndex[] { 0, 1 })).SetName("Edge to Vertex");

				yield return new TestCaseData(new SelectableConversionCase<Edge, Edge>(
					new Edge(0, 1),
					new Edge[] { new Edge(0, 1) })).SetName("Edge to Edge");

				yield return new TestCaseData(new SelectableConversionCase<Edge, Face>(
					new Edge(0, 1),
					new Face[0])).SetName("Edge to Face");

				yield return new TestCaseData(new SelectableConversionCase<Face, VertexIndex>(
					face,
					new VertexIndex[] { 0, 1, 2, 3 })).SetName("Face to Vertex");

				yield return new TestCaseData(new SelectableConversionCase<Face, Edge>(
					face,
					new Edge[]
					{
						new Edge(0, 1),
						new Edge(1, 3),
						new Edge(2, 3),
						new Edge(0, 2),
					})).SetName("Face to Edge");

				yield return new TestCaseData(new SelectableConversionCase<Face, Face>(
					face,
					new Face[] { face })).SetName("Face to Face");
			}
		}

		[Test, TestCaseSource(typeof(TestSelection), "TestConvertSelectableCases")]
		public void ConvertSelectableTypesWorks(ISelectableConversionTestCase testCase)
		{
			testCase.DoConvertAndCompare();
		}
	}
}
