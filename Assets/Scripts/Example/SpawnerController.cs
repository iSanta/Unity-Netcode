using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnerController : NetworkBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int maxObjectsInstanceCount = 3;

    private void Awake()
    {
        
    }

    public void SpawnObjects()
    {
        if (!IsServer) return;
        for (int i=0; i< maxObjectsInstanceCount; i++)
        {
            GameObject go = Instantiate(objectPrefab, new Vector3(Random.Range(-10,10), 10f, Random.Range(-10,10)), Quaternion.identity);
            go.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
