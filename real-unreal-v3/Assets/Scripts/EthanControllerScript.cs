using UnityEngine;
using System.Collections;

public class EthanControllerScript : MonoBehaviour
{
    Animator anim;
    int jumpHash = Animator.StringToHash("Armature|ArmatureAction");
 


    void Start()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        
        

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        
            //anim.SetTrigger(jumpHash);
        
    }
}