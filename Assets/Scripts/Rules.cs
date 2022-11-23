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