using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder.UI;
using UnityEditorInternal;
using UnityEngine.ProBuilder.MeshOperations;

namespace UnityEditor.ProBuilder
{
	/// <summary>
	/// Shape creation panel implementation.
	/// </summary>
	sealed class ShapeEditor2 : Editor
	{
		enum EditState
		{
			None,
			Base,
			Height
		}

		const float k_MinBoundingBoxSize = .01f;
		static readonly Rect k_SettingsWindowDragRect = new Rect(0f, 0f, 1000f, 20f);
		Rect m_SettingsRect = new Rect(4, 18, 200, 100);
		EditState m_EditState = EditState.None;
		Plane m_Plane;
		float m_ExtrusionOffset;
		Tool m_PreviousTool;
		static ShapeEditor2 s_Instance;
		Shape m_Shape;
		ShapeType m_ShapeType;
		BoxBoundsHandle m_BoundsEditor;

		Vector3 m_BoundingBoxOrigin;
		Vector3 m_BoundingBoxCorner;
		float m_ExtrudeDistance;

		public static void MenuOpenShapeCreator()
		{
			if(s_Instance == null)
				CreateInstance<ShapeEditor>();
		}

		void OnEnable()
		{
			s_Instance = this;
			m_PreviousTool = Tools.current;
			Tools.current = Tool.None;
			if (ProBuilderEditor.instance != null)
				ProBuilderEditor.selectMode = SelectMode.None;
			m_BoundsEditor = new BoxBoundsHandle();

			SceneView.onSceneGUIDelegate += OnSceneGUI;
			ProBuilderEditor.selectModeChanged += OnSelectModeChanged;
			EditorApplication.hierarchyChanged += HierarchyWindowChanged;
		}

		void OnDisable()
		{
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			ProBuilderEditor.selectModeChanged -= OnSelectModeChanged;
			EditorApplication.hierarchyChanged -= HierarchyWindowChanged;
		}

		void OnSelectModeChanged(SelectMode mode)
		{
			ExitShapeEditing();
		}

		void HierarchyWindowChanged()
		{
			if (m_Shape != null && m_Shape.gameObject == null)
			{
				m_Shape = null;
				SceneView.RepaintAll();
			}
		}

		void DoShapeSettingsGUI(int id)
		{
			GUI.DragWindow(k_SettingsWindowDragRect);
//			GUILayout.Label("state: " + m_EditState);

			m_ShapeType = (ShapeType) EditorGUILayout.EnumPopup(m_ShapeType);

			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Exit"))
				DestroyImmediate(this);
		}

		void OnSceneGUI(SceneView view)
		{
			m_SettingsRect = GUI.Window(GetInstanceID(), m_SettingsRect, DoShapeSettingsGUI, "Shape Settings");

			var evt = Event.current;

			if (Tools.current != Tool.None)
				ExitShapeEditing();

			if (evt.isKey && (evt.keyCode == KeyCode.Escape || evt.keyCode == KeyCode.Return))
				ExitShapeEditing();

			if (m_Shape != null)
			{
				if (m_EditState == EditState.None)
					DoEditStateBounds();
				else
					DrawBoundingBox();
			}

			if( EditorHandleUtility.SceneViewInUse(evt) )
				return;

			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			HandleUtility.AddDefaultControl(controlID);

			switch (m_EditState)
			{
				case EditState.None:
					DoEditStateNone();
					break;
				case EditState.Base:
					DoEditStateBase();
					break;
				case EditState.Height:
					DoEditStateHeight();
					break;
			}
		}

		float GetExtrusionDistance(Ray ray)
		{
			Vector3 p = Math.GetNearestPointRayRay(m_BoundingBoxCorner, m_Plane.normal, ray.origin, ray.direction);
			return Vector3.Distance(m_BoundingBoxCorner, p) * Mathf.Sign(Vector3.Dot(m_Plane.normal, p - m_BoundingBoxCorner));
		}

		/// <summary>
		/// Get a bounding box from the mouse drag coordinates.
		/// </summary>
		/// <returns></returns>
		OrientedBoundingBox GetBoundingBox()
		{
			var bb = new OrientedBoundingBox();
			bb.center = ((m_BoundingBoxCorner + m_BoundingBoxOrigin) * .5f) + (m_Plane.normal * (m_ExtrudeDistance * .5f));
			bb.rotation = (m_Plane.normal.sqrMagnitude < 1f ? Quaternion.identity : Quaternion.LookRotation(m_Plane.normal)) * Quaternion.Euler(new Vector3(90f, 0f, 0f));
			var inv = Quaternion.Inverse(bb.rotation);
			var origin = inv * m_BoundingBoxOrigin;
			var corner = inv * m_BoundingBoxCorner;
			origin.y = 0f;
			corner.y = m_ExtrudeDistance;
			bb.extents = Math.Abs(corner - origin) * .5f;
			return bb;
		}

