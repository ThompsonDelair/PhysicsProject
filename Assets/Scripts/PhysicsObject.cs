using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    //public Vector2 velocity;
    //public float mass;
    //public float restitution;
    //public int ID;

    //public float radius;

    //public Circle circ;

    public PhysicsData data;

    public Color color = Color.magenta;
    public int points = 16;

    public float circleRadius;

    // Start is called before the first frame update
    void Start()
    {
        data.circ = new Circle { radius = circleRadius };
        data.owner = gameObject;
        int i = GameManager.main.physicsEngine.AddNewObj(data);
        transform.GetChild(0).transform.localScale = Vector3.one * data.circ.radius * 2;
    }
        
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos() {

        float drawMod = GameObject.Find("GameManager").GetComponent<GameManager>().drawMod;

        // draw radius
        Utils.DrawDebugCircle(transform.position,points,data.circ.radius,Color.magenta);

        // draw velocity
        if (data.velocity != Vector2.zero) {
            Debug.DrawLine(transform.position,transform.position + (Vector3)(data.velocity.normalized * data.circ.radius * 1.5f),color);
        }
    }
}
