using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public class FlowMapCreator : MonoBehaviour
{

    public Texture2D flowMapToLoad;

    [Space(10)]
    [Header("Flow Vector Settings")]
    [Space(10)]

    [Range(-0.5f, 0.5f)]
    public float targetVectorX;
    [Range(-0.5f, 0.5f)]
    public float targetVectorY;

    [Range(0f, 1.0f)]
    public float baseFlowFactor = 0.3f;

    public Texture2D UVLookUp;

    [Range(0.1f, 10f)]
    public float depthCheckLength = 2f;

    [Space(10)]
    [Header("Flow Vector Settings")]
    [Space(10)]

    [Range(0.1f, 2f)]
    public float brushRadius;
    [Range(0f, 100f)]
    public float brushStrength;

    [Space(10)]
    [Header("Flowmap Export Settings")]
    [Space(10)]

    public Vector2 flowMapSize = new Vector2(512, 512);

    [Range(0, 30)]
    public int blurRadius = 4;

    [Range(0, 10)]
    public int blurIterations = 2;

    [Space(10)]
    [Header("Collision Detection Settings")]
    [Space(10)]

    [Range(0.001f, 2f)]
    public float collisionRadius = 0.7f;

    public LayerMask objectCollisionMask;

    [Space(10)]
    [Header("Data Storage")]
    [Space(10)]

    public List<Vector3> vertPoints = new List<Vector3>();
    public List<Vector3> vertPointEnds = new List<Vector3>();
    public List<float> vectorMagnitude = new List<float>();
    public Color[] pointColors;

    public GameObject endPointObject;
    public List<GameObject> endPointObjects = new List<GameObject>();
    public List<Vector2> vertDirection = new List<Vector2>();

    public Transform planeEndPointsRoot;

    [HideInInspector]
    public Vector3 brushPos;
    [HideInInspector]
    public Vector3 brushNormal;

    [Space(10)]
    [Header("Debug Values")]
    [Space(10)]

    public bool debugging = false;
    public GameObject VertexFinder;
    public GameObject displayPlane;
    public Texture2D currentFlowMap;

    [HideInInspector]
    public Vector3 previousMousePos;

    void Start () {
        

        InitializeFlowMapCreator();


        for (int i = 0; i < endPointObjects.Count; i++)
        {
            endPointObjects[i].transform.position = vertPointEnds[i];
            GetPointColors();

            endPointObjects[i].transform.GetComponent<MeshRenderer>().material.color = pointColors[i];
        }


        //DEBUGGING
        //let you see your flow map on the screen
        //makes a routine show you order vertPoints are stored in
        if (debugging)
        {
            VertexFinder.SetActive(true);
            displayPlane.SetActive(true);
            displayPlane.GetComponent<MeshRenderer>().materials[0].mainTexture = UVLookUp;
            StartCoroutine(FindVerts());
        }
        else
        {
            displayPlane.SetActive(false);
            VertexFinder.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateEndpoints();
        for (int x = 0; x < vertPoints.Count; x++)
        {
            Debug.DrawLine(vertPoints[x], vertPointEnds[x]);
        }

    }


    void OnDrawGizmosSelected()
    {
        
    }

    void UpdateEndpoints()
    {
        for (int i = 0; i < endPointObjects.Count; i++)
        {
            vertPointEnds[i] = endPointObjects[i].transform.position;

            float pointRange = 0.6f;

            endPointObjects[i].transform.position = 
                new Vector3(Mathf.Clamp(endPointObjects[i].transform.position.x, vertPoints[i].x - pointRange, vertPoints[i].x + pointRange), 
                Mathf.Clamp(endPointObjects[i].transform.position.y, vertPoints[i].y, vertPoints[i].y), 
                Mathf.Clamp(endPointObjects[i].transform.position.z, vertPoints[i].z - pointRange, vertPoints[i].z + pointRange));

            GetPointColors();

            endPointObjects[i].transform.GetComponent<MeshRenderer>().material.color = pointColors[i];
        }
    }

    //converts vertex data to world space coordinates, gives default values for vertPoints,
    //and intitalizes all lists that are needed
    void InitializeFlowMapCreator()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertPoints.Add(transform.TransformPoint(vertices[i]));
        }

        ReorderVerts();

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vert = vertPoints[i];
            vertPointEnds.Add(new Vector3(vert.x, vert.y, vert.z));

            GameObject endPoint = Instantiate(endPointObject, vertPointEnds[i], Quaternion.identity, planeEndPointsRoot);

            endPointObjects.Add(endPoint);

            vectorMagnitude.Add(0.3f);
        }
        pointColors = new Color[vertPoints.Count];
    }

    //takes the position of the mesh vertexes and reorganizes to start from bottom left.
    //this is done to match texture coordinates which have 0,0 in the bottom left corner.
    void ReorderVerts()
    {
        Vector3[] tempArray = new Vector3[vertPoints.Count];

        for (int i = 0; i < vertPoints.Count; i++)
        {
            tempArray[i] = vertPoints[i];
        }

        Array.Sort(tempArray, Vector3CompareX);
        Array.Sort(tempArray, Vector3CompareZ);
        for (int i = 0; i < tempArray.Length; i++)
        {
            vertPoints[i] = tempArray[i];
        }
    }

    //for each line between points it compares the end point to a UV lookup
    //texture. it then constucts a flowmap with these look up colors.
    //next it upscales the image and blurs it before saving it to your project.
    public void GetPointColors()
    {
        for (int x = 0; x < vertPoints.Count; x++)
        {
            Vector3 relativeEndPoint = vertPointEnds[x] - vertPoints[x];

            Vector2 samplePoint = new Vector2((relativeEndPoint.x + 0.5f) * UVLookUp.width - 1, (relativeEndPoint.z + 0.5f) * UVLookUp.height - 1);

            pointColors[x] = UVLookUp.GetPixel(Mathf.FloorToInt(samplePoint.x), Mathf.FloorToInt(samplePoint.y));
        }
    }

    public void ExportFlowMap()
    {
        Texture2D tex = new Texture2D((int)Mathf.Sqrt(vertPoints.Count), (int)Mathf.Sqrt(vertPoints.Count));

        tex.SetPixels(pointColors);
        tex.Apply();

        tex = TextureScaler.scaled(tex, (int)flowMapSize.x, (int)flowMapSize.y);
        tex = Blur.FastBlur(tex, 4, 2);

        GetComponent<MeshRenderer>().material.mainTexture = tex;

        currentFlowMap = tex;

        if (debugging)
        {
            displayPlane.GetComponent<MeshRenderer>().materials[0].mainTexture = tex;
        }

        SaveTextureToAssets.Save(tex);
    }

    //for each stored vertex position it calulate and stores a magnitude 
    //value based on how much space is below the vertex position
    public void SetMagnitudeBasedOnDepth()
    {
        for (int x = 0; x < vertPoints.Count; x++)
        {
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(vertPoints[x], Vector3.down, out hit, depthCheckLength))
            {
                float distanceFromGround = Vector3.Distance(hit.point, vertPoints[x]);

                float vertPointsMagnitude = distanceFromGround / depthCheckLength;

                vectorMagnitude[x] = vertPointsMagnitude;

                //this is just to visualize the raycasts
                Debug.DrawRay(vertPoints[x], Vector3.down * distanceFromGround, Color.white, 2f);
            }
            else
            {
                vectorMagnitude[x] = 0.05f;
            }
        }
    }

    //for each stored vertex position if creates a sphere that looks for 
    //objects in the objectCollisionMask. if it hits something it sets the
    //flow magnitude to 0
    public void DetectCollisionPerVertex()
    {
        for (int x = 0; x < vertPoints.Count; x++)
        {
            Collider[] hitColliders = Physics.OverlapSphere(vertPoints[x], collisionRadius, objectCollisionMask);
            if (hitColliders.Length != 0)
            {
                vectorMagnitude[x] = 0.05f;
            }
        }
    }

    //moves a box to each vert point in sequence
    //only for debugging
    IEnumerator FindVerts()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            yield return new WaitForSeconds(0.5f);

            VertexFinder.transform.position = vertPoints[i];
        }
    }

    //organizes point array from lowest x value to highest when called with array.sort
    private int Vector3CompareX(Vector3 value1, Vector3 value2)
    {
        if (value1.x < value2.x)
        {
            return -1;
        }
        else if (value1.x == value2.x)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
    //organizes point array from lowest y value to highest when called with array.sort
    private int Vector3CompareZ(Vector3 value1, Vector3 value2)
    {
        if (value1.z < value2.z)
        {
            return -1;
        }
        else if (value1.z == value2.z)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}
