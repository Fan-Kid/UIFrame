using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GUIFrame
{
    
    public class UIData {
        public bool isStartWindow       = false;
        public UIType uiType            = UIType.Normal;
        public UIShowMode showMode      = UIShowMode.DoNothing;
        public UIColliderMode collider  = UIColliderMode.None;

    }

    public class BackUISequenceData
    {
        public UIBase hideTargetUI;
        public List<UIID> backShowTargets;
    }

    public class ShowUIData
    {
        //  Reset窗口
        public bool forceResetUI            = false;
        // Clear导航信息
        public bool forceClearBackSeqData   = false;
        // Object 数据
        public object data;
    }

    public delegate bool BoolDelegate();
}
