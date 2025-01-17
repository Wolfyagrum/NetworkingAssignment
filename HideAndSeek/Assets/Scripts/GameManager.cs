using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public Player LocalPlayer;

    public NetworkVariable<float> timer;
    public List<Player> allPlayers = new List<Player>();
    [SerializeField] private List<Transform> lobbySpawnPoints;
    [SerializeField] private List<Transform> gameSpawnPoints;

    public NetworkVariable<int> Points = new NetworkVariable<int>(0);

    private float prepTime = 5;
    private float gameTime = 300;
    private bool gameHasStarted;
    private bool prepTimeFinished;

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

    private void Update()
    {
        if (!IsServer) return;

        if (gameHasStarted)
        {
            timer.Value -= Time.deltaTime;

            if (!prepTimeFinished && timer.Value <= 0)
            {
                //UnblindSeekerClientRpc();
                Debug.Log("Ended prep time");
                prepTimeFinished = true;
                timer.Value = gameTime;
            }
        }
    }
    
    public void MoveAllPlayers()
    {
        foreach (var player in NetworkManager.Singleton.ConnectedClients.Values)
        {
            Vector3 randomSpawn = GetRandomGameSpawn();
            // player.PlayerObject.transform.position = randomSpawn;
            player.PlayerObject.transform.position = randomSpawn;
            Debug.Log(player.ClientId + " is owned by sever: " + player.PlayerObject.IsOwnedByServer);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveServerRpc()
    {
        MoveAllPlayers();
    }

    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
    public void StartGameServerRpc()
    {
        Debug.Log("Hi I am Sever?");
        MoveAllPlayers();

        return;
        if (!IsServer) return;

        if (!gameHasStarted)
        {
            timer.Value = prepTime;
            gameHasStarted = true;
            allPlayers[Random.Range(0, allPlayers.Count)].IsSeeker.Value = true;

            //UnblindHidersClientRpc();
        }
    }

    [ClientRpc]
    private void UnblindHidersClientRpc()
    {
        foreach (var player in allPlayers)
        {   
            player.Blindfold(player.IsSeeker.Value);
        }
    }

    [ClientRpc]
    private void UnblindSeekerClientRpc()
    {
        Debug.Log("client Side");
        foreach (var player in allPlayers)
        {   
            player.Blindfold(false);
        }
    }

    public int ProvideId(Player player)
    {
        allPlayers.Add(player);
        return allPlayers.Count - 1;
    }

    public Vector3 GetRandomLobbySpawn()
    {
        if (lobbySpawnPoints.Count > 0)
        {
            return lobbySpawnPoints[Random.Range(0, lobbySpawnPoints.Count)].position;
        }

        return default;
    }

    public Vector3 GetRandomGameSpawn()
    {
        if (gameSpawnPoints.Count > 0)
        {
            return gameSpawnPoints[Random.Range(0, gameSpawnPoints.Count)].position;
        }

        return default;
    }

    public void CheckIfEveryoneIsTagged()
    {
        foreach (var player in allPlayers)
        {
            if (!player.IsSeeker.Value)
            {
                return;
            }
        }

        foreach (var player in allPlayers)
        {
            player.ResetPlayer();
        }
    }
    [Rpc(SendTo.Server)]
    internal void GivePointServerRpc()
    {
        Points.Value++;
    }

    public void JoinGame()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void HostGame()
    {
        NetworkManager.Singleton.StartHost();
    }
}