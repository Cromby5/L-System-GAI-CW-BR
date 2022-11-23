using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LSystem: MonoBehaviour
{
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
    public Vector3 startPosition = Vector3.zero;
    // start rotation
    public Quaternion startRotation = Quaternion.identity;
    // line renderer
    public LineRenderer lineRenderer;
    // stack for position and rotation
    Stack<TransformStore> positionStack = new Stack<TransformStore>();
    private List<GameObject> children = new List<GameObject>();

    [SerializeField] private Material branchMat;

    [SerializeField] private GameObject cube;

    // Sliders
    [SerializeField] private Slider iterationSlider;
    [SerializeField] private Slider angleSlider;
    [SerializeField] private Slider ruleSlider;
    
    [SerializeField] private TextMeshProUGUI iterationText;
    [SerializeField] private TextMeshProUGUI angleText;
    [SerializeField] private TextMeshProUGUI ruleText;
    // Start is called before the first frame update
    void Start()
    {
        TreeGen();
        // Sliders
        iterationSlider.value = iterations;
        angleSlider.value = angle;
        ruleSlider.value = currentRule;
        ruleSlider.maxValue = rules.Count - 1;

        iterationText.text = iterations.ToString();
        angleText.text = angle.ToString();
        ruleText.text = currentRule.ToString();

        iterationSlider.onValueChanged.AddListener(delegate { SetIterations(); });
        angleSlider.onValueChanged.AddListener(delegate { SetAngle(); });
        ruleSlider.onValueChanged.AddListener(delegate { SetRule(); });

    }
    public void TreeGen()
    {
        //destroy old tree
        foreach (Transform transform in transform)
        {
            Destroy(transform.gameObject);
        }
        startPosition = transform.position;
        // generate the string
        string generatedString = GenerateString(axiom, rules[currentRule].result, iterations);
        // draw the string
        DrawString(generatedString);
    }

    //generate the string
    string GenerateString(string axiom, string rule, int iterations)
    {
        // current string
        string currentString = axiom;
        // next string
        string nextString = "";

        // loop through the iterations
        for (int i = 0; i < iterations; i++)
        {
            // loop through the current string
            for (int j = 0; j < currentString.Length; j++)
            {
                // if the current character is an F
                if (currentString[j] == 'F')
                {
                    //add the rule to the next string
                    nextString += rule;
                }
                // if the current character is not an F
                else
                {
                    // add the current character to the next string
                    nextString += currentString[j];
                }
            }
            // set the current string to the next string
            Debug.Log(nextString);
            currentString = nextString;
            //reset the next string
            nextString = "";
        }

        // return the current string
        Debug.Log(currentString);
        return currentString;
    }

    //draw the string
    void DrawString(string generatedString)
    {
        //set the position
        Vector3 position = startPosition;
        //set the rotation
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
                    DrawLine(position, rotation, length, width);
                break;
                case 'f':
                    // move the position forward, without drawing line
                    position += rotation * Vector3.up * length;
                break;
                case 'G':
                    break;
                case '+':
                    // rotate the rotation left
                    rotation *= Quaternion.Euler(-angle, 0f, -angle);
                break;
                case '-':
                    // rotate the rotation right
                    rotation *= Quaternion.Euler(angle, 0f, angle);
                break;
                case '[':
                    // push the position and rotation to the stack
                    positionStack.Push(new TransformStore { position = position, rotation = rotation });
                    break;
                case ']':
                    // pop the position and rotation from the stack
                    TransformStore ti = positionStack.Pop();
                    // Place a leaf
                    children.Add(Instantiate(cube, position, rotation,transform));
                    // Restore the position and rotation
                    position = ti.position;
                    rotation = ti.rotation;
                    break;
            }
        }
    }

    //draw a line
    void DrawLine(Vector3 position, Quaternion rotation, float length, float width)
    {
        //create a new game object
        GameObject line = new("Line");
        children.Add(line);
        //set the parent
        line.transform.parent = transform;
        //set the position and rotation
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

}
