using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CodeCreatePlay.LocationTool;
// using CodeCreatePlay.NavMeshTools;


[System.Serializable]
public class LocationTool : EditorWindow
{
    [System.Serializable]
    public struct NavMeshToolsSettings
    {
        public bool navAreasToolFoldout;
    }
     
    Location activeLoc = null;

    bool mDown = false; // is any mouse button down
    bool keyUp = false; // is any keyboard key down
    bool btn0 = false;  // is mouse btn 0 down
    bool ctrl = false;  // is control pressed
    bool shift = false; // is shift pressed

    Vector3 lastPos = default; // last position of current selected location boundary or destination point 

    static readonly int right_indent = 5;
    static bool init = false;
    static LocationTool instance = null;
    int currentTabIndex = 0;
    public bool repaintEvent = false;  // set this true after Repaint event to avoid repeated Repaint calls 

    public NavMeshToolsSettings navMeshToolsSettings = new ();
    // [SerializeField] public NavMeshAreaTool navMeshAreaTool = new ();


    [MenuItem("CodeCreatePlay/LocationTool/LocationTool")]
    public static void ShowWindow()
    {
        _ = GetWindow(typeof(LocationTool));
    }

    [MenuItem("CodeCreatePlay/LocationTool/CreateLocation")]
    public static void CreateLocation()
    {
        GameObject gameObject = new ("New Location");
        gameObject.AddComponent<Location>();
        if (instance != null)
            instance.Repaint();
    }

    [MenuItem("CodeCreatePlay/LocationTool/ConvertToLocation")]
    public static void ConvertObjectToLocation()
    {
        if(Selection.activeGameObject != null && !Selection.activeGameObject.GetComponent<Location>())
        {
            Selection.activeGameObject.AddComponent<Location>();
            if(instance != null)
                instance.Repaint();
        }
    }

    [MenuItem("CodeCreatePlay/LocationTool/Add LT_Globals")]
    public static void Add_LT_Globals()
    {
        GameObject lt_Globals = null;

        try
        {
            lt_Globals = GameObject.FindGameObjectWithTag("LT_Globals");
        }
        catch (UnityException)
        {
            Debug.Log("[LocationTool] Tag LT_Globals not defined");
            return;
        }

        if (lt_Globals)
        {
            if(!lt_Globals.GetComponent<LT_Globals>())
                lt_Globals.AddComponent<LT_Globals>();
        }
        else
        {
            GameObject gameObject = new("LT_Globals");
            gameObject.AddComponent<LT_Globals>();
            gameObject.tag = "LT_Globals";
        }

        if (instance != null)
            instance.Repaint();
    }

    public void OnEnable()
    {
        instance = this;
        SceneView.duringSceneGui += this.OnSceneGUI;
        init = true;

        if (activeLoc != null)
        {
            activeLoc.editMode = false;
            activeLoc.editModeBtnTxt = "BeginEdit";
            activeLoc.selectedPoint = null;
        }
    }
      
    public void OnDisable()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    public void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    void OnGUI()
    {
        if (init == false)
            OnEnable();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        GUILayout.Label("LocationTool");
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        currentTabIndex = GUILayout.SelectionGrid(currentTabIndex, new string[]
        { "Location", "Scatter" }, 2, EditorStyles.toolbarButton);

        if (currentTabIndex == 0)
            DrawLocationInspector();

        else if (currentTabIndex == 1)
            DrawScatterInspector();

        else if (currentTabIndex == 2)
            DrawScatterInspector();

        GUILayout.Space(10F);
        if (currentTabIndex == 2)
            EditorGUILayout.HelpBox("Scatter tool is not yet implemented.", MessageType.Info);
        EditorGUILayout.HelpBox("Undo redo system is not yet implemented.", MessageType.Info);
    }

    #region LocationTool Inspector

