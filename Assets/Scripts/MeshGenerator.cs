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
    public float _radius = 0.1f;
    public float _height = 1f;

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
        float angle = angularStep * _radius;
        
        for (int j = 0; j < _basePolygon; j++)
        {
            angle += angularStep;

            Vector3 pos = new Vector3(Mathf.Cos(angle), position.y, Mathf.Sin(angle));
            Debug.Log(pos);
            GameObject cubes = Instantiate(cube, _radius * pos, rotation, transform);
            
            startVertices[j] = pos;
            startUv[j] = new Vector2(j*angularStep, startVertices[j].y);
        }
        //UpdateMesh();
    }
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }


}
