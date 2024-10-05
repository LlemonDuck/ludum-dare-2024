using UnityEngine;

public class Toothpick : BaseWeapon {
    [HideInInspector]
    Vector2 stabDirection = Vector2.zero;
    [HideInInspector]
    Vector2 initialOffset = Vector2.zero;

    public float moveDistance = 500.0f;

    public override void OnAttack() {
        if (!isOnCooldown()) {
            initialOffset = PlayerController.instance.transform.position - transform.position;
            stabDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - PlayerController.instance.transform.position).normalized;
            lastAttackTime = Time.time;
        }
    }

    void Update() {
        Vector2 attachedPos = (Vector2)PlayerController.instance.transform.position + initialOffset;
        if (isAttacking()) {
            transform.position = Vector2.Lerp(attachedPos, attachedPos + stabDirection * moveDistance, (Time.time - lastAttackTime)/attackDuration);
        } else if (isOnCooldown()) {
            transform.position = Vector2.Lerp(attachedPos + stabDirection * moveDistance, attachedPos, (Time.time - attackDuration - lastAttackTime)/attackCooldown);
        } else if (wasOnCooldownLastFrame()) {
            transform.position = attachedPos;
        }
    }

    public override void OnCollideWithEnemy(BaseEnemy enemy) {
        if (isAttacking()) {
            enemy.applyDamage(damageAmount);
        }
    }
}