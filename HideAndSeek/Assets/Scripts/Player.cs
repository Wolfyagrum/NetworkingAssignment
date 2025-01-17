using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public int playerId = -1;

    public NetworkVariable<bool> IsSeeker = new NetworkVariable<bool>(false);
    [SerializeField] private GameObject blindFold;
    [SerializeField] private AudioSource audioSource;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        playerId = GameManager.instance.ProvideId(this);
        transform.position = GameManager.instance.GetRandomGameSpawn();
        //blindFold.SetActive(true);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.TryGetComponent(out Player player))
        {
            if(!player.IsSeeker.Value)
            {
                player.Tagged();
            }
        }
    }

    public void Blindfold(bool putBlindFoldOn)
    {

        Debug.Log("I am owner");
        blindFold.SetActive(putBlindFoldOn);
       
    }

    public void Tagged()
    {
        IsSeeker.Value = true;
    }

    public void ResetPlayer()
    {
        IsSeeker.Value = false;
        transform.position = GameManager.instance.GetRandomLobbySpawn();
    }

    [ClientRpc]
    public void TestClientRpc(ulong id, Vector3 location)
    {
        if(id == NetworkManager.Singleton.LocalClient.ClientId)
        {
            Debug.Log("Hi I am Client " + NetworkManager.Singleton.LocalClient.ClientId + " and i am moving to " + location);
            MovePlayerTo(location);
        }
        else
        {
            MovePlayerTo(Vector3.zero);
        }
    }

    public void MovePlayerTo(Vector3 location)
    {
        transform.position = location;
    }


        // Teleport all players to a specific position
    [ServerRpc(RequireOwnership = false)]
    public void TeleportAllPlayersServerRpc(Vector3 newPosition)
    {
        // Iterate through all connected players
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            var playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            if (playerObject != null)
            {
                // Teleport the player
                playerObject.GetComponent<Player>().TeleportClientRpc(newPosition);
            }
        }
    }

    // Teleport a specific player (called on the client)
    [ClientRpc]
    public void TeleportClientRpc(Vector3 newPosition)
    {
        if (IsOwner)
        {
            transform.position = newPosition;
        }
    }

    [Rpc (SendTo.Server)]
    public void PlayAudioServerRpc(ulong id)
    {
        PlayAudioEveryoneRpc(id);
    }

    [Rpc (SendTo.Everyone)]
    private void PlayAudioEveryoneRpc(ulong id)
    {
        Debug.Log("I am: " + id);
        PlayAudio(id);
    }

    private void PlayAudio(ulong id)
    {
        Debug.Log("Local Call from: " + id);
        audioSource.Play();
    }
}
