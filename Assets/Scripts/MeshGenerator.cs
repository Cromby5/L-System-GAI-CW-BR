using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    
    public int _basePolygon = 20;
    public float _radius = 0.001f;
    public float _height = 1f;

    [SerializeField] private GameObject cube;
    [SerializeField] private Transform BranchHolder;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        UpdateMesh();
    }
    
    public void CreateShape(Vector3 startPosition, Vector3 position, Quaternion rotation,float width)
    {
        Vector3[] startVertices = new Vector3[_basePolygon];
        Vector2[] startUv = new Vector2[_basePolygon];
        
        float angularStep = 2f * Mathf.PI / (float)(_basePolygon);
        float angle = angularStep * _radius;
        
        for (int j = 0; j < _basePolygon; j++)
        {
            angle += angularStep;

            Vector3 pos = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Vector3 finalpos = position + pos * _radius;
            // Debug to visualise points that the vertices would be at
            GameObject cubes = Instantiate(cube, finalpos, rotation, BranchHolder);
            
            startVertices[j] = pos;
            startUv[j] = new Vector2(j*angularStep, startVertices[j].y);
        }

    }
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }


}
