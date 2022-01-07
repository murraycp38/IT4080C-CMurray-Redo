using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
public class MP_PlayerMovement : NetworkBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 100f;
    public Transform camT;
    CharacterController mpCharController;



    // Start is called before the first frame update
    void Start()
    {
        mpCharController = GetComponent<CharacterController>();
        //color changing
        if(IsOwner)
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
             GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        //disable other cmaeras
        if(!IsOwner)
        {
            camT.GetComponent<Camera>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOwner)
        {
             MPMovePlayer();
        }
    }

    void MPMovePlayer()
    {
        transform.Rotate(0, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime,0);
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        mpCharController.SimpleMove(forward * movementSpeed * Input.GetAxis("Vertical"));
       // Vector3 moveVect = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //mpCharController.SimpleMove(moveVect * movementSpeed);
    }
}

