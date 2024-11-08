using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonGameCommandParse
{
    //Json‚©‚çGameCommand‚Ö‚Ì•ÏŠ·ŠÖ”
    public static GameCommand JsonToGameCommand(string json)
    {
        return JsonConvert.DeserializeObject<GameCommand>(json);
    }

}
