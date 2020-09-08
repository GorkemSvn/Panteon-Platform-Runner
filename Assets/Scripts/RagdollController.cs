using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public Animator animator;
    public List<Rigidbody> bodyparts;

    public void Start()
    {
        SetRagDollActive(false);
    }

    public void SetRagDollActive(bool active)
    {
        animator.enabled = !active;
        foreach(Rigidbody rig in bodyparts)
        {
            rig.isKinematic = !active;
            rig.GetComponent<Collider>().enabled = active;
        }
    }
}
