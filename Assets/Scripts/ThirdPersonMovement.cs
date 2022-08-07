using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ThirdPersonMovement : NetworkBehaviour
{

    [SerializeField] private CharacterController controller;
    private float speed = 6f;
    [SerializeField] private float speedRuning;
    [SerializeField] private float speedWalkin;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private Transform Cam;
    [SerializeField] private Camera ownCamera;
    [SerializeField] private GameObject Cinemachine;
    [SerializeField] private Animator anim;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float jumpForce;
 

    private float fallingPos = 0;
    private float oldFallingPos = 0;

    private float fallingSpeed;

    private float turnSmoothVelocity;
    private int animState = 0;
    private int oldAnimState = 0;

    private bool isJumping = false;
    private bool isRunning = false;

    private float angle;
    private Vector3 movedir;

    private NetworkVariable<Quaternion> rotationNetwork = new NetworkVariable<Quaternion>();
    private NetworkVariable<Vector3> positionNetwork = new NetworkVariable<Vector3>();
    private NetworkVariable<int> animStateNetwork = new NetworkVariable<int>(0);
    private NetworkVariable<float> gravityNetwork = new NetworkVariable<float>(0);

    private void Start()
    {
        if (!IsOwner) { return; }

        speedRuning = speed * 2;
        ownCamera.enabled = true;
        Cinemachine.SetActive(true);
        fallingPos = transform.position.y;
        speed = speedWalkin;

    }

    private void Update()
    {

        if (controller.isGrounded) {
            fallingSpeed = -gravity*Time.deltaTime;
            if (isJumping) isJumping = false;
        }
        else
        {
            fallingSpeed -= gravity * Time.deltaTime;
        }


        if (!IsOwner) { return; }

        if (!IsLocalPlayer) { return; }
        movedir = new Vector3();


        if (Input.GetButtonDown("Run"))
        {
            
            isRunning = true;
        }
        if (Input.GetButtonUp("Run"))
        {
            
            isRunning = false;
        }

        if (isRunning && !isJumping) {
            animState = 2;
            speed = speedRuning;
        }
        else
        {
            speed = speedWalkin;
        }

        //controller.Move(new Vector3(0, fallingSpeed, 0));


        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

      


        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            if(isJumping == false && isRunning == false) animState = 1;
           


            movedir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;



            


            //transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //controller.Move(movedir.normalized * speed * Time.deltaTime);

            


        }
        else
        {
            if (isJumping == false) animState = 0;
        }
        if (animState != oldAnimState) {

            UpdateAnimStateServerRpc(animState);

            anim.SetInteger("AnimState", animState);
            oldAnimState = animState;
        }

        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            animState = 5;
            fallingSpeed = jumpForce;
        }

        

        movedir.y = fallingSpeed;
        //if (!controller.isGrounded && direction.magnitude >= 0.1f)
        //{
            UpdatePosRotServerRpc(angle, movedir);

        //}


        transform.rotation = rotationNetwork.Value;
        controller.Move(positionNetwork.Value);
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
