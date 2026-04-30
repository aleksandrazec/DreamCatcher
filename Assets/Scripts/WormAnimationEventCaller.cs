using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WormAnimationEventCaller : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Collider hitCollider;
    [SerializeField] private float slideSpeed=60;
    [SerializeField] private float slideTime = 2;
    private bool isSliding=false;

    
    private void Update()
    {
        if (isSliding)
        {
            Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z);
            Vector3 dashTo = transform.position + transform.forward;
            NavMeshHit hit;
            NavMesh.SamplePosition(dashTo, out hit, 20f, 1);
            Vector3 trueDashTo=hit.position* slideSpeed * Time.deltaTime;
            rigidBody.MovePosition(trueDashTo);
        }
    }
    private void StartSlide()
    {
        isSliding = true;
        hitCollider.enabled = true;
        StartCoroutine(SlideTimer());
    }
    private IEnumerator SlideTimer()
    {
        yield return new WaitForSeconds(slideTime);
        isSliding = false;
        hitCollider.enabled=false;
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
