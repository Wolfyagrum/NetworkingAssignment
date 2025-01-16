using Unity.Netcode;
using UnityEngine;

public class Door : NetworkBehaviour
{
    [SerializeField] private Transform door;
    private NetworkVariable<bool> isOpen = new NetworkVariable<bool>(false);

    public void ToggleDoor()
    {
        if (IsServer)
        {
            isOpen.Value = !isOpen.Value;
            UpdateDoorRotation();
        }
        else
        {
            ToggleDoorServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleDoorServerRpc()
    {
        isOpen.Value = !isOpen.Value;
        UpdateDoorRotation();
    }

    private void UpdateDoorRotation()
    {
        if (isOpen.Value)
        {
            door.rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            door.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
