using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private PlayerController controller;
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log(health);
        controller.TakeDamage();
    }
}
