using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentLight : MonoBehaviour
{
   
    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RenderSettings.reflectionIntensity = 0f;
        }
       
    }
}
