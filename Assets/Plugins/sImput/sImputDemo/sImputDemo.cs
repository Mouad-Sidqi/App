using UnityEngine;
using System.Collections;

public class sImputDemo : MonoBehaviour {


    public float Speed = 50;
    private string redButtonText;
    private string blueButtonText;
    private string yellowButtonText;
    private string vertiPlus;
    private string vertiMinus;
    private bool changingRed = false;
    private bool changingBlue = false;
    private bool changingYellow = false;
    private bool changingVerplus = false;
    private bool changingVerMin = false;

	void Start () {
        Debug.Log(Application.persistentDataPath);
        //If we don't have an existing axis list saved , create the vertical axis
        if (!sInput.ListInFileExists("sInputFileSaveDemo.spt",false))
        {
            sInput.SetAxis("Vertical", KeyCode.W, KeyCode.S, 3);
            sInput.SetKey("Red", KeyCode.R);
            sInput.SetKey("Blue", KeyCode.B);
            sInput.SetKey("Yellow", KeyCode.Y);
        }
        //if we do, load it
        else sInput.LoadListsInFile("sInputFileSaveDemo.spt",false);
      
        //Get what text to display on buttons
        redButtonText = sInput.GetKeyText("Red");
        blueButtonText = sInput.GetKeyText("Blue");
        yellowButtonText = sInput.GetKeyText("Yellow");
        vertiPlus = sInput.GetPositiveAxisKeyText("Vertical");
        vertiMinus = sInput.GetNegativeAxisKeyText("Vertical");
        
       
	}
	
	void Update () {
        Debug.Log(Input.inputString);
        if (Input.GetKeyUp(KeyCode.Less)) Debug.Log("K");
        //If you're not using Axises , get rid of this function
        //Note : If you're using Axises , this function should only be called ONCE , just put it in a single update function and it'll work for all other scripts
        //Calling it more than once won't affect it's functionality , but it may impact performance (not noticibile but may become if you have lots of scripts that call it)
        
        sInput.Update();



        //Stuff to do when we click a button , you can see it looks just like the Default except for an "s"
        GetComponent<Transform>().Rotate(new Vector3(0, sInput.GetAxis("Vertical") * Time.deltaTime * Speed, 0));
        if (sInput.GetButtonUp("Red")) GetComponent<Renderer>().material.color = Color.red;
        if (sInput.GetButtonUp("Blue")) GetComponent<Renderer>().material.color = Color.blue;
        if (sInput.GetButtonUp("Yellow")) GetComponent<Renderer>().material.color = Color.yellow;    
        //If we're changing button X , change text to press key
        if (changingRed)
        {
            redButtonText = "Press New Key";
            if (Input.anyKey)
            {
                //Changes the key
                sInput.ChangeKey("Red", sInput.keyPressed());
                //Sets new text
                redButtonText = sInput.GetKeyText("Red");
                //Back to default text values
                changingRed = false;
            }
            //Saves the list
            sInput.SaveListsInFile("sInputFileSaveDemo.spt",false);
        }
        //Same for the rest
        else if (changingBlue)
        {
            blueButtonText = "Press New Key";
            if (Input.anyKey)
            {

                sInput.ChangeKey("Blue", sInput.keyPressed());
                blueButtonText = sInput.GetKeyText("Blue");
                changingBlue = false;
            }
            sInput.SaveListsInFile("sInputFileSaveDemo.spt",false);
        }
        else if (changingYellow)
        {
            yellowButtonText = "Press New Key";
            if (Input.anyKey)
            {

                sInput.ChangeKey("Yellow", sInput.keyPressed());
                yellowButtonText = sInput.GetKeyText("Yellow");
                changingYellow = false;
            }
            sInput.SaveListsInFile("sInputFileSaveDemo.spt",false);
        }
        else if (changingVerplus)
        {
            vertiPlus = "Press New Key";
            if (Input.anyKey)
            {

                sInput.ChangeAxisButtons("Vertical", sInput.keyPressed(), sInput.GetNegativeAxisKeyCode("Vertical"));
                vertiPlus = sInput.GetPositiveAxisKeyText("Vertical");
                changingVerplus = false;
            }
            sInput.SaveListsInFile("sInputFileSaveDemo.spt",false);
        }
        else if (changingVerMin)
        {
            vertiMinus = "Press New Key";
            if (Input.anyKey)
            {

                sInput.ChangeAxisButtons("Vertical", sInput.GetPositiveAxisKeyCode("Vertical"), sInput.keyPressed());
                vertiMinus = sInput.GetNegativeAxisKeyText("Vertical");
                changingVerMin = false;
            }
            sInput.SaveListsInFile("sInputFileSaveDemo.spt",false);
        }


	}

    


    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 300, 300), "");
        GUI.Label(new Rect(10, 10, 300, 20), "Red Color : ");
        if (GUI.Button(new Rect(80, 10, 200, 20), redButtonText))
        {
            //see why i hate it? , We now have a new way , check demo 2 :D
            changingRed = true;
            changingBlue = false;
            changingYellow = false;
            changingVerplus = false;
            changingVerMin = false;
        }
        GUI.Label(new Rect(10, 50, 300, 20), "Blue Color : ");
        if (GUI.Button(new Rect(80, 50, 200, 20), blueButtonText)) {
            changingRed = false;
            changingBlue = true;
            changingYellow = false;
            changingVerplus = false;
            changingVerMin = false;
        }
        GUI.Label(new Rect(10, 100, 300, 20), "Yellow Color : ");
        if (GUI.Button(new Rect(90, 100, 200, 20), yellowButtonText)){
            changingRed = false;
            changingBlue = false;
            changingYellow = true;
            changingVerplus = false;
            changingVerMin = false;
        }
        GUI.Label(new Rect(10, 150, 300, 20), "Vertical+ : ");
        if (GUI.Button(new Rect(80, 150, 200, 20), vertiPlus)) {
            changingRed = false;
            changingBlue = false;
            changingYellow = false;
            changingVerplus = true;
            changingVerMin = false;
        }
        GUI.Label(new Rect(10, 200, 300, 20), "Vertical- : ");
        if (GUI.Button(new Rect(80, 200, 200, 20), vertiMinus)) {
            changingRed = false;
            changingBlue = false;
            changingYellow = false;
            changingVerplus = false;
            changingVerMin = true;
        }
    }
}
