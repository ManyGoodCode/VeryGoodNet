
namespace OxyPlot
{
    public class InputCommandBinding
    {
        public InputCommandBinding(OxyInputGesture gesture, IViewCommand command)
        {
            this.Gesture = gesture;
            this.Command = command;
        }

        public InputCommandBinding(OxyKey key, OxyModifierKeys modifiers, IViewCommand command)
            : this(new OxyKeyGesture(key, modifiers), command)
        {
        }

        public InputCommandBinding(OxyMouseButton mouseButton, OxyModifierKeys modifiers, IViewCommand command)
            : this(new OxyMouseDownGesture(mouseButton, modifiers), command)
        {
        }
        
        public OxyInputGesture Gesture { get; private set; }
        public IViewCommand Command { get; private set; }
    }
}
