using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardCrate : MonoBehaviour, ICrateHandler
{
    public void BreakCrate()
    {
        transform.gameObject.SetActive(false);
        //TODO: spawn a wumpa
    }
}
