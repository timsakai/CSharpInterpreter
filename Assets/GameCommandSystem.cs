using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using System.Globalization;

//�ϐ�
//System.Object�ŕۊǁA�R�}���h���ǂނƂ��ɕϊ�����B
public class Variable
{
    public string name;
    public System.Object value;
    public System.Type type;
}

//�R�}���h�p�f���Q�[�g
public delegate string ExecutableFunction(Dictionary<string,string> args);

public class DynamicVariable
{
    Dictionary<string, Variable> variables = new Dictionary<string, Variable>(); // ���I�ϐ��@�Ԃ�l���󂯎�����肷�邽�߂Ɏg��
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
    //TODO:DynamicVariable�̎�����ݒ�@�ǂ݂������ɏ����悤�ɂ���B
    DynamicVariable dynamicVariable = new DynamicVariable(); // ���I�ϐ��@�Ԃ�l���󂯎�����肷�邽�߂Ɏg��
    Dictionary<string, Variable> userVariable = new Dictionary<string, Variable>();�@// ���[�U�[�ϐ��@�}�C�N���̃X�R�A�{�[�h�̖���
    Dictionary<string, ExecutableFunction> commandFunctions = new Dictionary<string, ExecutableFunction>();�@//�R�}���h�̃��X�g�@<���O(���̕�����ŋN������),����ExecutableFunction�^�֐�>
    
    [SerializeField]TMP_InputField inputField;
    [SerializeField]TMP_InputField inputField2;
    [SerializeField]TMP_InputField inputField3;
    string command;
    string command2;
    string command3;

    List<GameCommand> commands = new List<GameCommand>();

    string[] words; //���I�ϐ��p�����_�����[�h
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
    //�R�}���h�o�^�֐�
    public void SubscribeCommand(string command,ExecutableFunction function)
    {
        commandFunctions.Add(command, function);
    }

    //�R�}���h�Ăяo���֐�
    public string ExecuteCommand(GameCommand gameCommand)
    {
        ExecutableFunction function = commandFunctions[gameCommand.command];
        return function.Invoke(gameCommand.arguments);
    }

    //�R�}���h�̗�
    //001 string output = MakeVariableID();
    //002 Variable output_variable = new Variable();
    // ... 
    //003 output_variable.value = "message";
    //004 dynamicVariable.Add(output, output_variable);
    //return output;
    //�ȏ�A003�ɂČv�Z���ʂ�Ԃ�
    //�������炩�̕Ԃ�l�͂���
    //----
    //string data = ReadAs<string>(arg);
    //�ȏ�ň�����ǂ�
    //ReadAs<T>()���A"var_"����n�܂�string���������[�U�[�ϐ��Ǎ��Ƃ��Ĉ����Ă����
    //���[�U�[�ϐ����璼��<T>�ɂ��镪�ɂ͖��Ȃ����Astring����<T>�ɂ���(�v����Ƀn�[�h�R�[�h)�ɂ́Astring����̕ϊ����K�v


    //�R�}���h�F
    //���[�U�[�ϐ���`
    //���́Fname=�ϐ���
    //�o�́F�쐬�����ϐ���
    string MakeVariable(Dictionary<string,string> args)
    {
        Variable variable = new Variable();
        userVariable.Add(args["name"], variable);
        return name;
    }

    //�R�}���h�F
    //���[�U�[�ϐ��ɑ��
    //���́Fname=�ϐ����@, value=�l
    //�o�́F��������ϐ���
    //string�Ƃ��đ�������
    string AssignmentToVariable(Dictionary<string, string> args)
    {
        if (userVariable.ContainsKey(args["name"])) { userVariable[args["name"]].value = ReadAs<string>(args["value"]); }
        return args["name"];
    }
        
    //�R�}���h�F
    //���[�U�[�ϐ��ɕ]���R�}���h�̕Ԃ�l����
    //���́Fname=�������ϐ����@command=�]���R�}���h
    //�o�́F�]���R�}���h�̕Ԃ�l���̂܂܁i�Đ��������̂ŉ��������j
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


