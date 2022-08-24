using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    /// <summary>
    /// 此类未有任何类继承 Animation ： 绘制。
    /// </summary>
    public class ScAnimation : IDisposable
    {
        // 包含 该类型对象的集合容器 
        Sc.ScLayer layer;

        // 定时器【定时执行的事件】
        System.Timers.Timer refreshTimer = null;

        // 设置定时器是执行一次（false）还是一直执行(true)
        bool autoRest = false;

        int durationMS = 20;
        int frameIndex = 0;
        bool isDisposed = false;


        public int animMS = 100;

        // 事件。调用此事件的时候传递的是this【此对象】 且调用此事件的对象为 System.Windows.Forms.Control
        public delegate void AnimationEventHandler(Sc.ScAnimation scAnimation);
        public event AnimationEventHandler AnimationEvent;

        // 委托。函数内封装了上面事件的 Invoke();
        delegate void UpdateCallback(object obj);
        UpdateCallback updateDet;

        public ScAnimation(Sc.ScLayer layer, bool autoRest)
        {
            this.layer = layer;
            this.autoRest = autoRest;
            // 设置委托
            updateDet = this.Update;

            layer.AppendAnimation(this);
        }

        public ScAnimation(Sc.ScLayer layer, int animMS, bool autoRest)
        {
            this.layer = layer;
            this.autoRest = autoRest;
            this.animMS = animMS;
            // 设置委托
            updateDet = this.Update;

            layer.AppendAnimation(this);
        }

        public int AnimMS
        {
            get { return animMS; }
        }

        public int DurationMS
        {
            get { return durationMS; }
            set { durationMS = value; }
        }

        public int FrameIndex
        {
            get { return frameIndex; }
        }

        public void Start()
        {
            frameIndex = 0;
            StartTimer(durationMS);
        }

        public void Stop()
        {
            StopTimer();
        }

        void StartTimer(int period)
        {
            if (isDisposed)
                return;

            refreshTimer = new System.Timers.Timer(period);   //实例化Timer类，设置间隔时间为period毫秒   
            refreshTimer.Elapsed += TimerWork; //到达时间的时候执行事件   
            refreshTimer.AutoReset = autoRest;   //设置是执行一次（false）还是一直执行(true)   
            refreshTimer.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件  
        }

        void StopTimer()
        {
            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
                refreshTimer = null;
            }
        }

        public void TimerWork(object source, EventArgs e)
        {
            if (layer == null || layer.ScMgr == null)
                return;

            frameIndex++;
            // updateDet 是将 this.Update 函数封装了一层。转换为 Delegate
            System.Windows.Forms.Control control = layer.ScMgr.control;
            control.Invoke(method: updateDet, args: this);
        }

        void Update(object obj)
        {
            // 调用此事件的时候传递的是this【此对象】 且调用此事件的对象为 System.Windows.Forms.Control
            this.AnimationEvent?.Invoke(this);
        }

        public void Dispose()
        {
            Stop();
            layer = null;
            isDisposed = true;
        }
    }

}
