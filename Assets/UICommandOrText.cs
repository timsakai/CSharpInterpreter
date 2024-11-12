using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//�R�}���h�E�f�[�^���̓I�u�W�F�N�g
public class UICommandOrText : MonoBehaviour
{
    [SerializeField] GameObject argumentElementPrefab;�@//�����v�f�I�u�W�F�N�g�̃v���n�u
    [SerializeField] Button cmdTxtSwitch;               //�R�}���h�E�f�[�^���̓��[�h�ؑփ{�^��
    [SerializeField] GameObject textPanel;              //���[�h�ɂ���ėL���E�����������I�u�W�F�N�g�A�f�[�^���͑�
    [SerializeField] TMP_InputField textModeText;       //�f�[�^���̓��[�h�̃e�L�X�g�t�B�[���h
    [SerializeField] GameObject commandPanel;           //���[�h�ɂ���ėL���E�����������I�u�W�F�N�g�A�R�}���h���͑�
    [SerializeField] TMP_InputField commandName;        //�R�}���h���̓��[�h�̃R�}���h��
    [SerializeField] Button argPlus;                    //�R�}���h���̓��[�h�̈����ǉ�
    [SerializeField] HorizontalLayoutGroup argumentLayoutGroup;                                 //�����I�u�W�F�N�g��z�u���郌�C�A�E�g�O���[�v
    [SerializeField] List<UIArgumentElement> argumentElements = new List<UIArgumentElement>();  //�����I�u�W�F�N�g�̃��X�g
    public IElementDefault attachedElement;             //���͂Ƃ��ăA�^�b�`����Ă���e
    [SerializeField] CommandOrText currentMode;         //���݂̃R�}���h�E�f�[�^���̓��[�h
    // Start is called before the first frame update
    void Start()
    {
        if (argumentElementPrefab == null) Debug.LogError("Please Attach argumentElementPrefab");
        if (cmdTxtSwitch == null) Debug.LogError("Please Attach cmdTxtSwith Button");
        if (argPlus == null) Debug.LogError("Please Attach argPlus Button");
        if (textPanel == null) Debug.LogError("Please Attach textPanel");
        if (commandPanel == null) Debug.LogError("Please Attach commandPanel");
        cmdTxtSwitch.onClick.AddListener(Switch);   //���[�h�ؑ֋@�\
        SwitchTo(attachedElement.commandOrText);    //���͋�Ԃ̐ݒ�̃f�t�H���g�̃��[�h��ǂݍ��݁A�؂�ւ�
        argPlus.onClick.AddListener(AddArgument);   //�����ǉ��@�\
    }

    //���[�h�ؑ�
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

    //�����ǉ��@�\
    void AddArgument()
    {
        GameObject instance = Instantiate(argumentElementPrefab);
        instance.transform.SetParent(argumentLayoutGroup.transform, false);
        UpdateArgument();
    }

    //�����폜�@�\
    public void RemoveArgument(UIArgumentElement element)
    {
        Destroy(element.gameObject);
        UpdateArgument();
    }

    //�������X�g�X�V
    //�V�[����̃��C�A�E�g�O���[�v�̎q����������I�u�W�F�N�g���X�g���쐬
    void UpdateArgument()
    {
        argumentElements.Clear();
        for (int i = 0; i < argumentLayoutGroup.transform.childCount; i++)
        {
            argumentElements.Add(argumentLayoutGroup.transform.GetChild(i).GetComponent<UIArgumentElement>());
            if (i == argumentLayoutGroup.transform.childCount - 1)
            {
                argPlus.transform.SetParent(argumentLayoutGroup.transform.GetChild(i), false);
            }
        }
        foreach (var element in argumentElements)
        {
            element.parent = this;
        }
    }

    //�R�}���h���͕�����
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
                argstream += comma + arg.GatherString(is_escape);// " "arg1":"value" "�@�ł��炤
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
