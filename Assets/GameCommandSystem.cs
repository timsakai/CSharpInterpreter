using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

//変数
//System.Objectで保管、コマンドが読むときに変換する。
public class Variable
{
    public string name;
    public System.Object value;
    public System.Type type;
}

//コマンド用デリゲート
public delegate string ExecutableFunction(Dictionary<string,string> args);

public class DynamicVariable
{
    Dictionary<string, Variable> variables = new Dictionary<string, Variable>(); // 動的変数　返り値を受け取ったりするために使う
    public Variable GetVariable(string name)
    {
        Variable v = variables[name];
        variables.Remove(name);
        return v;
    }
    public void AddVariable(string name,Variable variable)
    {
        variables.Add(name, variable);
    }
}
partial class GameCommandSystem : MonoBehaviour
{
    //TODO:DynamicVariableの寿命を設定　読みだし時に消すようにする。
    DynamicVariable dynamicVariable = new DynamicVariable(); // 動的変数　返り値を受け取ったりするために使う
    Dictionary<string, Variable> userVariable = new Dictionary<string, Variable>();　// ユーザー変数　マイクラのスコアボードの役割
    Dictionary<string, ExecutableFunction> commandFunctions = new Dictionary<string, ExecutableFunction>();　//コマンドのリスト　<名前(この文字列で起動する),処理ExecutableFunction型関数>
    
    [SerializeField]TMP_InputField inputField;
    [SerializeField]TMP_InputField inputField2;
    [SerializeField]TMP_InputField inputField3;
    string command;
    string command2;
    string command3;

    List<GameCommand> commands = new List<GameCommand>();

    string[] words; //動的変数用ランダムワード
    private void Awake()
    {
        words = new string[]
        {
            "butter", "jungle", "planet", "silver", "rocket", "wonder", "flower", "spider",
            "basket", "hunger", "danger", "castle", "charge", "fright", "pencil", "button",
            "tablet", "bright", "matter", "talent", "smooth", "ribbon", "rabbit", "prince",
            "circus", "forest", "guitar", "cradle", "travel", "wheels", "secret", "flight",
            "marble", "dragon", "branch", "pocket", "church", "soccer", "whales", "pirate",
            "vision", "finish", "garden", "wonder", "planet", "forest", "glider", "school",
            "violet", "lizard", "stream", "throne", "thrift", "glance", "letter", "praise",
            "helmet", "coffee", "summer", "winter", "spring", "autumn", "desert", "gentle"
        };

        SubscribeCommand("debuglog", Debuglog);
        SubscribeCommand("makeVariable", MakeVariable);
        SubscribeCommand("catchCommandReturn", CatchCommandReturn);
        SubscribeCommand("Operation", Operation);
        SubscribeCommand("Compare", Compare);
    }
    // Start is called before the first frame update
    void Start()
    {
        command = inputField.text;
    }

