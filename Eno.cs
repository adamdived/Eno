//-------------------------------
//            Eno
// Released under GNU GPL v3.0
// Developed by Linkage Games
// Developed for u/miraoister
// 
// Fixed by Marco Capelli
// for Unity 2022+ and Unity 6
//-------------------------------


using UnityEngine;
using System.Collections;
using UnityEditor;

public class Eno : EditorWindow
{
    public float maxDistance = 1000f;
    private enum measurementStates { first, second, showing, none };
    private measurementStates measurementState = measurementStates.none;
    private Vector3 first = Vector3.zero;

    private Vector3 second = Vector3.zero;


    [MenuItem("Window/Eno Measurement Tool")]
    static void Init()
    {
        Eno window = (Eno)EditorWindow.GetWindow(typeof(Eno));
        window.minSize = new Vector2(220f, 50f);
        window.maxSize = new Vector2(220f, 60f);
        window.position = new Rect(window.position.x, window.position.y, 215f, 22f);
        window.titleContent = new GUIContent("Eno Measurement Tool");
    }
    bool test;
    void OnGUI()
    {

        GUILayout.BeginHorizontal();

        if (measurementState.Equals(measurementStates.none))
        {
            if (GUILayout.Button("Measure"))
            {
                measurementState = measurementStates.first;
            }
        }
        else if (measurementState.Equals(measurementStates.first))
        {
            GUILayout.Label("Pick first point");
        }
        else if (measurementState.Equals(measurementStates.second))
        {
            GUILayout.Label("Pick last point");
        }
        else if (measurementState.Equals(measurementStates.showing))
        {
            GUILayout.Label(Vector3.Distance(first, second).ToString());
        }
        if (GUILayout.Button("Restart"))
        {
            Restart();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        maxDistance = EditorGUILayout.FloatField("Pick Max Distance", maxDistance);

        GUILayout.EndHorizontal();

    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    public void OnSceneGUI(SceneView sceneView)
    {


        if (measurementState.Equals(measurementStates.first) && DropPoint(out first))
        {

            measurementState = measurementStates.second;

        }
        else if (measurementState.Equals(measurementStates.first))
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else if (measurementState.Equals(measurementStates.second) && DropPoint(out second))
        {
            measurementState = measurementStates.showing;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else if (measurementState.Equals(measurementStates.second))
        {
            CheckCancel();
            Handles.SphereHandleCap(42, first, Quaternion.identity, 0.05f, EventType.Repaint);
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit, 100f))
            {
                Handles.color = Color.yellow;
                Handles.DrawLine(first, hit.point, 6f);
                Handles.SphereHandleCap(42, hit.point, Quaternion.identity, 0.1f, EventType.Repaint);
            }
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else if (measurementState.Equals(measurementStates.showing))
        {
            CheckCancel();
            HandleUtility.Repaint();
            Handles.color = Color.yellow;
            Handles.DrawLine(first, second, 6f);
            Handles.SphereHandleCap(42, first, Quaternion.identity, 0.1f, EventType.Repaint);
            Handles.SphereHandleCap(42, second, Quaternion.identity, 0.1f, EventType.Repaint);
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.white;
            Handles.Label(second + Vector3.up * 0.5f + Vector3.right * 0.5f, Vector3.Distance(first, second).ToString("F3") + (" meters"), style);

            if (Event.current != null && (Event.current.button == 0) && Event.current.type.Equals(EventType.MouseDown))
            {
                Restart();
            }
            this.Repaint();
        }
        else
        {
            HandleUtility.Repaint();
        }
        SceneView.RepaintAll();
    }


    private bool DropPoint(out Vector3 point)
    {
        point = Vector3.zero;
        Event e = Event.current;

        if (e.button == 0 && e.type.Equals(EventType.MouseDown))
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit, maxDistance))
            {
                point = hit.point;
                return true;
            }

        }
        return false;
    }

    private void Restart()
    {

        measurementState = measurementStates.none;
        first = Vector3.zero;
        second = Vector3.zero;
    }

    private void CheckCancel()
    {
        Event e = Event.current;
        if (e.keyCode == KeyCode.Escape && e.type.Equals(EventType.KeyDown))
        {
            Restart();
            this.Repaint();
        }
    }
}
