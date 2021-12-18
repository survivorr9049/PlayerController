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
    }
    public void Move(Vector3 move){
        Vector3 startPos = transform.position;
        RaycastHit hit;
        if(Physics.Linecast(transform.position, transform.position + move, out hit)){
            Vector3 leftOverVector = transform.position + move - hit.point;
            leftOverVector -= hit.normal * Vector3.Dot(hit.normal, leftOverVector);
            transform.position = hit.point - move.normalized * radius + leftOverVector;
            Debug.Log("linetest");
        }else{
            Vector3[] capsuleTip1 = {transform.position-Vector3.up*height/2, transform.position+Vector3.up*height/2}; 
                if(Physics.CapsuleCast(capsuleTip1[0], capsuleTip1[1], radius, move.normalized, out hit, move.magnitude)){
                        Vector3 translation = hit.point - transform.position;
                        Vector3 perpendicular1 = Vector3.Cross(move, Vector3.up).normalized;
                        Vector3 perpendicular2 = Vector3.Cross(perpendicular1, move).normalized;
                        float dot1 = Vector3.Dot(translation, perpendicular1);
                        float dot2 = Vector3.Dot(translation, perpendicular2);
                        translation -= dot1 * perpendicular1 + dot2 * perpendicular2;
                        DrawVector(transform.position, translation, 10, Color.red);
                        transform.position += translation;
                }else{
                    transform.position += move;
                }
            }
        Vector3 hitDirection = Vector3.zero;
        float minSeparation = 0;
        Vector3 averageNormal = Vector3.zero;
        Vector3 collisionPoint = Vector3.zero;
        for(int i = 0; i<=8; i++){
            Vector3[] capsuleTip = {transform.position-Vector3.up*height/2, transform.position+Vector3.up*height/2}; 
            collisions = Physics.OverlapCapsule(capsuleTip[0], capsuleTip[1], radius, ~layerMask);
            foreach(Collider col in collisions){
                bool overlap = Physics.ComputePenetration(playerCollider, transform.position, transform.rotation, col, col.transform.position, col.transform.rotation, out hitDirection, out minSeparation);
                collisionPoint = playerCollider.ClosestPoint(transform.position - hitDirection.normalized * height*10);
                DrawVector(collisionPoint, hitDirection, 100, Color.cyan);
                averageNormal += hitDirection;
                Debug.Log(overlap);
                if(overlap){
                    if(i <= 4)transform.position += hitDirection * minSeparation * 1.03f;
                    else if (i <= 6){
                        transform.position += move.normalized * minSeparation;
                    }
                    else{
                        transform.position = startPos;
                        AlternateMove(move);
                        break;
                    };
                }else{
                    break;
                }
            }
        }
        DrawVector(collisionPoint, averageNormal, 100, Color.red);
    }
        public void AlternateMove(Vector3 move){
        RaycastHit[] capsuleHits;
            Vector3[] capsuleTip1 = {transform.position-Vector3.up*height/2, transform.position+Vector3.up*height/2}; 
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
