using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    public void StartGame()
    {
        if(!IsHost)
        {
            return;
        }

        //GameManager.instance.StartGame();
    }
}
