using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//コマンド要素（行）
public class UITextCommandElement : MonoBehaviour , IElementDefault
{
    [SerializeField] TMP_InputField inputField; //素のインプットフィールド入力（廃止予定）
    [SerializeField] Button minus;　            //消去ボタン
    [SerializeField] Button moveUp;             //上に移動ボタン
    [SerializeField] Button moveDown;           //下に移動ボタン
    [SerializeField] string command;            //実行されるコマンド
    public UITextCommandList commandList;       //親のコマンドリスト
    public GameCommand preDeserialized { get; private set; }    //事前Jsonデシリアライズ
    public CommandOrText commandOrText { get { return CommandOrText.Command; } }    //デフォルトでコマンドモードで入力

    [SerializeField] UICommandOrText input;//入力オブジェクト

    // Start is called before the first frame update
    void Start()
    {
        if (inputField == null) Debug.LogError("Please Attach inputField");
        if (minus == null) Debug.LogError("Please Attach minus Button");
        if (moveUp == null) Debug.LogError("Please Attach moveUp Button");
        if (moveDown == null) Debug.LogError("Please Attach moveDown Button");
        if (commandList == null) Debug.LogError("Please Attach List");
        if (input == null) Debug.LogError("Please Attach input UICommandOrText");
        OnValueChanged(inputField.text);                            //最初に入力更新処理
        inputField.onValueChanged.AddListener(OnValueChanged);      //素のインプットフィールド入力変更時に事前デシリアライズなど
        minus.onClick.AddListener(RemoveSelf);                      //消去機能
        moveUp.onClick.AddListener(MoveUp);                         //上移動機能
        moveDown.onClick.AddListener(MoveDown);                     //下移動機能
        input.attachedElement = this;
    }

    //外から無理やりテキスト設定する（テスト用）
    public void SetText(string text)
    {
        inputField.text = text;
    }
    //外から直接テキスト読みだす（テスト用）
    public string GetText()
    {
        return inputField.text;
    }
    //素のインプットフィールド入力変更時に事前デシリアライズなど
    void OnValueChanged(string value)
    {
        command = value;
        //preDeserialized = JsonGameCommandParse.JsonToGameCommand(command);
    }
    //消去機能
    void RemoveSelf()
    {
        commandList.RemoveElement(this);
    }
    //上移動機能
    void MoveUp()
    {
        commandList.MoveElement(this, -1);
    }
    //下移動機能
    void MoveDown()
    {
        commandList.MoveElement(this, 1);
    }
    //入力からストリング(Jsonコマンド)を読み出し
    //Execute以前のいいタイミングで実行させたい
    void GatherString()
    {
        command = input.GatherString(false,false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}
