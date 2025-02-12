﻿using Sc;
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
    public class BindingData
    {
        public string Text;
    }

    public class GoodsListViewer : IDisposable
    {
        public Sc.ScMgr scMgr;
        Sc.ScGridView GirdView;
        Sc.ScLayer rootLayer;
        Sc.ScListView listView;

        List<BindingData> bindingDatas = new List<BindingData>();

        public GoodsListViewer(System.Windows.Forms.Control control)
        {
            scMgr = new Sc.ScMgr(control.Width, control.Height)
            {
                BackgroundColor = Color.FromArgb(255, 246, 245, 251)
            };

            control.Controls.Add(scMgr.control);

            rootLayer = scMgr.GetRootLayer();
            rootLayer.Dock = Sc.ScDockStyle.Fill;

            GirdView = new Sc.ScGridView(scMgr)
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
            GirdView.CreateHeaderTitleEvent += GridView_CreateHeaderTitleEvent;
            GirdView.CreateHeaderTitleLayer();

            //生成列
            CreateColumnSetting();

            rootLayer.Add(GirdView);

            scMgr.ReBulid();
            NewPostBindingData();
            UpdateDataSource();
        }


        private Sc.ScLayer GridView_CreateHeaderTitleEvent(ScMgr scmgr)
        {
            Sc.ScLabel headerLabel = new Sc.ScLabel(scmgr)
            {
                Name = "Title",
                Dock = ScDockStyle.Fill,
                Text = "订单列表",
                TextPadding = new Margin(20, 0, 0, 0),
                ForeFont = new D2DFont("微软雅黑", 35, SharpDX.DirectWrite.FontWeight.Bold),
                Alignment = TextAlignment.Leading,
                BackgroundColor = Color.FromArgb(100, 255, 0, 0)
            };

            return headerLabel;
        }


        /// <summary>
        /// 生成列
        /// </summary>
        void CreateColumnSetting()
        {
            for (int i = 1; i <= 10; i++)
            {
                ColumnSetting.CreateControlHandler createControlHandler = CreateItemControlField;
                ColumnSetting.DisplayItemHandler displayHandler = DisplayItem;
                // 控件
                if (i == 2)
                    createControlHandler = CreateItemControlField1;
                else if (i == 3)
                    createControlHandler = CreateItemControlField3;

                // 显示
                if (i == 2)
                    displayHandler = DisplayItem1;
                else if (i == 3)
                    displayHandler = DisplayItem3;

                Sc.ColumnSetting setting = new Sc.ColumnSetting(
                    name: string.Format("Test{0}", i),
                    text: string.Format("测试列{0}", i),
                    isHideText: true,
                    isHideColoum: false,
                    width: i == 1 ? 200 : 100);

                setting.CreateHeaderControl += CreateHeaderControlField;
                setting.CreateItemControl += createControlHandler;
                setting.DisplayItemValue += displayHandler;
                GirdView.AppendColumnSetting(setting);
            }

            GirdView.AppendColumnSettingEnd();
        }

        /// <summary>
        /// 创建头控件
        /// </summary>
        Sc.ScLayer CreateHeaderControlField(Sc.ScMgr scmgr, Sc.ColumnSetting columnSetting)
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

        /// <summary>
        /// 表格CheckBox
        /// </summary>
        ScLayer CreateItemControlField1(ScMgr scmgr, ColumnSetting columnSetting)
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

        /// <summary>
        /// 创建表格列名称
        /// </summary>
        Sc.ScLayer CreateItemControlField(Sc.ScMgr scmgr, Sc.ColumnSetting columnSetting)
        {
            Sc.ScLabel label = new Sc.ScLabel(scmgr)
            {
                Dock = Sc.ScDockStyle.Fill,
                ForeFont = new D2DFont("微软雅黑", 17, SharpDX.DirectWrite.FontWeight.Bold),
                ForeColor = Color.FromArgb(255, 58, 166, 254)
            };

            return label;
        }

        /// <summary>
        /// 创建可以拖动的视图  ScListView
        /// </summary>
        Sc.ScLayer CreateItemControlField3(Sc.ScMgr scmgr, Sc.ColumnSetting columnSetting)
        {
            listView = new Sc.ScListView(scmgr)
            {
                Name = "ListView",
                IsUseShadow = false,
                ShadowRange = 4,
                Margin = new Margin(10, 10, 10, 10),
                Dock = Sc.ScDockStyle.Fill
            };

            listView.DisplayItemValue += DisplayItem;
            listView.CreateDefaultContentInfoSeting();

            if (listView.IsUseShadow)
            {
                Sc.ScLayer listViewPack = new Sc.ScLayer()
                {
                    Name = "ListViewPack",
                    Dock = ScDockStyle.Fill
                };

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

            bool isPair = (dataRowIdx % 2 == 0);
            label.ForeColor = isPair
                ? Color.FromArgb(255, 0, 0, 0)
                : Color.FromArgb(255, 0, 0, 255);

            label.ForeFont = isPair
                ? new Sc.D2DFont("微软雅黑", 12, SharpDX.DirectWrite.FontWeight.Regular)
                : new Sc.D2DFont("微软雅黑", 17, SharpDX.DirectWrite.FontWeight.Bold);

            label.Value = label.Text = bindingDatas[dataRowIdx].Text;
        }

        void DisplayItem1(ScLayer columnItem, int dataRowIdx)
        {

        }

        void DisplayItem3(Sc.ScLayer columnItem, int dataRowIdx)
        {
            Sc.ScListView listView;
            if (columnItem.Name == "ListViewPack")
                listView = (Sc.ScListView)(columnItem.controls[1]);
            else
                listView = (Sc.ScListView)(columnItem);
            //listView.ResetDataRowCount(dataRowCount: bindingDatas.Count());
            listView.ResetDataRowCount(dataRowCount: 2);
        }
        public void UpdateDataSource()
        {
            //GirdView.ResetDataRowCount(dataRowCount: bindingDatas.Count());
            GirdView.ResetDataRowCount(dataRowCount: 3);
        }

        public void NewPostBindingData()
        {
            bindingDatas.Clear();
            for (int i = 0; i < 106; i++)
            {
                bindingDatas.Add(new BindingData()
                {
                    Text = "测试数据" + i
                });
            }
        }

        public void Dispose()
        {
            scMgr.Dispose();
        }
    }
}
