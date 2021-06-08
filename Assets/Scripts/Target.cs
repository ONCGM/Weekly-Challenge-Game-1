using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
    [SerializeField, Range(0, 12)] private int targetId;
    
    public static Action<int> onBulletHit;
    
    private void OnCollisionEnter(Collision other) {
        if(!other.gameObject.GetComponent<Bullet>()) return;
        
        other.gameObject.GetComponent<Bullet>()?.StopBullet();
        onBulletHit?.Invoke(targetId);
        Destroy(gameObject);
    }
}
