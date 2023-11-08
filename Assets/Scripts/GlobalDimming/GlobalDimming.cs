using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class GlobalDimming : MonoBehaviour
{
    void Start()
    {
        SetGlobalDimmingValue(0.5f);
    }

    public void SetGlobalDimmingValue(float value)
    {
        MLGlobalDimmer.SetValue(value);
    }
}
