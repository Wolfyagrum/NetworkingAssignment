using Cinemachine;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientPlayerMove : NetworkBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private Transform cameraFollow;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject eyes;

    private void Awake()
    {
        characterController.enabled = false;
        firstPersonController.enabled = false;
        playerInput.enabled = false;
        virtualCamera.enabled = false;
        eyes.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsClient;

        if (!IsOwner)
        {
            enabled = false;
            characterController.enabled = false;
            firstPersonController.enabled = false;
            playerInput.enabled = false;
            virtualCamera.enabled = false;
            eyes.SetActive(true);
            return;
        }

        virtualCamera.enabled = true;
        characterController.enabled = true;
        firstPersonController.enabled = true;
        playerInput.enabled = true;

        if (virtualCamera != null)
        {
            virtualCamera.Follow = cameraFollow;
            virtualCamera.LookAt = cameraFollow;
        }
    }
}
