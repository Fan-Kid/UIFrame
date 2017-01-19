using UnityEngine;
using System.Collections;
using System;

namespace GUIFrame
{
    interface IUIAnimation
    {
        /// <summary>
        /// 显示动画
        /// </summary>
        void EnterAnimation(EventDelegate.Callback onComplete);

        /// <summary>
        /// 隐藏动画
        /// </summary>
        void QuitAnimation(EventDelegate.Callback onComplete);

        /// <summary>
        /// 重置动画
        /// </summary>
        void ResetAnimation();
    }
}
