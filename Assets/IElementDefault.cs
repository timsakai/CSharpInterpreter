using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UICommandOrText(�R�}���h�E�f�[�^����)�I�u�W�F�N�g���ǂށA�f�t�H���g�̓��͋�Ԃ̐ݒ�
public enum CommandOrText
{
    Command,
    Text,
}
public interface IElementDefault
{
    public CommandOrText commandOrText { get; }�@//�f�t�H���g�̃R�}���h�E�f�[�^���̓��[�h
}
