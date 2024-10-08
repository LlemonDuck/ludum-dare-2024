using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkerAnt : BaseEnemy {
    public float lungeDamage = 20.0f;
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
    private Vector2 lungeStartPosition = Vector2.zero;

    public AnimationCurve lungeCurve;

    public enum BehaviourState {
        MOVING_TO_PLAYER,
        ATTACKING_PLAYER,
        PATROL,
    }

    void switchState(BehaviourState newState) {
        switch (newState) {
            case BehaviourState.ATTACKING_PLAYER:
                if (lungeTimer <= lungeCooldown) {
                    switchState(BehaviourState.MOVING_TO_PLAYER);
                    return;
                }
                rigidbody.velocity = Vector2.zero;
                lungeTimer = 0.0f;
                goto default;
            default:
                currentState = newState;
                break;
        }
    }

    public override void applyDamage(float damage) {
        switchState(BehaviourState.MOVING_TO_PLAYER);
        base.applyDamage(damage);
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
            // TODO: ADD AN ACTUAL PATROL STATE
        }
    }

    void attackState(Vector2 playerDirection) {
        if (lungeTimer == 0.0f) {
            lungeDirection = playerDirection.normalized;
            lungeStartPosition = transform.position;
        } else if (lungeTimer >= lungeDuration + lungeChargeTime) {
            switchState(BehaviourState.MOVING_TO_PLAYER);
        }

        float lungeCurveResult = lungeCurve.Evaluate(lungeTimer/(lungeDuration + lungeChargeTime));
        if (lungeCurveResult < 0) {
            rigidbody.position = Vector2.Lerp(lungeStartPosition, lungeStartPosition - lungeDirection * lungeDistance, Mathf.Abs(lungeCurveResult));
        } else {
            rigidbody.position = Vector2.Lerp(lungeStartPosition, lungeStartPosition + lungeDirection * lungeDistance, lungeCurveResult);
        }

        if (lungeTimer >= lungeChargeTime && lungeTimer <= lungeCooldown && isCollidingWithPlayer) {
            PlayerController.instance.applyDamage(lungeDamage);
        }
    }

    // Start is called before the first frame update
    void Start() {
    }

    void applyState() {
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

    // Update is called once per frame
    protected override void Update() {
        if (!alive) return;
        base.Update();

        BehaviourState stateBeforeUpdate = currentState;
        BehaviourState stateAfterUpdate = currentState;

        do {
            stateBeforeUpdate = currentState;

            applyState();

            stateAfterUpdate = currentState;
        } while (stateBeforeUpdate != stateAfterUpdate);

        lungeTimer += Time.deltaTime;
    }

    protected override void OnCollisionNotPlayer(Collision2D collision) {
        if (currentState == BehaviourState.ATTACKING_PLAYER) {
            switchState(BehaviourState.MOVING_TO_PLAYER);
        }
    }

    public void Revive () {
        GetComponent<BoxCollider2D>().enabled = true;
        health = 100;
        alive = true;
        rigidbody.isKinematic = false;
        GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<SpriteRenderer>().enabled = true;
    }
}
