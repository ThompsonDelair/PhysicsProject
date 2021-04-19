using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calc
{
    public static bool AABBvsAABB(AABB a, AABB b) {
        if (a.max.x < b.min.x || a.min.x > a.max.x) {
            return false;
        }
        if (a.max.y < b.max.y || a.min.y > b.max.y) {
            return false;
        }
        return true;
    }

    public static bool DetectCircleOverlap(PhysicsData a,PhysicsData b, Circle ca, Circle cb) {
        float distSqrd = (a.position + b.position).sqrMagnitude;
        float threshold = ca.radius + cb.radius;
        threshold *= threshold;
        return distSqrd < threshold;
    }

    public static void ResolveCollision(PhysicsData a,PhysicsData b, Vector2 normal, float penetration) {
        // calculate relative velocity
        Vector2 rv = b.velocity - a.velocity;

        // calculate relative velocity in terms of normal direcion
        float velAlongNormal = Vector2.Dot(rv,normal);

        // do not resolve if velocities are separating
        if (velAlongNormal > 0) {
            return;
        }

        // calculate restitution
        float e = Mathf.Min(a.restitution,b.restitution);

        float invMassA = (a.mass == 0) ? 0 : 1 / a.mass;
        float invMassB = (b.mass == 0) ? 0 : 1 / b.mass;

        // calculate impulse scalar
        float j = -(1 + e) * velAlongNormal;
        if (invMassA != 0 || invMassB != 0) {
            j /= invMassA + invMassB;
        } else {
            j = 0;
        }
        
        // apply impulse
        Vector2 impulse = j * normal;
        a.velocity -= invMassA * impulse; 
        b.velocity += invMassB * impulse;

        // positional correction, needed due to floating point error
        //if (invMassA != 0 || invMassB != 0) {
        //    const float correctPercent = 0.2f;
        //    const float slop = 0.01f;
        //    Vector2 correction = Mathf.Max(penetration - slop,0.0f) / (invMassA + invMassB) * correctPercent * normal;
        //    a.transform.position -= (Vector3)(invMassA * correction);
        //    b.transform.position += (Vector3)(invMassB * correction);
        //}
    }

    public static Vector2 GravityForce(PhysicsData a,PhysicsData b) {
        const float gravConst = 0.00000000006675f;
        return (Vector2)(b.position - a.position) * gravConst * (a.mass * b.mass / Vector3.Distance(a.position,b.position));
    }
}
