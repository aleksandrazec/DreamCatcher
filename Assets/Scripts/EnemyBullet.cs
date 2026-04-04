using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float life = 3;
    [SerializeField] private float shootDamage;
    [SerializeField] private Collider parentCollider;

    private void Awake()
    {
        Destroy(gameObject, life);
    }
    // to do: implement layers and utilize collision matrix
    // i.e. remove the parent collider shit
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthSystem player = other.GetComponent<PlayerHealthSystem>();
            player.TakeDamage(shootDamage, transform.forward);
        }
        if (other != parentCollider)
        {
            Destroy(gameObject);
        }
    }
    public void SetParentCollider(Collider parent)
    {
        parentCollider=parent;
    }
}
