using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace crates
{
    public class StandardCrate : MonoBehaviour, ICrateHandler
    {
        public int spawnMin, spawnMax;
        public GameObject spawnObject;
        public GameObject model;
        public float bloom;
        public bool isRespawnable;

        public void BreakCrate()
        {
            ScoringSystem.AddABox();
            Spawn();
            model.transform.gameObject.SetActive(false);
            transform.GetComponent<Collider>().enabled = false;
        }

        private void Spawn()
        {
            int spawn = Random.Range(spawnMin, spawnMax + 1);
            for(int i = 0; i < spawn; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(Random.Range(-bloom, bloom), Random.Range(-bloom, bloom), Random.Range(-bloom, bloom));
                GameObject _wumpa = Instantiate(spawnObject, spawnPos, Quaternion.identity);
                _wumpa.GetComponent<Collider>().enabled = false;
                StartCoroutine(ReactivateTrigger(_wumpa));
                _wumpa.GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }
        }

        private IEnumerator ReactivateTrigger(GameObject wumpa)
        {
            yield return new WaitForSeconds(1);
            wumpa.GetComponent<Collider>().enabled = true;
            wumpa.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
