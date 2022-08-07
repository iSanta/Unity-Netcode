using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ThirdPersonMovement : NetworkBehaviour
{

    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private Transform Cam;
    [SerializeField] private Camera ownCamera;
    [SerializeField] private GameObject Cinemachine;
    [SerializeField] private Animator anim;
    [SerializeField] private float gravity = 9.8f;


    private float fallingPos = 0;
    private float oldFallingPos = 0;

    private float fallingSpeed;

    private float turnSmoothVelocity;
    private int animState = 0;
    private int oldAnimState = 0;



    private NetworkVariable<Quaternion> rotationNetwork = new NetworkVariable<Quaternion>();
    private NetworkVariable<Vector3> positionNetwork = new NetworkVariable<Vector3>();
    private NetworkVariable<int> animStateNetwork = new NetworkVariable<int>(0);
    private NetworkVariable<float> gravityNetwork = new NetworkVariable<float>(0);

    private void Start()
    {
        if (!IsOwner) { return; }


        ownCamera.enabled = true;
        Cinemachine.SetActive(true);
        fallingPos = transform.position.y;

    }

    private void Update()
    {
        controller.Move(new Vector3(0, fallingSpeed, 0));

        if (controller.isGrounded) {
            fallingSpeed = -gravity*Time.deltaTime;
        }
        else
        {
            fallingSpeed -= gravity * Time.deltaTime;
        }


        if (!IsOwner) { return; }

        if (!IsLocalPlayer) { return; }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        if (!controller.isGrounded)
        {
            Debug.Log(fallingSpeed);
        }


        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            animState = 1;


            Vector3 movedir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;



            UpdatePosRotServerRpc(angle, movedir);


            //transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //controller.Move(movedir.normalized * speed * Time.deltaTime);

            transform.rotation = rotationNetwork.Value;
            controller.Move(positionNetwork.Value);


        }
        else
        {
            animState = 0;
        }
        if (animState != oldAnimState) {

            UpdateAnimStateServerRpc(animState);

            anim.SetInteger("AnimState", animState);
            oldAnimState = animState;
        }
    }



    [ServerRpc]
    private void UpdatePosRotServerRpc(float rot, Vector3 pos)
    {

        UpdatePosRotClientRpc(rot, pos);
        rotationNetwork.Value = Quaternion.Euler(0f, rot, 0f); ;
        positionNetwork.Value = pos.normalized * speed * Time.deltaTime;
    }

    [ClientRpc]
    private void UpdatePosRotClientRpc(float rot, Vector3 pos)
    {
        if (IsOwner) { return; }
        transform.rotation = rotationNetwork.Value;
        controller.Move(positionNetwork.Value);
    }

    [ServerRpc]
    private void UpdateAnimStateServerRpc(int a)
    {
        animStateNetwork.Value = a;
        UpdateAnimStateClientRpc();

    }
    [ClientRpc]
    private void UpdateAnimStateClientRpc()
    {
        if (IsOwner) { return; }
        anim.SetInteger("AnimState", animStateNetwork.Value);
    }
}
