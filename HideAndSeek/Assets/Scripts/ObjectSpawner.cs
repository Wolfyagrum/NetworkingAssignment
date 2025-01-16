using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    public static ObjectSpawner instance;
    [SerializeField] private List<Transform> locations;
    [SerializeField] private CollectionCube objectToSpawn;
    [SerializeField] private int maxObjectsInTheScene;

    private int currentObjects = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            StartCoroutine(SpawnObjects());
        }
    }

    private IEnumerator SpawnObjects()
    {
        while(true)
        {
            if(currentObjects < maxObjectsInTheScene)
            {
                yield return new WaitForSeconds(1);
                CollectionCube cube = Instantiate(objectToSpawn, locations[Random.Range(0, locations.Count)].position, Quaternion.identity, this.transform);

                NetworkObject networkObject = cube.GetComponent<NetworkObject>();
                if(networkObject)
                {
                    networkObject.Spawn();
                    currentObjects++;
                }
            }

            yield return null;
        }
    }

    internal void RemoveObject()
    {
        currentObjects--;
    }
}
