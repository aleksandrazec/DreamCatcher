using System.Collections;
using UnityEngine;

public class DressAnimationEventCaller : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Collider hitCollider;
    [SerializeField] private GameObject laser;
    [SerializeField] private float rotationTime;
    private void Spin()
    {
        hitCollider.enabled = true;
        laser.SetActive(true);
        StartCoroutine(Rotate(rotationTime));
    }
    private IEnumerator Rotate(float duration)
    {
        Quaternion startRot = transform.rotation;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            rigidBody.MoveRotation(startRot * Quaternion.AngleAxis(t / duration * 360f, Vector3.up));
            yield return null;
        }
        rigidBody.MoveRotation(startRot);
        laser.SetActive(false);
        animator.SetBool("isAttacking", false);
    }
    private void EndDamage()
    {
        animator.SetBool("isDamaged", false);
    }
    private void EndDead()
    {
        animator.SetBool("isDead", false);
    }
}
