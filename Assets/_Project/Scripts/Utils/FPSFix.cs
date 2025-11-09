using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSFix : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
#if !UNITY_ANDROID
        Application.targetFrameRate = 60;
#endif
    }
}