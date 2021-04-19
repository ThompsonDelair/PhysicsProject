using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsData
{
    public GameObject owner;

    public Vector3 position;
    public Vector2 velocity;
    public float mass;
    public float restitution;

    // circluar hitbox
    public Circle circ;

    public void CopyData(PhysicsData other) {
        position = other.position;
        velocity = other.velocity;
        mass = other.mass;
        circ = new Circle();
        circ.radius = other.circ.radius;
        restitution = other.restitution;
    }

}
