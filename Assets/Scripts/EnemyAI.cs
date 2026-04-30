using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private NavMeshAgent agent;
    public Transform playerTransform;
    [SerializeField] public PlayerHealthSystem playerHealthSystem;


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
    private Vector3 deadPosition=Vector3.zero;
    private float currentAlpha=1.0f;
    private bool setInvisible = false;

    private bool playerVisible=false;
    private bool playerInRange=false;
    private bool playerAlive=false;

    [SerializeField] private float knockbackSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private Renderer demonSkin;
    [SerializeField] private Material blackSkin;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] public EnemyType enemyType;

    public enum EnemyType
    {
        ghost,
        dress,
        bat,
        worm
    }
    Dictionary<EnemyType, float> DeadBaseOffset = new Dictionary<EnemyType, float>
    {
        {EnemyType.ghost, 3.5f },
        {EnemyType.bat, 0.5f},
        {EnemyType.worm, 4f },
        {EnemyType.dress, 3f }
    };
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
        if (playerHealthSystem != null)
        {
            playerAlive = !playerHealthSystem.isDead;
        }
    }
    private void UpdateBehaviorState()
    {   
        if (isDead)
        {
            if (setInvisible)
            {
                MakeTransparent();
            }
        }
        else if (isDamaged)
        {
            Knockback();
        }
        else if(((!playerVisible && !playerInRange) || (!playerAlive))&& agent.enabled)
        {
            PerformPatrol();
        }else if(playerVisible && !playerInRange && agent.enabled)
        {
            PerformChase();
        }else if((playerVisible && playerInRange) || !agent.enabled)
        {
            PerformAttack();
        }
    }
    private void MakeTransparent()
    {
        currentAlpha = Mathf.MoveTowards(currentAlpha, 0f, Time.deltaTime*0.5f);
        var color = demonSkin.material.color;
        var newColor = new Color(color.r, color.g, color.b, currentAlpha);
        demonSkin.material.color=newColor;
        if (currentAlpha==0)
        {
            Destroy(gameObject);
        }
    }
    private void Knockback()
    {

        transform.LookAt(playerTransform);
        Vector3 knockback = transform.position + knockbackDirection * knockbackSpeed * Time.deltaTime;
        rigidBody.MovePosition(knockback);
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
        agent.baseOffset = DeadBaseOffset[enemyType];
        demonSkin.material = blackSkin;
        animator.SetBool("isDead", true);
        isDead = true;
        agent.enabled = false;
        while (animator.GetBool("isDead"))
        {
            yield return null;
        }
        setInvisible = true;
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
        agent.enabled = false;
        animator.SetBool("isDamaged", true);
        while (animator.GetBool("isDamaged"))
        {
            yield return null;
        }
        animator.SetBool("isAttacking", false);
        agent.enabled = true;
        isDamaged = false;
        canBeDamaged = true;
    }   

    private void PerformAttack()
    {
        if (canAttack)
        {
            transform.LookAt(playerTransform);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            StartCoroutine(AttackRoutine());
        }
    }
    private IEnumerator AttackRoutine()
    {
        canAttack = false;
        agent.enabled = false;
        animator.SetBool("isAttacking", true);
        while (animator.GetBool("isAttacking"))
        {
            yield return null;
        }
        agent.enabled = true;
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
            transform.LookAt(currentPatrolPoint);
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
