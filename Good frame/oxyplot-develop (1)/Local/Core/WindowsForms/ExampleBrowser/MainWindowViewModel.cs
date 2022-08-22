namespace ExampleBrowser
{
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Linq;

    using ExampleLibrary;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IEnumerable<ExampleInfo> examples;

        private ExampleInfo selectedExample;

        public MainWindowViewModel()
        {
            // Category为Type特性修饰的分类名称
            // Title为方法特性修饰的名称
            // Method为具体方法
            this.Examples = ExampleLibrary.Examples.GetList().OrderBy(e => e.Category);
            this.SelectedExample = this.Examples.FirstOrDefault(ei => ei.Title == Properties.Settings.Default.SelectedExample);
        }

        // 接口实现
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<ExampleInfo> Examples
        {
            get
            { 
                return this.examples; 
            }
            set 
            {
                this.examples = value; 
                this.RaisePropertyChanged("Examples");
            }
        }

        public ExampleInfo SelectedExample
        {
            get { return this.selectedExample; }
            set
            {
                this.selectedExample = value; 
                this.RaisePropertyChanged("SelectedExample");
                Properties.Settings.Default.SelectedExample = value != null ? value.Title : null;
                Properties.Settings.Default.Save();
            }
        }

        protected void RaisePropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}