using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.EditorTools;
using UnityEngine.ProBuilder;
using UnityEditor.SettingsManagement;

namespace UnityEditor.ProBuilder
{
    [CustomEditor(typeof(ProBuilderEditor))]
    sealed class EditorToolbar : Editor
    {
        EditorToolbar()
        {
            m_ScrollIconDown = null;
        }

        [UserSetting("Toolbar", "Shift Key Tooltips", "Tooltips will only show when the Shift key is held")]
        internal static Pref<bool> s_ShiftOnlyTooltips = new Pref<bool>("editor.shiftOnlyTooltips", false, SettingsScope.User);

        Pref<Vector2> m_Scroll = new Pref<Vector2>("editor.scrollPosition", Vector2.zero, SettingsScope.User);

        bool m_IsIconMode = true;

        SimpleTuple<string, double> m_TooltipTimer = new SimpleTuple<string, double>("", 0.0);
        // the element currently being hovered
        string m_HoveringTooltipName = "";
        // the mouse has hovered > tooltipTimerRefresh
        bool m_ShowTooltipTimer = false;
        // how long a tooltip will wait before showing
        float m_TooltipTimerRefresh = 1f;

        Texture2D m_ScrollIconUp;
        Texture2D m_ScrollIconDown;
        Texture2D m_ScrollIconRight;
        Texture2D m_ScrollIconLeft;
        List<MenuAction> m_Actions;
        int m_ActionsLength = 0;

        // animated scrolling vars
        bool m_DoAnimateScroll;
        Vector2 m_ScrollOrigin = Vector2.zero;
        Vector2 m_ScrollTarget = Vector2.zero;
        double m_ScrollStartTime;
        float m_ScrollTotalTime;
        const float k_Scroll_Pixels_Per_Second = 1250f;

        const int k_ScrollButtonSize = 11;

        bool m_ShowScrollButtons;
        bool m_IsHorizontalMenu;
        int m_ContentWidth = 1;
        int m_ContentHeight = 1;

        int m_Columns;
        int m_Rows;

        EditorWindow m_ActiveToolWindow;

        EditorWindow window
        {
            get
            {
                if (!m_ActiveToolWindow)
                    m_ActiveToolWindow = EditorWindow.GetWindow<EditorToolWindow>();
                return m_ActiveToolWindow;
            }
        }

        int windowWidth
        {
            get { return (int) Mathf.Ceil(window.position.width); }
        }

        int windowHeight
        {
            get { return (int) Mathf.Ceil(window.position.height); }
        }

        void OnEnable()
        {
            m_Actions = EditorToolbarLoader.GetActions(true);
            m_ActionsLength = m_Actions.Count();

            ProBuilderEditor.selectionUpdated -= OnElementSelectionChange;
            ProBuilderEditor.selectionUpdated += OnElementSelectionChange;

            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            m_TooltipTimer.item1 = "";
            m_TooltipTimer.item2 = 0.0;
            m_ShowTooltipTimer = false;
            m_ScrollIconUp    = IconUtility.GetIcon("Toolbar/ShowNextPage_Up");
            m_ScrollIconDown  = IconUtility.GetIcon("Toolbar/ShowNextPage_Down");
            m_ScrollIconRight = IconUtility.GetIcon("Toolbar/ShowNextPage_Right");
            m_ScrollIconLeft  = IconUtility.GetIcon("Toolbar/ShowNextPage_Left");

            m_IsIconMode = ProBuilderEditor.s_IsIconGui;
            CalculateMaxIconSize();
        }

        void OnDisable()
        {
            // don't unsubscribe here because on exiting playmode OnEnable/OnDisable
            // is called.
            // EditorApplication.update -= Update;
            ProBuilderEditor.selectionUpdated -= OnElementSelectionChange;
        }

        void OnDestroy()
        {
            // store the scroll in both disable & destroy because there are
            // situations where one gets updated over the other and it's all
            // screwy.  script reloads in particular?
            MenuActionStyles.ResetStyles();
        }

