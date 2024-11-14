using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//コマンド・データ入力オブジェクト
public class UICommandOrText : MonoBehaviour
{
    [SerializeField] GameObject argumentElementPrefab;　//引数要素オブジェクトのプレハブ
    [SerializeField] Button cmdTxtSwitch;               //コマンド・データ入力モード切替ボタン
    [SerializeField] GameObject textPanel;              //モードによって有効・無効化されるオブジェクト、データ入力側
    [SerializeField] TMP_InputField textModeText;       //データ入力モードのテキストフィールド
    [SerializeField] GameObject commandPanel;           //モードによって有効・無効化されるオブジェクト、コマンド入力側
    [SerializeField] TMP_InputField commandName;        //コマンド入力モードのコマンド名
    [SerializeField] Button argPlus;                    //コマンド入力モードの引数追加
    [SerializeField] Transform argPlusDefault;                    //コマンド入力モードの引数追加
    [SerializeField] HorizontalLayoutGroup argumentLayoutGroup;                                 //引数オブジェクトを配置するレイアウトグループ
    [SerializeField] List<UIArgumentElement> argumentElements = new List<UIArgumentElement>();  //引数オブジェクトのリスト
    public IElementDefault attachedElement;             //入力としてアタッチされている親
    [SerializeField] CommandOrText currentMode;         //現在のコマンド・データ入力モード
    // Start is called before the first frame update
    void Start()
    {
        if (argumentElementPrefab == null) Debug.LogError("Please Attach argumentElementPrefab");
        if (cmdTxtSwitch == null) Debug.LogError("Please Attach cmdTxtSwith Button");
        if (argPlus == null) Debug.LogError("Please Attach argPlus Button");
        if (argPlusDefault == null) Debug.LogError("Please Attach argPlusDefaultParent");
        if (textPanel == null) Debug.LogError("Please Attach textPanel");
        if (commandPanel == null) Debug.LogError("Please Attach commandPanel");
        cmdTxtSwitch.onClick.AddListener(Switch);   //モード切替機能
        SwitchTo(attachedElement.commandOrText);    //入力空間の設定のデフォルトのモードを読み込み、切り替え
        argPlus.onClick.AddListener(AddArgument);   //引数追加機能
    }

    //モード切替
    void Switch()
    {
        if(currentMode == CommandOrText.Command)
        {
            SwitchTo(CommandOrText.Text);
        }
        else if(currentMode == CommandOrText.Text)
        {
            SwitchTo(CommandOrText.Command);
        }
    }

    void SwitchTo(CommandOrText commandOrText)
    {
        currentMode = commandOrText;
        if (currentMode == CommandOrText.Command)
        {
            textPanel.SetActive(false);
            commandPanel.SetActive(true);
        }
        else if (currentMode == CommandOrText.Text)
        {
            textPanel.SetActive(true);
            commandPanel.SetActive(false);
        }
    }

    //引数追加機能
    void AddArgument()
    {
        GameObject instance = Instantiate(argumentElementPrefab);
        instance.transform.SetParent(argumentLayoutGroup.transform, false);
        UpdateArgument();
    }

    //引数削除機能
    public void RemoveArgument(UIArgumentElement element)
    {
        argPlus.transform.SetParent(argPlusDefault, false);
        element.gameObject.tag = "Destroyed";
        Destroy(element.gameObject);
        UpdateArgument();
    }

    //引数リスト更新
    //シーン上のレイアウトグループの子供から引数オブジェクトリストを作成
    void UpdateArgument()
    {
        argumentElements.Clear();
        for (int i = 0; i < argumentLayoutGroup.transform.childCount; i++)
        {
            argumentElements.Add(argumentLayoutGroup.transform.GetChild(i).GetComponent<UIArgumentElement>());
            if (i == argumentLayoutGroup.transform.childCount - 1)
            {
                if(argumentLayoutGroup.transform.GetChild(i).CompareTag("Destroyed"))//先頭が削除済みならば
                {
                    if(i > 0) argPlus.transform.SetParent(argumentLayoutGroup.transform.GetChild(i-1), false);//残り数が0以外の時、一個前にプラスを配置
                }
                else
                {
                    argPlus.transform.SetParent(argumentLayoutGroup.transform.GetChild(i), false);//先頭にプラスを配置

                }
            }
        }
        foreach (var element in argumentElements)
        {
            element.parent = this;
        }
    }

    //コマンド入力文字列化
    public string GatherString(bool is_escape,bool is_cmd_argument)
    {
        string output = "";
        if(currentMode == CommandOrText.Command)
        {
            string doublequote = "\"";
            if (is_escape) doublequote = "\\" + doublequote;
            string commandkey = doublequote + "command" + doublequote;
            string commandname = commandName.text;
            string argstream = doublequote + "arguments" + doublequote + ":{";
            int loop = 0;
            foreach(var arg in argumentElements)
            {
                string comma = ",";
                if(loop <= 0) comma = "";
                argstream += comma + arg.GatherString(is_escape);// " "arg1":"value" "　でもらう
                loop++;
            }
            argstream += "}";
            string cmd_sign = "";
            if (is_cmd_argument) cmd_sign = "cmd_";
            output = cmd_sign + "{" + commandkey + ":" +  doublequote + commandname + doublequote + argstream + "}";
        }
        else if (currentMode == CommandOrText.Text)
        {
            output = textModeText.text;
        }
        return output;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
