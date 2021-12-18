using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public float height;
    public float speed;
    public float radius;
    [SerializeField]
    Collider[] collisions;
    CapsuleCollider playerCollider;
    LayerMask layerMask;
    
    // Start is called before the first frame update
    void Start()
    {
        playerCollider = gameObject.AddComponent<CapsuleCollider>();
        playerCollider.height = height;
        playerCollider.radius = radius;
        gameObject.layer = 3;
        layerMask = gameObject.layer;
        layerMask = ~layerMask;
    }

    // Update is called once per frame
    void Update()
    {
        Move(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"))*speed*Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.L)){
            Move(new Vector3(0, 0, 100));
        }
    }
    public void Move(Vector3 move){
        RaycastHit hit;
        RaycastHit[] capsuleHits;
                    DrawVector(transform.position, move, 1, Color.black);
                    //DrawVector(transform.position, perpendicular2, 100, Color.blue);
                    DrawVector(transform.position, move, 100, Color.black);
                    //DrawVector(transform.position, perpendicular1, 100, Color.blue);
        if(Physics.Linecast(transform.position, transform.position + move, out hit)){
            Vector3 leftOverVector = transform.position + move - hit.point;
            leftOverVector -= hit.normal * Vector3.Dot(hit.normal, leftOverVector);
            transform.position = hit.point - move.normalized * radius + leftOverVector;
            Debug.Log("linetest");
        }else{
            Vector3[] capsuleTip1 = {transform.position-Vector3.up*height/2, transform.position+Vector3.up*height/2}; 
                //Debug.Log("cpasuletest");
                /*if(Physics.CapsuleCast(capsuleTip1[0], capsuleTip1[1], radius, move.normalized, out hit, move.magnitude)){
                    if(Vector3.Distance(transform.position, hit.point - move.normalized * radius) > 0){
                        //transform.position = hit.point - move.normalized * radius;
                        //calculate funny vectors to fix capsulecast weird behaviour of setting hitpoints close to y = 0
                    }else{
                        transform.position += move;                        
                    }
                }else{
                    transform.position += move;
                }*/
                capsuleHits = Physics.CapsuleCastAll(capsuleTip1[0], capsuleTip1[1], radius, move.normalized, move.magnitude);
                if(capsuleHits.Length > 0){
                    List<Vector3> hitPositions = new List<Vector3>();
                    foreach(RaycastHit ray in capsuleHits){
                        hitPositions.Add(ray.point);
                    }
                    Vector3 medianPoint = Vector3.zero;
                    int count = 0;
                    foreach(Vector3 vec in hitPositions){
                        DrawVector(vec, Vector3.up, 100, Color.yellow);
                        if(Vector3.Distance(vec, transform.position) < radius*2){
                            medianPoint += vec;
                            count += 1;
                        }
                    }
                    medianPoint = medianPoint/count;
                    DrawVector(medianPoint, Vector3.up, 100, Color.green);
                    if(count > 0){
                        Vector3 translation = medianPoint - transform.position;
                        Vector3 perpendicular1 = Vector3.Cross(move, Vector3.up).normalized;
                        Vector3 perpendicular2 = Vector3.Cross(perpendicular1, move).normalized;
                        float dot1 = Vector3.Dot(translation, perpendicular1);
                        float dot2 = Vector3.Dot(translation, perpendicular2);
                        translation -= dot1 * perpendicular1 + dot2 * perpendicular2;
                        DrawVector(transform.position, translation, 10, Color.red);
                        transform.position += translation - move.normalized*radius;
                    }
                    else{
                        transform.position += move;
                    }
                }else{
                    transform.position += move;
                }
        }
        Vector3[] capsuleTip = {transform.position-Vector3.up*height/2, transform.position+Vector3.up*height/2}; 
        collisions = Physics.OverlapCapsule(capsuleTip[0], capsuleTip[1], radius, ~layerMask);
        foreach(Collider col in collisions){
            Vector3 hitDirection;
            float minSeparation;
            bool overlap = Physics.ComputePenetration(playerCollider, transform.position, transform.rotation, col, col.transform.position, col.transform.rotation, out hitDirection, out minSeparation);
            Debug.Log(overlap);
            if(overlap){
                transform.position += hitDirection * minSeparation;
            }
        }
    }
    public void DrawVector(Vector3 origin, Vector3 vector, float lengthMultiplier, Color color){
        Debug.DrawLine(origin, origin + vector * lengthMultiplier, color);
    }
}
