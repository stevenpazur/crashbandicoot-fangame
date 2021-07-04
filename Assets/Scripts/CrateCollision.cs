using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateCollision : MonoBehaviour
{
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.transform.tag == "Crate")
        {
            if(transform.GetComponent<Collider>().bounds.min.y >= hit.collider.bounds.max.y)
            {
                FindObjectOfType<CharacterControllerScript>().playerVelocity.y = 0;
                FindObjectOfType<CharacterControllerScript>().Jump();
                HandleCrateBreak(hit.transform.GetComponent<ICrateHandler>());
            }
        }
    }

    private void HandleCrateBreak(ICrateHandler crate)
    {
        //print(crate);
        crate.BreakCrate();
    }
}
