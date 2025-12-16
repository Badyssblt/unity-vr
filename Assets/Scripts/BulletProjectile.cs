using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private LayerMask collisionMask;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Spawn impact effect
        if (impactEffect != null)
        {
            ContactPoint contact = collision.contacts[0];
            GameObject effect = Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(effect, 2f);
        }

        // Check if hit a target
        Target target = collision.gameObject.GetComponent<Target>();
        if (target != null)
        {
            target.TakeDamage();
        }

        // Destroy bullet
        Destroy(gameObject);
    }
}
