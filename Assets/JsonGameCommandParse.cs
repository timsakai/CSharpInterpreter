using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonGameCommandParse
{
    //JsonからGameCommandへの変換関数
    public static GameCommand JsonToGameCommand(string json)
    {
        return JsonConvert.DeserializeObject<GameCommand>(json);
    }

}
