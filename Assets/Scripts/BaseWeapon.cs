using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour {
    [HideInInspector]
    protected float lastAttackTime = -100000.0f;

    protected float attackDuration = 0.2f;

    protected float attackCooldown = 0.6f;

    protected float damageAmount = 10.0f;

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

    public virtual void OnCollideWithEnemy(BaseEnemy enemy) {

    }

    void OnCollisionEnter2D(Collision2D collision) {
        collision.gameObject.TryGetComponent(out BaseEnemy enemy);
        if (enemy != null) {
            OnCollideWithEnemy(enemy);
        }
    }
}