    public void ExecuteTestCommand()
    {
        command = inputField.text;
        command2 = inputField2.text;
        command3 = inputField3.text;
        ExecuteCommand(JsonGameCommandParse.JsonToGameCommand(command));
        ExecuteCommand(JsonGameCommandParse.JsonToGameCommand(command2));
        ExecuteCommand(JsonGameCommandParse.JsonToGameCommand(command3));
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExecuteCommands(List<GameCommand> commands)
    {
        foreach (GameCommand command in commands)
        {
            ExecuteCommand(command);
        }
    }
    //コマンド登録関数
    public void SubscribeCommand(string command,ExecutableFunction function)
    {
        commandFunctions.Add(command, function);
    }

    //コマンド呼び出し関数
    public string ExecuteCommand(GameCommand gameCommand)
    {
        ExecutableFunction function = commandFunctions[gameCommand.command];
        return function.Invoke(gameCommand.arguments);
    }

    //コマンドの例
    //001 string output = MakeVariableID();
    //002 Variable output_variable = new Variable();
    // ... 
    //003 output_variable.value = "message";
    //004 dynamicVariable.Add(output, output_variable);
    //return output;
    //以上、003にて計算結果を返す
    //原則何らかの返り値はある
    //----
    //string data = ReadAs<string>(arg);
    //以上で引数を読む
    //ReadAs<T>()が、"var_"から始まるstring引数をユーザー変数読込として扱ってくれる
    //ユーザー変数から直接<T>にする分には問題ないが、stringから<T>にする(要するにハードコード)には、stringからの変換が必要


    //コマンド：
    //ユーザー変数定義
    //入力：name=変数名
    //出力：作成した変数名
    string MakeVariable(Dictionary<string,string> args)
    {
        Variable variable = new Variable();
        userVariable.Add(args["name"], variable);
        return name;
    }

    //コマンド：
    //ユーザー変数に代入
    //入力：name=変数名　, value=値
    //出力：代入した変数名
    //stringとして代入される
    string AssignmentToVariable(Dictionary<string, string> args)
    {
        if (userVariable.ContainsKey(args["name"])) { userVariable[args["name"]].value = ReadAs<string>(args["value"]); }
        return args["name"];
    }
        
    //コマンド：
    //ユーザー変数に評価コマンドの返り値を代入
    //入力：name=代入する変数名　command=評価コマンド
    //出力：評価コマンドの返り値そのまま（再生成されるので延命される）
    string CatchCommandReturn(Dictionary<string, string> args)
    {
        string output = MakeVariableID();
        Variable output_variable = new Variable();

        object retVal = ReadAs<object>(args["command"]);
        if (userVariable.ContainsKey(args["name"])) { userVariable[args["name"]].value = retVal; }

        output_variable.value = retVal;
        dynamicVariable.AddVariable(output, output_variable);

        return output;
    }


    //コマンド：
    //二つの値を四則演算
    //入力：A=左　B=右　operator=演算子
    //"add":加算,"sub":減算,"mul":乗算,"div":除算
    //出力：計算結果
    string Operation(Dictionary<string, string> args)
    {

        string output = MakeVariableID();
        Variable output_variable = new Variable();

        float A = ReadAs<float>(args["A"]);
        float B = ReadAs<float>(args["B"]);
        string op = ReadAs<string>(args["operator"]);

        float opereted = 0;
        bool is_vaild_oparator = true;

        switch (op)
        {
            case "add":
                opereted = A + B;
                break;
            case "sub":
                opereted = A - B;
                break;
            case "mul":
                opereted = A * B;
                break;
            case "div":
                opereted = A / B;
                break;

            default:
                is_vaild_oparator = false;
                break;
        }

        output_variable.value = opereted;
        if(!is_vaild_oparator) output_variable.value = "Error,operator is not valid";
        dynamicVariable.AddVariable(output, output_variable);

        Debug.Log(opereted);

        return output;
    }

    //コマンド：
    //二つの値を比較演算
    //入力：A=左　B=右　operator=演算子
    //"Equal":等値,"Less":未満,"Greater":より大きい,"LessOrEqual":以下,"GreaterOrEqual":以上
    //出力：真偽地
    string Compare
        (Dictionary<string, string> args)
    {

        string output = MakeVariableID();
        Variable output_variable = new Variable();

        float A = ReadAs<float>(args["A"]);
        float B = ReadAs<float>(args["B"]);
        string op = ReadAs<string>(args["operator"]);

        bool compared = false;
        bool is_vaild_oparator = true;

        switch (op)
        {
            case "Less":
                compared = A < B;
                break;
            case "Greater":
                compared = A > B;
                break;
            case "LessOrEqual":
                compared = A <= B;
                break;
            case "GreaterOrEqual":
                compared = A >= B;
                break;

            default:
                is_vaild_oparator = false;
                break;
        }

        output_variable.value = compared;
        if (!is_vaild_oparator) output_variable.value = "Error,operator is not valid";
        dynamicVariable.AddVariable(output, output_variable);

        Debug.Log(output);

        return output;
    }
    //コマンド：
    //デバッグログする
    //入力：value=値
    //出力：DebugLogした文字列
    string Debuglog(Dictionary<string, string> args)
    {
        string output = MakeVariableID();
        Variable output_variable = new Variable();

        string data = ReadAs<string>(args["value"]);

        string messagge = "Debuglog with " + data;
        Debug.Log(messagge);
        
        output_variable.value = data;
        dynamicVariable.AddVariable(output,output_variable);
        return output;
    }

    //stringから各種値に変換
    //もしくは変数を読み込み
    T ReadAs<T>(string value)
    {
        if(value.StartsWith("var_"))
        {
            //変数読込
            return (T)userVariable[value.Substring(4)].value;
        }
        else if (value.StartsWith("cmd_"))
        {
            //コマンド呼び出し
            string command_string = value.Substring(4);
            GameCommand gameCommand = JsonGameCommandParse.JsonToGameCommand(command_string);
            string d_var = ExecuteCommand(gameCommand);
            return (T)(dynamicVariable.GetVariable(d_var).value);
        }
        else
        {
            object object_value = (object)value;
            object output = null;
            if(typeof(T) == typeof(string))
            {
                output = object_value; // stringの時は変換無し
            }
            if(typeof(T) == typeof(float))
            {
                // floatの変換処理
                // stringとして変換後にパース
                string str = (string)object_value;
                float val = float.Parse(str);
                output = val;
            }
            if (typeof(T) == typeof(bool))
            {
                // boolの変換処理
                // stringとして変換後にパース
                string str = (string)object_value;
                bool val = bool.Parse(str);
                output = val;
            }
            //objectにしてからTにする、やべぇ
            //変換処理かますわけじゃないから、数値変換なんかもできるわけがない！
            //valueを数値変換→objectにしてからTにするってコト!?
            return (T)output;
        }
        return default(T);
    }

    //動的変数用のランダム文字列を出すだけの関数
    public string MakeVariableID()
    {
        int number = Random.Range(0, 999999); // 0から9999までのランダムな数字
        string formattedNumber = number.ToString("D6");
        int wordIdx = Random.Range(0, words.Length - 1);
        return words[wordIdx] + formattedNumber;
    }

}