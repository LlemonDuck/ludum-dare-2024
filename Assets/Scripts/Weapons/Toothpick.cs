using UnityEngine;

public class Toothpick : BaseWeapon {
    [HideInInspector]
    Vector2 stabDirection = Vector2.zero;

    public float moveDistance = 500.0f;

    public override void OnAttack() {
        if (!isOnCooldown()) {
            stabDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - PlayerController.instance.transform.position).normalized;
            lastAttackTime = Time.time;
        }
    }

    void Update() {
        if (!isAttacking() && !isOnCooldown()) {
            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

            transform.rotation = Quaternion.Euler(0, 0, rotation);
        }

        if (isAttacking()) {
            transform.localPosition = Vector2.Lerp(Vector2.zero, stabDirection * moveDistance, (Time.time - lastAttackTime)/attackDuration);
        } else if (isOnCooldown()) {
            transform.localPosition = Vector2.Lerp(stabDirection * moveDistance, Vector2.zero, (Time.time - attackDuration - lastAttackTime)/attackCooldown);
        } else if (wasOnCooldownLastFrame()) {
            transform.localPosition = Vector2.zero;
        }
    }

    public override void OnCollideWithEnemy(BaseEnemy enemy) {
        if (isAttacking()) {
            enemy.applyDamage(damageAmount);
        }
    }
}