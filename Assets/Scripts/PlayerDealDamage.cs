using UnityEngine;

public class PlayerDealDamage : MonoBehaviour
{
    [SerializeField] private float hitDamage;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    [SerializeField] private Collider wandCollider;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealthSystem enemy = other.GetComponent<EnemyHealthSystem>();
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
