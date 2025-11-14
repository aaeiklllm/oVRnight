using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMo : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        //Slow down to 50% speed
        animator.speed = 0.5f;
    }
}
