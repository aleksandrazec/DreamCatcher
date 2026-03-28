using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private float hitDamage;

    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    [SerializeField] private Collider wandCollider;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.GetComponent<HealthSystem>();
            enemy.TakeDamage(hitDamage);
        }
        wandCollider.enabled = false;

    }

    public void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);   
        bullet.GetComponent<Rigidbody>().linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
    }
}
