using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkerAnt : BaseEnemy {
    public float attackDistance = 0.0f;
    public float lungeDecisionDistance = 4.0f;
    public float lungeDistance = 7.0f;
    public float lungeChargeTime = 0.1f;
    public float lungeDuration = 0.3f;
    public float lungeCooldown = 0.8f;

    public float aggroDistance = 100.0f;
    public float unaggroDistance = 200.0f;

    public BehaviourState currentState = BehaviourState.PATROL;

    public float lungeTimer = 0.0f;

    private Vector2 lungeDirection = Vector2.zero;

    public enum BehaviourState {
        MOVING_TO_PLAYER,
        ATTACKING_PLAYER,
        PATROL,
    }

    void switchState(BehaviourState newState) {
        Debug.LogWarning(newState);

        switch (newState) {
            case BehaviourState.ATTACKING_PLAYER:
                rigidbody.velocity = Vector2.zero;
                lungeTimer = 0.0f;
                goto default;
            default:
                currentState = newState;
                break;
        }
    }

    public override void applyDamage(float damage) {
        base.applyDamage(damage);
        switchState(BehaviourState.MOVING_TO_PLAYER);
    }

    void moveToPlayerState(Vector2 playerDirection) {
        moveToPlayer();

        float distance = playerDirection.magnitude;
        if (distance < lungeDecisionDistance && timeSinceDamage > 0.3f) {
            switchState(BehaviourState.ATTACKING_PLAYER);
        } else if (distance > unaggroDistance) {
            switchState(BehaviourState.PATROL);
        }
    }

    void patrolState(Vector2 playerDirection) {
        if (playerDirection.magnitude < aggroDistance) {
            switchState(BehaviourState.MOVING_TO_PLAYER);
        } else {

        }
    }

    void attackState(Vector2 playerDirection) {
        if (lungeTimer == 0.0f) {
            lungeDirection = playerDirection.normalized;
        } else if (lungeTimer >= lungeChargeTime + lungeCooldown + lungeDuration) {
            switchState(BehaviourState.MOVING_TO_PLAYER);
        }

        lungeTimer += Time.deltaTime;
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        Vector2 playerDirection = getPlayerOffset();

        switch (currentState) {
            case BehaviourState.MOVING_TO_PLAYER:
                moveToPlayerState(playerDirection);
                break;
            case BehaviourState.ATTACKING_PLAYER:
                attackState(playerDirection);
                break;
            case BehaviourState.PATROL:
                patrolState(playerDirection);
                break;
        }
    }
}
