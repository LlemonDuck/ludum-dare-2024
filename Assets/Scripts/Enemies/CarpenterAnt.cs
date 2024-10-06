using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarpenterAnt : BaseEnemy {
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

    private bool isCollidingWithWood = false;

    private float woodCheckTimer = 0.0f;
    public float woodCheckFrequency = 5.0f;
    private float woodChopTimer = 0.0f;
    public float woodChopDuration = 2.0f;
    public float woodThrowDuration = 5.0f;
    public float woodThrowDistance = 50.0f;
    private float woodThrowTimer = 0.0f;
    private Vector2 woodThrowDirection = Vector2.zero;

    private Vector2 lungeDirection = Vector2.zero;
    private Vector2 lungeStartPosition = Vector2.zero;

    private List<GameObject> woodObjects = new List<GameObject>();

    public GameObject woodAttachPoint;
    private GameObject throwingWood;

    public AnimationCurve lungeCurve;

    public enum BehaviourState {
        MOVING_TO_PLAYER,
        ATTACKING_PLAYER,
        CHOPPING_WOOD,
        THROWING_WOOD,
        PATROL,
    }

    void switchState(BehaviourState newState) {
        Debug.Log(newState);

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

        if (lungeTimer >= lungeChargeTime && lungeTimer <= lungeDuration && isCollidingWithPlayer) {
            PlayerController.instance.applyDamage(lungeDamage);
        }
    }

    void moveToWood(Vector2 woodPosition) {
        Vector2 moveDirection = (woodPosition - (Vector2)transform.position).normalized;

        if (!isCollidingWithPlayer) {
            Vector2 maxSpeed = moveDirection * moveSpeed;

            rigidbody.velocity += maxSpeed * Time.deltaTime/0.1f;

            if (Mathf.Abs(Vector2.Dot(rigidbody.velocity.normalized, maxSpeed.normalized) - 1) < 0.2f) {
                if (rigidbody.velocity.magnitude >= maxSpeed.magnitude) {
                    rigidbody.velocity = maxSpeed;
                }
            }
        }
    }

    void attachWood(GameObject wood) {
        wood.transform.parent = woodAttachPoint.transform;
        woodThrowTimer = 0.0f;
        throwingWood = wood;
        woodThrowDirection = getPlayerOffset().normalized;
        throwingWood.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    void chopWood(GameObject wood) {
        woodChopTimer += Time.deltaTime;

        if (woodCheckTimer >= woodChopDuration) {
            switchState(BehaviourState.THROWING_WOOD);
            attachWood(wood);
        }
    }

    void choppingWoodState() {
        GameObject nearestWood = null;
        float nearestDistance = 10000000f;

        foreach(GameObject wood in woodObjects) {
            float woodDistance = Vector2.Distance(transform.position, wood.transform.position);

            if (woodDistance < nearestDistance) {
                nearestWood = wood;
                nearestDistance = woodDistance;
            }
        }

        if (nearestWood == null || timeSinceDamage < 0.1f) switchState(BehaviourState.MOVING_TO_PLAYER);
        
        if (!isCollidingWithWood) moveToWood(nearestWood.transform.position);
        if (isCollidingWithWood) chopWood(nearestWood);
    }

    void throwingWoodState() {
        rigidbody.velocity = Vector2.zero;
        throwingWood.transform.rotation = Quaternion.identity;
        transform.rotation = Quaternion.identity;

        if (woodThrowTimer >= woodThrowDuration) {
            switchState(BehaviourState.MOVING_TO_PLAYER);
            woodCheckTimer = 0.0f;
        }

        float woodThrowResult = lungeCurve.Evaluate(woodThrowTimer/woodThrowDuration);
        if (woodThrowResult < 0) {
            throwingWood.transform.position = Vector2.Lerp(woodAttachPoint.transform.position, (Vector2)woodAttachPoint.transform.position - woodThrowDirection * woodThrowDistance, Mathf.Abs(woodThrowResult));
        } else {
            throwingWood.transform.position = Vector2.Lerp(woodAttachPoint.transform.position, (Vector2)woodAttachPoint.transform.position + woodThrowDirection * woodThrowDistance, woodThrowResult);
        }

        if (woodThrowTimer >= woodThrowDuration) {
            switchState(BehaviourState.MOVING_TO_PLAYER);
        }
    }

    // Start is called before the first frame update
    void Start() {
    }

    void applyState() {
        Vector2 playerDirection = getPlayerOffset();

        if (currentState != BehaviourState.CHOPPING_WOOD && currentState != BehaviourState.THROWING_WOOD) {
            if (woodCheckTimer >= woodCheckFrequency) {
                if (Random.Range(0.0f, 1.0f) <= 0.4f && woodObjects.Count > 0) {
                    switchState(BehaviourState.CHOPPING_WOOD);
                }

                woodCheckTimer = 0.0f;
            }
        }

        switch (currentState) {
            case BehaviourState.MOVING_TO_PLAYER:
                moveToPlayerState(playerDirection);
                break;
            case BehaviourState.ATTACKING_PLAYER:
                attackState(playerDirection);
                break;
            case BehaviourState.CHOPPING_WOOD:
                choppingWoodState();
                break;
            case BehaviourState.THROWING_WOOD:
                throwingWoodState();
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
        woodCheckTimer += Time.deltaTime;
        woodThrowTimer += Time.deltaTime;
    }


    protected override void OnCollisionEnter2D(Collision2D collision) {
        isCollidingWithPlayer = collision.gameObject == PlayerController.instance.gameObject;

        collision.gameObject.TryGetComponent(out WoodLog logScript);
        if (logScript != null) {
            if (!isCollidingWithWood) woodChopTimer = 0.0f;
            isCollidingWithWood = true;
        }
    }

    // TODO: Change to be script reference
    protected override void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject == PlayerController.instance.gameObject) {
            isCollidingWithPlayer = false;
        }

        collision.gameObject.TryGetComponent(out WoodLog logScript);
        if (logScript != null) {
            isCollidingWithWood = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        Debug.Log($"TRIGGER ENTER {collider.gameObject.name}");
        collider.gameObject.TryGetComponent(out WoodLog logScript);

        if (logScript != null) {
            Debug.Log("WOOD ENTER");
            woodObjects.Add(logScript.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        collider.gameObject.TryGetComponent(out WoodLog logScript);

        if (logScript != null) {
            Debug.Log("WOOD REMOVE");
            woodObjects.Remove(logScript.gameObject);
        }
    }
}
