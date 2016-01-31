using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public Transform target;

	public float damping = 1;

	public Transform[] canvasPos;

	float offsetZ;
	//Vector3 lastTargetPosition;
	Vector3 currentVelocity;

	// Use this for initialization
	void Start () {
		//lastTargetPosition = target.position;
		offsetZ = (transform.position - target.position).z;
		transform.parent = null;
	}

	// Update is called once per frame
	void Update () {

		// only update lookahead pos if accelerating or changed direction
		//float xMoveDelta = (target.position - lastTargetPosition).x;

		Vector3 aheadTargetPos = target.position + Vector3.forward * offsetZ;
		Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref currentVelocity, damping);

		transform.position = newPos;

		//lastTargetPosition = target.position;		
	}

	public void CameraChangePos (int canvasPosNum) {
		target = canvasPos [canvasPosNum];
	}
}
