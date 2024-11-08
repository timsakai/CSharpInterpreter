
using System.Collections.Generic;

//コマンド実行に使用するデータ
//現在はJsonからデコードされる
public class GameCommand
{
    public string command { get; set; }
    public Dictionary<string, string> arguments { get; set; }
}
