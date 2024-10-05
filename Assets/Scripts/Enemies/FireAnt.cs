using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireAnt : BaseEnemy {
    public float lungeDamage = 20.0f;
    public float attackDistance = 0.0f;
    public float lungeDecisionDistance = 5.0f;
    public float lungeDistance = 14.0f;
    public float lungeChargeTime = 0.1f;
    public float lungeDuration = 0.3f;
    public float lungeCooldown = 0.8f;

    public float fireSpeed = 15.0f;
    private float timeSinceFire = 0.0f;
    public float fireDistance = 20.0f;
    public float fireMinDistance = 7.0f;
    public float fireCooldown = 0.4f;
    private int fireCount = 0;
    public int maxFireCount = 10;

    public float aggroDistance = 100.0f;
    public float unaggroDistance = 200.0f;

    public BehaviourState currentState = BehaviourState.PATROL;

    public float lungeTimer = 0.0f;

    private Vector2 lungeDirection = Vector2.zero;
    private Vector2 lungeStartPosition = Vector2.zero;

    public GameObject firePrefab;

    private GameObject[] fireObjects = null;

    public AnimationCurve lungeCurve;

    public enum BehaviourState {
        MOVING_TO_PLAYER,
        LUNGING_PLAYER,
        FIREBALL_PLAYER,
        PATROL,
    }

    void Awake() {
        fireObjects = new GameObject[maxFireCount];
        for(int i = 0; i < maxFireCount; i++) {
            fireObjects[i] = Instantiate(firePrefab);
        }
    }

    void switchState(BehaviourState newState) {
        switch (newState) {
            case BehaviourState.FIREBALL_PLAYER:
                if (timeSinceFire <= fireCooldown) {
                    switchState(BehaviourState.MOVING_TO_PLAYER);
                    return;
                }
                rigidbody.velocity = Vector2.zero;
                timeSinceFire = 0.0f;
                goto default;
            case BehaviourState.LUNGING_PLAYER:
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
            switchState(BehaviourState.LUNGING_PLAYER);
        } else if (distance < fireDistance && distance > fireMinDistance && timeSinceDamage > 0.3f) { // TODO: Add fireball behaviour
            switchState(BehaviourState.FIREBALL_PLAYER);
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

    void fireballState(Vector2 playerDirection) {
        if (timeSinceFire == 0.0f) {
            fireObjects[fireCount % maxFireCount].TryGetComponent(out Fireball fireball);
            if (!fireball.active) {
                fireball.Activate(transform.position, playerDirection.normalized, fireSpeed);
                fireCount ++;
            }
        }
        switchState(BehaviourState.MOVING_TO_PLAYER);
        return;
    }

    void attackState(Vector2 playerDirection) {
        if (lungeTimer == 0.0f) {
            lungeDirection = playerDirection.normalized;
            lungeStartPosition = transform.position;
        } else if (lungeTimer >= lungeChargeTime + lungeCooldown + lungeDuration) {
            switchState(BehaviourState.MOVING_TO_PLAYER);
        }

        float lungeCurveResult = lungeCurve.Evaluate(lungeTimer);
        if (lungeCurveResult < 0) {
            rigidbody.position = Vector2.Lerp(lungeStartPosition, lungeStartPosition - lungeDirection * lungeDistance, Mathf.Abs(lungeCurveResult));
        } else {
            rigidbody.position = Vector2.Lerp(lungeStartPosition, lungeStartPosition + lungeDirection * lungeDistance, lungeCurveResult);
        }

        if (lungeTimer >= lungeChargeTime && lungeTimer <= lungeChargeTime + lungeCooldown + lungeDuration && isCollidingWithPlayer) {
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
            case BehaviourState.LUNGING_PLAYER:
                attackState(playerDirection);
                break;
            case BehaviourState.FIREBALL_PLAYER:
                fireballState(playerDirection);
                break;
            case BehaviourState.PATROL:
                patrolState(playerDirection);
                break;
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    
        BehaviourState stateBeforeUpdate = currentState;
        BehaviourState stateAfterUpdate = currentState;

        do {
            stateBeforeUpdate = currentState;

            applyState();

            stateAfterUpdate = currentState;
        } while (stateBeforeUpdate != stateAfterUpdate);

        lungeTimer += Time.deltaTime;
        timeSinceFire += Time.deltaTime;
    }
}
