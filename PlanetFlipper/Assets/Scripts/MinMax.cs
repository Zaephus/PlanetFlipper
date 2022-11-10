using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax  {

    public float Min { get; private set; }
    public float Max { get; private set; }

    public MinMax() {
        Min = float.MaxValue;
        Max = float.MinValue;
    }

    public void AddValue(float _v) {

        if(_v > Max) {
            Max = _v;
        }
        if(_v < Min) {
            Min = _v;
        }

    }
}