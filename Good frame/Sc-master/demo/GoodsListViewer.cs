using Sc;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace demo
{
    public class TestData
    {
        public string test;
    }

    public class GoodsListViewer : IDisposable
    {
        public Sc.ScMgr scMgr;
        Sc.ScGridView gridView;
        Sc.ScLayer rootLayer;
        Sc.ScListView listView;

        List<TestData> testDatalistFront = new List<TestData>();
        List<TestData> testDatalistBack = new List<TestData>();

        public GoodsListViewer(System.Windows.Forms.Control control)
        {
            scMgr = new Sc.ScMgr(control.Width, control.Height)
            {
                BackgroundColor = Color.FromArgb(255, 246, 245, 251)
            };

            control.Controls.Add(scMgr.control);

            rootLayer = scMgr.GetRootLayer();
            rootLayer.Dock = Sc.ScDockStyle.Fill;

            gridView = new Sc.ScGridView(scMgr)
            {
                Dock = ScDockStyle.Fill,

                //透明度背景色设置
                Opacity = 1.0f,
                BackgroundColor = Color.FromArgb(255, 255, 255, 255),

                //滚动条设置
                VerScrollSize = 10,
                HorScrollSize = 10,
                ScrollBarSliderColor = Color.FromArgb(255, 100, 100, 100),

                //边距设置
                IsUseInside = true,
                Margin = new Utils.Margin(50, 70, 50, 70),
                SideSpacing = new Utils.Margin(10, 10, 10, 10),
                OutsideLineColor = Color.FromArgb(255, 220, 220, 220),
                InsideLineColor = Color.FromArgb(255, 200, 200, 200),

                //列头设置
                HeaderStyle = 0,
                HeaderSpacing = 1,
                HeaderHeight = 50,
                HeaderSizeMode = ScLayerLayoutViewerHeaderSizeMode.ADAPTIVE,
                HeaderControlerSize = 20,

                //内容行设置
                RowHeight = 60f,
                RowSpacing = 1f,
                ItemMinSize = 20,

                //阴影设置
                IsUseShadow = true,
                ShadowStyle = 2,
                ShadowRange = 15,
                ShadowCornersRadius = 1,
                ShadowColor = Color.FromArgb(250, 0, 0, 0),
            };

            //列头标题
            gridView.CreateHeaderTitleEvent += GridView_CreateHeaderTitleEvent;
            gridView.CreateHeaderTitleLayer();

            //生成列
            CreateColumnSetting();

            rootLayer.Add(gridView);

            scMgr.ReBulid();
            CreateBackDataList();

            List<TestData> tmp = testDatalistFront;
            testDatalistFront = testDatalistBack;
            testDatalistBack = tmp;
            testDatalistBack.Clear();

            UpdateDataSource();
        }


        private ScLayer GridView_CreateHeaderTitleEvent(ScMgr scmgr)
        {
            ScLabel headerLabel = new ScLabel(scmgr);
            headerLabel.Name = "Title";
            headerLabel.Dock = ScDockStyle.Fill;
            headerLabel.Text = "订单列表";
            headerLabel.TextPadding = new Margin(20, 0, 0, 0);
            headerLabel.ForeFont = new D2DFont("微软雅黑", 35, SharpDX.DirectWrite.FontWeight.Bold);
            headerLabel.Alignment = TextAlignment.Leading;
            headerLabel.BackgroundColor = Color.FromArgb(100, 255, 0, 0);

            return headerLabel;
        }


        /// <summary>
        /// 生成列
        /// </summary>
        void CreateColumnSetting()
        {
            Sc.ColumnSetting columnSetting = new Sc.ColumnSetting("Test", "测试列1", true, false, 200);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new Sc.ColumnSetting("Test2", "测试列2", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest1;
            columnSetting.DisplayItemValue += DisplayItem1;
            gridView.AppendColumnSetting(columnSetting);

            columnSetting = new Sc.ColumnSetting("Test3", "测试列3", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest3;
            columnSetting.DisplayItemValue += DisplayItem3;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new Sc.ColumnSetting("Test4", "测试列4", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new Sc.ColumnSetting("Test5", "测试列5", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new Sc.ColumnSetting("Test6", "测试列6", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new Sc.ColumnSetting("Test7", "测试列7", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new Sc.ColumnSetting("Test8", "测试列8", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new Sc.ColumnSetting("Test9", "测试列9", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);


            columnSetting = new Sc.ColumnSetting("Test10", "测试列10", false, false, 100);
            columnSetting.CreateHeaderControl += CreateHeaderControlFieldTest;
            columnSetting.CreateItemControl += CreateItemControlFieldTest;
            columnSetting.DisplayItemValue += DisplayItem;
            gridView.AppendColumnSetting(columnSetting);

            gridView.AppendColumnSettingEnd();
        }

        ScLayer CreateHeaderControlFieldTest(Sc.ScMgr scmgr, Sc.ColumnSetting columnSetting)
        {
            Sc.ScLabel label = new Sc.ScLabel(scmgr)
            {
                Dock = ScDockStyle.Fill,
                ForeFont = new D2DFont("微软雅黑", 17, SharpDX.DirectWrite.FontWeight.Bold)
            };

            if (!columnSetting.columnBaseInfo.isHideName)
                label.Text = columnSetting.columnBaseInfo.displayName;

            return label;
        }


        ScLayer CreateItemControlFieldTest1(ScMgr scmgr, ColumnSetting columnSetting)
        {
            Sc.ScLayer layer = new Sc.ScLayer(scmgr)
            {
                Dock = Sc.ScDockStyle.Fill
            };

            Sc.ScCheckBox checkBox = new Sc.ScCheckBox(scmgr)
            {
                CheckType = 0,
                boxSideWidth = 1,
                FillMargin = new Margin(2, 2, 3, 3),
                CheckColor = Color.DarkRed,
                Dock = Sc.ScDockStyle.Center,
                Size = new System.Drawing.SizeF(15, 15)
            };

            checkBox.SetDrawCheckDirectParentLayer(layer);
            layer.Add(checkBox);
            return layer;

        }

        Sc.ScLayer CreateItemControlFieldTest(Sc.ScMgr scmgr, Sc.ColumnSetting columnSetting)
        {
            Sc.ScLabel label = new Sc.ScLabel(scmgr)
            {
                Dock = Sc.ScDockStyle.Fill,
                ForeFont = new D2DFont("微软雅黑", 17, SharpDX.DirectWrite.FontWeight.Bold),
                ForeColor = Color.FromArgb(255, 58, 166, 254)
            };

            return label;
        }

        Sc.ScLayer CreateItemControlFieldTest3(Sc.ScMgr scmgr, Sc.ColumnSetting columnSetting)
        {
            listView = new Sc.ScListView(scmgr);
            listView.Name = "ListView";
            listView.IsUseShadow = false;
            listView.ShadowRange = 4;
            listView.Margin = new Margin(10, 10, 10, 10);

            listView.DisplayItemValue += DisplayItem;
            listView.CreateDefaultContentInfoSeting();
            listView.Dock = Sc.ScDockStyle.Fill;

            Sc.ScLayer listViewPack;
            if (listView.IsUseShadow)
            {
                listViewPack = new ScLayer();
                listViewPack.Name = "ListViewPack";
                listViewPack.Dock = ScDockStyle.Fill;
                listViewPack.Add(listView);
                return listViewPack;
            }
            else
            {
                return listView;
            }
        }

        void DisplayItem(Sc.ScLayer columnItem, int dataRowIdx)
        {
            Sc.ScLabel label = (Sc.ScLabel)columnItem;
            if (label == null)
                return;

            if (dataRowIdx % 2 == 0)
            {
                label.ForeColor = Color.FromArgb(255, 0, 0, 0);
                label.ForeFont = new D2DFont("微软雅黑", 12, SharpDX.DirectWrite.FontWeight.Regular);
            }
            else
            {
                label.ForeColor = Color.FromArgb(255, 0, 0, 255);
                label.ForeFont = new D2DFont("微软雅黑", 17, SharpDX.DirectWrite.FontWeight.Bold);
            }


            label.Text = testDatalistFront[dataRowIdx].test;
            label.Value = label.Text;
        }

        void DisplayItem1(ScLayer columnItem, int dataRowIdx)
        {

        }

        void DisplayItem3(ScLayer columnItem, int dataRowIdx)
        {
            ScListView listView;

            if (columnItem.Name == "ListViewPack")
                listView = (ScListView)(columnItem.controls[1]);
            else
                listView = (ScListView)(columnItem);

            listView.ResetDataRowCount(testDatalistFront.Count());

        }
        public void UpdateDataSource()
        {
            gridView.ResetDataRowCount(testDatalistFront.Count());
        }


        public void CreateBackDataList()
        {
            testDatalistBack.Clear();
            for (int i = 0; i < 106; i++)
            {
                TestData testData = new TestData();
                testData.test = "测试数据" + i;
                testDatalistBack.Add(testData);
            }
        }

        public void Dispose()
        {
            scMgr.Dispose();
        }
    }
}
