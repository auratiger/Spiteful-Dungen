using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    private Animator myAnimator;
    private static readonly int Rise = Animator.StringToHash("Rise");

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    public void OpenGate()
    {
        myAnimator.SetBool(Rise, true);
    }

    public void CloseGate()
    {
        myAnimator.SetBool(Rise, false);
    }
}
