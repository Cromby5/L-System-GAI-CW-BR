using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Rules
{
    public char symbol;
    public string result;
    public Rules(char symbol, string result)
    {
        this.symbol = symbol;
        this.result = result;
    }
}
public class TransformInfo
{
    public Vector3 position;
    public Quaternion rotation;
}

public class LSystem: MonoBehaviour
{
    // axiom
    public string axiom = "F";
    // static rule
    public string rule = "F-F++F-F";
    //public List<Rules> rules = new List<Rules>();
    // number of iterations
    public int iterations = 3;
    // angle
    public float angle = 90f;
    // length
    public float length = 1f;
    // width
    public float width = 1f;
    // start position
    public Vector3 startPosition = Vector3.zero;
    // start rotation
    public Quaternion startRotation = Quaternion.identity;
    // line renderer
    public LineRenderer lineRenderer;
    // stack for position and rotation
    Stack<TransformInfo> positionStack = new Stack<TransformInfo>();

    // Start is called before the first frame update
    void Start()
    {
        // generate the string
        string generatedString = GenerateString(axiom, rule, iterations);
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
                    rotation *= Quaternion.Euler(0f, 0f, -angle);
                break;
                case '-':
                    // rotate the rotation right
                    rotation *= Quaternion.Euler(0f, 0f, angle);
                break;
                case '[':
                    // push the position and rotation to the stack
                    positionStack.Push(new TransformInfo { position = position, rotation = rotation });
                    break;
                case ']':
                    // pop the position and rotation from the stack
                    TransformInfo ti = positionStack.Pop();
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
        //set the parent
        line.transform.parent = transform;
        //set the position and rotation
        line.transform.SetPositionAndRotation(position, rotation);
        //add a line renderer
        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        //set the start width
        lineRenderer.startWidth = width;
        //set the end width
        lineRenderer.endWidth = width;
        //set the position count
        lineRenderer.positionCount = 2;
        //set the start position
        lineRenderer.SetPosition(0, startPosition);
        //set the end position
        lineRenderer.SetPosition(1, position);
    }

}
