using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class sInput  {
    //Initialize a list of keys
    //check below for the "key" struct
    private static List<key> Keys = new List<key>();
    //Initialize the Axis List
    //check below
    private static List<axis> Axis = new List<axis>();
    //Key Menu Stuff
    private static string keyChanging = null;
    enum SavingMode { prefs,File,no_save };
    public enum ChangeType { axisPos, axisNeg, key };
    private static SavingMode sm;
    private static ChangeType st;
    private static string filename;
    private static string listid;
    private static bool izpath;

    public static void Update()
    {
        //Updates axis list
        for (int i = 0; i < Axis.Count; i++)
        {
            //If we are pressing positive increase the value
            if (Input.GetKey(Axis[i].PositiveKey)) Axis[i].axVal += Axis[i].gravity * Time.deltaTime;
            //If we are pressing negative decrease it
            if (Input.GetKey(Axis[i].NegativeKey)) Axis[i].axVal -= Axis[i].gravity * Time.deltaTime;
            //If we're not pressing any button
            if (!Input.GetKey(Axis[i].PositiveKey) && !Input.GetKey(Axis[i].NegativeKey))
            {
                //if the axis is greater than 0 , Decrease until it's below 0 , then set to 0
                if (Axis[i].axVal > 0)
                {
                    Axis[i].axVal -= Axis[i].gravity * Time.deltaTime;
                    if (Axis[i].axVal <= 0) Axis[i].axVal = 0;
                }
                //if the axis is less than 0 , Increase until it's greather than 0 , then set to 0
                if (Axis[i].axVal < 0)
                {
                    Axis[i].axVal += Axis[i].gravity * Time.deltaTime;
                    if (Axis[i].axVal >= 0) Axis[i].axVal = 0;
                }
            }
            //Clamp the value between -1 to 1
            Axis[i].axVal = Mathf.Clamp(Axis[i].axVal, -1, 1);
            //Since Clamping doesn't reach exactly 1/-1 , we force it to when it is close to 1 or -1
            if (Axis[i].axVal > 0.98f) Axis[i].axVal = 1;
            else if (Axis[i].axVal < -0.98f) Axis[i].axVal = -1;
        }
        changeKeyNew();



    }
    private static void changeKeyNew()
    {
        if (keyChanging != null && st == ChangeType.key)
        {
            ChangeKey(keyChanging, KeyCode.None);
            if (Input.anyKey)
            {
                ChangeKey(keyChanging, sInput.keyPressed());
                if (sm == SavingMode.File)
                {
                    SaveListsInFile(filename, izpath);
                }
                else if (sm == SavingMode.prefs)
                {
                    SaveKeys(listid);
                }
                keyChanging = null;
            }
        }
        if (keyChanging != null && st == ChangeType.axisPos)
        {
            axis ax = new axis("placeholder",KeyCode.None,KeyCode.None,0);
            foreach(axis a in Axis) {
                if (a.Name == keyChanging) ax = a;
            }
            if (ax.PositiveKey == KeyCode.None && ax.NegativeKey == KeyCode.None)
            {
                Debug.LogError("Axis doesn't exist , use SetAxis to add a new one");
            }
            ChangeAxisButtons(keyChanging, KeyCode.None, ax.NegativeKey);
            if (Input.anyKey)
            {
                ChangeAxisButtons(keyChanging, keyPressed(), ax.NegativeKey);
                if (sm == SavingMode.File)
                {
                    SaveListsInFile(filename, izpath);
                }
                else if (sm == SavingMode.prefs)
                {
                    SaveAxisList(listid);
                }
                keyChanging = null;
            }
        }
        if (keyChanging != null && st == ChangeType.axisNeg)
        {
            axis ax = new axis("placeholder", KeyCode.None, KeyCode.None, 0);
            foreach (axis a in Axis)
            {
                if (a.Name == keyChanging) ax = a;
            }
            if (ax.PositiveKey == KeyCode.None && ax.NegativeKey == KeyCode.None)
            {
                Debug.LogError("Axis doesn't exist , use SetAxis to add a new one");
            }
            ChangeAxisButtons(keyChanging, ax.PositiveKey, KeyCode.None);
            if (Input.anyKey)
            {
                ChangeAxisButtons(keyChanging, ax.PositiveKey , keyPressed());
                if (sm == SavingMode.File)
                {
                    SaveListsInFile(filename, izpath);
                }
                else if (sm == SavingMode.prefs)
                {
                    SaveAxisList(listid);
                }
                keyChanging = null;
            }
        }
    }
    public static void ChangeKeyInMenu(string name,string listID , ChangeType sat)
    {
        sm = SavingMode.prefs;
        keyChanging = name;
        listid = listID;
        st = sat;
      
    }
    public static void ChangeKeyInMenu(string name, string fileName, bool isPath, ChangeType sat)
    {
        sm = SavingMode.File;
        keyChanging = name;
        filename = fileName;
        izpath = isPath;
        st = sat;
    }
    public static void ChangeKeyInMenu(string name, ChangeType sat)
    {
        sm = SavingMode.no_save;
        keyChanging = name;
        st = sat;

    }


/// KEYS PART
    //Changes a key
    public static void ChangeKey(string name, KeyCode kc)
    {
        //Check if the key actually exists
        if (CheckIfKeyNameExists(name))
        {
            //if it does , we loop through the whole list (we used the for loop in this case because we can't modify a list in a foreach loop) , and change the key
            for (int i = 0; i < Keys.Count; i++)
            {
                if (Keys[i].Name == name) Keys[i] = new key(name, kc);
            }
            Debug.Log("Key " + name + " successfully changed");
        }
        //If it doesn't exist , just throw an error
        else
        {
            Debug.LogError("Key doesn't exist , use SetKey instead.");
        }
    }
    //This Loops through the whole keys list and checks if a key with the same name exists
    public static bool CheckIfKeyNameExists(string name)
    {
        foreach (key k in Keys)
        {
            if (k.Name == name) return true;
        }
        return false;
    }

    //Basically the same as the one above just check if a key is assigned , not it's name
    public static bool CheckIfKeyCodeExists(KeyCode code)
    {
        foreach (key k in Keys)
        {
            if (k.Key == code) return true;
        }
        return false;
    }
    //This gets the text of a key , for example , we have "Interact" assigned with button "E" , this returns E
    public static string GetKeyText(string name)
    {
        foreach (key k in Keys)
        {
            if (k.Name == name)
            {
                if (k.Key == KeyCode.Mouse0) return "Left Mouse Button";
                else if (k.Key == KeyCode.Mouse1) return "Right Mouse Button";
                else if (k.Key == KeyCode.Mouse2) return "Middle Mouse Button";
                else if (k.Key == KeyCode.LeftAlt) return "Left Alt";
                else if (k.Key == KeyCode.AltGr) return "Right Alt";
                else if (k.Key == KeyCode.LeftShift) return "Left Shift";
                else if (k.Key == KeyCode.RightShift) return "Right Shift";
                else if (k.Key == KeyCode.LeftControl) return "Left Control";
                else if (k.Key == KeyCode.RightControl) return "Right Control";
                else if (k.Key == KeyCode.KeypadEnter) return "Numpad Enter";
                else if (k.Key == KeyCode.Alpha0) return "0";
                else if (k.Key == KeyCode.Alpha1) return "1";
                else if (k.Key == KeyCode.Alpha2) return "2";
                else if (k.Key == KeyCode.Alpha3) return "3";
                else if (k.Key == KeyCode.Alpha4) return "4";
                else if (k.Key == KeyCode.Alpha5) return "5";
                else if (k.Key == KeyCode.Alpha6) return "6";
                else if (k.Key == KeyCode.Alpha7) return "7";
                else if (k.Key == KeyCode.Alpha8) return "8";
                else if (k.Key == KeyCode.Alpha9) return "9";
                else if (k.Key == KeyCode.ScrollLock) return "Scroll Lock";
                else if (k.Key == KeyCode.PageUp) return "Page Up";
                else if (k.Key == KeyCode.PageDown) return "Page Down";
                else if (k.Key == KeyCode.UpArrow) return "Up Arrow";
                else if (k.Key == KeyCode.DownArrow) return "Down Arrow";
                else if (k.Key == KeyCode.LeftArrow) return "Left Arrow";
                else if (k.Key == KeyCode.RightArrow) return "Right Arrow";
                else if (k.Key == KeyCode.None) return "...";


                return k.Key.ToString();
            }
        }
        return "";
    }
    //Gets the keycode from the key's name
    public static KeyCode getKeyfromName(string name)
    {
        if (CheckIfKeyNameExists(name))
        {
            foreach (key k in Keys)
            {
                if (k.Name == name) return k.Key;
            }
        }
        //Return a key that Doesn't exist,like f15
        Debug.LogError("Key doesn't exist , Create one using SetKey");
        return KeyCode.F15;
    }
    
    //Just like the Unity's
    public static bool GetButtonUp(string name)
    {
        if (CheckIfKeyNameExists(name))
        {
            return Input.GetKeyUp(getKeyfromName(name));
        }
        else
        {
            Debug.LogError("Key Not Assigned. Create one using SetKey");
            return false;
        }
    }
    //Just like the Unity's
    public static bool GetButtonDown(string name)
    {
        if (CheckIfKeyNameExists(name))
        {
            return Input.GetKeyDown(getKeyfromName(name));
        }
        else
        {
            Debug.LogError("Key Not Assigned. Create one using SetKey");
            return false;
        }
    }
    
    //Same Comment 
    public static bool GetButton(string name)
    {
        if (CheckIfKeyNameExists(name))
        {
            return Input.GetKey(getKeyfromName(name));
        }
        else
        {
            Debug.LogError("Key Not Assigned. Create one using SetKey");
            return false;
        }
    }
    //Deletes a Saved List
    public static void DeleteKeyList(string keyListID)
    {
        //Since we don't know how many keys we saved , we loop infinity times and stop when we finish
        for (int i = 0; i < Mathf.Infinity; i++)
        {
            //Checks if that key actually exists (it exists if it's simply not the default value "" which is nothing)
            if (PlayerPrefs.GetString(keyListID + i.ToString(), "") != "")
            {
                //delete the key by setting it to the default value
                PlayerPrefs.SetString(keyListID + i.ToString(), "");
                PlayerPrefs.SetString(keyListID + i.ToString() + "key", "");
                //Gets to the next string
                continue;
            }
            //If that key doesn't exist (means we're done) , break the infinite loop so we won't get a freeze.
            else break;
        }
    }
    //Saves a list of keys
    public static void SaveKeys(string keyListID)
    {
        //First , we get rid of the old list
        DeleteKeyList(keyListID);
        //Then , we save the keys
        for (int i = 0; i < Keys.Count;i++)
        {
            PlayerPrefs.SetString(keyListID + i.ToString(), Keys[i].Name);
            PlayerPrefs.SetString(keyListID + i.ToString() + "key", Keys[i].Key.ToString());
        }
        Debug.Log("List Successfully Saved");
    }
    public static void LoadKeys(string keyListID)
    {
        //We get rid of what's in Keys
        Keys.Clear();
        //We loop infinity times because we don't know how many keys we saved
        for (int i = 0; i < Mathf.Infinity;i++)
        {
            //If a key exists , add it
            if (PlayerPrefs.GetString(keyListID + i.ToString(), "") != "")
            {
                Keys.Add(new key(PlayerPrefs.GetString(keyListID + i.ToString(), ""), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyListID + i.ToString() + "key", ""))));
            }
            //when we're done , break
            else
            {
                Debug.Log("List successfully loaded.");
                break;
            }
        }
    }
    //Sets a key
    public static void SetKey(string name, KeyCode keycode)
    {
        //Checks if button is not assigned and name isn't already there
        if (!CheckIfKeyNameExists(name) && !CheckIfKeyCodeExists(keycode))
        {
            //Simply adds the key
            Keys.Add(new key(name, keycode));
            Debug.Log("Key " + name + " added successfully");
        }
        else
        {
            //If they already exist log errors
            if (CheckIfKeyNameExists(name))
                Debug.LogError("Name Already Used");
            if (CheckIfKeyCodeExists(keycode))
                Debug.LogError("Key Already Assigned");
        }
    }
   
