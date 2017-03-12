using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKinekt : MonoBehaviour {

    LinkedList<Blob> blops = new LinkedList<Blob>();
    public PlayerController controller;
    int count = 0;
   	void Start () {
		
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (count == 500)
        {
            int r = Random.Range(2, 8);
            float move = Random.Range(10, 100);
            move = move / 1000;
            int counter = 0;
            foreach (Blob b in blops)
            {
                b.center.x += move;
                if (b.center.x > 1)
                {
                    blops.Remove(b);
                    counter++;
                }
            }

            LinkedList<Blob> toadd = new LinkedList<Blob>();
            for (int i = 0; i < r - counter && blops.Count <8 ; i++)
            {
                float x = Random.Range(0, 100);
                float y = Random.Range(0, 100);

                toadd.AddLast(new Blob(new Vector2(x/100, y/100)));
            }

            count = 0;

            foreach (Blob b in toadd)
            {
                blops.AddLast(b);
                controller.AddPlayerTentakel(b.center.x, b.center.y);
            }
            Debug.Log(blops.Count);
            toadd.Clear();
           
        }
        count++;
    }
}
