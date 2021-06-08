using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    [SerializeField, Range(0, 12)] private int doorId;

    private Animator anim;
    private static readonly int OpenDoor = Animator.StringToHash("Open");

    private void Awake() {
        anim = GetComponent<Animator>();
        Target.onBulletHit += Open;
    }

    private void Open(int id) {
        if(id != doorId) return;
        
        anim.SetTrigger(OpenDoor);
    }
}
