using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Target : MonoBehaviour {
    [Header("Settings")]
    [SerializeField, Range(0, 12)] private int targetId;
    [SerializeField] private bool animate;
    [SerializeField, Range(0.1f, 10f)] private float animationTime = 3f;
    [SerializeField] private Vector3 movementAmount = new Vector3();
    
    public static Action<int> onBulletHit;

    private void Start() {
        if(!animate) return;

        AnimateRight();
    }

    private void AnimateRight() {
        transform.DOMove(transform.position + movementAmount, animationTime).SetEase(Ease.Linear).onComplete = AnimateLeft;
    }
    
    private void AnimateLeft() {
        transform.DOMove(transform.position - movementAmount, animationTime).SetEase(Ease.Linear).onComplete = AnimateRight;
    }

    private void OnCollisionEnter(Collision other) {
        if(!other.gameObject.GetComponent<Bullet>()) return;
        
        other.gameObject.GetComponent<Bullet>()?.StopBullet();
        
        onBulletHit?.Invoke(targetId);
        Destroy(gameObject);
    }
}
