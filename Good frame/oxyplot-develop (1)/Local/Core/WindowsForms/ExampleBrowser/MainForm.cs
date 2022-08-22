namespace ExampleBrowser
{
    using ExampleLibrary;
    using System.Drawing;
    using System.Windows.Forms;


    public partial class MainForm : Form
    {
        // 访问 ExampleLibrary.Examples 加载特性修饰的类型下用特性修饰的方法
        readonly MainWindowViewModel vm = new MainWindowViewModel();

        public MainForm()
        {
            this.InitializeComponent();
            // 将应用程序的图标显示在界面上
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.InitTree();
        }

        /// <summary>
        /// 加载特性修饰的方法 
        /// 1 ShowCases /  9 Rendering capabilities /  Annotations  /   AreaSeries  /  ArrowAnnotation  /   Axis examples  /   BarSeries  /     BoxPlotSeries   
        /// CandleStickAndVolumeSeries  /    CandleStickSeries
        /// </summary>
        private void InitTree()
        {
            TreeNode node = null;
            foreach (ExampleInfo ex in this.vm.Examples)
            {
                // 如果当前 ExampleInfo 与 上一个 ExampleInfo不属于同一个Type【或者父类】的方法，则在树上新建节点
                if (node == null || node.Text != ex.Category)
                {
                    node = new TreeNode(text: ex.Category);
                    this.treeView1.Nodes.Add(node);
                }

                // 节点的Tag绑定对象
                TreeNode exNode = new TreeNode(ex.Title)
                {
                    Tag = ex
                };

                node.Nodes.Add(exNode);
                if (ex == this.vm.SelectedExample)
                {
                    this.treeView1.SelectedNode = exNode;
                }
            }

            this.treeView1.AfterSelect += this.TreeView1AfterSelect;
        }

        void TreeView1AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.vm.SelectedExample = e.Node.Tag as ExampleInfo;
            this.InitPlot();

            this.transposedCheck.Enabled = this.vm.SelectedExample?.IsTransposable ?? false;
            this.reversedCheck.Enabled = this.vm.SelectedExample?.IsReversible ?? false;
        }

        private void InitPlot()
        {
            if (this.vm.SelectedExample == null)
            {
                this.plot1.Model = null;
                this.plot1.Controller = null;
            }
            else
            {
                ExampleFlags flags = ExampleInfo.PrepareFlags(
                    this.transposedCheck.Enabled && this.transposedCheck.Checked,
                    this.reversedCheck.Enabled && this.reversedCheck.Checked);

                this.plot1.Model = this.vm.SelectedExample.GetModel(flags);
                this.plot1.Controller = this.vm.SelectedExample.GetController(flags);
            }
        }

        private void transposedCheck_CheckedChanged(object sender, System.EventArgs e)
        {
            InitPlot();
        }

        private void reversedCheck_CheckedChanged(object sender, System.EventArgs e)
        {
            InitPlot();
        }
    }
}
