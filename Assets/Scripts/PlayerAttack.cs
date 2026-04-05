using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public LayerMask targetLayers;
    [SerializeField] private float hitDamage;
    [SerializeField] private Collider attackCollider;
    private List<int> objectsCollided = new List<int>();

    private void OnTriggerEnter(Collider other)
    {
        int enemyId = other.gameObject.transform.root.gameObject.GetEntityId().GetHashCode();
        if (targetLayers == (targetLayers | (1 << other.gameObject.layer)) && !objectsCollided.Contains(enemyId))
        {
            EnemyHealthSystem enemy = other.GetComponentInParent<EnemyHealthSystem>();
            enemy.TakeDamage(hitDamage, transform.forward);
            objectsCollided.Add(enemyId);
        }
    }
    public void AttackEnded()
    {
        objectsCollided.Clear();
    }
}