        void OnElementSelectionChange(ProBuilderMesh[] selection)
        {
            Repaint();
        }

        void ShowTooltip(Rect rect, string content, Vector2 scrollOffset)
        {
            TooltipContent c = TooltipContent.TempContent;
            c.summary = content;
            ShowTooltip(rect, c, scrollOffset);
        }

        void ShowTooltip(Rect rect, TooltipContent content, Vector2 scrollOffset)
        {
            Rect buttonRect = new Rect(
                    (window.position.x + rect.x) - scrollOffset.x,
                    (window.position.y + rect.y) - scrollOffset.y,
                    rect.width,
                    rect.height);

            TooltipEditor.Show(buttonRect, content);
        }

        void Update()
        {
            if (!window)
                return;

            if (!s_ShiftOnlyTooltips)
            {
                if (!m_TooltipTimer.item1.Equals(m_HoveringTooltipName))
                {
                    m_TooltipTimer.item1 = m_HoveringTooltipName;
                    m_TooltipTimer.item2 = EditorApplication.timeSinceStartup;
                }

                if (!string.IsNullOrEmpty(m_TooltipTimer.item1))
                {
                    if (EditorApplication.timeSinceStartup - m_TooltipTimer.item2 > m_TooltipTimerRefresh)
                    {
                        if (!m_ShowTooltipTimer)
                        {
                            m_ShowTooltipTimer = true;
                            window.Repaint();
                        }
                    }
                    else
                    {
                        m_ShowTooltipTimer = false;
                    }
                }
            }

            // do scroll animations
            if (m_DoAnimateScroll)
            {
                double scrollTimer = EditorApplication.timeSinceStartup - m_ScrollStartTime;
                m_Scroll.value = Vector2.Lerp(m_ScrollOrigin, m_ScrollTarget, (float)scrollTimer / m_ScrollTotalTime);

                if (scrollTimer >= m_ScrollTotalTime)
                    m_DoAnimateScroll = false;

                window.Repaint();
            }
        }

        void CalculateMaxIconSize()
        {
            if (!window) return;

            m_IsHorizontalMenu = window.position.width > window.position.height;

            Vector2 iconSize = m_Actions[0].GetSize(m_IsHorizontalMenu);

            m_ContentWidth = (int)iconSize.x + 4;
            m_ContentHeight = (int)iconSize.y + 4;

            // if not in icon mode, we have to iterate all buttons to figure out what the maximum size is
            if (!m_IsIconMode)
            {
                for (int i = 1; i < m_Actions.Count; i++)
                {
                    iconSize = m_Actions[i].GetSize(m_IsHorizontalMenu);
                    m_ContentWidth = System.Math.Max(m_ContentWidth, (int)iconSize.x);
                    m_ContentHeight = System.Math.Max(m_ContentHeight, (int)iconSize.y);
                }

                m_ContentWidth += 4;
                m_ContentHeight += 4;
            }

            window.minSize = new Vector2(m_ContentWidth + 6, m_ContentHeight + 4);
            window.Repaint();
        }

        void StartScrollAnimation(float x, float y)
        {
            m_ScrollOrigin = m_Scroll;
            m_ScrollTarget.x = x;
            m_ScrollTarget.y = y;
            m_ScrollStartTime = EditorApplication.timeSinceStartup;
            m_ScrollTotalTime = Vector2.Distance(m_ScrollOrigin, m_ScrollTarget) / k_Scroll_Pixels_Per_Second;
            m_DoAnimateScroll = true;
        }

        bool IsActionValid(MenuAction action)
        {
            return !action.hidden && (!m_IsIconMode || action.icon != null);
        }

