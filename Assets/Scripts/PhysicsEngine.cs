using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsEngine
{
    public PhysicsData[] physEnts = new PhysicsData[16];
    
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void PhysicsUpdate() {
        for (int i = 0; i < physEnts.Length; i++) {
            if (physEnts[i] == null) {
                continue;       
            }

            for (int j = i+1; j < physEnts.Length; j++) {

                if (physEnts[j] == null) {
                    continue;
                }

                PhysicsData a = physEnts[i];
                PhysicsData b = physEnts[j];

                Manifold m = new Manifold { a = a,b = b };

                if (CircleVsCircle(m,a.circ,b.circ)) {
                    Calc.ResolveCollision(a,b,m.normal,m.penetration);
                }
                // do collision stuff for a and b?
            }
        }

        // gravity
        for (int i = 0; i < physEnts.Length; i++) {
            if (physEnts[i] == null) {
                continue;
            }
            for (int j = i + 1; j < physEnts.Length; j++) {
                if (physEnts[j] == null) {
                    continue;
                }
                Vector2 forceA = Calc.GravityForce(physEnts[i],physEnts[j]);
                physEnts[i].velocity += forceA / physEnts[i].mass * (1f/60f);

                Vector2 forceB = Calc.GravityForce(physEnts[j],physEnts[i]);
                physEnts[j].velocity += forceB / physEnts[j].mass * (1f / 60f);
            }
        }

        for (int i = 0; i < physEnts.Length; i++) {
            if (physEnts[i] == null || physEnts[i].mass == 0) {
                continue;
            }
            physEnts[i].position += (Vector3)(physEnts[i].velocity) * (1.0f / 60.0f);
            physEnts[i].owner.transform.position = physEnts[i].position;
        }
    }

    public int AddNewObj(PhysicsData obj) {
        for (int i = 0; i < physEnts.Length; i++) {
            if (physEnts[i] == null) {
                physEnts[i] = obj;
                return i;
            }                
        }
        int oldSize = physEnts.Length;
        Array.Resize(ref physEnts, physEnts.Length * 2);
        physEnts[oldSize] = obj;
        return oldSize;
    }

    bool CircleVsCircle(Manifold m, Circle ca, Circle cb) {
        PhysicsData a = m.a;
        PhysicsData b = m.b;

        // vector from a to b
        Vector2 atob = b.position - a.position;

        float r = ca.radius + cb.radius;
        r *= r;

        if (atob.sqrMagnitude > r) {
            return false;
        }

        float dist = atob.magnitude;

        if (dist != 0) {
            m.penetration = r - dist;
            m.normal = atob.normalized;
        } else {
            // circles are in the same position!
            m.penetration = ca.radius;
            m.normal = new Vector2(1,0);
        }

        return true;
    }

    public Vector2[] gravityPreview(PhysicsData[] others,PhysicsData moon,int points) {
        Vector2[] arr = new Vector2[points];
        PhysicsData[] ents = new PhysicsData[others.Length+1];

        for(int i = 1; i < ents.Length; i++) {
            if (others[i-1] == null)
                continue;
            ents[i] = new PhysicsData();
            ents[i].CopyData(others[i-1]);
        }

        ents[0] = new PhysicsData();
        ents[0].CopyData(moon);

        for (int i = 0; i < points; i++) {

            for(int j = 0; j < ents.Length; j++) {
                if (ents[j] == null)
                    continue;
                PhysicsData a = ents[j];
                for(int k = j + 1; k < ents.Length; k++) {
                    if (ents[k] == null)
                        continue;
                    PhysicsData b = ents[k];

                    Vector2 forceA = Calc.GravityForce(a,b);
                    a.velocity += forceA / a.mass * (1f / 60f);

                    Vector2 forceB = Calc.GravityForce(b,a);
                    b.velocity += forceB / b.mass * (1f / 60f);
                }

                ents[j].position += (Vector3)ents[j].velocity * (1f / 60f);
            }

            arr[i] = ents[0].position;
        }

        return arr;
    }

    public void Purge(int start) {
        for (int i = start; i < physEnts.Length; i++) { 
            if(physEnts[i] != null) {
                GameObject.Destroy(physEnts[i].owner.gameObject);
                physEnts[i] = null;
            }
        }
    }
}
