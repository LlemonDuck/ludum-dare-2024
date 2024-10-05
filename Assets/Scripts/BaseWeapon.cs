using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour {
    [HideInInspector]
    protected float lastAttackTime = -100000.0f;

    protected float attackDuration = 0.2f;

    protected float attackCooldown = 0.6f;

    public float damageAmount = 10.0f;

    protected bool isOnCooldown() {
        return Time.time - lastAttackTime <= attackCooldown;
    }

    protected bool wasOnCooldownLastFrame() {
        return Time.time - Time.deltaTime - lastAttackTime <= attackCooldown;
    }

    protected bool isAttacking() {
        return Time.time - lastAttackTime <= attackDuration;
    }

    public virtual void OnAttack() {

    }

    public virtual void OnRemoved() {

    }

    public virtual void onCollideWithEnemy(BaseEnemy enemy) {

    }

    public virtual void Update() {
    }

    void tryCollideEnemy(Collider2D collider) {
        collider.gameObject.TryGetComponent(out BaseEnemy enemy);

        if (enemy != null) {
            onCollideWithEnemy(enemy);
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        tryCollideEnemy(collider);
    }

    void OnTriggerStay2D(Collider2D collider) {
        tryCollideEnemy(collider);
    }

    public void attachToPlayer() {
        transform.SetParent(PlayerController.instance.weaponSocket.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    protected virtual IEnumerator addFreezeFrames(float time) {
        Debug.Log("Starting freeze frames");

        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1.0f;

        Debug.Log("Ending freeze frames");
    }
}
