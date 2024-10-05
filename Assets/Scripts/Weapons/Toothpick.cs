using UnityEngine;

public class Toothpick : BaseWeapon {



    public override void OnAttack() {
        if (!isOnCooldown()) {
            lastAttackTime = Time.time;
        }
    }

    void Update() {
    }

    public override void OnCollideWithEnemy(BaseEnemy enemy) {
        if (isAttacking()) {
            enemy.applyDamage(damageAmount);
        }
    }
}