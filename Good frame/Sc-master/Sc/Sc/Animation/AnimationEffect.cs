using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class AnimationEffect
    {
        protected bool isStop = false;

        /// <summary>
        /// 判断当前值 【从 startValue 开始的值增加】是否超出停止值，超出的话，标记停止
        /// 
        /// 返回当前的值
        /// </summary>
        public virtual float GetCurtValue()
        {
            return 0;
        }

        public bool IsStop
        {
            get { return isStop; }
            set { isStop = value; }
        }
    }

    public class ScLinearAnimation : AnimationEffect
    {
        float step = 0;
        Sc.ScAnimation scAnim = null;
        float startValue;
        float stopValue;

        public ScLinearAnimation(float startValue, float stopValue, Sc.ScAnimation scAnimation)
        {
            scAnim = scAnimation;
            this.startValue = startValue;
            this.stopValue = stopValue;
            this.step = (stopValue - startValue) / scAnim.AnimMS * scAnim.DurationMS;
        }

        /// <summary>
        /// 判断当前值 【从 startValue 开始的值增加】是否超出停止值，超出的话，标记停止
        /// </summary>
        public override float GetCurtValue()
        {
            float val = startValue + scAnim.FrameIndex * this.step;

            if ((this.step >= 0 && val >= stopValue) || (this.step <= 0 && val <= stopValue))
            {
                val = stopValue;
                isStop = true;
            }

            return val;
        }
    }

    public class ScStepAnimation : AnimationEffect
    {
        float step = 0;
        Sc.ScAnimation scAnim = null;
        float startValue;

        public ScStepAnimation(float startValue, float step, Sc.ScAnimation scAnimation)
        {
            scAnim = scAnimation;
            this.startValue = startValue;
            this.step = step;
        }


        public override float GetCurtValue()
        {
            float val = startValue + scAnim.FrameIndex * this.step;
            return val;
        }
    }
}