		void DoEditStateNone()
		{
			var evt = Event.current;

			if (evt.type == EventType.MouseDown && evt.button == 0 && !EditorHandleUtility.IsAppendModifier(evt.modifiers))
			{
				if (!EditorHandleUtility.GetPlane(evt.mousePosition, out m_Plane))
					m_Plane.SetNormalAndPosition(m_Plane.normal, SceneView.lastActiveSceneView.pivot);

				Ray ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);

				float distance;

				if (m_Plane.Raycast(ray, out distance))
				{
					m_Shape = ShapeUtility.GetShapeGenerator(m_ShapeType);
					m_Shape.Initialize();
					m_Shape.gameObject.SetActive(false);

					m_BoundingBoxOrigin = ray.GetPoint(distance);
					m_BoundingBoxCorner = m_BoundingBoxOrigin;
					m_ExtrudeDistance = 0f;

					m_EditState = EditState.Base;

					evt.Use();
				}
			}
		}

		void DoEditStateBase()
		{
			var evt = Event.current;

			if (evt.isMouse && !EditorHandleUtility.IsAppendModifier(evt.modifiers))
			{
				Ray ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
				float distance;

				if (m_Plane.Raycast(ray, out distance))
					m_BoundingBoxCorner = ray.GetPoint(distance);

				if (evt.type == EventType.MouseUp)
				{
					var bb = GetBoundingBox();

					if (bb.size.x < k_MinBoundingBoxSize
						|| bb.size.z < k_MinBoundingBoxSize
						|| bb.size.magnitude < Mathf.Pow(k_MinBoundingBoxSize, 2f))
					{
						ClearShape();
						m_EditState = EditState.None;
					}
					else
					{
						m_ExtrusionOffset = GetExtrusionDistance(ray);
						m_EditState = EditState.Height;
					}
				}

				RebuildShape();
				SceneView.lastActiveSceneView.Repaint();
				evt.Use();
			}
		}

		void DoEditStateHeight()
		{
			var evt = Event.current;
			var ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);

			m_ExtrudeDistance = (GetExtrusionDistance(ray) - m_ExtrusionOffset);

			if(evt.isMouse)
				RebuildShape();

			if (evt.type == EventType.MouseDown ||
				evt.type == EventType.MouseUp)
			{
				evt.Use();
				MeshSelection.SetSelection(m_Shape.gameObject);
				m_BoundsEditor.center = Vector3.zero;
				m_BoundsEditor.size = GetBoundingBox().size;
				m_EditState = EditState.None;
			}

			if(evt.isMouse)
				SceneView.RepaintAll();
		}

		void DoEditStateBounds()
		{
			var shapeBounds = GetBoundingBox();

			if (shapeBounds.size.sqrMagnitude < 0.1f)
				return;

			var matrix = Matrix4x4.TRS(shapeBounds.center, shapeBounds.rotation, Vector3.one);

			using (new Handles.DrawingScope(matrix))
			{
				m_BoundsEditor.SetColor(Handles.selectedColor);

				EditorGUI.BeginChangeCheck();
				m_BoundsEditor.DrawHandle();
				if (EditorGUI.EndChangeCheck())
				{
					shapeBounds.center = matrix.MultiplyPoint3x4(m_BoundsEditor.center);
					shapeBounds.size = m_BoundsEditor.size;
					m_Shape.Rebuild(shapeBounds.size);
					ProBuilderEditor.Refresh();
				}
			}
		}

		void ExitShapeEditing()
		{
			if (m_Shape != null)
			{
				if (m_EditState == EditState.Base)
				{
					m_Shape.Cancelled();
				}
				else
				{
					m_Shape.Complete();
					MeshSelection.SetSelection(m_Shape.gameObject);
				}
			}
			Tools.current = m_PreviousTool == Tool.None ? Tool.Move : m_PreviousTool;
			DestroyImmediate(this);
			SceneView.RepaintAll();
		}

		void RebuildShape()
		{
			if (m_Shape == null)
				return;

			var bb = GetBoundingBox();
			m_Shape.Rebuild(bb.size);
			m_Shape.transform.position = bb.center;
			m_Shape.transform.rotation = bb.rotation;

			m_Shape.gameObject.SetActive(true);
		}

		void ClearShape()
		{
			m_Shape.Cancelled();
			m_Shape = null;
		}

		void DrawBoundingBox()
		{
			var bb = GetBoundingBox();

			using (new Handles.DrawingScope(Handles.selectedColor, Matrix4x4.TRS(bb.center, bb.rotation, Vector3.one)))
			{
				var corners = bb.corners;
				const float length = .15f;

				DrawCorner(corners[0], corners[1], corners[2], corners[4], length);
				DrawCorner(corners[1], corners[0], corners[3], corners[5], length);
				DrawCorner(corners[2], corners[0], corners[3], corners[6], length);
				DrawCorner(corners[3], corners[1], corners[2], corners[7], length);

				DrawCorner(corners[4], corners[5], corners[6], corners[0], length);
				DrawCorner(corners[5], corners[4], corners[7], corners[1], length);
				DrawCorner(corners[6], corners[4], corners[7], corners[2], length);
				DrawCorner(corners[7], corners[5], corners[6], corners[3], length);
			}
		}

		void DrawCorner(Vector3 origin, Vector3 a, Vector3 b, Vector3 c, float size)
		{
			size = HandleUtility.GetHandleSize(origin) * size;

			Handles.DrawLine(origin, origin + (a-origin).normalized * size);
			Handles.DrawLine(origin, origin + (b-origin).normalized * size);
			Handles.DrawLine(origin, origin + (c-origin).normalized * size);
		}

	}
}
