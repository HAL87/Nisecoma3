﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeParameter : MonoBehaviour
{
    //左上が0番で右上が6番、右下が27番や
    [SerializeField] private int nodeID;
    //エントリーとかゴールとかそういう情報
    //エッジ情報もいるのかなあ
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public int GetNodeID()
    {
        return nodeID;
    }
}