/// END KEY PART

/// AXIS PART
    //Repeat the Check booleans
    public static bool CheckIfAxisNameExists(string name)
    {
        foreach (axis ax in Axis)
        {
            if (ax.Name == name) return true;
        }
        return false;
    }
    //Checks if postive and negative keys are used
    public static bool CheckIfAxisCodeExists(KeyCode positive,KeyCode negative)
    {
        foreach (axis ax in Axis)
        {
            if (ax.PositiveKey == positive && ax.NegativeKey == negative) return true;
        }
        return false;
    }
    //Gets Postitive key name
    public static string GetPositiveAxisKeyText(string name)
    {
        if (CheckIfAxisNameExists(name))
        {
            foreach (axis ax in Axis)
            {
                if (ax.Name == name)
                {
                    if (ax.PositiveKey == KeyCode.Mouse0) return "Left Mouse Button";
                    else if (ax.PositiveKey == KeyCode.Mouse1) return "Right Mouse Button";
                    else if (ax.PositiveKey == KeyCode.Mouse2) return "Middle Mouse Button";
                    else if (ax.PositiveKey == KeyCode.LeftAlt) return "Left Alt";
                    else if (ax.PositiveKey == KeyCode.AltGr) return "Right Alt";
                    else if (ax.PositiveKey == KeyCode.LeftShift) return "Left Shift";
                    else if (ax.PositiveKey == KeyCode.RightShift) return "Right Shift";
                    else if (ax.PositiveKey == KeyCode.LeftControl) return "Left Control";
                    else if (ax.PositiveKey == KeyCode.RightControl) return "Right Control";
                    else if (ax.PositiveKey == KeyCode.KeypadEnter) return "Numpad Enter";
                    else if (ax.PositiveKey == KeyCode.Alpha0) return "0";
                    else if (ax.PositiveKey == KeyCode.Alpha1) return "1";
                    else if (ax.PositiveKey == KeyCode.Alpha2) return "2";
                    else if (ax.PositiveKey == KeyCode.Alpha3) return "3";
                    else if (ax.PositiveKey == KeyCode.Alpha4) return "4";
                    else if (ax.PositiveKey == KeyCode.Alpha5) return "5";
                    else if (ax.PositiveKey == KeyCode.Alpha6) return "6";
                    else if (ax.PositiveKey == KeyCode.Alpha7) return "7";
                    else if (ax.PositiveKey == KeyCode.Alpha8) return "8";
                    else if (ax.PositiveKey == KeyCode.Alpha9) return "9";
                    else if (ax.PositiveKey == KeyCode.ScrollLock) return "Scroll Lock";
                    else if (ax.PositiveKey == KeyCode.PageUp) return "Page Up";
                    else if (ax.PositiveKey == KeyCode.PageDown) return "Page Down";
                    else if (ax.PositiveKey == KeyCode.UpArrow) return "Up Arrow";
                    else if (ax.PositiveKey == KeyCode.DownArrow) return "Down Arrow";
                    else if (ax.PositiveKey == KeyCode.LeftArrow) return "Left Arrow";
                    else if (ax.PositiveKey == KeyCode.RightArrow) return "Right Arrow";
                    else if (ax.PositiveKey == KeyCode.None) return "...";

                    return ax.PositiveKey.ToString();
                }
            }
        }
        Debug.LogError("Axis Doesn't Exist.Create one using SetAxis");
        return "F15";
    }
    //Gets Negative key name
    public static string GetNegativeAxisKeyText(string name)
    {
        if (CheckIfAxisNameExists(name))
        {
            foreach (axis ax in Axis)
            {
                if (ax.Name == name)
                {
                    if (ax.NegativeKey == KeyCode.Mouse0) return "Left Mouse Button";
                    else if (ax.NegativeKey == KeyCode.Mouse1) return "Right Mouse Button";
                    else if (ax.NegativeKey == KeyCode.Mouse2) return "Middle Mouse Button";
                    else if (ax.NegativeKey == KeyCode.LeftAlt) return "Left Alt";
                    else if (ax.NegativeKey == KeyCode.AltGr) return "Right Alt";
                    else if (ax.NegativeKey == KeyCode.LeftShift) return "Left Shift";
                    else if (ax.NegativeKey == KeyCode.RightShift) return "Right Shift";
                    else if (ax.NegativeKey == KeyCode.LeftControl) return "Left Control";
                    else if (ax.NegativeKey == KeyCode.RightControl) return "Right Control";
                    else if (ax.NegativeKey == KeyCode.KeypadEnter) return "Numpad Enter";
                    else if (ax.NegativeKey == KeyCode.Alpha0) return "0";
                    else if (ax.NegativeKey == KeyCode.Alpha1) return "1";
                    else if (ax.NegativeKey == KeyCode.Alpha2) return "2";
                    else if (ax.NegativeKey == KeyCode.Alpha3) return "3";
                    else if (ax.NegativeKey == KeyCode.Alpha4) return "4";
                    else if (ax.NegativeKey == KeyCode.Alpha5) return "5";
                    else if (ax.NegativeKey == KeyCode.Alpha6) return "6";
                    else if (ax.NegativeKey == KeyCode.Alpha7) return "7";
                    else if (ax.NegativeKey == KeyCode.Alpha8) return "8";
                    else if (ax.NegativeKey == KeyCode.Alpha9) return "9";
                    else if (ax.NegativeKey == KeyCode.ScrollLock) return "Scroll Lock";
                    else if (ax.NegativeKey == KeyCode.PageUp) return "Page Up";
                    else if (ax.NegativeKey == KeyCode.PageDown) return "Page Down";
                    else if (ax.NegativeKey == KeyCode.UpArrow) return "Up Arrow";
                    else if (ax.NegativeKey == KeyCode.DownArrow) return "Down Arrow";
                    else if (ax.NegativeKey == KeyCode.LeftArrow) return "Left Arrow";
                    else if (ax.NegativeKey == KeyCode.RightArrow) return "Right Arrow";
                    else if (ax.NegativeKey == KeyCode.None) return "...";
                    return ax.NegativeKey.ToString();
                }
            }
        }
        Debug.LogError("Axis Doesn't Exist. Create one using SetAxis");
        return "F15";
    }
    //Gets the positive Code
    public static KeyCode GetPositiveAxisKeyCode(string name)
    {
        if (CheckIfAxisNameExists(name))
        {
            foreach (axis ax in Axis)
            {
                if (ax.Name == name) return ax.PositiveKey;
            }
        }
        Debug.LogError("Axis Doesn't Exist. Create one using SetAxis");
        return KeyCode.F15;
    }
    //Gets the Negative Code
    public static KeyCode GetNegativeAxisKeyCode(string name)
    {
        if (CheckIfAxisNameExists(name))
        {
            foreach (axis ax in Axis)
            {

                if (ax.Name == name) { 

                    return ax.NegativeKey;
                }
            }
        }
        Debug.LogError("Axis Doesn't Exist. Create one using SetAxis");
        return KeyCode.F15;
    }
    //Same as unity's
    public static float GetAxis(string name)
    {
        if (CheckIfAxisNameExists(name))
        {
            foreach (axis ax in Axis)
            {
                if (ax.Name == name) return ax.axVal;
            }
        }
        Debug.LogError("Axis Doesn't Exist. Create one using SetAxis");
        return 0f;
    }
    public static void SetAxis(string name,KeyCode pos,KeyCode neg,float grav)
    {
        if (!CheckIfAxisNameExists(name) && !CheckIfAxisCodeExists(pos,neg))
        {
            Axis.Add(new axis(name, pos, neg, grav));
        }
        else
        {
            if (CheckIfAxisNameExists(name)) Debug.LogError("Axis already exists.");
            if (CheckIfAxisCodeExists(pos, neg)) Debug.LogError("Buttons already assigned.");
        }
    }
    //Deletes a Saved List (same way)
    public static void DeleteAxisList(string axisListID)
    {
        for (int i = 0; i < Mathf.Infinity; i++)
        {
            if (PlayerPrefs.GetString(axisListID + i.ToString(), "") != "")
            {
                PlayerPrefs.SetString(axisListID + i.ToString(), "");
                PlayerPrefs.SetString(axisListID + i.ToString() + "poskey", "");
                PlayerPrefs.SetString(axisListID + i.ToString() + "negkey", "");
                PlayerPrefs.SetFloat(axisListID + i.ToString() + "gravity", -99f);

                continue;
            }
            else break;
        }
    }
    //Saves the list the same way
    public static void SaveAxisList(string axisListID)
    {
        DeleteAxisList(axisListID);
        for (int i = 0; i < Axis.Count;i++)
        {
            PlayerPrefs.SetString(axisListID + i.ToString(), Axis[i].Name);
            PlayerPrefs.SetString(axisListID + i.ToString() + "poskey", Axis[i].PositiveKey.ToString());
            PlayerPrefs.SetString(axisListID + i.ToString() + "negkey", Axis[i].NegativeKey.ToString());
            PlayerPrefs.SetFloat(axisListID + i.ToString() + "gravity", Axis[i].gravity);
        }
        Debug.Log("Saving Axis Done");

    }
    //Also loads it the same way
    public static void LoadAxisList(string axisListID)
    {
        Axis.Clear();
        for (int i = 0; i < Mathf.Infinity; i++)
        {
            if (PlayerPrefs.GetString(axisListID + i.ToString(), "") != "")
            {
                Axis.Add(new axis(PlayerPrefs.GetString(axisListID + i.ToString(), ""), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(axisListID + i.ToString() + "poskey")), (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(axisListID + i.ToString() + "negkey")), PlayerPrefs.GetFloat(axisListID + i.ToString() + "gravity")));

                //Gets to the next string
                continue;
            }
            else
            {
                Debug.Log("Loading Axis List Done");
                break;
            }
        }
    }
    public static void ChangeAxisButtons(string name,KeyCode pos,KeyCode neg)
    {
        if (CheckIfAxisNameExists(name))
        {
            for (int i = 0; i < Axis.Count;i++)
            {
                if (Axis[i].Name == name)
                {
                    Axis[i] = new axis(name, pos, neg, Axis[i].gravity);
                }
            }
        }
        else
        {
            Debug.LogError("Axis doesn't exist , use SetAxis instead.");
        }
    }
    public static KeyCode keyPressed()
    {
///Handles All Keycodes
        if (Input.GetKey(KeyCode.A)) return KeyCode.A;
        else if (Input.GetKey(KeyCode.B)) return KeyCode.B;
        else if (Input.GetKey(KeyCode.C)) return KeyCode.C;
        else if (Input.GetKey(KeyCode.D)) return KeyCode.D;
        else if (Input.GetKey(KeyCode.E)) return KeyCode.E;
        else if (Input.GetKey(KeyCode.F)) return KeyCode.F;
        else if (Input.GetKey(KeyCode.G)) return KeyCode.G;
        else if (Input.GetKey(KeyCode.H)) return KeyCode.H;
        else if (Input.GetKey(KeyCode.I)) return KeyCode.I;
        else if (Input.GetKey(KeyCode.J)) return KeyCode.J;
        else if (Input.GetKey(KeyCode.K)) return KeyCode.K;
        else if (Input.GetKey(KeyCode.L)) return KeyCode.L;
        else if (Input.GetKey(KeyCode.M)) return KeyCode.M;
        else if (Input.GetKey(KeyCode.N)) return KeyCode.N;
        else if (Input.GetKey(KeyCode.O)) return KeyCode.O;
        else if (Input.GetKey(KeyCode.P)) return KeyCode.P;
        else if (Input.GetKey(KeyCode.Q)) return KeyCode.Q;
        else if (Input.GetKey(KeyCode.R)) return KeyCode.R;
        else if (Input.GetKey(KeyCode.S)) return KeyCode.S;
        else if (Input.GetKey(KeyCode.T)) return KeyCode.T;
        else if (Input.GetKey(KeyCode.U)) return KeyCode.U;
        else if (Input.GetKey(KeyCode.V)) return KeyCode.V;
        else if (Input.GetKey(KeyCode.W)) return KeyCode.W;
        else if (Input.GetKey(KeyCode.X)) return KeyCode.X;
        else if (Input.GetKey(KeyCode.Y)) return KeyCode.Y;
        else if (Input.GetKey(KeyCode.Z)) return KeyCode.Z;
        else if (Input.GetKey(KeyCode.LeftAlt)) return KeyCode.LeftAlt;
        else if (Input.GetKey(KeyCode.AltGr)) return KeyCode.AltGr;
        else if (Input.GetKey(KeyCode.LeftControl)) return KeyCode.LeftControl;
        else if (Input.GetKey(KeyCode.LeftShift)) return KeyCode.LeftShift;
        else if (Input.GetKey(KeyCode.RightAlt)) return KeyCode.RightAlt;
        else if (Input.GetKey(KeyCode.RightControl)) return KeyCode.RightControl;
        else if (Input.GetKey(KeyCode.RightShift)) return KeyCode.RightShift;
        else if (Input.GetKey(KeyCode.Return)) return KeyCode.Return;
        else if (Input.GetKey(KeyCode.Backspace)) return KeyCode.Backspace;
        else if (Input.GetKey(KeyCode.Mouse0)) return KeyCode.Mouse0;
        else if (Input.GetKey(KeyCode.Mouse1)) return KeyCode.Mouse1;
        else if (Input.GetKey(KeyCode.Mouse2)) return KeyCode.Mouse2;
        else if (Input.GetKey(KeyCode.Mouse3)) return KeyCode.Mouse3;
        else if (Input.GetKey(KeyCode.Mouse4)) return KeyCode.Mouse4;
        else if (Input.GetKey(KeyCode.F1)) return KeyCode.F1;
        else if (Input.GetKey(KeyCode.F2)) return KeyCode.F2;
        else if (Input.GetKey(KeyCode.F3)) return KeyCode.F3;
        else if (Input.GetKey(KeyCode.F4)) return KeyCode.F4;
        else if (Input.GetKey(KeyCode.F5)) return KeyCode.F5;
        else if (Input.GetKey(KeyCode.F6)) return KeyCode.F6;
        else if (Input.GetKey(KeyCode.F7)) return KeyCode.F7;
        else if (Input.GetKey(KeyCode.F8)) return KeyCode.F8;
        else if (Input.GetKey(KeyCode.F9)) return KeyCode.F9;
        else if (Input.GetKey(KeyCode.F10)) return KeyCode.F10;
        else if (Input.GetKey(KeyCode.F11)) return KeyCode.F11;
        else if (Input.GetKey(KeyCode.F12)) return KeyCode.F12;
        else if (Input.GetKey(KeyCode.Keypad0)) return KeyCode.Keypad0;
        else if (Input.GetKey(KeyCode.Keypad1)) return KeyCode.Keypad1;
        else if (Input.GetKey(KeyCode.Keypad2)) return KeyCode.Keypad2;
        else if (Input.GetKey(KeyCode.Keypad3)) return KeyCode.Keypad3;
        else if (Input.GetKey(KeyCode.Keypad4)) return KeyCode.Keypad4;
        else if (Input.GetKey(KeyCode.Keypad5)) return KeyCode.Keypad5;
        else if (Input.GetKey(KeyCode.Keypad6)) return KeyCode.Keypad6;
        else if (Input.GetKey(KeyCode.Keypad7)) return KeyCode.Keypad7;
        else if (Input.GetKey(KeyCode.Keypad8)) return KeyCode.Keypad8;
        else if (Input.GetKey(KeyCode.Keypad9)) return KeyCode.Keypad9;
        else if (Input.GetKey(KeyCode.KeypadEnter)) return KeyCode.KeypadEnter;
        else if (Input.GetKey(KeyCode.LeftArrow)) return KeyCode.LeftArrow;
        else if (Input.GetKey(KeyCode.RightArrow)) return KeyCode.RightArrow;
        else if (Input.GetKey(KeyCode.UpArrow)) return KeyCode.UpArrow;
        else if (Input.GetKey(KeyCode.DownArrow)) return KeyCode.DownArrow;
        else if (Input.GetKey(KeyCode.Tab)) return KeyCode.Tab;
        else if (Input.GetKey(KeyCode.CapsLock)) return KeyCode.CapsLock;
        else if (Input.GetKey(KeyCode.Delete)) return KeyCode.Delete;
        else if (Input.GetKey(KeyCode.Insert)) return KeyCode.Insert;
        else if (Input.GetKey(KeyCode.End)) return KeyCode.End;
        else if (Input.GetKey(KeyCode.Home)) return KeyCode.Home;
        else if (Input.GetKey(KeyCode.PageUp)) return KeyCode.PageUp;
        else if (Input.GetKey(KeyCode.PageDown)) return KeyCode.PageDown;
        else if (Input.GetKey(KeyCode.Print)) return KeyCode.Print;
        else if (Input.GetKey(KeyCode.ScrollLock)) return KeyCode.ScrollLock;
        else if (Input.GetKey(KeyCode.Pause)) return KeyCode.Pause;
        else if (Input.GetKey(KeyCode.KeypadPeriod)) return KeyCode.KeypadPeriod;
        else if (Input.GetKey(KeyCode.KeypadPlus)) return KeyCode.KeypadPlus;
        else if (Input.GetKey(KeyCode.KeypadMinus)) return KeyCode.KeypadMinus;
        else if (Input.GetKey(KeyCode.KeypadMultiply)) return KeyCode.KeypadMultiply;
        else if (Input.GetKey(KeyCode.KeypadDivide)) return KeyCode.KeypadDivide;
        else if (Input.GetKey(KeyCode.Numlock)) return KeyCode.Numlock;
        else if (Input.GetKey(KeyCode.Clear)) return KeyCode.Clear;
        else if (Input.GetKey(KeyCode.Escape)) return KeyCode.Escape;
        else if (Input.GetKey(KeyCode.Alpha0)) return KeyCode.Alpha0;
        else if (Input.GetKey(KeyCode.Alpha1)) return KeyCode.Alpha1;
        else if (Input.GetKey(KeyCode.Alpha2)) return KeyCode.Alpha2;
        else if (Input.GetKey(KeyCode.Alpha3)) return KeyCode.Alpha3;
        else if (Input.GetKey(KeyCode.Alpha4)) return KeyCode.Alpha4;
        else if (Input.GetKey(KeyCode.Alpha5)) return KeyCode.Alpha5;
        else if (Input.GetKey(KeyCode.Alpha6)) return KeyCode.Alpha6;
        else if (Input.GetKey(KeyCode.Alpha7)) return KeyCode.Alpha7;
        else if (Input.GetKey(KeyCode.Alpha8)) return KeyCode.Alpha8;
        else if (Input.GetKey(KeyCode.Alpha9)) return KeyCode.Alpha9;


        else if (Input.GetKey(KeyCode.Joystick1Button0)) return KeyCode.Joystick1Button0;
        else if (Input.GetKey(KeyCode.Joystick1Button1)) return KeyCode.Joystick1Button1;
        else if (Input.GetKey(KeyCode.Joystick1Button2)) return KeyCode.Joystick1Button2;
        else if (Input.GetKey(KeyCode.Joystick1Button3)) return KeyCode.Joystick1Button3;
        else if (Input.GetKey(KeyCode.Joystick1Button4)) return KeyCode.Joystick1Button4;
        else if (Input.GetKey(KeyCode.Joystick1Button5)) return KeyCode.Joystick1Button5;
        else if (Input.GetKey(KeyCode.Joystick1Button6)) return KeyCode.Joystick1Button6;
        else if (Input.GetKey(KeyCode.Joystick1Button7)) return KeyCode.Joystick1Button7;
        else if (Input.GetKey(KeyCode.Joystick1Button8)) return KeyCode.Joystick1Button8;
        else if (Input.GetKey(KeyCode.Joystick1Button9)) return KeyCode.Joystick1Button9;
        else if (Input.GetKey(KeyCode.Joystick1Button10)) return KeyCode.Joystick1Button10;
        else if (Input.GetKey(KeyCode.Joystick1Button11)) return KeyCode.Joystick1Button11;
        else if (Input.GetKey(KeyCode.Joystick1Button12)) return KeyCode.Joystick1Button12;
        else if (Input.GetKey(KeyCode.Joystick1Button13)) return KeyCode.Joystick1Button13;
        else if (Input.GetKey(KeyCode.Joystick1Button14)) return KeyCode.Joystick1Button14;
        else if (Input.GetKey(KeyCode.Joystick1Button15)) return KeyCode.Joystick1Button15;
        else if (Input.GetKey(KeyCode.Joystick1Button16)) return KeyCode.Joystick1Button16;
        else if (Input.GetKey(KeyCode.Joystick1Button17)) return KeyCode.Joystick1Button17;
        else if (Input.GetKey(KeyCode.Joystick1Button18)) return KeyCode.Joystick1Button18;
        else if (Input.GetKey(KeyCode.Joystick1Button19)) return KeyCode.Joystick1Button19;

        else if (Input.GetKey(KeyCode.Joystick2Button0)) return KeyCode.Joystick2Button0;
        else if (Input.GetKey(KeyCode.Joystick2Button1)) return KeyCode.Joystick2Button1;
        else if (Input.GetKey(KeyCode.Joystick2Button2)) return KeyCode.Joystick2Button2;
        else if (Input.GetKey(KeyCode.Joystick2Button3)) return KeyCode.Joystick2Button3;
        else if (Input.GetKey(KeyCode.Joystick2Button4)) return KeyCode.Joystick2Button4;
        else if (Input.GetKey(KeyCode.Joystick2Button5)) return KeyCode.Joystick2Button5;
        else if (Input.GetKey(KeyCode.Joystick2Button6)) return KeyCode.Joystick2Button6;
        else if (Input.GetKey(KeyCode.Joystick2Button7)) return KeyCode.Joystick2Button7;
        else if (Input.GetKey(KeyCode.Joystick2Button8)) return KeyCode.Joystick2Button8;
        else if (Input.GetKey(KeyCode.Joystick2Button9)) return KeyCode.Joystick2Button9;
        else if (Input.GetKey(KeyCode.Joystick2Button10)) return KeyCode.Joystick2Button10;
        else if (Input.GetKey(KeyCode.Joystick2Button11)) return KeyCode.Joystick2Button11;
        else if (Input.GetKey(KeyCode.Joystick2Button12)) return KeyCode.Joystick2Button12;
        else if (Input.GetKey(KeyCode.Joystick2Button13)) return KeyCode.Joystick2Button13;
        else if (Input.GetKey(KeyCode.Joystick2Button14)) return KeyCode.Joystick2Button14;
        else if (Input.GetKey(KeyCode.Joystick2Button15)) return KeyCode.Joystick2Button15;
        else if (Input.GetKey(KeyCode.Joystick2Button16)) return KeyCode.Joystick2Button16;
        else if (Input.GetKey(KeyCode.Joystick2Button17)) return KeyCode.Joystick2Button17;
        else if (Input.GetKey(KeyCode.Joystick2Button18)) return KeyCode.Joystick2Button18;
        else if (Input.GetKey(KeyCode.Joystick2Button19)) return KeyCode.Joystick2Button19;

        else if (Input.GetKey(KeyCode.Joystick3Button0)) return KeyCode.Joystick3Button0;
        else if (Input.GetKey(KeyCode.Joystick3Button1)) return KeyCode.Joystick3Button1;
        else if (Input.GetKey(KeyCode.Joystick3Button2)) return KeyCode.Joystick3Button2;
        else if (Input.GetKey(KeyCode.Joystick3Button3)) return KeyCode.Joystick3Button3;
        else if (Input.GetKey(KeyCode.Joystick3Button4)) return KeyCode.Joystick3Button4;
        else if (Input.GetKey(KeyCode.Joystick3Button5)) return KeyCode.Joystick3Button5;
        else if (Input.GetKey(KeyCode.Joystick3Button6)) return KeyCode.Joystick3Button6;
        else if (Input.GetKey(KeyCode.Joystick3Button7)) return KeyCode.Joystick3Button7;
        else if (Input.GetKey(KeyCode.Joystick3Button8)) return KeyCode.Joystick3Button8;
        else if (Input.GetKey(KeyCode.Joystick3Button9)) return KeyCode.Joystick3Button9;
        else if (Input.GetKey(KeyCode.Joystick3Button10)) return KeyCode.Joystick3Button10;
        else if (Input.GetKey(KeyCode.Joystick3Button11)) return KeyCode.Joystick3Button11;
        else if (Input.GetKey(KeyCode.Joystick3Button12)) return KeyCode.Joystick3Button12;
        else if (Input.GetKey(KeyCode.Joystick3Button13)) return KeyCode.Joystick3Button13;
        else if (Input.GetKey(KeyCode.Joystick3Button14)) return KeyCode.Joystick3Button14;
        else if (Input.GetKey(KeyCode.Joystick3Button15)) return KeyCode.Joystick3Button15;
        else if (Input.GetKey(KeyCode.Joystick3Button16)) return KeyCode.Joystick3Button16;
        else if (Input.GetKey(KeyCode.Joystick3Button17)) return KeyCode.Joystick3Button17;
        else if (Input.GetKey(KeyCode.Joystick3Button18)) return KeyCode.Joystick3Button18;
        else if (Input.GetKey(KeyCode.Joystick3Button19)) return KeyCode.Joystick3Button19;

        else if (Input.GetKey(KeyCode.Joystick4Button0)) return KeyCode.Joystick4Button0;
        else if (Input.GetKey(KeyCode.Joystick4Button1)) return KeyCode.Joystick4Button1;
        else if (Input.GetKey(KeyCode.Joystick4Button2)) return KeyCode.Joystick4Button2;
        else if (Input.GetKey(KeyCode.Joystick4Button3)) return KeyCode.Joystick4Button3;
        else if (Input.GetKey(KeyCode.Joystick4Button4)) return KeyCode.Joystick4Button4;
        else if (Input.GetKey(KeyCode.Joystick4Button5)) return KeyCode.Joystick4Button5;
        else if (Input.GetKey(KeyCode.Joystick4Button6)) return KeyCode.Joystick4Button6;
        else if (Input.GetKey(KeyCode.Joystick4Button7)) return KeyCode.Joystick4Button7;
        else if (Input.GetKey(KeyCode.Joystick4Button8)) return KeyCode.Joystick4Button8;
        else if (Input.GetKey(KeyCode.Joystick4Button9)) return KeyCode.Joystick4Button9;
        else if (Input.GetKey(KeyCode.Joystick4Button10)) return KeyCode.Joystick4Button10;
        else if (Input.GetKey(KeyCode.Joystick4Button11)) return KeyCode.Joystick4Button11;
        else if (Input.GetKey(KeyCode.Joystick4Button12)) return KeyCode.Joystick4Button12;
        else if (Input.GetKey(KeyCode.Joystick4Button13)) return KeyCode.Joystick4Button13;
        else if (Input.GetKey(KeyCode.Joystick4Button14)) return KeyCode.Joystick4Button14;
        else if (Input.GetKey(KeyCode.Joystick4Button15)) return KeyCode.Joystick4Button15;
        else if (Input.GetKey(KeyCode.Joystick4Button16)) return KeyCode.Joystick4Button16;
        else if (Input.GetKey(KeyCode.Joystick4Button17)) return KeyCode.Joystick4Button17;
        else if (Input.GetKey(KeyCode.Joystick4Button18)) return KeyCode.Joystick4Button18;
        else if (Input.GetKey(KeyCode.Joystick4Button19)) return KeyCode.Joystick4Button19;

        else if (Input.GetKey(KeyCode.Exclaim)) return KeyCode.Exclaim;
        else if (Input.GetKey(KeyCode.DoubleQuote)) return KeyCode.DoubleQuote;
        else if (Input.GetKey(KeyCode.Hash)) return KeyCode.Hash;
        else if (Input.GetKey(KeyCode.Dollar)) return KeyCode.Dollar;
        else if (Input.GetKey(KeyCode.Ampersand)) return KeyCode.Ampersand;
        else if (Input.GetKey(KeyCode.Quote)) return KeyCode.Quote;
        else if (Input.GetKey(KeyCode.LeftParen)) return KeyCode.LeftParen;
        else if (Input.GetKey(KeyCode.RightParen)) return KeyCode.RightParen;
        else if (Input.GetKey(KeyCode.Asterisk)) return KeyCode.Asterisk;
        else if (Input.GetKey(KeyCode.Comma)) return KeyCode.Comma;
        else if (Input.GetKey(KeyCode.Plus)) return KeyCode.Plus;
        else if (Input.GetKey(KeyCode.Minus)) return KeyCode.Minus;
        else if (Input.GetKey(KeyCode.Period)) return KeyCode.Period;
        else if (Input.GetKey(KeyCode.Slash)) return KeyCode.Slash;
        else if (Input.GetKey(KeyCode.Colon)) return KeyCode.Colon;
        else if (Input.GetKey(KeyCode.Semicolon)) return KeyCode.Semicolon;
        else if (Input.GetKey(KeyCode.Less)) return KeyCode.Less;
        else if (Input.GetKey(KeyCode.Equals)) return KeyCode.Equals;
        else if (Input.GetKey(KeyCode.Greater)) return KeyCode.Greater;
        else if (Input.GetKey(KeyCode.Question)) return KeyCode.Question;
        else if (Input.GetKey(KeyCode.At)) return KeyCode.At;
        else if (Input.GetKey(KeyCode.LeftBracket)) return KeyCode.LeftBracket;
        else if (Input.GetKey(KeyCode.Backslash)) return KeyCode.Backslash;
        else if (Input.GetKey(KeyCode.RightBracket)) return KeyCode.RightBracket;
        else if (Input.GetKey(KeyCode.Caret)) return KeyCode.Caret;
        else if (Input.GetKey(KeyCode.Underscore)) return KeyCode.Underscore;
        else if (Input.GetKey(KeyCode.BackQuote)) return KeyCode.BackQuote;
        else if (Input.GetKey(KeyCode.Help)) return KeyCode.Help;
        else if (Input.GetKey(KeyCode.SysReq)) return KeyCode.SysReq;
        else if (Input.GetKey(KeyCode.Break)) return KeyCode.Break;
        else if (Input.GetKey(KeyCode.Menu)) return KeyCode.Menu;









        



        //this'll return a non-existent button if the button pressed doesn't exist , you can change the default
        return KeyCode.F15;
    }

    
    //Check if an axis/keys list exists
    public static bool ListExists(string listID)
    {
        if (PlayerPrefs.GetString(listID + "0", "") != "") return true;
        else return false;
    }
    //DOES NOT WORK ON WEB
    //Saves both lists in a file
    public static void SaveListsInFile(string keyfileName,bool ispath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file;
        //if we didn't set a path , put it in persistentDataPath
        if (!ispath)
        {
            file = File.Open(Application.persistentDataPath + "/" + keyfileName, FileMode.OpenOrCreate);
        }
        //else save in the directroy specified
        else
        {
            file = File.Open(keyfileName,FileMode.OpenOrCreate);
        }
        //Intilializing 2 list instances of the serializable key and axis
        List<keySer> kList = new List<keySer>();
        List<axSer> aList = new List<axSer>();
        //Add keys to the serializable list
        foreach (key k in Keys)
        {
            kList.Add(new keySer(k.Name, k.Key));
        }
        //Add axises
        foreach (axis a in Axis)
        {
            aList.Add(new axSer(a.Name, a.PositiveKey, a.NegativeKey, a.gravity));
        }
        //Makes the serializable class
        keyAxisSaveData ksd = new keyAxisSaveData(kList, aList);
        //Serializes it
        formatter.Serialize(file, ksd);
        file.Close();
    }
    //Loading
    public static void LoadListsInFile(string keyfileName,bool ispath)
    {
        //If it exists do this
        if (File.Exists(Application.persistentDataPath + "/" + keyfileName))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file;
            //Same path checking method
            if (!ispath)
            {
                file = File.Open(Application.persistentDataPath + "/" + keyfileName, FileMode.OpenOrCreate);
            }
            else
            {
                file = File.Open(keyfileName, FileMode.OpenOrCreate);
            }
            //deserialize the ksd
            keyAxisSaveData ksd = formatter.Deserialize(file) as keyAxisSaveData;
            //delete keys and axis list
            Keys.Clear();
            Axis.Clear();
            //Add keys and axises
            foreach (keySer k in ksd.keys)
            {
                SetKey(k.Name, k.Key);
            }
            foreach (axSer ax in ksd.axiss)
            {
                SetAxis(ax.Name, ax.pos, ax.neg, ax.gravity);
            }
            file.Close();
        }
    }
    //Checks if list exists
    public static bool ListInFileExists(string keyfileName,bool ispath)
    {
        if (!ispath)
        {
            return File.Exists(Application.persistentDataPath + "/" + keyfileName);
        }
        else return File.Exists(keyfileName);
    }

}
//List serializer class
[System.Serializable]
class keyAxisSaveData
{
    public List<keySer> keys;
    public List<axSer> axiss;
    public keyAxisSaveData(List<keySer> keyz, List<axSer> axisz)
    {
        keys = keyz;
        axiss = axisz;
    }
}
//key serializer class
[System.Serializable]
class keySer
{
    public string Name;
    public KeyCode Key;
    public keySer(string name,KeyCode kc)
    {
        Name = name;
        Key = kc;
    }
}
//Axis serializer class
[System.Serializable]
class axSer
{
    public string Name;
    public KeyCode pos;
    public KeyCode neg;
    public float gravity;
    public axSer(string name,KeyCode p,KeyCode n,float g)
    {
        Name = name;
        pos = p;
        neg = n;
        gravity = g;
    }
}
//The key struct , stores a name and a keycode

[System.Serializable]
public struct key
{
    public string Name;
    public KeyCode Key;
    //Constructor requires a name and a keycode
    public key(string name, KeyCode kc)
    {
        Name = name;
        Key = kc;
    }
}
//The axis class (i used a class because the axVal needs to be changed at runtime without recreating the whole axis)
[System.Serializable]
public class axis
{
    public string Name;
    public KeyCode PositiveKey;
    public KeyCode NegativeKey;
    public float axVal;
    public float gravity;
    //Setting this to 3 is default
    public axis(string name,KeyCode positivekc,KeyCode negativekc,float Gravity)
    {
        Name = name;
        PositiveKey = positivekc;
        NegativeKey = negativekc;
        axVal = 0;
        gravity = Gravity;
    }
}
