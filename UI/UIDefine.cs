﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GUIFrame
{
    public enum UIID
    {
        UIID_Invaild = 0,
    }

    public enum UIType
    {
        Normal,         // 可推出界面(UIMainMenu,UIRank等)
        Fixed,          // 固定窗口(UITopBar等)
        PopUp,          // 模式窗口
    }

    public enum UIShowMode
    {
        DoNothing,
        HideOther,      // 关闭其他界面
        NeedBack,       // 点击返回按钮关闭当前,不关闭其他界面(需要调整好层级关系)
        NoNeedBack,     // 关闭TopBar,关闭其他界面,不加入backSequence队列
    }

    public enum UIColliderMode
    {
        None,           // 显示该界面不包含碰撞背景
        Normal,         // 碰撞透明背景
        WithBg,         // 碰撞非透明背景
    }

    public class UIDefine
    {
        public static Dictionary<UIID, string> uiPrefabPath = new Dictionary<UIID, string>()
        {
            {UIID.UIID_Invaild, "" },
        };

        public static string UIPrefabPath = "UIPrefab/";
    }
}
