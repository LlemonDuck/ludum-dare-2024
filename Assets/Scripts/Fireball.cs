
using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour {
    public SpriteRenderer sprite;

    public new CircleCollider2D collider;

    public new Rigidbody2D rigidbody;

    public bool active = false;

    public float lifeCycle = 5.0f;

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

        deactivateCoroutine = DeactivateAfterSeconds(lifeCycle);
        StartCoroutine(deactivateCoroutine);
    }

    // TODO: Destroy when it hits non enemy non player objects
    // TODO: Damage player on hit.
    // TODO: Light branches on fire? 
    // TODO: come up with more ideas :) 
}