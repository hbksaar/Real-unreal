using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController :MonoBehaviour {
   
    /*
     Das Koordinatensystem hat seine Aufhaengepunkt in der Mitte des Dachateliers bei 0,0
     die Spielflache hat an der x Achse eine Ausdenhnung von -2,5 bis 2,5 und an der y Achse -4 bis 4.
     Beim Überschreiten dieser Koordinaten wird das  dahinterliegende Gameobjekt geloescht und dessen 
     Attribut alive auf false gesetzt, sodass man von hand aus dem Playerkontroller werfen kann.
    */

    //ScriptableObject
    //beide müssen in Unity auf die dazugehörigen Playerobjekte (Gnome und Tentakel) gesetzt sein
    public GameObject Gnome, pl2, scheinGut, scheinBad;

    public RuntimeAnimatorController controller;

    //Spielfeld besitzt umrechnungs Methoden für die x / z Position im bezug aud das Kinektraster
    private Spielfeld feld = new Spielfeld(1,1);
    LinkedList<Player> players = new LinkedList<Player>();
    
   
	/*
	 * Assign new Positions to players
	*/
	public void UpdatePlayers(LinkedList<Blob> updatedBlobs)
	{
		UnflagPlayers ();

		Vector2 newPosition = new Vector2();
		float bestDistance = 100f;
		float distanceBuff = 0f;
		float currentDistance = 0f;
		foreach(Blob blob in updatedBlobs) {
			foreach (Player player in players) {
				if (player.visited)
					continue;
				currentDistance = Vector2.Distance (player.pos, blob.center);
				if (currentDistance < bestDistance) {
					bestDistance = currentDistance;
					newPosition = blob.center;
				}
				updatedBlobs.Remove (blob);
				player.move (newPosition);
				player.visited = true;
			}
		}
	}

	public void UnflagPlayers()
	{
		foreach (Player player in players) {
			player.visited = false;
		}
	}

    public void AddPlayerGnom(float x, float z)
    {

        //Player p = ScriptableObject.CreateInstance("Gnom") as Player;
        Player p = ScriptableObject.CreateInstance("Player") as Player;
        p.init(feld.getCordX(x), 0.5f,feld.getCordZ( z), Gnome, scheinBad, controller);
         p.updateScale(0.4f);
         players.AddFirst(p);
    }

    public void AddPlayerTentakel(float x, float z) // y axis remove
    {
        Player p = ScriptableObject.CreateInstance("Player") as Player;
        p.init(feld.getCordX(x),1.5f ,feld.getCordZ(z), pl2, scheinBad, controller);

        p.updateScale(0.4f);
        players.AddFirst(p);
    }

    void RemovePlayer(int id)
    {
        for (int i = 0; i < players.Count; i++)
        {
            var curr = players.First;
            if (curr.Value.pid == id)
            {
                players.Remove(curr.Value);
            }
            curr = curr.Next; 
        }
    }

    void movePlayer(int id,Vector3 move)
    {
        for (int i = 0; i < players.Count; i++)
        {
            var curr = players.First;
            if (curr.Value.pid == id)
            {
                curr.Value.move(move);
            }
            curr = curr.Next;
        }
    }

    // Use this for initialization
    void Start () {

       

        foreach (Display d in Display.displays)
        {
            d.Activate();
        }

        AddPlayerTentakel(0, 0);
      /*  AddPlayerTentakel(1, 0);
        AddPlayerTentakel(0, 1);
        AddPlayerTentakel(1, 1);*/

    }
	void FixedUpdate()
   {


        if (Input.GetKey("y"))
        {

            float a = Random.Range(0, 100);
            float b = Random.Range(0, 100);
           // Debug.Log(a + "   " + b);
            AddPlayerTentakel(a/100f, b/100f);
           // Debug.Log(players.Count);
        }
        

        foreach(Player p in players)
        {
            if (p.isAlive)
            {
                float a = Random.Range(0, 100);            
                p.TestMove(new Vector2(((a) /300) * Time.deltaTime, 0));
               // players.Remove(p);
            }
        }
        //players.First.Value.move(0.1f, 0.2f, 0.3f);
    
   }


// Update is called once per frames
// just for testing
void Update() {
}  
       
}
