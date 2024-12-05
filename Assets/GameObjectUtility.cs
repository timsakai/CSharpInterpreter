using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GameObjectUtility
{
    public static int DestroyChildrenWithTag( this GameObject _gameObject,string _tag)
    {
        //デストロイ本体
        int all_child_count = _gameObject.transform.childCount;
        Transform[] all_childs = new Transform[0];
        all_childs = _gameObject.GetComponentsInChildren<Transform>();
        foreach (var item in all_childs)
        {
            if (item.CompareTag(_tag))
                GameObject.Destroy(item.gameObject);
        }
        return 1;
    }
}
