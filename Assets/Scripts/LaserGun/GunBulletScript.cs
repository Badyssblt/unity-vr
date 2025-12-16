using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBulletScript : MonoBehaviour
{
    Rigidbody rb;
    void Start()
    {
        // Destroy bullet 2s after being spawned.
        Destroy(this.gameObject, 2f);
    }
}
