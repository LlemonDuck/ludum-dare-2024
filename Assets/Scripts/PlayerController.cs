using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController instance;

    public GameObject weaponSocket;

    public new Rigidbody2D rigidbody;

    public float movementSpeed = 10.0f;

    public float health = 100.0f;

    float damageTickTimeSeconds = 1.0f;

    public BaseWeapon equippedWeapon;

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
        transform.position = DungeonGenerator.playerStartPosition;
    }

    private bool isInIFrame() {
        return Time.time - lastCollisionTime <= damageTickTimeSeconds;
    }

    // Update is called once per frame
    void Update() {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalmovement = Input.GetAxis("Vertical");

        rigidbody.velocity = new Vector2(horizontalMovement, verticalmovement).normalized * movementSpeed;

        if (Input.GetAxis("Fire1") > 0) {
            if (equippedWeapon != null) {
                equippedWeapon.OnAttack();
            }
        }
    }

    public void applyDamage(float damage) {
        if (!isInIFrame()) {
            health -= damage;
            lastCollisionTime = Time.time;
        }

        if (health <= 0) {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
