using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TNTCrate : MonoBehaviour, ICrateHandler
{
    public GameObject model;
    public bool isRespawnable = true;
    public bool activated = false;
    public float countdown = 4.0f;
    [HideInInspector] public float timer;
    public GameObject tnt, t3, t2, t1;
    public bool exploded = false;
    public float explosionRadius = 2f;
    public float explosionDelay = 0.2f;

    public void BreakCrate()
    {
        ScoringSystem.AddABox();
        activated = true;
        timer = countdown;
    }

    private void Update()
    {
        if (activated)
        {
            timer -= Time.deltaTime;
            print(timer);
        }

        if(activated && timer > 2.8f)
        {
            tnt.SetActive(false);
            t3.SetActive(true);
        }

        if (timer > 1.2f && timer <= 2.8f)
        {
            t3.SetActive(false);
            t2.SetActive(true);
        }

        if (timer <= 1.2f)
        {
            t2.SetActive(false);
            t1.SetActive(true);
        }

        if(timer <= 0)
        {
            Explode();
            model.transform.gameObject.SetActive(false);
            transform.GetComponent<Collider>().enabled = false;
        }
    }

    private void Explode()
    {
        exploded = true;
        Collider[] allNearbyObjects = Physics.OverlapSphere(transform.GetComponent<Collider>().bounds.center, explosionRadius);
        List<GameObject> nearbyCrates = new List<GameObject>();
        for(int i = 0; i < allNearbyObjects.Length; i++)
        {
            if(allNearbyObjects[i].transform.GetComponent<ICrateHandler>() != null)
            {
                nearbyCrates.Add(allNearbyObjects[i].transform.gameObject);
            }
        }
        StartCoroutine(DelayExplosion(nearbyCrates));
    }

    private IEnumerator DelayExplosion(List<GameObject> crates)
    {
        yield return new WaitForSeconds(explosionDelay);
        crates.ForEach(e => e.transform.GetComponent<ICrateHandler>().BreakCrate());
    }
}
