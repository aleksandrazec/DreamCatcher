using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private NavMeshAgent agent;
    public Transform playerTransform;
    [SerializeField] private PlayerHealthSystem playerHealthSystem;

    [SerializeField] private float patrolRadius = 10f;
    private Vector3 currentPatrolPoint;
    private bool hasPatrolPoint;

    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float visionRange = 20f;
    [SerializeField] private float engagementRange = 10f;

    private bool canAttack = true;
    private bool canBeDamaged=true;
    private bool isDamaged=false;
    private bool isDead=false;
    private Vector3 knockbackDirection=Vector3.zero;

    [SerializeField] private float knockbackSpeed;

    [SerializeField] private Collider hitCollider;
    [SerializeField] private Animator animator;

    private bool playerVisible;
    private bool playerInRange;
    private bool playerAlive;

    private void Update()
    {
        DetectPlayer();
        UpdateBehaviorState();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, engagementRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
    private void DetectPlayer()
    {
        playerVisible = Physics.CheckSphere(transform.position, visionRange, playerMask);
        playerInRange = Physics.CheckSphere(transform.position, engagementRange, playerMask);
        playerAlive = !playerHealthSystem.isDead;
    }
    private void UpdateBehaviorState()
    {   
        if (isDead)
        {
            agent.SetDestination(transform.position);
        }
        else if (isDamaged)
        {
            Knockback();
        }
        else if((!playerVisible && !playerInRange) || !playerAlive)
        {
            PerformPatrol();
        }else if(playerVisible && !playerInRange)
        {
            PerformChase();
        }else if(playerVisible && playerInRange)
        {
            PerformAttack();
        }
    }
    private void Knockback()
    {
        transform.LookAt(playerTransform);
        Vector3 knockback = transform.position + knockbackDirection * knockbackSpeed * Time.deltaTime;
        agent.SetDestination(knockback);
    }
    public void Die()
    {
        StartCoroutine(DeadRoutine());
    }
    private IEnumerator DeadRoutine()
    {
        while (!canBeDamaged)
        {
            yield return null;
        }
        canBeDamaged = false;
        isDead = true;
        animator.SetBool("isDead", true);
    }
    public void TakeDamage(Vector3 knockbackDirection)
    {
        this.knockbackDirection = knockbackDirection;
        if (canBeDamaged)
        {
            StartCoroutine(DamageRoutine());
        }
    }
    private IEnumerator DamageRoutine()
    {
        canBeDamaged = false;
        isDamaged = true;
        animator.SetBool("isDamaged", true);
        while (animator.GetBool("isDamaged"))
        {
            yield return null;
        }
        isDamaged = false;
        canBeDamaged = true;
    }

    private void PerformAttack()
    {
        if (canAttack)
        {
            transform.LookAt(playerTransform);
            StartCoroutine(AttackRoutine());
        }
    }
    private void EnableCollider()
    {
        hitCollider.enabled = true;
    }

    private void DisableCollider()
    {
        hitCollider.enabled = false;
    }
    private void EndAttack()
    {
        animator.SetBool("isAttacking", false);
    }
    private void EndDamage()
    {
        animator.SetBool("isDamaged", false);
    }
    private IEnumerator AttackRoutine()
    {
        canAttack = false;
        animator.SetBool("isAttacking", true);
        while (animator.GetBool("isAttacking"))
        {
            yield return null;
        }
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void PerformChase()
    {
        animator.SetBool("isMoving", true);
        if (playerTransform != null)
        {
            transform.LookAt(playerTransform);
            agent.SetDestination(playerTransform.position);
        }
    }

    private void PerformPatrol()
    {
        if (!hasPatrolPoint)
        {
            FindPatrolPoint();
        }
        if (hasPatrolPoint)
        {
            animator.SetBool("isMoving", true);
            agent.SetDestination(currentPatrolPoint);
        }

        if (Vector3.Distance(transform.position, currentPatrolPoint) < 1f)
        {
            hasPatrolPoint = false;
        }
    }
    private void FindPatrolPoint()
    {
        float randomX = UnityEngine.Random.Range(-patrolRadius, patrolRadius);
        float randomZ = UnityEngine.Random.Range(-patrolRadius, patrolRadius);


        Vector3 potentialPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(potentialPoint, -transform.up, 10f, groundMask))
        {
            currentPatrolPoint = potentialPoint;
            hasPatrolPoint = true;
        }
    }

}