    void DrawLocationInspector()
    {
        GUILayout.Space(10);
        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Location>())
        {
            activeLoc = Selection.activeGameObject.GetComponent<Location>();

            // Location inspector
            activeLoc.AutoEditor.Build(hOffset: 15);

            // ======================================================================================================================= //
            // Destinations inspector
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(right_indent);

            activeLoc.destinationsFoldout = EditorGUILayout.Foldout(activeLoc.destinationsFoldout, "Destinations");
            GUILayout.EndHorizontal();

            Color tempColor;

            if (activeLoc.destinationsFoldout)
            {
                for (int i = 0; i < activeLoc.locationBase.destinations.Count; i++)
                {
                    // ---------------------------------------------------------------------------- //
                    GUILayout.BeginHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(15f);

                    // name of destination
                    GUILayout.Label(i.ToString(), GUILayout.MaxWidth(20));

                    // 
                    activeLoc.locationBase.destinations[i].destName =
                        EditorGUILayout.TextField(activeLoc.locationBase.destinations[i].destName);

                    GUILayout.EndHorizontal();
                    // ---------------------------------------------------------------------------- //

                    // select button
                    tempColor = GUI.backgroundColor;

                    if (activeLoc.locationBase.destinations[i] == activeLoc.selectedPoint)
                    { GUI.backgroundColor = Color.green; }
                    else
                    { GUI.backgroundColor = new Color(0.85f, 0.85f, 0.85f, 1f); }

                    if (GUILayout.Button("Select"))
                    {
                        activeLoc.selectedPoint = activeLoc.locationBase.destinations[i];
                        SceneView.RepaintAll();
                    }

                    // revert to old colour.
                    GUI.backgroundColor = tempColor;

                    // remove button
                    tempColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;

                    if (GUILayout.Button("-"))
                    { activeLoc.locationBase.RemoveDestination(i); }

                    // revert to old colour.
                    GUI.backgroundColor = tempColor;

                    GUILayout.EndHorizontal();
                }
            }

            tempColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.85f, 0.85f, 0.85f, 1f);

