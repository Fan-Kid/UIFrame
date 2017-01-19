using UnityEngine;
using System.Collections;
using System;

namespace GUIFrame
{
    public class UIBase: MonoBehaviour {
        protected UIPanel originPanel;

        // 如果需要可以添加一个BoxCollider屏蔽事件
        private bool isLock     = false;
        protected bool isShown  = false;
    
        // 当前界面ID
        protected UIID uiID = UIID.UIID_Invaild;

        // 指向上一级界面ID（BackSequence无内容，返回上一级）
        protected UIID preUIID = UIID.UIID_Invaild;

        public UIData uiData = new UIData();

        private event BoolDelegate returnPreLogic = null;

        protected Transform mTrs;
        protected virtual void Awake()
        {
            this.gameObject.SetActive(true);
            mTrs = this.gameObject.transform;
            InitUIOnAwake();
        }

        public bool IsLock
        {
            get{ return isLock;}
            set{ isLock = value;}
        }

        private  int minDepth = 1;
        public int MinDepth
        {
            get {return minDepth; }
            set {minDepth = value; }
        }

        public UIID GetID
        {
            get
            {
                if (this.uiID == UIID.UIID_Invaild)
                    Debug.LogError("ui id is " + UIID.UIID_Invaild );
                return uiID;
            }
            private set{ uiID = value;}
        }

        public UIID GetPreUIID
        {
            get { return preUIID;}
            private set { preUIID= value;}
        }

        /// <summary>
        /// 能否添加到导航数据中 
        /// </summary>
        public bool CanAddToBackSeq
        {
            get
            {
                if (this.uiData.uiType == UIType.PopUp)
                    return false;
                if (this.uiData.uiType == UIType.Fixed)
                    return false;
                if (this.uiData.showMode == UIShowMode.NoNeedBack)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// 界面是否要刷新BackSequence
        /// 1.显示NoNeedBack或者从NoNeedBack显示新界面,不更新BackSequence（隐藏自身即可）
        /// 2.HideOther
        /// 3.NeedBack
        /// </summary>
        public bool RefreshBackSeqData
        {
            get
            {
                if (this.uiData.showMode == UIShowMode.HideOther
                        || this.uiData.showMode == UIShowMode.NeedBack)
                    return true;
                return false;

            }
        }

        /// <summary>
        /// 在Awake中调用，初始化界面
        /// </summary>
        public virtual void InitUIOnAwake()
        {
        }

        /// <summary>
        /// 活得窗口管理类
        /// </summary>
        public UIManagerBase GetUIManager
        {
            get
            {
                UIManagerBase baseManager = this.gameObject.GetComponent<UIManagerBase>();
                return baseManager;
            }
            private set {}
        }

        /// <summary>
        /// 重置UI
        /// </summary>
        public virtual void ResetUI()
        {
        }

        /// <summary>
        /// 初始化UI数据
        /// </summary>
        public virtual void InitUIData()
        {
            if (uiData == null)
                uiData = new UIData();
        }

        public virtual void ShowUI()
        {
            isShown = true;
            NGUITools.SetActive(this.gameObject, true);
        }

        public virtual void HideUI(Action action = null)
        {
            IsLock = true;
            isShown = false;
            NGUITools.SetActive(this.gameObject, false);
            if(action != null)
                action();
        }

        /// <summary>
        /// 直接隐藏UI
        /// </summary>
        public void HideUIDirectly()
        {
            IsLock = true;
            isShown = false;
            NGUITools.SetActive(this.gameObject, false);
        }

        public virtual void DestroyUI()
        {
            BeforeDestroyUI();
            GameObject.Destroy(this.gameObject);
        }

        public virtual void BeforeDestroyUI()
        {
        }

        /// <summary>
        /// 界面在退出或者用户点击返回之前都可以注册执行逻辑
        /// </summary>
        protected void RegisterReturnLogic(BoolDelegate newLogic)
        {
            returnPreLogic = newLogic;
        }

        public bool ExccuteReturnLogic()
        {
            if (returnPreLogic == null)
                return false;
            else
                return returnPreLogic();
        }
        
    	// Use this for initialization
    	void Start () {
    	
    	}
    	
    	// Update is called once per frame
    	void Update () {
    	
    	}
    }
}
