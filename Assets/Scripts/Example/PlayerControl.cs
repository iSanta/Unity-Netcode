using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class PlayerControl : NetworkBehaviour
{

    [SerializeField] private Camera cam;

    [SerializeField]
    private float walkSpeed = 3.5f;

    [SerializeField]
    private float runSpeed = 6.0f;

    [SerializeField]
    private float rotationSpeed = 3.5f;

    [SerializeField]
    private Vector2 defaultInitialPositionOnPlane = new Vector2(-4, 4);

    [SerializeField]
    private NetworkVariable<Vector3> networkPositionDirection = new NetworkVariable<Vector3>();

    [SerializeField]
    private NetworkVariable<Vector3> networkRotationDirection = new NetworkVariable<Vector3>();

    [SerializeField] private NetworkVariable<int> animStateNetwork = new NetworkVariable<int>();


    private float speed;
    private bool isRunning;

    private CharacterController characterController;

    // client caches positions
    private Vector3 oldInputPosition = Vector3.zero;
    private Vector3 oldInputRotation = Vector3.zero;

    [SerializeField]
    private Animator animator;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y), 0,
                   Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y));

            cam.enabled = true;
            speed = walkSpeed;
            isRunning = false;
        }
    }

    void Update()
    {
        if (IsClient && IsOwner)
        {
            ClientInput();
        }

        ClientMoveAndRotate();
        ClientVisuals();
    }

    private void ClientMoveAndRotate()
    {
        if (networkPositionDirection.Value != Vector3.zero)
        {
            characterController.SimpleMove(networkPositionDirection.Value * Time.deltaTime);
        }
        if (networkRotationDirection.Value != Vector3.zero)
        {
            transform.Rotate(networkRotationDirection.Value * Time.deltaTime, Space.World);
        }
    }

    private void ClientVisuals()
    {
        animator.SetInteger("AnimState", animStateNetwork.Value);
    }

    private void ClientInput()
    {
        
        // left & right rotation
        Vector3 inputRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);

        // forward & backward direction
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float forwardInput = Input.GetAxis("Vertical");
        Vector3 inputPosition = direction * forwardInput;

        if (Input.GetButtonDown("Run")) { isRunning = true; speed = runSpeed; }
        else if (Input.GetButtonUp("Run")) { isRunning = false; speed = walkSpeed; }


        UpdateClientPositionAndRotationServerRpc(inputPosition * speed, inputRotation * rotationSpeed);


        if (forwardInput > 0 && !isRunning)
        {
            UpdatePlayerStateServerRpc(1);
        }
        else if (forwardInput > 0 && isRunning)
        {
            UpdatePlayerStateServerRpc(2);
        }
        else if (forwardInput<0)
        {
            UpdatePlayerStateServerRpc(-1);
        }
        else
        {
            UpdatePlayerStateServerRpc(0);
        }

    }



    [ServerRpc]
    public void UpdateClientPositionAndRotationServerRpc(Vector3 newPosition, Vector3 newRotation)
    {
        networkPositionDirection.Value = newPosition;
        networkRotationDirection.Value = newRotation;
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(int state)
    {
        animStateNetwork.Value = state;
    }
}