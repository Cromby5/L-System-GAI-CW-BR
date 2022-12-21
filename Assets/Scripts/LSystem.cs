using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class LSystem: MonoBehaviour
{
    [Header("Rules")]
    // axiom
    public string axiom = "F";
    public List<Rules> rules = new List<Rules>();
    // current rule
    [SerializeField] private int currentRule = 0;
    // number of iterations
    [SerializeField] private int iterations = 3;
    // angle
    [SerializeField] private float angle = 25f;
    // length
    [SerializeField] private float length = 1f;
    // width
    [SerializeField] private float width = 1f;
    // start position
    private Vector3 startPosition = Vector3.zero;
    // start rotation
    private Quaternion startRotation = Quaternion.identity;
    // stack for position and rotation when branches need to be made
    Stack<TransformStore> positionStack = new Stack<TransformStore>();

    // Separate for the mesh combination to allow for different textures, ideally uvs could be done for this instead
    [SerializeField] private Transform BranchHolder;
    [SerializeField] private Transform LeafHolder;

    [Header("Objects")]
    [SerializeField] private Material branchMat; // Material for the Branch
    [SerializeField] private Material leafMat; // Material for the Leaf
    // 3D objects (no custom mesh)
    [SerializeField] private GameObject leaves; // Leaf Object
    [SerializeField] private GameObject cylinder; // Trunk/Branch Object
    [SerializeField] private GameObject temp;

    [Header("Sliders")]
    // Sliders
    [SerializeField] private Slider iterationSlider;
    [SerializeField] private Slider angleSlider;
    [SerializeField] private Slider ruleSlider;
    [SerializeField] private Slider lengthSlider;
    [SerializeField] private Slider widthSlider;
    [SerializeField] private TMP_Dropdown selectionDropdown;

    [SerializeField] private TextMeshProUGUI iterationText;
    [SerializeField] private TextMeshProUGUI angleText;
    [SerializeField] private TextMeshProUGUI ruleText;
    [SerializeField] private TextMeshProUGUI lengthText;
    [SerializeField] private TextMeshProUGUI widthText;

    MeshGenerator meshGenerator;
    public Select mySelection;
    public enum Select
    {
        Line,
        Cylinders,
        Mesh,
    }

    // Start is called before the first frame update
    void Start()
    {
        meshGenerator = GetComponent<MeshGenerator>();
        TreeGen();
        // Sliders
        StartSliders();
    }
    public void TreeGen()
    {
        // destroy old tree, cleanup
        foreach (Transform transform in BranchHolder)
        {
            Destroy(transform.gameObject);
        }
        foreach (Transform transform in LeafHolder)
        {
            Destroy(transform.gameObject);
        }
        if (temp != null)
        {
            Destroy(temp);
        }
        startPosition = transform.position;
        // generate the string
        string generatedString = GenerateString(axiom, rules[currentRule].result, iterations);
        // draw the string
        DrawString(generatedString);
    }

    // generate the string
    string GenerateString(string axiom, string rule, int iterations)
    {
        string currentString = axiom;
        string nextString = "";

        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < currentString.Length; j++)
            {
                // Probablily of rules taken in to account here
                // This decides what rule to use randomly based on the probability.
                float r = Random.Range(0f, 1.0f);
                float sum = 0.0f;
                for (int k = 0; k < rules.Count; k++)
                {
                    sum += rules[k].probability;
                    if (r < sum)
                    {
                        rule = rules[k].result;
                        break;
                    }
                }
                if (currentString[j] == 'F')
                {
                    // add the rule to the next string
                    nextString += rule;
                }
                else
                {
                    // add the current character to the next string
                    nextString += currentString[j];
                }
            }
            // set the current string to the next string
            currentString = nextString;
            // reset the next string
            nextString = "";
        }
        return currentString;
    }

    //draw the string
    void DrawString(string generatedString)
    {
        //set the position and rotation
        Vector3 position = startPosition;
        Quaternion rotation = startRotation;

        //loop through the generated string
        for (int i = 0; i < generatedString.Length; i++)
        {
            switch(generatedString[i])
            {
                case 'F':
                    startPosition = position;
                    // move the position
                    position += rotation * Vector3.up * length;
                    // move forward draw a line
                    switch (mySelection)
                    {
                        case Select.Line:
                            // 2D LineRender Method
                            DrawLine(position, rotation, length, width);
                            break;
                        case Select.Cylinders:
                            // 3D Cylinder Method
                            DrawCylinders(position, rotation, length, width);
                            break;
                        case Select.Mesh:
                            // 3D Mesh Method
                            meshGenerator.CreateShape(startPosition, position, rotation, width);
                            break;
                    }
                    break;
                case 'f':
                    // move the position forward, without drawing line
                    position += rotation * Vector3.up * length;
                break;
                case 'G':
                    // Nothing - Could be used to generate more in the string building phase with a provided rule 
                    break;
                // Used to rotate both x and z axis at same time
                case '+':
                    // rotate the rotation left
                    rotation *= Quaternion.Euler(-angle , 0f, -angle);
                break;
                case '-':
                    // rotate the rotation right
                    rotation *= Quaternion.Euler(angle, 0f, angle);
                    break;
                    
                // X AXIS
                case 'X':
                    // rotate the rotation left
                    rotation *= Quaternion.Euler(-angle, 0f, 0f);
                    break;
                case 'x':
                    // rotate the rotation right
                    rotation *= Quaternion.Euler(angle, 0f, 0f);
                    break;
                // Y AXIS
                case 'Y':
                    // rotate the rotation left
                    rotation *= Quaternion.Euler(0f, -angle, 0f);
                    break;
                case 'y':
                    // rotate the rotation right
                    rotation *= Quaternion.Euler(0f, angle, 0f);
                    break;
                // Z AXIS
                case 'Z':
                    // rotate the rotation left
                    rotation *= Quaternion.Euler(0f, 0f, -angle);
                    break;
                case 'z':
                    // rotate the rotation right
                    rotation *= Quaternion.Euler(0f, 0f, angle);
                    break;
                    
                case '[':
                    // push the position and rotation to the stack
                    positionStack.Push(new TransformStore { position = position, rotation = rotation, width = width});
                    // Change the width of the branchs when we push to the stack, so that the branches get thinner each split
                    width *= 0.5f;
                    break;
                case ']':
                    // pop the position and rotation from the stack
                    TransformStore ti = positionStack.Pop();
                    // Place a leaf
                    Instantiate(leaves, position, rotation,LeafHolder);
                    // Restore the position and rotation
                    position = ti.position;
                    rotation = ti.rotation;
                    width = ti.width;
                    break;
            }
        }
        // Combining the final mesh after drawing is complete
        switch (mySelection)
        {
            case Select.Line:

                break;
            case Select.Cylinders:
                // 3D Cylinder Method
                CombineCylinderMesh();
                break;
            case Select.Mesh:

                break;
        }
    }

    // Draw a line in 2d using the line renderer 
    void DrawLine(Vector3 position, Quaternion rotation, float length, float width)
    {
        //create a new game object
        GameObject line = new("Line");
        //set the parent
        line.transform.parent = BranchHolder;
        line.transform.SetPositionAndRotation(position, rotation);
        //add a line renderer
        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        //set the width
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.material = branchMat;
        //set the position count
        lineRenderer.positionCount = 2;
        //set the start and end position
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, position);
    }

    void DrawCylinders(Vector3 position, Quaternion rotation, float length, float width)
    {
        GameObject branch = Instantiate(cylinder, startPosition,Quaternion.identity, BranchHolder);
        var offset = position - startPosition;
        branch.transform.localScale = new Vector3(width, width, offset.magnitude / 2);

        branch.transform.LookAt(position);
        GameObject body = branch.transform.GetChild(0).gameObject;
        body.transform.localPosition += new Vector3(0f,0f,1f);
    }
    
    void CombineCylinderMesh()
    {
        MeshFilter[] BranchmeshFilters = BranchHolder.GetComponentsInChildren<MeshFilter>();
        MeshFilter[] LeafmeshFilters = LeafHolder.GetComponentsInChildren<MeshFilter>();
        
        CombineInstance[] combineBranch = new CombineInstance[BranchmeshFilters.Length];
        CombineInstance[] combineLeaf = new CombineInstance[LeafmeshFilters.Length];

        int i = 0;
        while (i < BranchmeshFilters.Length)
        {
            combineBranch[i].mesh = BranchmeshFilters[i].sharedMesh;
            combineBranch[i].transform = BranchmeshFilters[i].transform.localToWorldMatrix;
            BranchmeshFilters[i].gameObject.SetActive(false);

            i++;
        }
        int j = 0;
        while (j < LeafmeshFilters.Length)
        {
            combineLeaf[j].mesh = LeafmeshFilters[j].sharedMesh;
            combineLeaf[j].transform = LeafmeshFilters[j].transform.localToWorldMatrix;
            LeafmeshFilters[j].gameObject.SetActive(false);

            j++;
        }
        temp = new GameObject("Tree");
        //temp mesh filter
        MeshFilter tempMeshFilter = temp.AddComponent<MeshFilter>();
        //temp mesh renderer
        MeshRenderer tempMeshRenderer = temp.AddComponent<MeshRenderer>();
        tempMeshRenderer.material = branchMat;
        //combine the meshes
        tempMeshFilter.mesh = new Mesh();
        tempMeshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        tempMeshFilter.mesh.CombineMeshes(combineBranch);
        
        GameObject Treeleaves = new GameObject("Leaves");
        Treeleaves.transform.parent = temp.transform;
        MeshFilter tempMeshFilterLeaf = Treeleaves.AddComponent<MeshFilter>();
        MeshRenderer tempMeshRendererLeaf = Treeleaves.AddComponent<MeshRenderer>();
        tempMeshRendererLeaf.material = leafMat;
        tempMeshFilterLeaf.mesh = new Mesh();
        tempMeshFilterLeaf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        tempMeshFilterLeaf.mesh.CombineMeshes(combineLeaf);


        //transform.GetComponent<MeshFilter>().mesh = new Mesh();
        //transform.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        //transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        //transform.gameObject.SetActive(true);
    }

    private void StartSliders()
    {
        if (iterationSlider != null)
        {
            // Initialise the sliders, (I don't know if there is a better way to do this)
            iterationSlider.value = iterations;
            angleSlider.value = angle;
            ruleSlider.value = currentRule;
            ruleSlider.maxValue = rules.Count - 1;
            lengthSlider.value = length;
            widthSlider.value = width;

            iterationText.text = iterations.ToString();
            angleText.text = angle.ToString();
            ruleText.text = currentRule.ToString();
            lengthText.text = length.ToString();
            widthText.text = width.ToString();

            iterationSlider.onValueChanged.AddListener(delegate { SetIterations(); });
            angleSlider.onValueChanged.AddListener(delegate { SetAngle(); });
            ruleSlider.onValueChanged.AddListener(delegate { SetRule(); });
            lengthSlider.onValueChanged.AddListener(delegate { SetLength(); });
            widthSlider.onValueChanged.AddListener(delegate { SetWidth(); });
        }
    }
    public void SetIterations()
    {
        iterations = (int)iterationSlider.value;
        iterationText.text = iterations.ToString();
    }
    public void SetAngle()
    {
        angle = (int)angleSlider.value;
        angleText.text = angle.ToString();
    }
    public void SetRule()
    {
        currentRule = (int)ruleSlider.value;
        ruleText.text = currentRule.ToString();
    }
    public void SetLength()
    {
        length = lengthSlider.value;
        lengthText.text = length.ToString();
    }
    public void SetWidth()
    {
        width = widthSlider.value;
        widthText.text = width.ToString();
    }

    public void SetSelection()
    {
        mySelection = (Select)selectionDropdown.value;
    }

}
