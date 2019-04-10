using UnityEngine;
using System.Collections;

public class sInputDemo2 : MonoBehaviour {

    public float Speed = 50;
    private string redButtonText;
    private string blueButtonText;
    private string yellowButtonText;
    private string vertiPlus;
    private string vertiMinus;
	// Use this for initialization
    //You can clearly see how it became much easier to do a rebind keys menu
	void Start () {
        //the old load stuff
        if (!sInput.ListInFileExists("sInputFileSaveDemo2.spt", false))
        {
            sInput.SetAxis("Vertical", KeyCode.W, KeyCode.S, 3);
            sInput.SetKey("Red", KeyCode.R);
            sInput.SetKey("Blue", KeyCode.B);
            sInput.SetKey("Yellow", KeyCode.Y);
        }
        //if we do, load it
        else sInput.LoadListsInFile("sInputFileSaveDemo2.spt", false);

        //Get what text to display on buttons
        redButtonText = sInput.GetKeyText("Red");
        blueButtonText = sInput.GetKeyText("Blue");
        yellowButtonText = sInput.GetKeyText("Yellow");
        vertiPlus = sInput.GetPositiveAxisKeyText("Vertical");
        vertiMinus = sInput.GetNegativeAxisKeyText("Vertical");
	}
	
	// Update is called once per frame
	void Update () {
        sInput.Update();
        redButtonText = sInput.GetKeyText("Red");
        blueButtonText = sInput.GetKeyText("Blue");
        yellowButtonText = sInput.GetKeyText("Yellow");
        vertiPlus = sInput.GetPositiveAxisKeyText("Vertical");
        vertiMinus = sInput.GetNegativeAxisKeyText("Vertical");
        GetComponent<Transform>().Rotate(new Vector3(0, sInput.GetAxis("Vertical") * Time.deltaTime * Speed, 0));
        if (sInput.GetButtonUp("Red")) GetComponent<Renderer>().material.color = Color.red;
        if (sInput.GetButtonUp("Blue")) GetComponent<Renderer>().material.color = Color.blue;
        if (sInput.GetButtonUp("Yellow")) GetComponent<Renderer>().material.color = Color.yellow;   
	}
    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 300, 300), "");
        GUI.Label(new Rect(10, 10, 300, 20), "Red Color : ");
        if (GUI.Button(new Rect(80, 10, 200, 20), redButtonText))
        {
            //just a simple function is enough :D
            sInput.ChangeKeyInMenu("Red", "sInputFileSaveDemo2.spt",false,sInput.ChangeType.key);
        }
        GUI.Label(new Rect(10, 50, 300, 20), "Blue Color : ");
        if (GUI.Button(new Rect(80, 50, 200, 20), blueButtonText))
        {
            sInput.ChangeKeyInMenu("Blue", "sInputFileSaveDemo2.spt", false,sInput.ChangeType.key);
        }
        GUI.Label(new Rect(10, 100, 300, 20), "Yellow Color : ");
        if (GUI.Button(new Rect(90, 100, 200, 20), yellowButtonText))
        {
            sInput.ChangeKeyInMenu("Yellow", "sInputFileSaveDemo2.spt", false, sInput.ChangeType.key);
        }
        GUI.Label(new Rect(10, 150, 300, 20), "Vertical+ : ");
        if (GUI.Button(new Rect(80, 150, 200, 20), vertiPlus))
        {
            sInput.ChangeKeyInMenu("Vertical", "sInputFileSaveDemo2.spt", false, sInput.ChangeType.axisPos);
        }
        GUI.Label(new Rect(10, 200, 300, 20), "Vertical- : ");
        if (GUI.Button(new Rect(80, 200, 200, 20), vertiMinus))
        {
            sInput.ChangeKeyInMenu("Vertical", "sInputFileSaveDemo2.spt", false, sInput.ChangeType.axisNeg);
        }
    }
}
