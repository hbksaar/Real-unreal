 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Player :ScriptableObject{
	static int id;

    GameObject playerObject;
    GameObject floorEffect;
    public Rigidbody playerRigidbody;
    public int pid;
	float scale;

	public float speed;
	public bool visited = false;
    public Vector3 pos;


    public bool isAlive = true;
	// TODO: entferne Tentakle und Gnom
	public void init(float x, float y, float z, GameObject b, GameObject sc, RuntimeAnimatorController controller)
	{
		pos = new Vector3(x, y, z);
       
		pid = id++;
		playerObject = Instantiate<GameObject>(b, new Vector3(x, y, z), Quaternion.identity);
		floorEffect = Instantiate<GameObject>(sc, new Vector3(x, y, z), Quaternion.identity);
		//playerObject.transform.Translate(x,y,z, Space.World);

        playerRigidbody = playerObject.GetComponent<Rigidbody>();
        playerObject.GetComponent<Animator>().runtimeAnimatorController = controller;

		//floorEffect.transform.Translate(new Vector3(x, y+1, z), Space.World);
		floorEffect.transform.localScale *= 15;
	}

	// TODO: methode returning position in Vector2
	// TODO: move with Vector2
	public void move(Vector2 newPosition)
	{
        
        floorEffect.transform.Translate(newPosition.x, 0, newPosition.y, Space.Self);
        playerObject.transform.Translate(newPosition.x, 0, newPosition.y, Space.Self); 
    }

    public void Randomrotate()
    {     
        int y = Random.Range(0, 100);
        playerObject.transform.Rotate(new Vector3(0, (y-50)/10, 0), Space.World);
        floorEffect.transform.Rotate(new Vector3(0, (y - 50) / 10, 0), Space.World);
    }

    public void updateScale(float x)
    {
        playerObject.transform.localScale *= x;
		floorEffect.transform.localScale *= x;
    }

    public void Testdestruct()
    {
        Debug.Log("dead");
        isAlive = false;
        int r = Random.Range(0, 1);
        Destroy(playerObject, r);
        Destroy(floorEffect, r);
    }

    private bool isOut(float x, float z)
    {
        if (x < -3.21 || x > 5 || z < -4.5 || z > 2) return true;
        return false;
    }

    public void TestMove(Vector2 move)
    {
        this.move(move);
        Randomrotate();
        if(isOut(playerObject.transform.position.x, playerObject.transform.position.z)){
            Testdestruct();
            isAlive = false;
        }
    }
  }

