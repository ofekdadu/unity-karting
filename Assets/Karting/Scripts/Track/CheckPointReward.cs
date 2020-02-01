using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointReward : MonoBehaviour
{
	public bool isAchived = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void CheckPointAchived()
	{
		if(!isAchived) {
	    isAchived = true;
	    Debug.Log("Im here");
		}
	}
	public void ResetIsAchived(){
		isAchived = false;
	}
}
