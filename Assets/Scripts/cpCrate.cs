using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class cpCrate : MonoBehaviour, ICrateHandler
{
    public GameObject model;

    public void BreakCrate()
    {
        FindObjectOfType<CharacterControllerScript>().playerVelocity.y = 0;
        FindObjectOfType<CharacterControllerScript>().Jump();
        ScoringSystem.AddABox();
        model.transform.gameObject.SetActive(false);
        transform.GetComponent<Collider>().enabled = false;
        FindObjectOfType<CharacterControllerScript>().respawnPos = transform.position + Vector3.up * 2f;
        SetRespawnables();
    }

    private void SetRespawnables()
    {
        List<GameObject> crates = GameObject.FindGameObjectsWithTag("Crate").ToList();

        for(int i = 0; i < crates.Count; i++)
        {
            if (crates[i].transform.GetComponent<crates.StandardCrate>() != null && !crates[i].transform.GetComponent<crates.StandardCrate>().model.activeInHierarchy)
            {
                crates[i].transform.GetComponent<crates.StandardCrate>().isRespawnable = false;
            }
            if(crates[i].transform.GetComponent<TNTCrate>() != null && crates[i].transform.GetComponent<TNTCrate>().activated)
            {
                crates[i].transform.GetComponent<TNTCrate>().isRespawnable = false;
            }
        }
    }
}
