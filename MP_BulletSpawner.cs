using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI; 
using MLAPI.Messaging;
using System;

public class MP_BulletSpawner : NetworkBehaviour
{
    public Rigidbody bullet;
    public Transform bulletPos;
    private float bulletSpeed = 30f;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1") && IsOwner ) //click to fire 
        {
            FireServerRpc(bulletSpeed, gameObject.GetComponent<MP_PlayerAttribs>().powerUp.Value);
            //FireServerRpc(BulletSpeed)
        }
    }
    [ServerRpc]
    private void FireServerRpc(float speed, bool powerUp, ServerRpcParams serverRpcParams = default)
    {

        Rigidbody bulletClone = Instantiate(bullet, bulletPos.position, transform.rotation);//creates new clone of bullet //spawns locally
        bulletClone.velocity = transform.forward * speed; // set the velocity to go forward // gives it velocity
        bulletClone.GetComponent<MP_BulletScript>().spawnerPlayerId = serverRpcParams.Receive.SenderClientId;
        bulletClone.gameObject.GetComponent<NetworkObject>().Spawn(); // telling the network to spawn bullet
        Destroy(bulletClone.gameObject, 3);//Destroys bullet object after 3 seconds //


        if(powerUp)
        {
            Vector3 temp = new Vector3(1, 0, 0);
            bulletPos.Translate(temp, bulletPos);
            
            bulletClone = Instantiate(bullet, bulletPos.position, transform.rotation);
            bulletClone.velocity = transform.forward * speed;
            bulletClone.GetComponent<MP_BulletScript>().spawnerPlayerId = serverRpcParams.Receive.SenderClientId;
            bulletClone.gameObject.GetComponent<NetworkObject>().Spawn();
            Destroy(bulletClone.gameObject, 3);


            temp = new Vector3(-2, 0, 0);
            bulletPos.Translate(temp, bulletPos);

            bulletClone = Instantiate(bullet, bulletPos.position, transform.rotation);
            bulletClone.velocity = transform.forward * speed;
            bulletClone.GetComponent<MP_BulletScript>().spawnerPlayerId = serverRpcParams.Receive.SenderClientId;
            bulletClone.gameObject.GetComponent<NetworkObject>().Spawn();
            Destroy(bulletClone.gameObject, 3);

            temp = new Vector3(1, 0, 0);
            bulletPos.Translate(temp, bulletPos);
        }
    }
}
