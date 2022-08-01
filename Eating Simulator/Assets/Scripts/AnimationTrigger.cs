using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    Animator animator;

    
    void Start()
    {
        animator = GetComponentInParent<Animator>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            animator.SetBool("PlayerNearby", true);
    }
}
