using UnityEngine;
using static EnemyAI;

public class Enemy : MonoBehaviour
{
    public EnemyAI enemyAI;
    public EnemyType enemyType;
    public EnemyHealthSystem healthSystem;
    public Billboard enemyUI;
}
