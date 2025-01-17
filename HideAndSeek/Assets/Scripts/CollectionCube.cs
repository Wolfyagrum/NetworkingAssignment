using Unity.Netcode;
using UnityEngine;

public class CollectionCube : NetworkBehaviour, IIntractable
{
    public void Interact()
    {
        GameManager.instance.GivePointServerRpc();
        ObjectSpawner.instance.RemoveObject();

        if(IsClient)
        {
            DestroyServerRpc();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Rpc(SendTo.Server)]
    private void DestroyServerRpc()
    {
        Destroy(gameObject);
    }
}
