
using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour {
    public SpriteRenderer sprite;

    public new CircleCollider2D collider;

    public new Rigidbody2D rigidbody;

    public bool active = false;

    public float lifeCycle = 5.0f;

    public float playerDamage = 5.0f;

    IEnumerator deactivateCoroutine;

    void Deactivate() {
        active = false;
        sprite.enabled = false;
        collider.enabled = false;
    }

    void Awake() {
        Deactivate();
    }

    public IEnumerator DeactivateAfterSeconds(float time) {
        yield return new WaitForSeconds(time);
        Deactivate();
        deactivateCoroutine = null;
    }

    public void Activate(Vector2 spawnPosition, Vector2 direction, float speed) {
        transform.position = spawnPosition;
        collider.enabled = true;
        sprite.enabled = true;
        active = true;
        rigidbody.velocity = direction * speed;
        
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);

        deactivateCoroutine = DeactivateAfterSeconds(lifeCycle);
        StartCoroutine(deactivateCoroutine);
    }

    // TODO: Light branches on fire? 
    // TODO: come up with more ideas :) 

    void OnCollisionEnter2D(Collision2D collision) {
        if (!active) return;
        collision.gameObject.TryGetComponent(out PlayerController player);
        collision.gameObject.TryGetComponent(out BaseEnemy enemy);

        if (player != null) {
            player.applyDamage(playerDamage);
        }

        if (enemy == null && player == null) {
            Deactivate();
        } 
    }
}