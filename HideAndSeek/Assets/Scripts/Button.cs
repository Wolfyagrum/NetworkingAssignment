using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Button : NetworkBehaviour, IIntractable
{
    [SerializeField] private UnityEvent unityEvent;
    public void Interact()
    {
        unityEvent.Invoke();
    }
}
