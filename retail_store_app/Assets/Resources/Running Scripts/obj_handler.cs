﻿using UnityEngine;
using System.Collections;

public class obj_handler: MonoBehaviour {
	
	private Camera _cam;
	private Light _light;
	private Vector3 new_hit_point; 
	private Texture2D tex;
	public float horizontalSpeed = 2.0F;

	void Start () {
		Messenger<GameObject>.AddListener("selected", OnSelection);
		_cam = this.gameObject.transform.FindChild ("CubeCamera").gameObject.GetComponent<Camera>();
		_light = this.gameObject.transform.FindChild ("SpotLight").gameObject.GetComponent<Light> ();
	}
	
	public void StartMove(Vector3 hit_point){
		StartCoroutine (MoveLight (hit_point));
	}

	IEnumerator MoveLight(Vector3 hit_point)
	{
		var overTime = 3f;
		var dirVector = hit_point - _light.transform.position;
	
		dirVector.x = Mathf.Clamp (dirVector.x, -1.5f, 1.5f);
		dirVector.z = Mathf.Clamp (dirVector.z, -1.5f, 1.5f);

	    float startTime = Time.time;
	    while(Time.time < startTime + overTime)
	    {
			_light.transform.rotation = Quaternion.Lerp (_light.transform.rotation, Quaternion.LookRotation(dirVector, Vector3.down), Time.deltaTime);// (Time.time - startTime)/overTime);
        	yield return null;
	    }
	} 
	
	public void OnSelection(GameObject obj)
	{
		if (obj == this.gameObject) {
			Messenger<GameObject>.Broadcast("was_selected", this.gameObject);
			_cam.enabled = true;
		} else {
			_cam.enabled = false;
		}
	}
	
	void FixedUpdate () {
		if (_cam.enabled == true) {

			if (Input.GetKey ("up"))
				_light.spotAngle++;
			
			if (Input.GetKey ("down"))
				_light.spotAngle--;

			if (Input.GetKey ("right"))
				_light.intensity=_light.intensity + 0.05f;
			
			if (Input.GetKey ("left"))
				_light.intensity=_light.intensity - 0.05f;
		}
	}

}