        public override void OnInspectorGUI()
        {
            Event e = Event.current;
            Vector2 mpos = e.mousePosition;
            bool forceRepaint = false;

            // if icon mode and no actions are found, that probably means icons failed to load. revert to text mode.
            int menuActionsCount = 0;

            for (int i = 0; i < m_Actions.Count; i++)
                if (IsActionValid(m_Actions[i]))
                    menuActionsCount++;

            if (m_IsIconMode && menuActionsCount < 1)
            {
                m_IsIconMode = false;
                ProBuilderEditor.s_IsIconGui.value = m_IsIconMode;
                CalculateMaxIconSize();
                Debug.LogWarning("ProBuilder: Toolbar icons failed to load, reverting to text mode.  Please ensure that the ProBuilder folder contents are unmodified.  If the menu is still not visible, try closing and re-opening the Editor Window.");
                return;
            }

            int availableWidth = windowWidth;
            int availableHeight = windowHeight;
            bool isHorizontal = windowWidth > windowHeight * 2;

            if (m_IsHorizontalMenu != isHorizontal || m_Rows < 1 || m_Columns < 1)
                CalculateMaxIconSize();

            if (e.type == EventType.Layout)
            {
                if (isHorizontal)
                {
                    m_Rows = ((windowHeight - 4) / m_ContentHeight);
                    m_Columns = System.Math.Max(windowWidth / m_ContentWidth, (menuActionsCount / m_Rows) + (menuActionsCount % m_Rows != 0 ? 1 : 0));
                }
                else
                {
                    m_Columns = System.Math.Max((windowWidth - 4) / m_ContentWidth, 1);
                    m_Rows = (menuActionsCount / m_Columns) + (menuActionsCount % m_Columns != 0 ? 1 : 0);
                }
            }

            // happens when maximizing/unmaximizing the window
            if (m_Rows < 1 || m_Columns < 1)
                return;

            int contentWidth = (menuActionsCount / m_Rows) * m_ContentWidth + 4;
            int contentHeight = m_Rows * m_ContentHeight + 4;

            bool showScrollButtons = isHorizontal ? contentWidth > availableWidth : contentHeight > availableHeight;

            if (showScrollButtons)
            {
                availableHeight -= k_ScrollButtonSize * 2;
                availableWidth -= k_ScrollButtonSize * 2;
            }

            if (isHorizontal && e.type == EventType.ScrollWheel && e.delta.sqrMagnitude > .001f)
            {
                m_Scroll.value = new Vector2(m_Scroll.value.x + e.delta.y * 10f, m_Scroll.value.y);
                forceRepaint = true;
            }

            // the math for matching layout group width for icons is easy enough, but text
            // is a lot more complex.  so for horizontal text toolbars always show the horizontal
            // scroll buttons.
            int maxHorizontalScroll = !m_IsIconMode ? 10000 : contentWidth - availableWidth;
            int maxVerticalScroll = contentHeight - availableHeight;

            // only change before a layout event
            if (m_ShowScrollButtons != showScrollButtons && e.type == EventType.Layout)
                m_ShowScrollButtons = showScrollButtons;

            if (m_ShowScrollButtons)
            {
                if (isHorizontal)
                {
                    GUILayout.BeginHorizontal();

                    GUI.enabled = ((Vector2)m_Scroll).x > 0;

                    if (GUILayout.Button(m_ScrollIconLeft, UI.EditorGUIUtility.ButtonNoBackgroundSmallMarginStyle, GUILayout.ExpandHeight(true)))
                        StartScrollAnimation(Mathf.Max(((Vector2)m_Scroll).x - availableWidth, 0f), 0f);

                    GUI.enabled = true;
                }
                else
                {
                    GUI.enabled = ((Vector2)m_Scroll).y > 0;

                    if (GUILayout.Button(m_ScrollIconUp, UI.EditorGUIUtility.ButtonNoBackgroundSmallMarginStyle))
                        StartScrollAnimation(0f, Mathf.Max(((Vector2)m_Scroll).y - availableHeight, 0f));

                    GUI.enabled = true;
                }
            }

            m_Scroll.value = GUILayout.BeginScrollView(m_Scroll.value, false, false, GUIStyle.none, GUIStyle.none, GUIStyle.none);

            bool    tooltipShown = false,
                    hovering = false;

            Rect optionRect = new Rect(0f, 0f, 0f, 0f);

            GUILayout.BeginHorizontal();

            // e.mousePosition != mpos at this point - @todo figure out why
            bool windowContainsMouse =  mpos.x > 0 && mpos.x < window.position.width &&
                mpos.y > 0 && mpos.y < window.position.height;

            int columnCount = 0;

            for (int actionIndex = 0; actionIndex < m_ActionsLength; actionIndex++)
            {
                MenuAction action = m_Actions[actionIndex];

                if (!IsActionValid(action))
                    continue;

                if (m_IsIconMode)
                {
                    if (action.DoButton(isHorizontal, e.alt, ref optionRect, GUILayout.MaxHeight(m_ContentHeight + 12)) && !e.shift)
                    {
                        // test for alt click / hover
                        optionRect.x -= m_Scroll.value.x;
                        optionRect.y -= m_Scroll.value.y;

                        if (windowContainsMouse &&
                            e.type != EventType.Layout &&
                            optionRect.Contains(e.mousePosition))
                        {
                            m_HoveringTooltipName = action.tooltip.title + "_alt";
                            m_TooltipTimerRefresh = .5f;
                            hovering = true;

                            if (m_ShowTooltipTimer)
                            {
                                tooltipShown = true;
                                ShowTooltip(optionRect, "Alt + Click for Options", m_Scroll);
                            }
                        }
                    }
                }
                else
                {
                    if (m_Columns < 2)
                        action.DoButton(isHorizontal, e.alt, ref optionRect);
                    else
                        action.DoButton(isHorizontal, e.alt, ref optionRect, GUILayout.MinWidth(m_ContentWidth));
                }

                Rect buttonRect = GUILayoutUtility.GetLastRect();

                if (windowContainsMouse &&
                    e.type != EventType.Layout &&
                    !hovering &&
                    buttonRect.Contains(e.mousePosition))
                {
                    m_HoveringTooltipName = action.tooltip.title;
                    m_TooltipTimerRefresh = 1f;

                    if (e.shift || m_ShowTooltipTimer)
                    {
                        tooltipShown = true;
                        ShowTooltip(buttonRect, action.tooltip, m_Scroll);
                    }

                    hovering = true;
                    forceRepaint = true;
                }

                if (++columnCount >= m_Columns)
                {
                    columnCount = 0;

                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            if (m_ShowScrollButtons)
            {
                if (isHorizontal)
                {
                    GUI.enabled = m_Scroll.value.x < maxHorizontalScroll - 2;
                    if (GUILayout.Button(m_ScrollIconRight, UI.EditorGUIUtility.ButtonNoBackgroundSmallMarginStyle, GUILayout.ExpandHeight(true)))
                        StartScrollAnimation(Mathf.Min(m_Scroll.value.x + availableWidth + 2, maxHorizontalScroll), 0f);
                    GUI.enabled = true;

                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUI.enabled = m_Scroll.value.y < maxVerticalScroll - 2;
                    if (GUILayout.Button(m_ScrollIconDown, UI.EditorGUIUtility.ButtonNoBackgroundSmallMarginStyle))
                        StartScrollAnimation(0f, Mathf.Min(m_Scroll.value.y + availableHeight + 2, maxVerticalScroll));
                    GUI.enabled = true;
                }
            }

            if ((e.type == EventType.Repaint || e.type == EventType.MouseMove) && !tooltipShown)
                TooltipEditor.Hide();

            if (e.type != EventType.Layout && !hovering)
                m_TooltipTimer.item1 = "";

            if (forceRepaint || (EditorWindow.mouseOverWindow == this && e.delta.sqrMagnitude > .001f) || e.isMouse)
                window.Repaint();
        }
    }
}
