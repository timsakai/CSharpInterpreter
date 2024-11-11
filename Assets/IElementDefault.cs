using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UICommandOrText(コマンド・データ入力)オブジェクトが読む、デフォルトの入力空間の設定
public enum CommandOrText
{
    Command,
    Text,
}
public interface IElementDefault
{
    public CommandOrText commandOrText { get; }　//デフォルトのコマンド・データ入力モード
}
