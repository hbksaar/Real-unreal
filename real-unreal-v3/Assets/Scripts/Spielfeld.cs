using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spielfeld {

    float xKinekt, zKinekt;
    float xProjekt, zProjekt;
    Vector3 length;

    public Spielfeld(float xKinekt, float zKinekt)
    {
        this.xKinekt = xKinekt;
        this.zKinekt = zKinekt;
        xProjekt = -3.2f;
        zProjekt = 1.2f;
        length = new Vector3(8, 0, 5);
    }

    public float getCordX(float xCurrKinekt)
    {
     
       // Debug.Log("output x : " + (xProjekt + (length.x * xCurrKinekt)));
        return (xProjekt + length.x * xCurrKinekt);
    }

    public float getCordZ(float zCurrKinekt)
    {
  
        //Debug.Log("output z : " + zProjekt + length.z * zCurrKinekt);
        return zProjekt - length.z * zCurrKinekt;

    }


  

}
