using UnityEngine;
using UnityEditor;
using UnityEditor.ProBuilder.UI;
using System.Linq;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace UnityEditor.ProBuilder.Actions
{
    sealed class SetPivotToSelection : MenuAction
    {
        public override ToolbarGroup group { get { return ToolbarGroup.Geometry; } }
        public override Texture2D icon { get { return IconUtility.GetIcon("Toolbar/Pivot_CenterOnElements", IconSkin.Pro); } }
        public override TooltipContent tooltip { get { return _tooltip; } }
        public override string menuTitle { get { return "Set Pivot"; } }

        static readonly TooltipContent _tooltip = new TooltipContent
            (
                "Set Pivot to Center of Selection",
                @"Moves the pivot point of each mesh to the average of all selected elements positions.  This means the pivot point moves to where-ever the handle currently is.",
                keyCommandSuper, 'J'
            );

        public override SelectMode validSelectModes
        {
            get { return SelectMode.Vertex | SelectMode.Edge | SelectMode.Face; }
        }

        public override bool enabled
        {
            get
            {
                return base.enabled && (MeshSelectionOld.selectedVertexCount > 0
                                        || MeshSelectionOld.selectedEdgeCount > 0
                                        || MeshSelectionOld.selectedFaceCount > 0);
            }
        }

        public override ActionResult DoAction()
        {
            if (MeshSelectionOld.selectedObjectCount < 1)
                return ActionResult.NoSelection;

            Object[] objects = new Object[MeshSelectionOld.selectedObjectCount * 2];

            for (int i = 0, c = MeshSelectionOld.selectedObjectCount; i < c; i++)
            {
                objects[i] = MeshSelectionOld.topInternal[i];
                objects[i + c] = MeshSelectionOld.topInternal[i].transform;
            }

            UndoUtility.RegisterCompleteObjectUndo(objects, "Set Pivot");

            foreach (var mesh in MeshSelectionOld.topInternal)
            {
                TransformUtility.UnparentChildren(mesh.transform);
                mesh.CenterPivot(mesh.selectedIndexesInternal);
                mesh.Optimize();
                TransformUtility.ReparentChildren(mesh.transform);
            }

            ProBuilderEditor.Refresh();

            return new ActionResult(ActionResult.Status.Success, "Set Pivot");
        }
    }
}
