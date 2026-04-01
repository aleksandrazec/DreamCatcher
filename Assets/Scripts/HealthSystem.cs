using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private string typeOfObject;
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log(typeOfObject + health);
    }
}
