using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    public enum limits
    {
        noLimit=0,
        limit10=10,
        limit20=20,
        limit30 = 30,
        limit60=60,
        limit120=120,
    }

    public limits limit;

    void Awake()
    {
        Application.targetFrameRate = (int) limit;
    }

    void Update()
    {
        Application.targetFrameRate = (int) limit;
    }

}
