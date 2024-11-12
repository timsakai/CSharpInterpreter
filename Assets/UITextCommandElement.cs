using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//�R�}���h�v�f�i�s�j
public class UITextCommandElement : MonoBehaviour , IElementDefault
{
    [SerializeField] TMP_InputField inputField; //�f�̃C���v�b�g�t�B�[���h���́i�p�~�\��j
    [SerializeField] Button minus;�@            //�����{�^��
    [SerializeField] Button moveUp;             //��Ɉړ��{�^��
    [SerializeField] Button moveDown;           //���Ɉړ��{�^��
    [SerializeField] string command;            //���s�����R�}���h
    public UITextCommandList commandList;       //�e�̃R�}���h���X�g
    public GameCommand preDeserialized { get; private set; }    //���OJson�f�V���A���C�Y
    public CommandOrText commandOrText { get { return CommandOrText.Command; } }    //�f�t�H���g�ŃR�}���h���[�h�œ���

    [SerializeField] UICommandOrText input;//���̓I�u�W�F�N�g

    // Start is called before the first frame update
    void Start()
    {
        if (inputField == null) Debug.LogError("Please Attach inputField");
        if (minus == null) Debug.LogError("Please Attach minus Button");
        if (moveUp == null) Debug.LogError("Please Attach moveUp Button");
        if (moveDown == null) Debug.LogError("Please Attach moveDown Button");
        if (commandList == null) Debug.LogError("Please Attach List");
        if (input == null) Debug.LogError("Please Attach input UICommandOrText");
        OnValueChanged(inputField.text);                            //�ŏ��ɓ��͍X�V����
        inputField.onValueChanged.AddListener(OnValueChanged);      //�f�̃C���v�b�g�t�B�[���h���͕ύX���Ɏ��O�f�V���A���C�Y�Ȃ�
        minus.onClick.AddListener(RemoveSelf);                      //�����@�\
        moveUp.onClick.AddListener(MoveUp);                         //��ړ��@�\
        moveDown.onClick.AddListener(MoveDown);                     //���ړ��@�\
        input.attachedElement = this;
    }

    //�O���疳�����e�L�X�g�ݒ肷��i�e�X�g�p�j
    public void SetText(string text)
    {
        inputField.text = text;
    }
    //�O���璼�ڃe�L�X�g�ǂ݂����i�e�X�g�p�j
    public string GetText()
    {
        return inputField.text;
    }
    //�f�̃C���v�b�g�t�B�[���h���͕ύX���Ɏ��O�f�V���A���C�Y�Ȃ�
    void OnValueChanged(string value)
    {
        command = value;
        //preDeserialized = JsonGameCommandParse.JsonToGameCommand(command);
    }
    //�����@�\
    void RemoveSelf()
    {
        commandList.RemoveElement(this);
    }
    //��ړ��@�\
    void MoveUp()
    {
        commandList.MoveElement(this, -1);
    }
    //���ړ��@�\
    void MoveDown()
    {
        commandList.MoveElement(this, 1);
    }
    //���͂���X�g�����O(Json�R�}���h)��ǂݏo��
    //Execute�ȑO�̂����^�C�~���O�Ŏ��s��������
    void GatherString()
    {
        command = input.GatherString(false,false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}
