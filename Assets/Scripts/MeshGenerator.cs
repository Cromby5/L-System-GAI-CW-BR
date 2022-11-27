using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    
    public int _basePolygon = 20;
    
    [SerializeField] private GameObject cube;
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
        

        for (int j = 0; j < _basePolygon; j++)
        {
            Vector3 pos = new Vector3(Mathf.Cos(j * angularStep), 0f, Mathf.Sin(j * angularStep)) + position;
            pos *= 0.6f;
            GameObject cubes = Instantiate(cube, pos, Quaternion.identity, transform);
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
