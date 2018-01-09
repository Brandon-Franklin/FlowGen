using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FlowMapCreator))]
class FlowMapCreatorEditor : Editor
{
    FlowMapCreator selectedMesh;

    Vector3 brushPos, brushNormal;

    Vector3 lastPos, curPos, dirVec;

    Collider[] hitColliders;

    List<int> curIndexes = new List<int>();
    void OnEnable()
    {
        selectedMesh = target as FlowMapCreator;
    }

    void OnSceneGUI()
    {
        SceneView.RepaintAll();

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        

        //Event.current.Use();

        Handles.CircleHandleCap(0, brushPos, Quaternion.LookRotation(brushNormal),
            selectedMesh.brushRadius, EventType.Repaint);
        Handles.DrawLine(brushPos, (brushPos + brushNormal / 2f));

        Event e = Event.current;

        if (Camera.current != null && e.mousePosition != null)
        {
            RaycastHit hit;

            if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y)), out hit, 1000f))
            {
                brushPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                brushNormal = hit.normal;
            }
        }
        if (e.type == EventType.ScrollWheel)
        {
            if (e.control == true)
            {
                Event.current.Use();
            }
        }

        if (e.type == EventType.keyDown)
        {
            if (Event.current.keyCode == (KeyCode.LeftControl))
            {
                if (e.type == EventType.ScrollWheel)
                {
                    selectedMesh.brushRadius += e.delta.y / 100f; Event.current.Use();

                }
            }
        }

        if (e.type == EventType.MouseDown)
        {
            lastPos = Camera.current.ScreenToWorldPoint(new Vector3(e.mousePosition.x, e.mousePosition.y, Camera.current.nearClipPlane));
            //Debug.Log(lastPos);

            hitColliders = Physics.OverlapSphere(brushPos, selectedMesh.brushRadius);
            Event.current.Use();
        }

        if (Event.current.type == EventType.MouseDrag)
        {
            curPos = Camera.current.ScreenToWorldPoint(new Vector3(e.mousePosition.x, e.mousePosition.y, Camera.current.nearClipPlane));

            dirVec = (lastPos - curPos).normalized / 15f * (selectedMesh.brushStrength / 100);
            Debug.Log(dirVec);

            foreach (Collider col in hitColliders)
            {
                if (col.transform.tag == "VectorBall")
                {
                    curIndexes.Add(
                        col.transform.GetComponent<EndPointBall>().index);

                    col.transform.position -= new Vector3(dirVec.x, 0f, -dirVec.z);
                }

            }

            selectedMesh.UpdateEndpoints(curIndexes);
            curIndexes = new List<int>();
        }
 
        //if (Event.current.type == EventType.MouseUp)
        //{
        //    curPos = Camera.current.ScreenToWorldPoint(new Vector3(e.mousePosition.x, e.mousePosition.y, Camera.current.nearClipPlane));
        //    //Debug.Log(curPos);

        //    dirVec = (lastPos - curPos).normalized;
        //    Debug.Log(dirVec);

        //    foreach (Collider col in hitColliders)
        //    {
        //        if (col.transform.tag == "VectorBall")
        //        {
        //            curIndexes.Add(
        //                col.transform.GetComponent<EndPointBall>().index);

        //            col.transform.position -= new Vector3(dirVec.x, dirVec.y, 0f);
        //        }

        //    }

        //    selectedMesh.UpdateEndpoints(curIndexes);
        //    curIndexes = new List<int>();
        //}

    }


    public override void OnInspectorGUI()
    {



        //shows public varaibles for editor
        DrawDefaultInspector();

        //function calling buttons for inspector
        EditorGUILayout.Space();
        if (GUILayout.Button("Match Target Direction"))
        {
            CustomAngle();
        }
        if (GUILayout.Button("Detect Depth"))
        {
            selectedMesh.SetMagnitudeBasedOnDepth();
            CustomAngle();
        }
        if (GUILayout.Button("Detect Water Collision"))
        {
            selectedMesh.DetectCollisionPerVertex();
            CustomAngle();
        }
        if (GUILayout.Button("Reset Magnitude to Base Flow"))
        {
            ResetMagToBaseFlowFactor();
        }
        if (GUILayout.Button("Reset"))
        {
            selectedMesh.IntitializeFlow();
        }
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Flow Up"))
        {
            Up();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Flow Left"))
        {
            Left();
        }
        if (GUILayout.Button("Flow Right"))
        {
            Right();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Flow Down"))
        {
            Down();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("Make Flow Map"))
        {
            RenderFlow();
        }

    }
    
    //calls the flowmap rendering function
    void RenderFlow()
    {
        selectedMesh.GetPointColors();
        selectedMesh.ExportFlowMap();
    }


    //lets you map the flow angle to your custom vector
    void CustomAngle()
    {
        for (int x = 0; x < selectedMesh.vertPoints.Count; x++)
        {
            selectedMesh.endPointObjects[x].transform.position = new Vector3(selectedMesh.vertPoints[x].x + selectedMesh.targetVectorX * 2 * selectedMesh.vectorMagnitude[x], selectedMesh.vertPoints[x].y, selectedMesh.vertPoints[x].z + selectedMesh.targetVectorY * 2 * selectedMesh.vectorMagnitude[x]);
        }
    }


    //removes depth flow magnitude and switches it for the uniform base flow factor
    void ResetMagToBaseFlowFactor()
    {
        for (int m = 0; m < selectedMesh.vectorMagnitude.Count; m++)
        {
            selectedMesh.vectorMagnitude[m] = selectedMesh.baseFlowFactor;
        }
    }


    //points vectors up
    void Up()
    {
        for (int x = 0; x < selectedMesh.vertPoints.Count; x++)
        {
            selectedMesh.endPointObjects[x].transform.position = new Vector3(selectedMesh.vertPoints[x].x, selectedMesh.vertPoints[x].y, selectedMesh.vertPoints[x].z + selectedMesh.baseFlowFactor * 2 * selectedMesh.vectorMagnitude[x]);
        }
    }

    //points vectors down
    void Down()
    {
        for (int x = 0; x < selectedMesh.vertPoints.Count; x++)
        {
            selectedMesh.endPointObjects[x].transform.position = new Vector3(selectedMesh.vertPoints[x].x, selectedMesh.vertPoints[x].y, selectedMesh.vertPoints[x].z - selectedMesh.baseFlowFactor * 2 * selectedMesh.vectorMagnitude[x]);
        }
    }

    //points vectors left
    void Left()
    {
        for (int x = 0; x < selectedMesh.vertPoints.Count; x++)
        {
            selectedMesh.endPointObjects[x].transform.position = new Vector3(selectedMesh.vertPoints[x].x - selectedMesh.baseFlowFactor * 2 * selectedMesh.vectorMagnitude[x], selectedMesh.vertPoints[x].y, selectedMesh.vertPoints[x].z);
        }
    }
    //points vectors right
    void Right()
    {
        for (int x = 0; x < selectedMesh.vertPoints.Count; x++)
        {
            selectedMesh.endPointObjects[x].transform.position = new Vector3(selectedMesh.vertPoints[x].x + selectedMesh.baseFlowFactor * 2 * selectedMesh.vectorMagnitude[x], selectedMesh.vertPoints[x].y, selectedMesh.vertPoints[x].z);
        }
    }
}
