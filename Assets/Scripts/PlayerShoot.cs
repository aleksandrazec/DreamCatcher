using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private PlayerBullet bulletPrefab;
    public float bulletSpeed = 10f;
    public float bulletDamage = 0;
    public void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        if (bulletDamage > 0)
        {
            bullet.shootDamage = bulletDamage;
        }
        bullet.GetComponent<Rigidbody>().linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
    }
    public void SetUpShoot(float bulletDamage,int attackUpgrades)
    {
        this.bulletDamage = bulletDamage*(1+0.1f*attackUpgrades);
    }
}
