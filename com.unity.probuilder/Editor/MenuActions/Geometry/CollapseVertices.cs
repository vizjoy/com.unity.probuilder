using UnityEngine;
using UnityEditor;
using UnityEditor.ProBuilder.UI;
using System.Linq;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using EditorGUILayout = UnityEditor.EditorGUILayout;
using EditorStyles = UnityEditor.EditorStyles;

namespace UnityEditor.ProBuilder.Actions
{
    sealed class CollapseVertices : MenuAction
    {
        public override ToolbarGroup group
        {
            get { return ToolbarGroup.Geometry; }
        }

        public override Texture2D icon
        {
            get { return IconUtility.GetIcon("Toolbar/Vert_Collapse", IconSkin.Pro); }
        }

        public override TooltipContent tooltip
        {
            get { return s_Tooltip; }
        }

        static readonly TooltipContent s_Tooltip = new TooltipContent
            (
                "Collapse Vertices",
                @"Collapse all selected vertices into a single new vertex, placed at the current Handle position.",
                keyCommandAlt, 'C'
            );

        public override SelectMode validSelectModes
        {
            get { return SelectMode.Vertex; }
        }

        public override bool enabled
        {
            get { return base.enabled && MeshSelection.selectedVertexCountObjectMax > 1; }
        }

        // protected override MenuActionState optionsMenuState
        // {
        //     get { return MenuActionState.VisibleAndEnabled; }
        // }

        public override ActionResult DoAction()
        {
            if (MeshSelection.selectedObjectCount < 1)
                return ActionResult.NoSelection;

            bool success = false;

            UndoUtility.RecordSelection("Collapse Vertices");

            foreach (var mesh in MeshSelection.topInternal)
            {
                //-- 
                // Get the World position of Handle, then convert that position to Local Space
                // This way, we can collapse the selected verts to Handle's position
                // No more need for "Collapse to First", just use the Handle location (Center or Pivot)
                GameObject o = Selection.activeGameObject;
                Transform t = o.transform;
                Vector3 collapsePoint = t.InverseTransformPoint(MeshSelection.GetHandlePosition());
                //--

                if (mesh.selectedIndexesInternal.Length > 1)
                {
                    int newIndex = mesh.MergeVertices(mesh.selectedIndexesInternal, collapsePoint);

                    success = newIndex > -1;

                    if (success)
                        mesh.SetSelectedVertices(new int[] { newIndex });

                    mesh.ToMesh();
                    mesh.Refresh();
                    mesh.Optimize();
                }
            }

            ProBuilderEditor.Refresh();

            if (success)
                return new ActionResult(ActionResult.Status.Success, "Collapse Vertices");

            return new ActionResult(ActionResult.Status.Failure, "Collapse Vertices\nNo Vertices Selected");
        }
    }
}
