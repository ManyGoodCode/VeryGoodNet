
namespace OxyPlot
{
    using System;

    public abstract partial class Element
    {
        [Obsolete("Will be removed in v4.0 (#111)")]
        public event EventHandler<OxyKeyEventArgs> KeyDown;

        [Obsolete("Will be removed in v4.0 (#111)")]
        public event EventHandler<OxyMouseDownEventArgs> MouseDown;

        [Obsolete("Will be removed in v4.0 (#111)")]
        public event EventHandler<OxyMouseEventArgs> MouseMove;

        [Obsolete("Will be removed in v4.0 (#111)")]
        public event EventHandler<OxyMouseEventArgs> MouseUp;

        [Obsolete("Will be removed in v4.0 (#111)")]
        public event EventHandler<OxyTouchEventArgs> TouchStarted;

        [Obsolete("Will be removed in v4.0 (#111)")]
        public event EventHandler<OxyTouchEventArgs> TouchDelta;

        [Obsolete("Will be removed in v4.0 (#111)")]
        public event EventHandler<OxyTouchEventArgs> TouchCompleted;

        [Obsolete("Will be removed in v4.0 (#111)")]
        protected internal virtual void OnMouseDown(OxyMouseDownEventArgs e)
        {
            var handler = this.MouseDown;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        [Obsolete("Will be removed in v4.0 (#111)")]
        protected internal virtual void OnMouseMove(OxyMouseEventArgs e)
        {
            var handler = this.MouseMove;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        [Obsolete("Will be removed in v4.0 (#111)")]
        protected internal virtual void OnKeyDown(OxyKeyEventArgs e)
        {
            var handler = this.KeyDown;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        [Obsolete("Will be removed in v4.0 (#111)")]
        protected internal virtual void OnMouseUp(OxyMouseEventArgs e)
        {
            var handler = this.MouseUp;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        [Obsolete("Will be removed in v4.0 (#111)")]
        protected internal virtual void OnTouchStarted(OxyTouchEventArgs e)
        {
            var handler = this.TouchStarted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        [Obsolete("Will be removed in v4.0 (#111)")]
        protected internal virtual void OnTouchDelta(OxyTouchEventArgs e)
        {
            var handler = this.TouchDelta;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        [Obsolete("Will be removed in v4.0 (#111)")]
        protected internal virtual void OnTouchCompleted(OxyTouchEventArgs e)
        {
            var handler = this.TouchCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
