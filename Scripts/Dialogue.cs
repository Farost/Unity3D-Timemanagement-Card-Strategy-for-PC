using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {
    public string fact;
    public string[] sentences;
    public string[] answers;

    public Dialogue(int i)
    {
        fact = "";
        sentences = new string[i];
        answers = new string[i];
    }

}
