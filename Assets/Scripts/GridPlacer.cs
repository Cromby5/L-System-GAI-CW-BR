using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlacer : MonoBehaviour
{
    [SerializeField] private float x , z;
    [SerializeField] private int xLength, zLength;
    [SerializeField] private GameObject lSystemPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < xLength; i++)
        {
            for (int j = 0; j < zLength; j++)
            {
                Instantiate(lSystemPrefab, new Vector3(transform.position.x + i * x, 0, transform.position.z + j * z), Quaternion.identity);
            }
        }

    }
}