            // ======================================================================================================================= //
            // buttons
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);

            if (GUILayout.Button("Clear Boundaries   "))
            { activeLoc.locationBase.ClearBoundaries(); SceneView.RepaintAll(); activeLoc.selectedPoint = null; }

            if (GUILayout.Button("Clear Destinations"))
            { activeLoc.locationBase.ClearDestinations(); SceneView.RepaintAll(); activeLoc.selectedPoint = null; }

            GUILayout.EndHorizontal();

            
            // undo redo buttons
            using(new GUILayout.HorizontalScope())
            {
                GUILayout.Space(15);

                if (GUILayout.Button("Undo")) { }
                if (GUILayout.Button("Redo")) { }
            }

            // button SwitchEditMode
            using(new GUILayout.HorizontalScope())
            {
                GUILayout.Space(15);
                if (GUILayout.Button(activeLoc.editModeBtnTxt))
                    SwitchEditMode();
            }

            GUI.backgroundColor = tempColor;

            /*
            // other buttons
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(15);
                if (GUILayout.Button("SnapToGround"))
                    activeLoc.locationBase.SnapToGround();
            }
            */

            // init tools
            using(new GUILayout.HorizontalScope())
            {
                GUILayout.Space(15);
                if (GUILayout.Button("Re_Initialize"))
                    OnEnable();
            }
        }
    }

    public void SwitchEditMode()
    {
        activeLoc.editMode = !activeLoc.editMode;

        if (activeLoc.editMode)
            activeLoc.editModeBtnTxt = "EndEdit";

        else if (!activeLoc.editMode)
        {
            activeLoc.editModeBtnTxt = "BeginEdit";
            activeLoc.selectedPoint = null;
        }
    }

    #endregion

    #region NavMeshTools inspector

    void DrawNavToolsInspector()
    {
        /*
        GUILayout.Space(10);

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Space(right_indent);

        navMeshToolsSettings.navAreasToolFoldout = EditorGUILayout.Foldout(navMeshToolsSettings.navAreasToolFoldout, "NavigationAreas");

        GUILayout.EndHorizontal();

        if (navMeshToolsSettings.navAreasToolFoldout)
        {
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            navMeshAreaTool.SettingsAutoEd.Build();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            if (GUILayout.Button("Create Areas"))
            {
                navMeshAreaTool.CreateNavMeshAreas();
                SceneView.RepaintAll(); 
            }

            if (GUILayout.Button("Clear"))
            {
                navMeshAreaTool.Clear();
                SceneView.RepaintAll();
            }
            GUILayout.EndHorizontal();
        }
        */
    }

    #endregion

    #region ScatterTools Inspector

    void DrawScatterInspector()
    {
        //GUILayout.Space(10F);
        //EditorGUILayout.HelpBox("Scatter tool is not yet implemented", MessageType.Info);
    }

    #endregion

    public void OnSceneGUI(SceneView _sv)
    {
        if (activeLoc != null)
        {
            // --------------------------------------------------------------------------------- #
            if (activeLoc.transform.position != activeLoc.lastPos ||
                activeLoc.transform.localScale != activeLoc.lastScale ||
                activeLoc.transform.rotation != activeLoc.lastRot)
                activeLoc.UpdatePositions();

            activeLoc.lastPos = activeLoc.transform.position;
            activeLoc.lastScale = activeLoc.transform.localScale;
            activeLoc.lastRot = activeLoc.transform.rotation;
            // --------------------------------------------------------------------------------- #

            // sv = _sv;

            if (activeLoc.editMode)
            {
                HandleInput();
                UpdateGizmos();
            }

            DrawHandles();

            if (activeLoc.editMode)
            {
                Selection.activeGameObject = activeLoc.gameObject;
                HandleUtility.Repaint();
            }
            else if(Selection.activeGameObject != null && activeLoc != null)
            {
                // HandleUtility.Repaint();
            }
        }
         
        /*
        // --------------------------------------------------------------------------------- #
        // debug for NavMeshAreasTool
        if (navMeshAreaTool.settings.debug)
            navMeshAreaTool.DebugAreas();
        // --------------------------------------------------------------------------------- #
        */

        Repaint();
    }

    void HandleInput()
    {
        Event currentEvt = Event.current;

        mDown = currentEvt.type == EventType.MouseDown;
        keyUp = currentEvt.type == EventType.KeyUp;

        btn0 = currentEvt.button == 0;

        ctrl = currentEvt.modifiers == EventModifiers.Control;
        shift = currentEvt.modifiers == EventModifiers.Shift;

        EditLocation();
    }

    void EditLocation()
    {
        Vector3 mousePos = GetMousePosition();

        if (ctrl && mDown && btn0 && !keyUp)
        {
            activeLoc.selectedPoint = activeLoc.locationBase.AddBoundaryPoint(mousePos);
            activeLoc.selectedPoint.localPosition = activeLoc.transform.InverseTransformPoint(activeLoc.selectedPoint.position);
        }

        if (shift && mDown && btn0 && !keyUp)
        {
            activeLoc.selectedPoint = activeLoc.locationBase.AddDestinationPoint(mousePos);
            activeLoc.selectedPoint.localPosition = activeLoc.transform.InverseTransformPoint(activeLoc.selectedPoint.position);
        }
    }

    void UpdateGizmos()
    {
        if (activeLoc.selectedPoint != null && activeLoc.locationBase.drawHandles)
        {
            lastPos = activeLoc.selectedPoint.position;

            activeLoc.selectedPoint.position = Handles.PositionHandle(activeLoc.selectedPoint.position, Quaternion.identity);

            if (activeLoc.selectedPoint.position != lastPos)
                activeLoc.selectedPoint.localPosition = activeLoc.transform.InverseTransformPoint(activeLoc.selectedPoint.position);
        }
    }

    void DrawHandles()
    {
        int index = 0;

        // ============================================================================================================== //
        // draw location boundaries
        List<Point> points = activeLoc.locationBase.boundaries;
        if(activeLoc.locationBase.drawBoundaries)
        {
            foreach (var item in points)
            {
                activeLoc.locationBase.markerRadius = GetPointSize(item);

                TrySelectPoint(item, activeLoc.locationBase.boundaryColor);

                Handles.DrawSolidArc(points[index].position, Vector3.up, Vector3.right, 360,
                    activeLoc.locationBase.markerRadius);

                index++;

                // join lines
                Point nextDest = new ();
                if (index > points.Count - 1)
                {
                    nextDest = points[0];
                    index = 0;
                }
                else
                {
                    nextDest = points[index];
                }

                Handles.color = Color.gray;
                Handles.DrawLine(item.position, nextDest.position);
            }
        }

          
        // ============================================================================================================== //
        // now for destinations
        if(activeLoc.locationBase.drawDestinations)
        {
            foreach (var item in activeLoc.locationBase.destinations)
            {
                activeLoc.locationBase.markerRadius = GetPointSize(item);

                TrySelectPoint(item, activeLoc.locationBase.destinationColor);

                Handles.DrawSolidArc(item.position, Vector3.up, Vector3.right, 360,
                    activeLoc.locationBase.markerRadius);
            }
        }
    }
     
    void TrySelectPoint(Point item, Color itemColor)
    {
        var pointUnderMouse = PointUnderMouse(item);
        if (pointUnderMouse)
        {
            Handles.color = Color.yellow;
            if (mDown)
            {
                activeLoc.selectedPoint = item;
            }
        }
        else if (activeLoc.selectedPoint == item)
            Handles.color = Color.yellow;
        else
            Handles.color = itemColor;
    }

    bool PointUnderMouse(Point point)
    {
        return Vector3.Distance(GetMousePosition(), point.position) < GetPointSize(point);
    }

    float GetPointSize(Point point)
    {
        return Vector3.Distance(Camera.current.transform.position, point.position) / 60f;
    }

    public static Vector3 GetMousePosition()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        Vector3 mPos;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mPos = hit.point;
        }
        else
        {
            float drawPlaneHeight = 0;
            float dstToDrawPlane = (drawPlaneHeight - ray.origin.y) / ray.direction.y;
            mPos = ray.GetPoint(dstToDrawPlane);
        }

        mPos.y += Location.POINT_DISTANCE_FROM_GROUND;
        return mPos;
    }
}
