using UnityEngine;
using strange.extensions.mediation.impl;
using UnityEngine.UI;

public class GameStartUI_View : EventView
{
    private Transform Login;
    private Button Btn_Start;
    private InputField Name_Input;
    public void Init()
    {
        Login = transform.Find("Login");
        Btn_Start = Login.Find("Btn_Start").GetComponent<Button>();
        Btn_Start.onClick.AddListener(Click_BtnStart);
        Name_Input = Login.Find("Name_Input").GetComponent<InputField>();
    }
    private void Click_BtnStart()
    {
        if(!string.IsNullOrEmpty(Name_Input.text))
        {
            Debug.Log("Click Start");
            dispatcher.Dispatch(ViewConst.Click_StartGame, Name_Input.text);
        }
    }

    protected override void Start()
    {
        base.Start();
        setDesignContentScale();
        print("resolution");
    }
    private int scaleWidth = 0;
    private int scaleHeight = 0;
    public void setDesignContentScale()
    {
        if (scaleWidth == 0 && scaleHeight == 0)
        {
            int width = Screen.currentResolution.width;
            int height = Screen.currentResolution.height;
            int designWidth = 960;
            int designHeight = 640;
            float s1 = (float)designWidth / (float)designHeight;
            float s2 = (float)width / (float)height;
            if (s1 < s2)
            {
                designWidth = (int)Mathf.FloorToInt(designHeight * s2);
            }
            else if (s1 > s2)
            {
                designHeight = (int)Mathf.FloorToInt(designWidth / s2);
            }
            float contentScale = (float)designWidth / (float)width;
            if (contentScale < 1.0f)
            {
                scaleWidth = designWidth;
                scaleHeight = designHeight;
            }
        }
        if (scaleWidth > 0 && scaleHeight > 0)
        {
            if (scaleWidth % 2 == 0)
            {
                scaleWidth += 1;
            }
            else
            {
                scaleWidth -= 1;
            }
            Screen.SetResolution(scaleWidth, scaleHeight, true);
        }
    }

    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
        }
        else
        {
            setDesignContentScale();
        }
    }
}