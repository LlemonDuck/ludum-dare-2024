using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleRandomWalkParameters", menuName = "PCG/SimpleRandomWalkData")]
public class SimpleRandomWalkSO : ScriptableObject {
    public int iterations = 10, walkLen = 10;
    public bool shouldRandomizeStartPerIteration = true;
}
