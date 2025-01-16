using UnityEngine;

public class DoorInteraction : MonoBehaviour, IIntractable
{
    [SerializeField] private Door door;

    public void Interact()
    {
        ToggleDoor();
    }

    public void ToggleDoor()
    {
        if(door == null) return;

        door.ToggleDoor();
    }
}