    //�R�}���h�F
    //��̒l���l�����Z
    //���́FA=���@B=�E�@operator=���Z�q
    //"add":���Z,"sub":���Z,"mul":��Z,"div":���Z
    //�o�́F�v�Z����
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

    //�R�}���h�F
    //��̒l���r���Z
    //���́FA=���@B=�E�@operator=���Z�q
    //"Equal":���l,"Less":����,"Greater":���傫��,"LessOrEqual":�ȉ�,"GreaterOrEqual":�ȏ�
    //�o�́F�^�U�n
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
    //�R�}���h�F
    //�f�o�b�O���O����
    //���́Fvalue=�l
    //�o�́FDebugLog����������
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

    //string����e��l�ɕϊ�
    //�������͕ϐ���ǂݍ���
    T ReadAs<T>(string value)
    {
        object outobj = default(T);
        //���ϐ��̎�---------------------------------
        if (value.StartsWith("var_"))
        {
            outobj = ReadObjectAs<T>(userVariable[value.Substring(4)].value);�@// T�Ŏw�肳�ꂽ�^�œǂݍ���
        }
        //���R�}���h�̎�---------------------------------
        else if (value.StartsWith("cmd_"))
        {
            string command_string = value.Substring(4);//"cmd_��r��"
            GameCommand gameCommand = JsonGameCommandParse.JsonToGameCommand(command_string);//�R�}���h�Ƀp�[�X�i�G�X�P�[�v�t���j
            string d_var = ExecuteCommand(gameCommand);//�R�}���h�Ăяo��

            outobj = ReadObjectAs<T>( dynamicVariable.GetVariable(d_var).value);//�Ԃ�l��ۑ����Ă���ϐ����w�肵���^�œǂݍ���
        }
        //�����͒l�̎�---------------------------------
        else
        {
            object parsed_value = null;
            ParseToFloatOrNotParsedString(value, out parsed_value); // �m���string�Ȃ̂ŁAstring����l�̃p�[�X
            outobj = ReadObjectAs<T>(parsed_value);//�l����T�̌^�œǂݍ���
        }
        return (T)outobj;
    }

    T ReadObjectAs<T>(object _object)
    {
        object object_value = _object;
        object output = object_value;
        //���ϊ��悪string�������Ƃ�---------------------------------
        if (typeof(T) == typeof(string))
        {
            output = GetAsFormatedString(object_value);//�t�H�[�}�b�g������string�ɕϊ�
        }
        return (T)output;
    }

    //�\��������string����l(����Float�Œ�)�ɕϊ��A�s�\��������string�̃f�[�^���̂܂�
    public bool ParseToFloatOrNotParsedString(string _string,out object _output)
    {
        bool is_parsed = false;
        _output = null;
        float value = 0;
        bool boolean = false;
        //�����l�ɕϊ��\�ȕ�����Ȃ�---------------------------------
        if (float.TryParse(_string,out value))
        {
            
            //Float�ɕϊ�
            _output = new Float(value);
            is_parsed = true;
        }
        //���u�[���A���ɕϊ��\�ȕ�����Ȃ�---------------------------------
        if (bool.TryParse(_string,out boolean))
        {
            _output = boolean;
            is_parsed = true;

        }
        //���ϊ��s�\��������---------------------------------
        else
        {
            //�����̃f�[�^���̂܂�
            _output = _string;
        }
        //Object�ŕԂ�
        return is_parsed;
    }
    public string GetAsFormatedString(object value)
    {
        //��string ���@string�̕ϊ���������---------------------------------
        if (value is string)
        {
            return (string)value;//���̂܂ܕԂ�
        }
        //�����l����ϊ��\�ȕ�����Ȃ�---------------------------------
        var formattable = (IFormattable)value;
        return formattable.ToString(null, CultureInfo.InvariantCulture);//IFormattable.ToString()���g��
    }

    //���I�ϐ��p�̃����_����������o�������̊֐�
    public string MakeVariableID()
    {
        int number = UnityEngine.Random.Range(0, 999999); // 0����9999�܂ł̃����_���Ȑ���
        string formattedNumber = number.ToString("D6");
        int wordIdx = UnityEngine.Random.Range(0, words.Length - 1);
        return words[wordIdx] + formattedNumber;
    }

}