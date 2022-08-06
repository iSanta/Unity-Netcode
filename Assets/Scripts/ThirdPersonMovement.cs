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
  
    private float turnSmoothVelocity;



    private NetworkVariable<Quaternion> rotationNetwork = new NetworkVariable<Quaternion>();
    private NetworkVariable<Vector3> positionNetwork = new NetworkVariable<Vector3>();

    private void Start()
    {
        if (!IsOwner) { return; }

        ownCamera.enabled = true;
        Cinemachine.SetActive(true);
        controller.enabled = true;
    }

    private void Update()
    {
        if (!IsOwner) { return; }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);




            Vector3 movedir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            UpdatePosRotServerRpc(angle, movedir);

            //transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //controller.Move(movedir.normalized * speed * Time.deltaTime);

            transform.rotation = rotationNetwork.Value;
            controller.Move(positionNetwork.Value);
        }
    }

    [ServerRpc]
    private void UpdatePosRotServerRpc(float rot, Vector3 pos)
    {
        UpdatePosRotClientRpc(rot, pos);
    }

    [ClientRpc]
    private void UpdatePosRotClientRpc(float rot, Vector3 pos)
    {
        rotationNetwork.Value = Quaternion.Euler(0f, rot, 0f); ;
        positionNetwork.Value = pos.normalized * speed * Time.deltaTime;
    }
}
