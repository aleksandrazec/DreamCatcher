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
            transform.position += new Vector3(0, -30, 0);
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
