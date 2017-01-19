using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GUIFrame
{
    public abstract class UIManagerBase : MonoBehaviour {
    
        protected Dictionary<UIID, UIBase> allUIs;
        protected Dictionary<UIID, UIBase> shownUIs;
        protected Stack<BackUISequenceData> backSequence;
        // 当前显示活跃的界面
        protected UIBase curShownNormalUI   = null;
        // 上一活跃的界面
        protected UIBase lastShownNormalUI  = null;

        // 是否等待关闭结束
        // 开启：等待界面关闭结束，处理后续逻辑
        // 关闭：不等待界面关闭结束，处理后续逻辑
        protected bool isNeedWaitHideOver = false;

        // 管理界面ID
        protected int managedUIId = 0;

        // 界面按MinDepth排序
        protected class CompareBaseUI : IComparer<UIBase>
        {
            public int Compare(UIBase left, UIBase right)
            {
                return left.MinDepth - right.MinDepth;
            }
        }

        protected virtual void Awake()
        {
            if (allUIs == null)
                allUIs = new Dictionary<UIID, UIBase>();
            if (shownUIs == null)
                shownUIs = new Dictionary<UIID, UIBase>();
            if (backSequence == null)
                backSequence = new Stack<BackUISequenceData>();
        }

        public virtual UIBase GetGameUI(UIID id)
        {
            if (!IsUIInControl(id))
                return null;
            if (allUIs.ContainsKey(id))
                return allUIs[id];
            else
                return null;
        }

        public virtual T GetGameUIScript<T>(UIID id) where T : UIBase
        {
            UIBase baseUI = GetGameUI(id);
            if (baseUI != null)
                return (T)baseUI;
            return (T)((object)null);
        }

        /// <summary>
        /// 初始化当前界面管理类
        /// </summary>
        public virtual void InitUIManager()
        {
            if (allUIs != null)
                allUIs.Clear();
            if (shownUIs != null)
                shownUIs.Clear();
            if (backSequence != null)
                backSequence.Clear();
        }

        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name = "id"> 界面id </param>
        /// <param name = "data"> 显示数据 </param>
        public virtual void ShowUI(UIID id, ShowUIData data = null)
        {

        }

        /// <summary>
        /// 延时显示界面
        /// </summary>
        /// <param name = "delayTime"> 延时时间 </param>
        /// <param name = "id"> 界面id </param>
        /// <param name = "data"> 显示数据 </param>
        public virtual void ShowUIDelay(float delayTime, UIID id, ShowUIData data = null)
        {
            StartCoroutine(_ShowUIDelay(delayTime, id, data));
        }

        private IEnumerator _ShowUIDelay(float delayTime, UIID id, ShowUIData data = null)
        {
            yield return new WaitForSeconds(delayTime);
            ShowUI(id, data);
        }

        protected virtual UIBase ReadyToShowBaseUI(UIID id, ShowUIData data = null)
        {
            return null;
        }

        /// <summary>
        /// 显示界面
        /// </summary>
        protected virtual void RealShowUI(UIBase baseUI, UIID id)
        {
            baseUI.ShowUI();
            shownUIs[id] = baseUI;
            if (baseUI.uiData.uiType == UIType.Normal)
            {
                lastShownNormalUI = curShownNormalUI;
                curShownNormalUI = baseUI;
            }
        }

        /// <summary>
        /// 直接打开窗口
        /// </summary>
        protected void ShowUIForBack(UIID id)
        {
            if (!this.IsUIInControl(id))
            {
                Debug.Log("UIManager has no control power of " + id.ToString());
                return;
            }
            if (shownUIs.ContainsKey(id))
                return;

            UIBase baseUI = GetGameUI(id);
            baseUI.ShowUI();
            shownUIs[baseUI.GetID] = baseUI;
        }

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="id"> 界面id</param>
        public virtual void HideUI(UIID id, Action onCompleted = null)
        {
            CheckDirectlyHide(id, onCompleted);
        }

        protected virtual void CheckDirectlyHide(UIID id, Action onCompleted)
        {
            if (!IsUIInControl(id))
            {
                Debug.Log("UIManager has no control power of " + id.ToString());
                return;
            }
            if (shownUIs.ContainsKey(id))
                return;

            if (!isNeedWaitHideOver)
            {
                if (onCompleted != null)
                    onCompleted();
                shownUIs[id].HideUI(null);
                shownUIs.Remove(id);
                return;
            }

            if (shownUIs.ContainsKey(id))
            {
                if (onCompleted != null)
                {
                    onCompleted += delegate
                    {
                        shownUIs.Remove(id);
                    };
                    shownUIs[id].HideUI(onCompleted);
                }
                else
                {
                    shownUIs[id].HideUI(onCompleted);
                    shownUIs.Remove(id);
                }
            }

        }

        /// <summary>
        /// 返回逻辑
        /// </summary>
        public virtual bool ReturnUI()
        {
            return false;
        }

        private bool ReturnUIManager(UIBase baseUI)
        {
            UIManagerBase baseUIManager = baseUI.GetUIManager;
            bool isValid = false;
            if (baseUIManager != null)
                isValid = baseUIManager.ReturnUI();
            return isValid;
        }

        /// <summary>
        /// 界面导航返回
        /// </summary>
        protected bool RealReturnUI()
        {
            if (backSequence.Count == 0)
            {
                if (curShownNormalUI == null)
                    return false;
                if (ReturnUIManager(curShownNormalUI))
                    return true;

                UIID preUIId = curShownNormalUI.GetPreUIID;
                if (preUIId != UIID.UIID_Invaild)
                {
                    HideUI(curShownNormalUI.GetID, delegate
                            {
                                ShowUI(preUIId, null);
                            });
                }
                else
                    Debug.LogWarning( "currentShowUI " + curShownNormalUI.GetID + "preUIId is " + UIID.UIID_Invaild);
                return false;
            }
            BackUISequenceData backData = backSequence.Peek();
            if(backData != null)
            {
                // 退出当前界面子界面
                if (ReturnUIManager(backData.hideTargetUI))
                    return true;

                UIID hideId = backData.hideTargetUI.GetID;
                if (backData.hideTargetUI != null && shownUIs.ContainsKey(hideId))
                {
                    HideUI(hideId, delegate
                            {
                                if (backData.backShowTargets != null)
                                {
                                    for (int i = 0; i < backData.backShowTargets.Count; i++)
                                    {
                                        UIID backId = backData.backShowTargets[i];
                                        ShowUIForBack(backId);
                                        if (i == backData.backShowTargets.Count - 1)
                                        {
                                            Debug.Log("change currentShownNormalUI:" + backId);
                                            this.lastShownNormalUI = this.curShownNormalUI;
                                            this.curShownNormalUI = GetGameUI(backId);
                                        }
                                    }
                                }

                                //隐藏当前界面
                                backSequence.Pop();
                            });
                }
                else
                    return false;

            }
            return true;
        }

        /// <summary>
        /// 清空导航信息
        /// </summary>
        public void ClearBackSequence()
        {
            if (backSequence != null)
            {
                backSequence.Clear();
            }
        }

        /// <summary>
        /// 清空所有界面
        /// </summary>
        public virtual void ClearAllUI()
        {
            if (allUIs != null)
            {
                foreach ( KeyValuePair<UIID, UIBase> ui in allUIs)
                {
                    UIBase baseUI = ui.Value;
                    baseUI.DestroyUI();
                }
                allUIs.Clear();
                shownUIs.Clear();
                backSequence.Clear();
            }
        }

        protected void HideAllShownUI(bool includeFixed = true)
        {
            List<UIID> removeKey = null;
            if (!includeFixed)
            {
                foreach(KeyValuePair<UIID, UIBase> ui in shownUIs)
                {
                    if (ui.Value.uiData.uiType == UIType.Fixed)
                        continue;
                    if (removeKey == null)
                        removeKey = new List<UIID>();

                    removeKey.Add(ui.Key);
                    ui.Value.HideUIDirectly();
                }

                if (removeKey != null)
                {
                    for (int i = 0; i< removeKey.Count; i++)
                    {
                        shownUIs.Remove(removeKey[i]);
                    }
                }
            }
            else
            {
                foreach ( KeyValuePair<UIID, UIBase> ui in shownUIs)
                    ui.Value.HideUIDirectly();
                shownUIs.Clear();
            }
        }

        protected bool IsUIInControl(UIID id)
        {
            int targetId = 1 << ((int)id);
            return ((managedUIId & targetId) == targetId);
        }

        protected void AddUIInControl(UIID id)
        {
            int targetId = 1 << ((int)id);
            managedUIId |= targetId;
        }

        protected abstract void InitUIControl();
        public virtual void ResetAllInControlUI()
        {
        }
    }
}
