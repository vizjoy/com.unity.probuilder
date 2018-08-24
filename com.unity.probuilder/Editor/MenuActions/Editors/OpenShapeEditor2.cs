using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProBuilder.UI;

namespace UnityEditor.ProBuilder.Actions
{
	sealed class OpenShapeEditor2 : MenuAction
	{
		public override ToolbarGroup group { get { return ToolbarGroup.Tool; } }
		public override Texture2D icon { get { return IconUtility.GetIcon("Toolbar/Panel_Shapes", IconSkin.Pro); } }
		public override TooltipContent tooltip { get { return _tooltip; } }
		public override string menuTitle { get { return "Draw Shape"; } }
		public override int toolbarPriority { get { return 0; } }
		protected override bool hasFileMenuEntry { get { return false; } }

		static readonly TooltipContent _tooltip = new TooltipContent
		(
			"Draw Shape",
			""
		);

		public override bool enabled
		{
			get { return true; }
		}

		public override ActionResult DoAction()
		{
			ScriptableObject.CreateInstance<ShapeEditor2>();
			return new ActionResult(ActionResult.Status.Success, "Draw Shape");
		}
	}
}
