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

    public float gcdTime = 0.3f;
    public float gcdTimer = 0.0f;


    private float circlingTimer = 1000.0f;
    public float circlingCooldown = 20.0f;
    public float circlingTime = 7.0f;
    public float desiredCircleDistance = 8.0f;

    public float moveAwayMinTime = 0.8f;
    private float moveAwayTimer = 0.0f;

    private Vector2 lungeDirection = Vector2.zero;
    private Vector2 lungeStartPosition = Vector2.zero;

    public GameObject firePrefab;

    private GameObject[] fireObjects = null;

    public AnimationCurve lungeCurve;

    public enum BehaviourState {
        MOVING_TO_PLAYER,
        MOVING_FROM_PLAYER,
        LUNGING_PLAYER,
        FIREBALL_PLAYER,
        CIRCLE_AND_FIRE,
        PATROL,
        STAND_STILL,
    }

    void Awake() {
        fireObjects = new GameObject[maxFireCount];
        StartCoroutine("spawnFireBalls");
    }

    IEnumerator spawnFireBalls() {
        AsyncInstantiateOperation<GameObject> fireballs = InstantiateAsync(firePrefab, maxFireCount);

        while (!fireballs.isDone) {
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < maxFireCount; i++) {
            fireObjects[i] = fireballs.Result[i];
        }
    }

    void switchState(BehaviourState newState) {
        Debug.Log(newState);

        int random = Random.Range(0, 5);

        switch (newState) {
            case BehaviourState.FIREBALL_PLAYER:
                if (random == 0) {
                    goto CircleFire;
                }

                if (timeSinceFire <= fireCooldown) {
                    switchState(BehaviourState.MOVING_TO_PLAYER);
                    return;
                }
                rigidbody.velocity = Vector2.zero;
                timeSinceFire = 0.0f;
                goto default;
            case BehaviourState.LUNGING_PLAYER:
                if (random == 0) {
                    goto CircleFire;
                }

                if (lungeTimer <= lungeCooldown) {
                    switchState(BehaviourState.MOVING_FROM_PLAYER);
                    return;
                }
                rigidbody.velocity = Vector2.zero;
                lungeTimer = 0.0f;
                goto default;
            case BehaviourState.STAND_STILL:
                gcdTimer = 0.0f;
                goto default;
            case BehaviourState.MOVING_FROM_PLAYER:
                moveAwayTimer = 0.0f;
                goto default;
            case BehaviourState.CIRCLE_AND_FIRE: 
            CircleFire:
                Debug.Log(BehaviourState.CIRCLE_AND_FIRE);
                if (circlingTimer <= circlingCooldown) {
                    switchState(BehaviourState.MOVING_TO_PLAYER);
                    return;
                }
                currentState = BehaviourState.CIRCLE_AND_FIRE;
                circlingTimer = 0.0f;
                break;
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

    void moveFromPlayerState(Vector2 playerDirection) {
        moveFromPlayer();

        if (timeSinceDamage < 0.1f) {
            switchState(BehaviourState.MOVING_TO_PLAYER);
        }

        float distance = playerDirection.magnitude;
        if (moveAwayTimer >= moveAwayMinTime) {
            if (distance > fireMinDistance + 0.2f && timeSinceFire >= fireCooldown) {
                switchState(BehaviourState.FIREBALL_PLAYER);
            } else if (distance < lungeDecisionDistance && lungeTimer >= lungeCooldown) {
                switchState(BehaviourState.LUNGING_PLAYER);
            }
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
            shootFireball(playerDirection);
        }
        switchState(BehaviourState.MOVING_TO_PLAYER);
        return;
    }

    void shootFireball(Vector2 playerDirection) {
        GameObject fireObj = fireObjects[fireCount % maxFireCount];
        if (fireObj != null) {
            fireObj.TryGetComponent(out Fireball fireball);
            if (!fireball.active) {
                fireball.Activate(transform.position, playerDirection.normalized, fireSpeed);
                fireCount ++;
            }
        }
    }

    void attackState(Vector2 playerDirection) {
        if (lungeTimer == 0.0f) {
            lungeDirection = playerDirection.normalized;
            lungeStartPosition = transform.position;
        } else if (lungeTimer >= lungeDuration) {
            switchState(BehaviourState.STAND_STILL);
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

    void circleState(Vector2 playerDirection) {
        Vector2 clockwiseDirection = new Vector2(playerDirection.y, -playerDirection.x);

        Vector2 movementDirection = (playerDirection * (playerDirection.magnitude - desiredCircleDistance)) + clockwiseDirection;

        if (timeSinceDamage <= 0.1f) {
            switchState(BehaviourState.MOVING_TO_PLAYER);
        }

        movementDirection = movementDirection.normalized * moveSpeed * 1.5f;

        rigidbody.velocity = movementDirection;

        if (circlingTimer >= circlingTime) {
            switchState(BehaviourState.MOVING_TO_PLAYER);
        }

        if (timeSinceFire >= fireCooldown/2) {
            timeSinceFire = 0.0f;
            shootFireball(playerDirection);
        }
    }

    // Start is called before the first frame update
    void Start() {
    }

    void applyState() {
        Vector2 playerDirection = getPlayerOffset();
        
        if (currentState == BehaviourState.LUNGING_PLAYER || currentState == BehaviourState.STAND_STILL) {
            var angle = Mathf.Atan2(lungeDirection.y, lungeDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        } else {
            var angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        switch (currentState) {
            case BehaviourState.MOVING_TO_PLAYER:
                moveToPlayerState(playerDirection);
                break;
            case BehaviourState.LUNGING_PLAYER:
                attackState(playerDirection);
                break;
            case BehaviourState.MOVING_FROM_PLAYER:
                moveFromPlayerState(playerDirection);
                break;
            case BehaviourState.FIREBALL_PLAYER:
                fireballState(playerDirection);
                break;
            case BehaviourState.STAND_STILL:
                if(gcdTimer >= gcdTime) {
                    switchState(BehaviourState.MOVING_TO_PLAYER);
                }
                break;
            case BehaviourState.CIRCLE_AND_FIRE:
                circleState(playerDirection);
                break;
            case BehaviourState.PATROL:
                patrolState(playerDirection);
                break;
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    
        BehaviourState stateBeforeUpdate;
        BehaviourState stateAfterUpdate;

        do {
            stateBeforeUpdate = currentState;

            applyState();

            stateAfterUpdate = currentState;
        } while (stateBeforeUpdate != stateAfterUpdate);

        lungeTimer += Time.deltaTime;
        timeSinceFire += Time.deltaTime;
        gcdTimer += Time.deltaTime;
        moveAwayTimer += Time.deltaTime;
        circlingTimer += Time.deltaTime;
    }


    void OnDestroy() {
        foreach (GameObject fireball in fireObjects) {
            Destroy(fireball);
        }
    }
}
