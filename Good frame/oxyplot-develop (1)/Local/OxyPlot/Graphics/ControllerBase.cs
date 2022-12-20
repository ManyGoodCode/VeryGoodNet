
namespace OxyPlot
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class ControllerBase : IController
    {
        private readonly object syncRoot = new object();
        protected ControllerBase()
        {
            this.InputCommandBindings = new List<InputCommandBinding>();
            this.MouseDownManipulators = new List<ManipulatorBase<OxyMouseEventArgs>>();
            this.MouseHoverManipulators = new List<ManipulatorBase<OxyMouseEventArgs>>();
            this.TouchManipulators = new List<ManipulatorBase<OxyTouchEventArgs>>();
        }

        public List<InputCommandBinding> InputCommandBindings { get; private set; }
        protected IList<ManipulatorBase<OxyMouseEventArgs>> MouseDownManipulators { get; private set; }
        protected IList<ManipulatorBase<OxyMouseEventArgs>> MouseHoverManipulators { get; private set; }
        protected IList<ManipulatorBase<OxyTouchEventArgs>> TouchManipulators { get; private set; }
        public virtual bool HandleGesture(IView view, OxyInputGesture gesture, OxyInputEventArgs args)
        {
            var command = this.GetCommand(gesture);
            return this.HandleCommand(command, view, args);
        }

        public virtual bool HandleMouseDown(IView view, OxyMouseDownEventArgs args)
        {
            lock (this.GetSyncRoot(view))
            {
                if (view.ActualModel != null)
                {
                    view.ActualModel.HandleMouseDown(this, args);
                    if (args.Handled)
                    {
                        return true;
                    }
                }

                var command = this.GetCommand(new OxyMouseDownGesture(args.ChangedButton, args.ModifierKeys, args.ClickCount));
                return this.HandleCommand(command, view, args);
            }
        }

        public virtual bool HandleMouseEnter(IView view, OxyMouseEventArgs args)
        {
            lock (this.GetSyncRoot(view))
            {
                if (view.ActualModel != null)
                {
                    view.ActualModel.HandleMouseEnter(this, args);
                    if (args.Handled)
                    {
                        return true;
                    }
                }

                var command = this.GetCommand(new OxyMouseEnterGesture(args.ModifierKeys));
                return this.HandleCommand(command, view, args);
            }
        }

        public virtual bool HandleMouseLeave(IView view, OxyMouseEventArgs args)
        {
            lock (this.GetSyncRoot(view))
            {
                if (view.ActualModel != null)
                {
                    view.ActualModel.HandleMouseLeave(this, args);
                    if (args.Handled)
                    {
                        return true;
                    }
                }

                foreach (var m in this.MouseHoverManipulators.ToArray())
                {
                    m.Completed(args);
                    this.MouseHoverManipulators.Remove(m);
                }

                return args.Handled;
            }
        }

        public virtual bool HandleMouseMove(IView view, OxyMouseEventArgs args)
        {
            lock (this.GetSyncRoot(view))
            {
                if (view.ActualModel != null)
                {
                    view.ActualModel.HandleMouseMove(this, args);
                    if (args.Handled)
                    {
                        return true;
                    }
                }

                foreach (var m in this.MouseDownManipulators)
                {
                    m.Delta(args);
                }

                foreach (var m in this.MouseHoverManipulators)
                {
                    m.Delta(args);
                }

                return args.Handled;
            }
        }

        public virtual bool HandleMouseUp(IView view, OxyMouseEventArgs args)
        {
            lock (this.GetSyncRoot(view))
            {
                if (view.ActualModel != null)
                {
                    view.ActualModel.HandleMouseUp(this, args);
                    if (args.Handled)
                    {
                        return true;
                    }
                }

                foreach (var m in this.MouseDownManipulators.ToArray())
                {
                    m.Completed(args);
                    this.MouseDownManipulators.Remove(m);
                }

                return args.Handled;
            }
        }

        public virtual bool HandleMouseWheel(IView view, OxyMouseWheelEventArgs args)
        {
            lock (this.GetSyncRoot(view))
            {
                var command = this.GetCommand(new OxyMouseWheelGesture(args.ModifierKeys));
                return this.HandleCommand(command, view, args);
            }
        }

        public virtual bool HandleTouchStarted(IView view, OxyTouchEventArgs args)
        {
            lock (this.GetSyncRoot(view))
            {
                if (view.ActualModel != null)
                {
                    view.ActualModel.HandleTouchStarted(this, args);
                    if (args.Handled)
                    {
                        return true;
                    }
                }

                var command = this.GetCommand(new OxyTouchGesture());
                return this.HandleCommand(command, view, args);
            }
        }

        public virtual bool HandleTouchDelta(IView view, OxyTouchEventArgs args)
        {
            lock (this.GetSyncRoot(view))
            {
                if (view.ActualModel != null)
                {
                    view.ActualModel.HandleTouchDelta(this, args);
                    if (args.Handled)
                    {
                        return true;
                    }
                }

                foreach (var m in this.TouchManipulators)
                {
                    m.Delta(args);
                }

                return args.Handled;
            }
        }

        public virtual bool HandleTouchCompleted(IView view, OxyTouchEventArgs args)
        {
            lock (this.GetSyncRoot(view))
            {
                if (view.ActualModel != null)
                {
                    view.ActualModel.HandleTouchCompleted(this, args);
                    if (args.Handled)
                    {
                        return true;
                    }
                }

                foreach (var m in this.TouchManipulators.ToArray())
                {
                    m.Completed(args);
                    this.TouchManipulators.Remove(m);
                }

                return args.Handled;
            }
        }
        
        public virtual bool HandleKeyDown(IView view, OxyKeyEventArgs args)
        {
            lock (this.GetSyncRoot(view))
            {
                if (view.ActualModel == null)
                {
                    return false;
                }

                view.ActualModel.HandleKeyDown(this, args);
                if (args.Handled)
                {
                    return true;
                }

                var command = this.GetCommand(new OxyKeyGesture(args.Key, args.ModifierKeys));
                return this.HandleCommand(command, view, args);
            }
        }

        public virtual void AddMouseManipulator(
            IView view,
            ManipulatorBase<OxyMouseEventArgs> manipulator,
            OxyMouseDownEventArgs args)
        {
            this.MouseDownManipulators.Add(manipulator);
            manipulator.Started(args);
        }
        
        public virtual void AddHoverManipulator(
            IView view,
            ManipulatorBase<OxyMouseEventArgs> manipulator,
            OxyMouseEventArgs args)
        {
            this.MouseHoverManipulators.Add(manipulator);
            manipulator.Started(args);
        }

        public virtual void AddTouchManipulator(
            IView view,
            ManipulatorBase<OxyTouchEventArgs> manipulator,
            OxyTouchEventArgs args)
        {
            this.TouchManipulators.Add(manipulator);
            manipulator.Started(args);
        }
        
        public virtual void Bind(OxyMouseDownGesture gesture, IViewCommand<OxyMouseDownEventArgs> command)
        {
            this.BindCore(gesture, command);
        }

        public virtual void Bind(OxyMouseEnterGesture gesture, IViewCommand<OxyMouseEventArgs> command)
        {
            this.BindCore(gesture, command);
        }

        public virtual void Bind(OxyMouseWheelGesture gesture, IViewCommand<OxyMouseWheelEventArgs> command)
        {
            this.BindCore(gesture, command);
        }

        public virtual void Bind(OxyTouchGesture gesture, IViewCommand<OxyTouchEventArgs> command)
        {
            this.BindCore(gesture, command);
        }

        public virtual void Bind(OxyKeyGesture gesture, IViewCommand<OxyKeyEventArgs> command)
        {
            this.BindCore(gesture, command);
        }
        
        public virtual void Unbind(OxyInputGesture gesture)
        {
            // ReSharper disable once RedundantNameQualifier
            foreach (var icb in this.InputCommandBindings.Where(icb => icb.Gesture.Equals(gesture)).ToArray())
            {
                this.InputCommandBindings.Remove(icb);
            }
        }

        public virtual void Unbind(IViewCommand command)
        {
            // ReSharper disable once RedundantNameQualifier
            foreach (var icb in this.InputCommandBindings.Where(icb => object.ReferenceEquals(icb.Command, command)).ToArray())
            {
                this.InputCommandBindings.Remove(icb);
            }
        }

        public virtual void UnbindAll()
        {
            this.InputCommandBindings.Clear();
        }

        protected void BindCore(OxyInputGesture gesture, IViewCommand command)
        {
            var current = this.InputCommandBindings.FirstOrDefault(icb => icb.Gesture.Equals(gesture));
            if (current != null)
            {
                this.InputCommandBindings.Remove(current);
            }

            if (command != null)
            {
                this.InputCommandBindings.Add(new InputCommandBinding(gesture, command));
            }
        }

        protected virtual IViewCommand GetCommand(OxyInputGesture gesture)
        {
            var binding = this.InputCommandBindings.FirstOrDefault(b => b.Gesture.Equals(gesture));
            if (binding == null)
            {
                return null;
            }

            return binding.Command;
        }
        
        protected virtual bool HandleCommand(IViewCommand command, IView view, OxyInputEventArgs args)
        {
            if (command == null)
            {
                return false;
            }

            command.Execute(view, this, args);
            return args.Handled;
        }

        protected object GetSyncRoot(IView view)
        {
            return view.ActualModel != null ? view.ActualModel.SyncRoot : this.syncRoot;
        }
    }
}
