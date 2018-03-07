using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skeletonHitBox : MonoBehaviour {

    public float collisionForce = 1000f;
    public chase skeletonChase;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            //transform.position += Vector3.up; // bandaid, rigidbodyfirstpersoncontroller hard sets velocity if grounded
            skeletonChase.GetComponent<Rigidbody>().AddForce(Vector3.up * collisionForce, ForceMode.Acceleration);
        }
    }
}


