using UnityEngine;

using strange.extensions.command.impl;
using strange.extensions.context.api;

public class ShowGameStartUICommand :EventCommand
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }
    public override void Execute()
    {
        Transform canvas = ContextView.transform.Find("Canvas");
        GameObject GameStartUI = null;
        if (canvas.Find("GameStartUI(Clone)") != null)//已经加载过了
        {
            Debug.Log("ShowGameStartUICommand");
            GameStartUI = canvas.Find("GameStartUI(Clone)").gameObject;
            GameStartUI.transform.SetSiblingIndex(canvas.childCount - 1);//显示在最前面
            return;
        }
        GameObject go = Resources.Load<GameObject>("Prefabs/GameStartUI");
        GameStartUI = Object.Instantiate(go) as GameObject;
        GameStartUI.AddComponent<GameStartUI_View>();
        GameStartUI.transform.SetParent(canvas, false);
    }
}