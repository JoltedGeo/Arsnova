using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FEPortalManager : MonoBehaviour
{
    [SerializeField] List<GameObject> portalSpawns = new List<GameObject>();
    public GameObject FEPrefab;

    public int portalSpawnersAmount;
    public float spawnRate = 5;
    private float spawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;

        if(spawnTimer > spawnRate)
        {
            spawnTimer = 0;

            int randomPortal = Random.Range(0, portalSpawnersAmount);

            if (portalSpawns.Count > randomPortal)
            {
                GameObject specificGameObject = portalSpawns[randomPortal];
                Debug.Log("Picked: " + specificGameObject + randomPortal);
                Instantiate(FEPrefab, specificGameObject.transform.position, Quaternion.identity);
            }
        }
    }
}
