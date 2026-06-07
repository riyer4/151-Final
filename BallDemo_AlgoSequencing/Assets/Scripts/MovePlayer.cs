using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//************** use UnityOSC namespace...
using UnityOSC;
//*************

public class MovePlayer : MonoBehaviour {

	public float speed;
	public Text countText;

	private Rigidbody rb;
	private int count;


	// Use this for initialization
	void Start () 
	{
		Application.runInBackground = true; //allows unity to update when not in focus

		//************* Instantiate the OSC Handler...
	    OSCHandler.Instance.Init ();
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/playseq", 1);
        //*************

        rb = GetComponent<Rigidbody> ();
		count = 0;
		setCountText ();
	}
	

	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");


        // ************* send the balls position to pd
        float xPos = rb.position.x;
        float zPos = rb.position.z;
        xPos = (xPos + 9.25f) / (18.5f);
        zPos = (zPos + 9.25f) / (18.5f);

        int selectSeq = 1;
        if(xPos < 0.333)
        {
            selectSeq = 1;
        }
        else if(xPos < 0.666)
        {
            selectSeq = 2;
        }
        else if(xPos < 1.0)
        {
            selectSeq = 3;
        }
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/selectSeq", selectSeq);


        int selectOrch = 1;
        if (zPos < 0.333)
        {
            selectOrch = 1;
        }
        else if (zPos < 0.666)
        {
            selectOrch = 2;
        }
        else if (zPos < 1.0)
        {
            selectOrch = 3;
        }
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/selectOrch", selectOrch);


        Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);
		rb.AddForce (movement*speed);

	}
		

	void OnTriggerEnter(Collider other) 
    {
        //Debug.Log("-------- COLLISION!!! ----------");

        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            setCountText();


            // stop playing the sequence after picking up 7 balls
            if (count < 2)
            {
                // do nothing...
            }
            if (count < 3)
            {
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/probBass", 0.4f);
            }
            else if (count < 6)
            {
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/probBass", 0.6f);

                OSCHandler.Instance.SendMessageToClient("pd", "/unity/probChimes", 0.5f);
            }
            else if (count < 7)
            {
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/probBass", 0.75f);

                OSCHandler.Instance.SendMessageToClient("pd", "/unity/probChimes", 0.65f);
            }
            else if (count < 8)
            {
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/probBass", 0.9f);

                OSCHandler.Instance.SendMessageToClient("pd", "/unity/probChimes", 0.8f);
            }
        }
    }


	void setCountText()
	{
		countText.text = "Count: " + count.ToString ();

		//************* Send the message to the client...
		OSCHandler.Instance.SendMessageToClient ("pd", "/unity/trigger", count);
		//*************
	}
		
}
