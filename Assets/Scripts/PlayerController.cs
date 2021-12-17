using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float height;
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
        Move(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"))*1000);
        if(Input.GetKeyDown(KeyCode.L)){
            Move(new Vector3(0, 0, 0));
        }
    }
    public void Move(Vector3 move){
        RaycastHit hit;
        Vector3[] capsuleTip = {transform.position + Vector3.up * 0.5f * height, transform.position+Vector3.up * height * -0.5f};
        if(Physics.Linecast(transform.position, transform.position + move, out hit)){
            transform.position = hit.point - move.normalized * radius; 
        }else{
            transform.position += move;
            if(Physics.CapsuleCast(capsuleTip[0], capsuleTip[1], radius, move.normalized, out hit, move.magnitude)){
                transform.position = hit.point - move.normalized * radius; 
            }else{
                transform.position += move;
            }
        }
        collisions = Physics.OverlapCapsule(capsuleTip[0], capsuleTip[1], radius, ~layerMask);
        foreach(Collider col in collisions){
            Vector3 hitDirection;
            float minSeparation;
            bool overlap = Physics.ComputePenetration(playerCollider, transform.position, transform.rotation, col, col.transform.position, col.transform.rotation, out hitDirection, out minSeparation);
            Debug.Log(overlap);
            if(overlap){
                //gameObject.SendMessage("PlayerOnCollisionEnter", )
                //aa
                transform.position += hitDirection * minSeparation;
            }
        }
    }
}
