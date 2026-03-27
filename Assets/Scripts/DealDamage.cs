using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private float hitDamage;
    [SerializeField] private Collider wandCollider;

    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.GetComponent<HealthSystem>();
            enemy.TakeDamage(hitDamage);
        }
    }
    //to do: relate enable and disable to animation events
    public void EnableWandCollider()
    {
        wandCollider.enabled = true;
    }
    public void DisableWandCollider()
    {
        wandCollider.enabled = false;
    }
    public void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);   
        bullet.GetComponent<Rigidbody>().linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
    }
}
