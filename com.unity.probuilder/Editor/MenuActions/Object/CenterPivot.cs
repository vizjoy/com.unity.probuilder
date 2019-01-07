using UnityEngine.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

namespace UnityEditor.ProBuilder.Actions
{
    sealed class CenterPivot : MenuAction
    {
        public override ToolbarGroup group
        {
            get { return ToolbarGroup.Object; }
        }

        public override Texture2D icon
        {
            get { return IconUtility.GetIcon("Toolbar/Pivot_CenterOnObject", IconSkin.Pro); }
        }

        public override TooltipContent tooltip
        {
            get { return s_Tooltip; }
        }

        static readonly TooltipContent s_Tooltip = new TooltipContent
            (
                "Center Pivot",
                @"Set the pivot point of this object to the center of it's bounds."
            );

        public override bool enabled
        {
            get { return base.enabled && MeshSelectionOld.selectedObjectCount > 0; }
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

            UndoUtility.RegisterCompleteObjectUndo(objects, "Center Pivot");

            foreach (var mesh in MeshSelectionOld.topInternal)
            {
                TransformUtility.UnparentChildren(mesh.transform);
                mesh.CenterPivot(null);
                mesh.Optimize();
                TransformUtility.ReparentChildren(mesh.transform);
            }

            ProBuilderEditor.Refresh();

            return new ActionResult(ActionResult.Status.Success, "Center Pivot");
        }
    }
}
