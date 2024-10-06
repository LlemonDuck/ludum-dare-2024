using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallColliderGenerator : MonoBehaviour {
    public void Clear() {
        Collider2D[] colliders = gameObject.GetComponents<Collider2D>();
        foreach (var col in colliders) {
            DestroyImmediate(col);
        }
        foreach(var child in gameObject.GetComponentsInChildren<BoxCollider2D>()) {
            if (child.gameObject != gameObject) DestroyImmediate(child.gameObject);
        }
    }

    public void CreateColliders(List<Rect> wallColliders) {
        foreach(var wall in wallColliders) {
            GameObject wallObj = new();
            wallObj.transform.parent = transform;
            Rigidbody2D rb = wallObj.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.isKinematic = true;
            var collider = wallObj.AddComponent<BoxCollider2D>();
            var xOffset = wall.x + (wall.width + 1) / 2;
            var yOffset = wall.y + (wall.height + 2) / 2;
            collider.offset = new Vector2(xOffset, yOffset);
            collider.enabled = true;
            collider.size = new Vector2(wall.width + 1, wall.height + 1);
        }
    }
}
