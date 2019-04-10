using UnityEngine;
using System.Collections;

public class Setup_sInput : MonoBehaviour {

    public keyGenerator[] Keys;
    public axisGenerator[] Axis;
    public enum LoadType
    {
        Do_Not_Load,
        Load_From_Playerprefs,
        Load_From_File
    };
    public string m_KeyListName;
    public string m_AxisListName;
    public string m_FileName;
    public bool m_isPath;

    public LoadType m_LoadType;

    void Awake()
    {
        if (m_LoadType == LoadType.Do_Not_Load)
        {
            for (int i = 0; i < Keys.Length; i++)
            {
                if (Keys[i].Name.Length > 0)
                {
                    sInput.SetKey(Keys[i].Name, Keys[i].m_KeyCode);
                }
            }
            for (int i = 0; i < Axis.Length; i++)
            {
                if (Axis[i].Gravity == 0) Axis[i].Gravity = 3;
                if (Axis[i].Name.Length > 0)
                {
                    sInput.SetAxis(Axis[i].Name, Axis[i].PositiveKey, Axis[i].NegativeKey, Axis[i].Gravity);
                }
            }
        }
        else if (m_LoadType == LoadType.Load_From_Playerprefs)
        {
            if (sInput.ListExists(m_KeyListName))
            {
                sInput.LoadKeys(m_KeyListName);
            }
            else 
            {
                for (int i = 0; i < Keys.Length; i++)
                {
                    if (Keys[i].Name.Length > 0)
                    {
                        sInput.SetKey(Keys[i].Name, Keys[i].m_KeyCode);
                    }
                }
            }
            if (sInput.ListExists(m_AxisListName))
            {
                sInput.LoadAxisList(m_AxisListName);
            }
            else
            {
                for (int i = 0; i < Axis.Length;i++)
                {
                    if (Axis[i].Gravity == 0) Axis[i].Gravity = 3;
                    if (Axis[i].Name.Length > 0)
                    {
                        sInput.SetAxis(Axis[i].Name, Axis[i].PositiveKey, Axis[i].NegativeKey, Axis[i].Gravity);
                    }
                }
            }
        }
        else if (m_LoadType == LoadType.Load_From_File)
        {
            if (sInput.ListInFileExists(m_FileName,m_isPath))
            {
                sInput.LoadListsInFile(m_FileName, m_isPath);
            }
            else
            {
                for (int i = 0; i < Keys.Length; i++)
                {
                    if (Keys[i].Name.Length > 0)
                    {
                        sInput.SetKey(Keys[i].Name, Keys[i].m_KeyCode);
                    }
                }
                for (int i = 0; i < Axis.Length; i++)
                {
                    if (Axis[i].Gravity == 0) Axis[i].Gravity = 3;
                    if (Axis[i].Name.Length > 0)
                    {
                        sInput.SetAxis(Axis[i].Name, Axis[i].PositiveKey, Axis[i].NegativeKey, Axis[i].Gravity);
                    }
                }
            }
        }
        
    }

	void Start () {
	
	}
	
	void Update () {
        sInput.Update();  
	}
}

[System.Serializable]
public class keyGenerator
{
    public string Name;
    public KeyCode m_KeyCode;
}
[System.Serializable]
public class axisGenerator
{
    public string Name;
    public KeyCode PositiveKey;
    public KeyCode NegativeKey;
    public float Gravity = 3;
}
