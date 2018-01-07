using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FlowMapCreator))]
class FlowMapCreatorEditor : Editor
{
    FlowMapCreator selectedMesh;

    Vector3 mouseDir;

    void OnEnable()
    {

    }

    void OnSceneGUI()
    {
        selectedMesh = target as FlowMapCreator;



        if (Camera.current != null && Event.current.mousePosition != null)
        {
            RaycastHit hit;
            //if (Physics.Raycast(Camera.current.ScreenPointToRay(new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y)), out hit, 1000f))
            //{
            //    Debug.Log(hit.point);
            //    Debug.DrawRay(new Vector3(hit.point.x, hit.point.y, -hit.point.z), hit.normal);
            //    //Gizmos.DrawWireSphere(new Vector3(hit.point.x, selectedMesh.transform.position.y, -hit.point.z), 1f);
            //    selectedMesh.spherePos = new Vector3(hit.point.x, selectedMesh.transform.position.y, -hit.point.z);

            //    //selectedMesh.endPointObject.transform.position = new Vector3(hit.point.x, selectedMesh.transform.position.y, -hit.point.z);
            //}

            if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y)), out hit, 1000f))
            {
                selectedMesh.brushPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                selectedMesh.brushNormal = hit.normal;
            }
        }


        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        //Event.current.Use();

        Handles.CircleHandleCap(0, selectedMesh.brushPos, Quaternion.LookRotation(selectedMesh.brushNormal),
            selectedMesh.brushRadius, EventType.Repaint);
        Handles.DrawLine(selectedMesh.brushPos, (selectedMesh.brushPos + selectedMesh.brushNormal / 2f));


        if (Event.current.capsLock == true) //Event.current.button == 0)
        {
            Collider[] hitColliders = Physics.OverlapSphere(selectedMesh.brushPos, selectedMesh.brushRadius);
            foreach (Collider col in hitColliders)
            {
                if (col.transform.tag == "VectorBall")
                {
                    col.transform.position += mouseDir;
                }

            }
        }

    }

    public IEnumerator sampleMousePos()
    {
        selectedMesh.previousMousePos = Event.current.mousePosition;
        yield return new WaitForSeconds(1f);
        mouseDir = (selectedMesh.previousMousePos - new Vector3(Event.current.mousePosition.x, Event.current.mousePosition.y, 0)).normalized;
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
