using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {
    public new Rigidbody2D rigidbody;

    public float moveSpeed = 10.0f;

    private bool isCollidingWithPlayer = false;

    public float health = 100;

    protected float timeSinceDamage = 0.0f;

    public float flashTime = 1.0f;

    public Color damageColor = new Color(0.7f, 0.1f, 0.1f);

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    protected virtual void Update() {
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

    void OnCollisionEnter2D(Collision2D collision) {
        isCollidingWithPlayer = collision.gameObject == PlayerController.instance.gameObject;
    }

    void OnCollisionExit2D(Collision2D collision) {
        isCollidingWithPlayer = !(collision.gameObject == PlayerController.instance.gameObject);
    }

    public virtual void applyDamage(float damage) {
        health -= damage;

        if (health <= 0) {
            GameObject.Destroy(gameObject);
        }
    }

    public virtual void applyDamage(float damage, Vector2 direction, float intensity) {
        timeSinceDamage = 0.0f;
        applyDamage(damage);
        rigidbody.velocity = Vector2.zero;

        rigidbody.AddForce(direction * intensity, ForceMode2D.Impulse);
    }
}
