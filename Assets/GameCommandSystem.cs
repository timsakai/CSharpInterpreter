using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using System.Globalization;

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

public class Float : IFormattable
{
    public float value;
    public Float(float value)
    {
        this.value = value;
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return value.ToString(format,formatProvider);
    }

    public static implicit operator string(Float self)
    {
        return self.value.ToString();
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

    private void AddLine(ref Queue<GameCommand> Queue, string command_json, int index)
    {
        GameCommand _command = JsonGameCommandParse.JsonToGameCommand(command_json);
        Queue.Append(_command);
    }
    public void ExecuteCommands(List<GameCommand> commands)
    {
        Queue<GameCommand> queue = new Queue<GameCommand>();
        foreach (GameCommand item in commands)
        {
            queue.Enqueue(item);
        }
        for (int i = 0;queue.Count > 0; i++)
        {
            GameCommand _command = queue.Dequeue();
            ExecuteCommand(_command);

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

        float A = ReadAs<Float>(args["A"]).value;
        float B = ReadAs<Float>(args["B"]).value;
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

        Float outobj = new Float(opereted);
        output_variable.value = outobj;
        if(!is_vaild_oparator) output_variable.value = "Error,operator is not valid";
        dynamicVariable.AddVariable(output, output_variable);

        Debug.Log(output_variable.value);

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

        float A = ReadAs<Float>(args["A"]).value;
        float B = ReadAs<Float>(args["B"]).value;
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

    string Branch(Dictionary<string, string> args)
    {
        bool condition = ReadAs<bool>(args["condition"]);
        float Process = 0;
        if (condition)
        {
            Process = ReadAs<Float>(args["OnTrue"]).value;
        }
        else
        {
            Process = ReadAs<Float>(args["OnFalse"]).value;
        }


        return "";
    }

    string While(Dictionary<string, string> args)
    {
        bool condition = ReadAs<bool>(args["condition"]);
        float Process = 0;
        if (condition)
        {
            Process = ReadAs<Float>(args["OnTrue"]).value;
        }
        else
        {
        }


        return "";
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
        object outobj = default(T);
        //★変数の時---------------------------------
        if (value.StartsWith("var_"))
        {
            outobj = ReadObjectAs<T>(userVariable[value.Substring(4)].value);　// Tで指定された型で読み込み
        }
        //★コマンドの時---------------------------------
        else if (value.StartsWith("cmd_"))
        {
            string command_string = value.Substring(4);//"cmd_を排除"
            GameCommand gameCommand = JsonGameCommandParse.JsonToGameCommand(command_string);//コマンドにパース（エスケープ付き）
            string d_var = ExecuteCommand(gameCommand);//コマンド呼び出し

            outobj = ReadObjectAs<T>( dynamicVariable.GetVariable(d_var).value);//返り値を保存している変数を指定した型で読み込み
        }
        //★入力値の時---------------------------------
        else
        {
            object parsed_value = null;
            ParseToFloatOrNotParsedString(value, out parsed_value); // 確定でstringなので、stringから値のパース
            outobj = ReadObjectAs<T>(parsed_value);//値からTの型で読み込み
        }
        return (T)outobj;
    }

    T ReadObjectAs<T>(object _object)
    {
        object object_value = _object;
        object output = object_value;
        //★変換先がstringだったとき---------------------------------
        if (typeof(T) == typeof(string))
        {
            output = GetAsFormatedString(object_value);//フォーマット処理でstringに変換
        }
        return (T)output;
    }

    //可能だったらstringから値(現状Float固定)に変換、不可能だったらstringのデータそのまま
    public bool ParseToFloatOrNotParsedString(string _string,out object _output)
    {
        bool is_parsed = false;
        _output = null;
        float value = 0;
        bool boolean = false;
        //★数値に変換可能な文字列なら---------------------------------
        if (float.TryParse(_string,out value))
        {
            
            //Floatに変換
            _output = new Float(value);
            is_parsed = true;
        }
        //★ブーリアンに変換可能な文字列なら---------------------------------
        if (bool.TryParse(_string,out boolean))
        {
            _output = boolean;
            is_parsed = true;

        }
        //★変換不能だったら---------------------------------
        else
        {
            //引数のデータそのまま
            _output = _string;
        }
        //Objectで返す
        return is_parsed;
    }
    public string GetAsFormatedString(object value)
    {
        //★string →　stringの変換だったら---------------------------------
        if (value is string)
        {
            return (string)value;//そのまま返す
        }
        //★数値から変換可能な文字列なら---------------------------------
        var formattable = (IFormattable)value;
        return formattable.ToString(null, CultureInfo.InvariantCulture);//IFormattable.ToString()を使う
    }

    //動的変数用のランダム文字列を出すだけの関数
    public string MakeVariableID()
    {
        int number = UnityEngine.Random.Range(0, 999999); // 0から9999までのランダムな数字
        string formattedNumber = number.ToString("D6");
        int wordIdx = UnityEngine.Random.Range(0, words.Length - 1);
        return words[wordIdx] + formattedNumber;
    }

}