using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyAI enemyAI;
    public EnemyAI.EnemyType enemyType;
    public EnemyHealthSystem healthSystem;
    public Billboard enemyUI;
    public EnemyAttack enemyAttack;
    public BatAnimationEventCaller batAttack;

    public void MakeBoss(float damage, float health, Vector3 scale, float speedBoost)
    {
        if (enemyType == EnemyAI.EnemyType.bat)
        {
            batAttack.bulletDamage= damage;
            transform.position += new Vector3(0, -60, 0);
            enemyAI.agent.baseOffset = 2;
        }else if (enemyType == EnemyAI.EnemyType.ghost)
        {
            enemyAI.agent.acceleration = 100;
            enemyAI.agent.speed = 500;
            enemyAI.agent.angularSpeed = 500;
        }else if(enemyType == EnemyAI.EnemyType.worm)
        {
            enemyAI.agent.acceleration = 50;
            enemyAI.agent.speed = 150;
            enemyAI.agent.angularSpeed = 150;
        }
        else
        {
            enemyAttack.SetAttackDamage(damage);
        }
        healthSystem.SetMaxHealth(health);
        transform.localScale = scale;
        enemyAI.agent.speed*= speedBoost;
        enemyAI.agent.angularSpeed*= speedBoost;
        enemyAI.visionRange*= 5;
        enemyAI.engagementRange *= 2;
        enemyAI.boss = true;
    }
}
