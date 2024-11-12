using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//コマンド内、引数要素オブジェクト
//データの入力はUICommandOrText(コマンド・データ入力)オブジェクトから読み込む
public class UIArgumentElement : MonoBehaviour,IElementDefault
{
    public UICommandOrText parent;　                 //thisはこのparent内の、引数のひとつです
    [SerializeField] TMP_InputField argumentName;　  //引数名の指定インプット
    public CommandOrText commandOrText { get { return CommandOrText.Text; } }　//引数はデフォルトでデータ入力モード
    [SerializeField] UICommandOrText input;     //入力オブジェクト
    [SerializeField] Button argMinus;           //消去ボタン
    
    //引数として文字列に変換
    //「 "argumentName.text":"input.GatherString(is_escape = true)" 」
    //値は常にエスケープ処理を入れます。
    public string GatherString(bool is_escape)
    {
        string doublequote = "\"";
        if (is_escape) doublequote = "\\" + doublequote;
        
        string name = doublequote + argumentName.text + doublequote;
        
        string value = doublequote + input.GatherString(true,true) + doublequote;
        
        string output = name + ":" + value;

        return output;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (argumentName == null) Debug.LogError("Please Attach ArgumentName");
        if (argMinus == null) Debug.LogError("Please Attach argMinus Button");
        if (input == null) Debug.LogError("Please Attach input UICommandOrText");
        argMinus.onClick.AddListener(Remove);   //消去処理
        input.attachedElement = this;
    }

    void Remove()
    {
        parent.RemoveArgument(this);    //消去処理
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
