using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {
    public new Rigidbody2D rigidbody;

    public float moveSpeed = 10.0f;

    protected bool isCollidingWithPlayer = false;

    public float health = 100;

    protected float timeSinceDamage = 10.0f;

    public float flashTime = 1.0f;

    public float knockbackDampingFactor = 1.0f;

    public Color damageColor = new Color(0.7f, 0.1f, 0.1f);

    public Collider2D damageCollider;
    
    public bool alive = true;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    protected virtual void Update() {
        if (!alive) return;
        timeSinceDamage += Time.deltaTime;
        TryGetComponent(out SpriteRenderer sprite);

        if (sprite != null && timeSinceDamage <= flashTime) {
            sprite.color = Color.Lerp(damageColor, Color.white, timeSinceDamage/flashTime);
        } else if (timeSinceDamage - Time.deltaTime <= flashTime) {
            sprite.color = Color.white;
        }
    }

    protected Vector2 getPlayerOffset() {
        return PlayerController.instance.transform.position - transform.position;
    }

    protected void moveToPlayer() {
        if (!alive) return;
        Vector2 moveDirection = getPlayerOffset().normalized;

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
    
    protected void moveFromPlayer() {
        if (!alive) return;
        Vector2 moveDirection = -getPlayerOffset().normalized;

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

    protected virtual void OnCollisionNotPlayer(Collision2D collision) {

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject == PlayerController.instance.gameObject) {
            isCollidingWithPlayer = true;
        } else {
            OnCollisionNotPlayer(collision);
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject == PlayerController.instance.gameObject) {
            isCollidingWithPlayer = false;
        }
    }

    public virtual void applyDamage(float damage) {
        if (!alive || timeSinceDamage < 1.0f) return;
        health -= damage;

        if (health <= 0) {
            OnKilled();
        }
        
        timeSinceDamage = 0.0f;
    }

    public virtual void applyDamage(float damage, Vector2 direction, float intensity) {
        if (!alive || timeSinceDamage < 1.0f) return;
        applyDamage(damage);
        rigidbody.velocity = Vector2.zero;

        rigidbody.AddForce(direction * intensity / knockbackDampingFactor, ForceMode2D.Impulse);
    }

    public virtual void OnKilled() {
        alive = false;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = new Color(0.3f, 0, 0, 1);
        rigidbody.isKinematic = true;
        rigidbody.velocity = Vector2.zero;
    }
}
