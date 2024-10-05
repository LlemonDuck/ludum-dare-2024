using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public new Rigidbody2D rigidbody;

    public float movementSpeed = 10.0f;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalmovement = Input.GetAxis("Vertical");

        rigidbody.velocity = new Vector2(horizontalMovement, verticalmovement).normalized * movementSpeed;
    }

    public void FixedUpdate() { 

    }
}
