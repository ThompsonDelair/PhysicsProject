using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager main;
    public PhysicsEngine physicsEngine;

    public float drawMod = 1.0f;

    enum inputMode { NONE, SCALE, VELOCITY }
    inputMode currMode = inputMode.NONE;

    Vector2 position;
    float radius;
    Vector2 velocity;

    public GameObject physObjPrefab;

    bool runPhysics = true;

    int[] speeds = { 30,1,2,3,6,9,12,15,20,25 };

    int steps = 1;

    public bool gravity;

    private void Awake() {
        main = this;
        physicsEngine = new PhysicsEngine();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)) {
            //Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (currMode == inputMode.NONE) {
                currMode = inputMode.SCALE;
                position = mousePos;
            } else if (currMode == inputMode.SCALE) {                
                radius = Vector2.Distance(position,mousePos)/2f;
                currMode = inputMode.VELOCITY;
            } else if (currMode == inputMode.VELOCITY) {
                velocity = (mousePos - position);
                MakeNewPhysObject();
                currMode = inputMode.NONE;
            }           
        }

        if (Input.GetMouseButtonDown(1)) { 
            if(currMode == inputMode.VELOCITY) {
                velocity = Vector2.zero;
                MakeNewPhysObject();
                currMode = inputMode.NONE;
            }
        }

        if (currMode == inputMode.SCALE) {
            Utils.DrawDebugCircle(position,32,Vector2.Distance(position,mousePos)/2f,Color.white);
        } else if (currMode == inputMode.VELOCITY) {
            Debug.DrawLine(position,mousePos);
            Utils.DrawDebugCircle(position,32,radius,Color.white);

            if (gravity) {
                PhysicsData dat = new PhysicsData();
                dat.velocity = (mousePos - position);
                dat.position = (position);
                dat.circ = new Circle { radius = radius };
                float m = Mathf.PI * radius * radius;
                dat.mass = m * m * m;

                PhysicsData[] bigEnts = new PhysicsData[3];
                float[] highMass = new float[3];

                for (int i = 0; i < physicsEngine.physEnts.Length; i++) {

                    PhysicsData other = physicsEngine.physEnts[i];
                    if (other == null) {
                        continue;
                    }

                    Vector2 gForce = Calc.GravityForce(other,dat);
                    float f = gForce.magnitude;
                    float low = Mathf.Min(highMass);
                    if (f > low) {
                        for (int j = 0; j < highMass.Length; j++) {
                            if (highMass[j] == low) {
                                highMass[j] = f;
                                bigEnts[j] = physicsEngine.physEnts[i];
                                break;
                            }
                        }
                    }
                }

                if (highMass[0] != 0f) {

                    Vector2[] preview = physicsEngine.gravityPreview(bigEnts,dat,400);

                    for (int i = 0; i < preview.Length - 1; i++) {
                        Debug.DrawLine(preview[i],preview[i + 1],Color.magenta);
                    }
                }
            }
        }

        for (int i = 0; i < 10; i++) {
            if (Input.GetKeyDown(i.ToString())) {
                steps = speeds[i];
            }
        }

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f) {
            //Debug.Log(Camera.main.orthographicSize * f);
            Camera.main.orthographicSize += (Camera.main.orthographicSize * scroll * 0.04f);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            runPhysics = !runPhysics;
        }

        if (Input.GetKey(KeyCode.W)) {
            Camera.main.transform.position += Vector3.up * Camera.main.orthographicSize * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A)) {
            Camera.main.transform.position += Vector3.left * Camera.main.orthographicSize * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S)) {
            Camera.main.transform.position += Vector3.down * Camera.main.orthographicSize * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D)) {
            Camera.main.transform.position += Vector3.right * Camera.main.orthographicSize * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Backspace)) {
            physicsEngine.Purge(1);
        }

        if (Input.GetKey(KeyCode.Delete)) {
            physicsEngine.Purge(0);
        }
    }

    void MakeNewPhysObject() {
        GameObject go = GameObject.Instantiate(physObjPrefab);
        go.transform.position = position;
        PhysicsObject obj = go.GetComponent<PhysicsObject>();
        obj.data = new PhysicsData();
        obj.data.restitution = 0.5f;
        obj.data.velocity = velocity;
        obj.data.circ = new Circle();
        obj.data.circ.radius = radius;
        obj.data.position = obj.transform.position;
        obj.circleRadius = radius;
        float m = Mathf.PI * radius * radius;
        obj.data.mass = m * m * m;
    }

    private void FixedUpdate() {
        if (runPhysics) {
            for (int i = 0; i < steps; i++) {
                physicsEngine.PhysicsUpdate();
            }
        }       
    }

    private void OnDrawGizmos() {
        //if (currMode == inputMode.SCALE) {
        //    Gizmos.DrawSphere(position,1f);
        //    Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    radius = Vector2.Distance(position,clickPos);
        //    Debug.DrawLine(position,clickPos,Color.white);
        //} if(currMode == inputMode.VELOCITY) {

        //}
    }
}

