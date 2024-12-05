using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

//コマンドリスト(コマンド関数)
public class UITextCommandList : MonoBehaviour
{
    [SerializeField] GameObject textCommandElementPrefab;       //コマンド要素オブジェクトのプレファブ
    [SerializeField] VerticalLayoutGroup verticalLayoutGroup;   //コマンド要素を配置するレイアウトグループ
    [SerializeField] Button plus;                               //コマンド追加ボタン
    [SerializeField] Transform plusDefault;                               //コマンド追加ボタン
    [SerializeField] Button execute;                            //関数実行ボタン
    [SerializeField] List<UITextCommandElement> textCommandElements = new List<UITextCommandElement>(); //コマンド要素リスト
    [SerializeField] List<GameCommand> commands = new List<GameCommand>();  //Jsonからデシリアライズ済みコマンドリスト
    [SerializeField] GameCommandSystem gameCommandSystem;                   //コマンドシステム
    // Start is called before the first frame update
    private void Awake()
    {
        UpdateElements();
    }
    void Start()
    {
        if (textCommandElementPrefab == null) Debug.LogError("Please Attach textCommandElementPrefab");
        if (verticalLayoutGroup == null) Debug.LogError("Please Attach verticalLayoutGroup ");
        if (execute == null) Debug.LogError("Please Attach execute Button");
        if (gameCommandSystem == null) Debug.LogError("Please Attach GameCommandSystem");

        if (plus == null) Debug.LogError("Please Attach plus Button");
        if (plusDefault == null) Debug.LogError("Please Attach plusDefault Transform");
        plus.onClick.AddListener(AddElement);
        execute.onClick.AddListener(Execute);
    }

    public void AddElement()
    {
        GameObject instance = Instantiate(textCommandElementPrefab);
        
        Transform parent = verticalLayoutGroup.transform;
        instance.transform.SetParent(parent, false);
        UITextCommandElement element_component = instance.GetComponent<UITextCommandElement>();
        if (element_component == null) Debug.LogError(textCommandElementPrefab + " is not UITextCommanElement");
        //追加はUpdateElementで行う//textCommandElements.Add(element_component);
        element_component.SetText(gameCommandSystem.MakeVariableID());
        UpdateElements();
    }

    public void RemoveElement(UITextCommandElement element)
    {
        plus.transform.SetParent(plusDefault, false);
        
        element.gameObject.tag = "Destroyed";//デストロイは更新処理で行うためここではタグ付けのみ

        UpdateElements();
    }

    public void MoveElement(UITextCommandElement element,int move)
    {
        //if (textCommandElements.Count >= textCommandElements.IndexOf(element) + move) return;
        int next = element.transform.GetSiblingIndex() + move;
        if (next < 0) return;
        //System.Exception exception = null;
        //������UpdateElements�Őݒ�//textCommandElements.TrySwap(textCommandElements.IndexOf(element), textCommandElements.IndexOf(element) + move, out exception);
        //if (exception != null) throw exception;
        element.transform.SetSiblingIndex(textCommandElements.IndexOf(element) + move);
        UpdateElements();
    }
    void UpdateElements()
    {
        textCommandElements.Clear();
        int count = verticalLayoutGroup.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            if (!verticalLayoutGroup.transform.GetChild(i).CompareTag("Destroyed"))//削除済みでなければ
            {
                textCommandElements.Add(verticalLayoutGroup.transform.GetChild(i).GetComponent<UITextCommandElement>());
            }
            if (i == verticalLayoutGroup.transform.childCount - 1)
            {
                if (verticalLayoutGroup.transform.GetChild(i).CompareTag("Destroyed"))//先頭が削除済みならば
                {
                    if (i > 0) plus.transform.SetParent(verticalLayoutGroup.transform.GetChild(i - 1), false);//残り数が0以外の時、一個前にプラスボタンを配置

                }
                else
                {
                    plus.transform.SetParent(verticalLayoutGroup.transform.GetChild(i), false);//先頭にプラスボタンを配置

                }
            }
        }
        foreach (var element in textCommandElements)
        {
            element.commandList = this;
        }

        //デストロイ本体
        verticalLayoutGroup.gameObject.DestroyChildrenWithTag("Destroyed");
    }
    void Execute()
    {
        commands = new List<GameCommand>();
        foreach (UITextCommandElement element in textCommandElements) 
        {
            Debug.Log(element.GatherString());
            commands.Add(JsonGameCommandParse.JsonToGameCommand(element.GatherString()));//シリアライズここ
        }
        gameCommandSystem.ExecuteCommands(commands);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
