# Video: UV Editor Toolbar

[![UV Editor Toolbar Video](../images/VideoLink_YouTube_768.png)](@todo)

---

<div style="text-align:center">
<img src="../../images/UVToolbar.png">
</div>

---

## Select, Move, Rotate, Scale

This first group of buttons contains shortcuts to the standard Unity manipulation modes. Clicking these will have the exact same effect as clicking on the main Unity toolbar buttons.

---

## Vertex, Edge Face

The second button group contains shortcuts to ProBuilder's [Element Editing Modes](@todo). When using [Manual UV Editing](@todo), this allows you to select and manipulate UVs by Vertex, Edge, or Face.

---

## ![In-Scene Controls Toggle](../images/icons/ProBuilderGUI_UV_Manip_On.png) In-Scene Controls

<div class="info-box warning">
Section Video: <a href="@todo">UV Editor Toolbar: In-Scene Controls</a>
</div>

When **On**, you can use Unity's standard Move, Rotate, and Scale tools to directly manipulate UVs in the scene, without affecting geometry.

Toolbar Icon | Description
:---:|---
![In-Scene ON](../images/icons/ProBuilderGUI_UV_Manip_On.png) | **On** : Move, Rotate, and Scale tools will affect UVs, geometry will not be affected
![In-Scene OFF](../images/icons/ProBuilderGUI_UV_Manip_OFF.png) |  **Off** : Move, Rotate, and Scale tools will return to normal geometry actions

<div style="text-align:center">
<img src="../../images/UV_InSceneControls.png">
</div>


> Snap to increments by holding `CTRL` . You can customize these increment values via the [ProBuilder Preferences](@todo)

---

## ![Texture Preview Toggle](../images/icons/ProBuilderGUI_UV_ShowTexture_On.png) Texture Preview

<div class="info-box warning">
Section Video: <a href="@todo">UV Editor Toolbar: Texture Preview</a>
</div>

When **On**, the selected face's main texture will be displayed in the [UV Viewer](@todo).

Toolbar Icon | Description
:---:|---
![In-Scene ON](../images/icons/ProBuilderGUI_UV_ShowTexture_On.png) | **On** : Selected element's Texture will be displayed in the [UV Viewer](@todo)
![In-Scene OFF](../images/icons/ProBuilderGUI_UV_ShowTexture_Off.png) |  **Off** : No texture will be displayed in the [UV Viewer](@todo)

<div style="text-align:center">
<img src="../../images/ShowTexturePreview_Example.png">
</div>

---

## ![Render UV Template Button](../images/icons/ProBuilderGUI_UV_Manip_On.png) Render UV Template

<div class="info-box warning">
Section Video: <a href="@todo">UV Editor Toolbar: Render UV Template</a>
</div>

Opens the Render UVs tool panel, for rendering UV Templates to be used with texture map painting, atlasing, sprite sheets, etc.

<div style="text-align:center">
<img src="../../images/RenderUVsPanel.png">
</div>

* **Image Size** : Total size of the rendered template (always square)

* **Hide Grid** : Should the grid be hidden in the render?

* **Line Color** : What color should UV lines be rendered as?

* **Transparent Background** : Should the background be rendered transparent?

* **Background Color** : If you want a non-transparent background, set the color here

* **Save UV Template** : Click to render the UV Template - a file dialogue will be opened to save the file. 
  

   