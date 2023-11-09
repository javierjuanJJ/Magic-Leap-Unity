using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class SegmentedDimming : MonoBehaviour
{
    // object with SegmentedDimmer material applied
    public GameObject segmentedDimmingObject;
    private static readonly int Value = Shader.PropertyToID("_DimmingValue");

    void Start()
    {
        MLSegmentedDimmer.Activate();
    }

    // method to set opacity, takes a float value between 0-1
    public void SetSegmentedDimmingOpacity(float DimmingValue)
    {
        segmentedDimmingObject.GetComponent<MeshRenderer>().material.SetFloat(Value, DimmingValue);
    }

    // method to enable or disable the mask GameObject, takes a bool value
    public void ActivateSegmentedDimming(bool active)
    {
        segmentedDimmingObject.SetActive(active);
    }
}
