using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonGameCommandParse
{
    //Json����GameCommand�ւ̕ϊ��֐�
    public static GameCommand JsonToGameCommand(string json)
    {
        return JsonConvert.DeserializeObject<GameCommand>(json);
    }

}
