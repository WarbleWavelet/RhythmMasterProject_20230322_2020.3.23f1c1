using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{


	Rigidbody rgb;
	string eventID;
	float jumpSpeed = 3f;

	// Use this for initialization
	void Start () {

        jumpSpeed = 5f;
        eventID = "Piano";
		rgb=GetComponent<Rigidbody>();
		Koreographer.Instance.RegisterForEvents(eventID, BallJump);
	}


	void BallJump(KoreographyEvent evt)
	{
		transform.position = Vector3.zero;	//打的点密，不归零来不及
        Vector3 v = rgb.velocity;
        v.y = jumpSpeed;
        rgb.velocity = v;		
	}
}
