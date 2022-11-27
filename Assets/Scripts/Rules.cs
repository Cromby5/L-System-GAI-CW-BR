using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Rules
{
    public char symbol;
    public string result;
    public float probability;
    public Rules(char symbol, string result, float probability)
    {
        this.symbol = symbol;
        this.result = result;
        this.probability = probability;
    }
}