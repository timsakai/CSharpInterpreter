using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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
public class GameCommandSystem : MonoBehaviour
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

        GameCommand game_command = JsonGameCommandParse.JsonToGameCommand(args["command"]);
        string retID = ExecuteCommand(game_command);
        Variable retVal = dynamicVariable.GetVariable(retID);
        if (userVariable.ContainsKey(args["name"])) { userVariable[args["name"]] = retVal; }

        output_variable.value = retVal.value;
        dynamicVariable.AddVariable(output, output_variable);

        return output;
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
        if(value.StartsWith("var_"))
        {
            Debug.Log("is variable");
            Debug.Log("is variable2");
            return (T)userVariable[value.Substring(4)].value;
        }
        else
        {

            //object�ɂ��Ă���T�ɂ���A��ׂ�
            //�ϊ��������܂��킯����Ȃ�����A���l�ϊ��Ȃ񂩂��ł���킯���Ȃ��I
            //value�𐔒l�ϊ���object�ɂ��Ă���T�ɂ�����ăR�g!?
            return (T)(object)value;
        }
        return default(T);
    }

    //���I�ϐ��p�̃����_����������o�������̊֐�
    public string MakeVariableID()
    {
        int number = Random.Range(0, 999999); // 0����9999�܂ł̃����_���Ȑ���
        string formattedNumber = number.ToString("D6");
        int wordIdx = Random.Range(0, words.Length - 1);
        return words[wordIdx] + formattedNumber;
    }

}