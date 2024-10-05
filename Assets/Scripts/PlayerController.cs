using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController instance;

    public new Rigidbody2D rigidbody;

    public float movementSpeed = 10.0f;

    public float health = 100.0f;

    float damageTickTimeSeconds = 2.0f;

    float collisionDamage = 5.0f;

    [HideInInspector]
    int numCollidingEnemies = 0;

    [HideInInspector]
    float lastCollisionTime = 0;

    void Awake() {
        if (instance == null) {
            PlayerController.instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalmovement = Input.GetAxis("Vertical");

        rigidbody.velocity = new Vector2(horizontalMovement, verticalmovement).normalized * movementSpeed;


        // Apply Damage from colliding enemies
        if (numCollidingEnemies > 0) {
            if (Time.time - lastCollisionTime > damageTickTimeSeconds) {
                health -= collisionDamage;
            }
        }

        Debug.Log(health);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<BaseEnemy>() != null) {
            numCollidingEnemies ++;
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<BaseEnemy>() != null) {
            numCollidingEnemies --;
        }
    }
}
