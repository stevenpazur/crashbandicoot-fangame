using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandleDeaths : MonoBehaviour
{
    private bool forceFall = false;
    private Transform player;

    private void Start()
    {
        player = FindObjectOfType<CharacterControllerScript>().transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            other.transform.GetComponent<CharacterControllerScript>().enabled = false;
            forceFall = true;
            StartCoroutine(RespawnPlayerAfterFall(other.transform));
        }
    }

    private IEnumerator RespawnPlayerAfterFall(Transform player)
    {
        yield return new WaitForSeconds(2);
        FindObjectOfType<CharacterControllerScript>().transform.position = FindObjectOfType<CharacterControllerScript>().respawnPos + new Vector3(0, 0.1f, 0);
        HandleBoxes();
        DestroyAllSpawnedWumpas();
        yield return new WaitForSeconds(0.1f);
        player.GetComponent<CharacterControllerScript>().enabled = true;
        forceFall = false;
    }

    private void Update()
    {
        if (forceFall)
        {
            player.position = new Vector3(player.position.x, player.position.y - 0.1f, player.position.z);
        }
    }

    private void HandleBoxes()
    {
        List<GameObject> crates = GameObject.FindGameObjectsWithTag("Crate").ToList();
        for (int i = 0; i < crates.Count; i++)
        {
            var crate = crates[i].transform.GetComponent<crates.StandardCrate>();
            if (crate != null && crate.isRespawnable && !crate.model.activeInHierarchy)
            {
                crates[i].transform.GetComponent<crates.StandardCrate>().model.SetActive(true);
                crates[i].transform.GetComponent<Collider>().enabled = true;
                ScoringSystem.RemoveABox();
            }

            var tnt = crates[i].transform.GetComponent<TNTCrate>();
            if (tnt != null && tnt.isRespawnable && (!tnt.model.activeInHierarchy || tnt.activated))
            {
                tnt.model.SetActive(true);
                tnt.timer = crates[i].transform.GetComponent<TNTCrate>().countdown;
                tnt.activated = false;
                tnt.t1.SetActive(false);
                tnt.t2.SetActive(false);
                tnt.t3.SetActive(false);
                tnt.tnt.SetActive(true);
                crates[i].transform.GetComponent<Collider>().enabled = true;
                ScoringSystem.RemoveABox();
            }
        }
    }

    private void DestroyAllSpawnedWumpas()
    {
        List<GameObject> spawns = GameObject.FindGameObjectsWithTag("SpawnedObject").ToList();
        for(int i = 0; i < spawns.Count; i++)
        {
            Destroy(spawns[i]);
        }
    }
}
