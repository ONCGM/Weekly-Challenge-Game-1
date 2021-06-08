using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour {
    [Header("Physics Settings")]
    [SerializeField, Range(10f, 1000f)] private float bulletSpeed = 42f;
    [SerializeField, Range(0, 10)] private int maxBounces = 5;
    private int currentBounces;
    
    [Header("Animation Settings")]
    [SerializeField, Range(0.01f, 1f)] private float growAnimationLenght = 0.3f;
    [SerializeField] private Vector3 initialSize = new Vector3(0.03f, 0.03f, 0.03f);
    [SerializeField] private Vector3 finalSize = new Vector3(0.25f, 0.25f, 0.25f);
    
    // Components
    private Rigidbody rb;

    // Animation and Physics.
    private void Awake() {
        transform.localScale = initialSize;
        transform.DOScale(finalSize, growAnimationLenght);
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
        Invoke(nameof(StopBullet), 15f);
    }

    // Rotates the bullet towards moving 
    private void FixedUpdate() {
        if(rb.isKinematic) return;
        transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    /// <summary>
    /// Destroys the bullet.
    /// </summary>
    public void StopBullet() {
        rb.isKinematic = true;
        foreach(var mesh in GetComponentsInChildren<MeshRenderer>()) {
            mesh.enabled = false;
        }
        Destroy(gameObject, 2f);
    }
    
    private void OnCollisionEnter(Collision other) {
        currentBounces++;
        if(currentBounces >= maxBounces) StopBullet();
    }
}
