using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chase : MonoBehaviour {

    public Transform player;
    static Animator anim;
    public bool isSwinging;

	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
        isSwinging = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 direction = player.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);
        if (Vector3.Distance(player.position, this.transform.position) < 10 && angle < 30)
        {
           
            direction.y = 0;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
            anim.SetBool("isIdle", false);
            
            if (direction.magnitude > 5)
            {
                this.transform.Translate(0, 0, 0.05f);
                anim.SetBool("isWalking", true);
                anim.SetBool("isIdle", false);
                anim.SetBool("isAttacking", false);
                isSwinging = false;
                anim.speed = 1f;
            }
            else
            {
                anim.SetBool("isWalking", false);
                anim.SetBool("isIdle", false);
                anim.SetBool("isAttacking", true);
                isSwinging = true;
                anim.speed = 0.5f;
            }
        }
        else
        {
            anim.SetBool("isIdle", true);
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", false);
            isSwinging = false;
            anim.speed = 1f;
        }
    }

    public void SetSwinging(bool isSwinging)
    {
        this.isSwinging = isSwinging;
    }
}
