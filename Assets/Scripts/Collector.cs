using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public enum itemType
    {
        Wumpa, Life
    }
    public itemType type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if(type == itemType.Wumpa)
            {
                ScoringSystem.AddAWumpa();
                transform.gameObject.SetActive(false);
            }
        }
    }
}
