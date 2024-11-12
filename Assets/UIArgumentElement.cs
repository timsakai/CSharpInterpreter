using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//�R�}���h���A�����v�f�I�u�W�F�N�g
//�f�[�^�̓��͂�UICommandOrText(�R�}���h�E�f�[�^����)�I�u�W�F�N�g����ǂݍ���
public class UIArgumentElement : MonoBehaviour,IElementDefault
{
    public UICommandOrText parent;�@                 //this�͂���parent���́A�����̂ЂƂł�
    [SerializeField] TMP_InputField argumentName;�@  //�������̎w��C���v�b�g
    public CommandOrText commandOrText { get { return CommandOrText.Text; } }�@//�����̓f�t�H���g�Ńf�[�^���̓��[�h
    [SerializeField] UICommandOrText input;     //���̓I�u�W�F�N�g
    [SerializeField] Button argMinus;           //�����{�^��
    
    //�����Ƃ��ĕ�����ɕϊ�
    //�u "argumentName.text":"input.GatherString(is_escape = true)" �v
    //�l�͏�ɃG�X�P�[�v���������܂��B
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
        argMinus.onClick.AddListener(Remove);   //��������
        input.attachedElement = this;
    }

    void Remove()
    {
        parent.RemoveArgument(this);    //��������
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
