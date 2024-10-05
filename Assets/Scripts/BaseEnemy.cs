using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {
    public new Rigidbody2D rigidbody;

    public float moveSpeed = 10.0f;

    private bool isCollidingWithPlayer = false;

    public float health = 100;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        Vector2 moveDirection = (PlayerController.instance.transform.position - transform.position).normalized;

        if (!isCollidingWithPlayer) {
            rigidbody.velocity = moveDirection * moveSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        isCollidingWithPlayer = collision.gameObject == PlayerController.instance.gameObject;
    }

    void OnCollisionExit2D(Collision2D collision) {
        isCollidingWithPlayer = !(collision.gameObject == PlayerController.instance.gameObject);
    }

    public void applyDamage(float damage) {
        health -= damage;
    }
}
