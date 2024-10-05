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
            Vector2 maxSpeed = moveDirection * moveSpeed;

            rigidbody.velocity += maxSpeed * Time.deltaTime/0.1f;

            if (Mathf.Abs(Vector2.Dot(rigidbody.velocity.normalized, maxSpeed.normalized) - 1) < 0.2f) {
                if (rigidbody.velocity.magnitude >= maxSpeed.magnitude) {
                    rigidbody.velocity = maxSpeed;
                }
            }
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

        if (health <= 0) {
            GameObject.Destroy(gameObject);
        }
    }

    public void applyDamage(float damage, Vector2 direction, float intensity) {
        applyDamage(damage);

        rigidbody.AddForce(direction * intensity, ForceMode2D.Impulse);
    }
}
