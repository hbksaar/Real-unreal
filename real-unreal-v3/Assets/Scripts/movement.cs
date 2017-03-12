using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class movement : MonoBehaviour {

   // private AudioSource audio;
    public AudioClip footstep;
    public float speed;
    private float dx, dy;
	// Use this for initialization
	void Start () {
        //audio = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
 
         float idx = (Input.GetAxis("Horizontal")) ;
        float idy = (Input.GetAxis("Vertical"));
        Transform t = GetComponent<Transform>();
        transform.Translate(idx * speed *Time.deltaTime,0, idy* speed * Time.deltaTime, Space.Self); 
        transform.Translate(new Vector3 ( 0, -t.forward.y * (idy) * Time.deltaTime * speed, 0),Space.World);
        //Debug.Log(t.forward.x + "  "+ t.forward.y + "   " + t.forward.z);
        dx += Abs(idx)*speed;
        dy += Abs(idy) * speed;

        if (Input.GetKey("g"))   
            transform.Rotate(0, 10 * Time.deltaTime, 0, Space.World);
        if (Input.GetKey("j"))
            transform.Rotate(0, -10 * Time.deltaTime, 0, Space.World);
       if (Input.GetKey("z"))
            transform.Rotate(8 * Time.deltaTime, 0, 0, Space.Self);
        if (Input.GetKey("h"))
            transform.Rotate(-8 * Time.deltaTime, 0, 0, Space.Self);



        if ((dx) + dy >7 )
        {
           // float volume = Random.Range(0.3f, 1.2F);
           // audio.PlayOneShot(footstep,volume);
            dx = 0;
            dy = 0;
        }
      
        //print();
        
    }

    private float Abs (float a)
    {
        if (a <= 0) return -a;
        return a;
    }
}
