using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletSpawnTransform;
    public void Shoot()
    {
        GameObject newBullet = Instantiate(bullet, bulletSpawnTransform.position, transform.rotation);
        Rigidbody newBulletRb = newBullet.GetComponent<Rigidbody>();
        newBulletRb.AddForce(bulletSpawnTransform.forward * 10f, ForceMode.Impulse);
        Debug.Log("Shoot!");
    }
}
