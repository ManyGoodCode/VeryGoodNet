using SharpDX;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Utils;

using Rectangle = System.Drawing.Rectangle;
using RectangleF = System.Drawing.RectangleF;
using Matrix = System.Drawing.Drawing2D.Matrix;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Bitmap = System.Drawing.Bitmap;
using WicBitmap = SharpDX.WIC.Bitmap;
using SharpDX.Direct2D1;
using System.Runtime.InteropServices;

namespace Sc
{
    public class ScMgr : IDisposable
    {
        private ScLayer rootParent;
        private ScLayer rootScLayer = new ScLayer();
        private ScReDrawTree reDrawTree = new ScReDrawTree();
        private ScLayer cacheRootScLayer = null;


        public ScLayer FocusScControl { get; set; }

        /// <summary>
        /// System.Windows.Forms
        /// </summary>
        public System.Windows.Forms.Control control;

        /// <summary>
        /// System.Drawing.Bitmap
        /// </summary>
        public Bitmap bitmap;

        /// <summary>
        /// SharpDX.WIC.Bitmap
        /// </summary>
        public WicBitmap wicBitmap;


        private List<ScLayer> mouseMoveScControlList;
        private List<ScLayer> oldMouseMoveScControlList;

        public List<ScLayer> rebulidLayerList = new List<ScLayer>();
        public Dictionary<string, Dot9BitmapD2D> dot9BitmaShadowDict = new Dictionary<string, Dot9BitmapD2D>();
        public Color? BackgroundColor { get; set; }
        public System.Drawing.Image BackgroundImage { get; set; }


        private ScGraphics graphics = null;
        private SizeF sizeScale;
        private GraphicsType graphicsType = GraphicsType.D2D;
        private DrawType drawType = DrawType.NoImage;


        public ControlType controlType = ControlType.StdControl;
        public Matrix matrix = new Matrix();


        private Point mouseOrgLocation; //记录鼠标指针的坐标
        private Point controlOrgLocation;
        private bool isMouseDown = false; //记录鼠标按键是否按下


        public ScMgr(Control stdControl, bool isUsedUpdateLayerFrm = false)
        {
            if (isUsedUpdateLayerFrm)
            {
                stdControl = stdControl ?? new Sc.UpdateLayerFrm();
                Init(form: stdControl as Sc.UpdateLayerFrm);
            }
            else
            {
                Init(width: stdControl.Width, height: stdControl.Height, drawType: DrawType.NoImage);
                stdControl.Controls.Add(control);
            }
        }

        public ScMgr(int width, int height, DrawType drawType = DrawType.NoImage)
        {
            Init(width, height, drawType);
        }


        private void Init(int width, int height, DrawType drawType = DrawType.NoImage)
        {
            cacheRootScLayer = rootScLayer;
            this.drawType = drawType;
            this.graphicsType = GraphicsType.D2D;

            if (drawType == DrawType.NoImage)
            {
                // 创建Control
                control = new Sc.ScLayerControl(scMgr: this)
                {
                    Width = width,
                    Height = height,
                    Dock = DockStyle.Fill
                };
            }
            else
            {
                bitmap = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                wicBitmap = new SharpDX.WIC.Bitmap(
                    factory: new SharpDX.WIC.ImagingFactory(),
                    width: width,
                    height: height,
                    pixelFormat: SharpDX.WIC.PixelFormat.Format32bppPBGRA,
                    option: SharpDX.WIC.BitmapCreateCacheOption.CacheOnLoad);
            }


            GraphicsType = graphicsType;
            D2DGraphics d2dGraph = (D2DGraphics)graphics;
            SharpDX.Size2 pixelSize = d2dGraph.renderTarget.PixelSize;
            SharpDX.Size2F logicSize = d2dGraph.renderTarget.Size;
            sizeScale = new System.Drawing.SizeF(
                width: logicSize.Width / pixelSize.Width,
                height: logicSize.Height / pixelSize.Height);


            rootScLayer.ScMgr = this;
            rootScLayer.Dock = ScDockStyle.Fill;
            rootScLayer.Name = "__root__";
            rootScLayer.D2DPaint += RootScControl_D2DPaint;

            rootParent = new Sc.ScLayer(this)
            {
                DirectionRect = rootScLayer.DirectionRect,
                DrawBox = rootScLayer.DirectionRect
            };

            rootParent.Add(rootScLayer);
            RegControlEvent();
        }


        private void Init(Sc.UpdateLayerFrm form)
        {
            cacheRootScLayer = rootScLayer;
            drawType = DrawType.Image;
            controlType = ControlType.UpdateLayerForm;
            form.scMgr = this;
            control = form;

            bitmap = new Bitmap(control.Width, control.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            SharpDX.WIC.ImagingFactory wicFactory = new SharpDX.WIC.ImagingFactory();
            wicBitmap = new WicBitmap(
                wicFactory,
                control.Width, control.Height,
                SharpDX.WIC.PixelFormat.Format32bppPBGRA,
                BitmapCreateCacheOption.CacheOnLoad);

            form.bitmap = bitmap;

            this.graphicsType = Sc.GraphicsType.D2D;
            GraphicsType = Sc.GraphicsType.D2D;

            Sc.D2DGraphics d2dGraph = (Sc.D2DGraphics)graphics;
            SharpDX.Size2 pixelSize = d2dGraph.renderTarget.PixelSize;
            SharpDX.Size2F logicSize = d2dGraph.renderTarget.Size;
            sizeScale = new System.Drawing.SizeF(logicSize.Width / pixelSize.Width, logicSize.Height / pixelSize.Height);

            rootScLayer.ScMgr = this;
            rootScLayer.Dock = Sc.ScDockStyle.Fill;
            rootScLayer.Name = "__root__";
            rootScLayer.D2DPaint += RootScControl_D2DPaint;

            rootParent = new Sc.ScLayer(this);
            rootParent.DirectionRect = rootScLayer.DirectionRect;
            rootParent.DrawBox = rootScLayer.DirectionRect;
            rootParent.Add(rootScLayer);

            RegControlEvent();
        }

        public Sc.GraphicsType GraphicsType
        {
            get { return graphicsType; }
            set
            {
                graphicsType = value;
                CreateGraphics();
            }
        }

        public Sc.ScGraphics Graphics
        {
            get { return graphics; }
        }


        private void CreateGraphics()
        {
            switch (GraphicsType)
            {
                case Sc.GraphicsType.D2D:
                    CreateD2D();
                    break;
            }
        }

        private bool CreateD2D()
        {
            foreach (KeyValuePair<string, Dot9BitmapD2D> item in dot9BitmaShadowDict)
            {
                item.Value.Dispose();
            }

            dot9BitmaShadowDict.Clear();
            if (graphics != null)
                graphics.Dispose();

            if (cacheRootScLayer != null)
                rootScLayer = cacheRootScLayer;

            if (drawType == Sc.DrawType.NoImage &&
                (control.Width <= 0 || control.Height <= 0))
            {
                rootScLayer = null;
                return false;
            }
            else if (drawType != Sc.DrawType.NoImage &&
                (bitmap.Width <= 0 || bitmap.Height <= 0))
            {
                rootScLayer = null;
                return false;
            }

            if (drawType == Sc.DrawType.NoImage)
                graphics = new Sc.D2DGraphics(control: control);
            else
                graphics = new Sc.D2DGraphics(wicBitmap: wicBitmap);

            foreach (Sc.ScLayer layer in rebulidLayerList)
            {
                layer.ScReBulid();
            }

            return true;
        }

        private void ReBulidD2D()
        {
            if (drawType == Sc.DrawType.NoImage)
            {
                graphics.ReSize(width: control.Width, height: control.Height);
                foreach (Sc.ScLayer layer in rebulidLayerList)
                {
                    layer.ScReBulid();
                }
            }
        }

        public void AddReBulidLayer(Sc.ScLayer layer)
        {
            rebulidLayerList.Add(layer);
        }

        public void ClearBitmapRect(System.Drawing.RectangleF clipRect)
        {
            if (bitmap != null && controlType == Sc.ControlType.UpdateLayerForm)
            {
                System.Drawing.Rectangle rc = new System.Drawing.Rectangle(
                    (int)clipRect.Left, (int)clipRect.Top,
                    (int)clipRect.Width, (int)clipRect.Height);

                ColorDisplace.Displace(bitmap, Color.FromArgb(0, 0, 0, 0), rc, true);
            }
        }

        private void RootScControl_D2DPaint(D2DGraphics g)
        {
            SharpDX.Mathematics.Interop.RawRectangleF rect = new SharpDX.Mathematics.Interop.RawRectangleF(0, 0, rootScLayer.Width, rootScLayer.Height);

            if (BackgroundColor != null)
            {
                SharpDX.Mathematics.Interop.RawColor4 color = Sc.GDIDataD2DUtils.TransToRawColor4(BackgroundColor.Value);
                g.RenderTarget.Clear(color);
            }
            else if (controlType == Sc.ControlType.UpdateLayerForm)
            {
                g.RenderTarget.Clear(new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 0));
            }
        }


        public void SetImeWindowsPos(int x, int y)
        {
            if (controlType == ControlType.StdControl)
                ((Sc.ScLayerControl)control).SetImeWindowsPos(x, y);
            else
                ((Sc.UpdateLayerFrm)control).SetImeWindowsPos(x, y);
        }


        public void PaintToBitmap()
        {
            if (bitmap == null || drawType != DrawType.Image)
                return;

            System.Drawing.Rectangle clipRect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            PaintToBitmap(clipRect);
        }

        public void PaintToBitmap(System.Drawing.Rectangle rc)
        {
            if (bitmap == null || drawType != DrawType.Image)
                return;

            graphics.BeginDraw();
            Sc.ScDrawNode rootNode = reDrawTree.ReCreateReDrawTree(rootScLayer, rc);
            reDrawTree.Draw(graphics);
            graphics.EndDraw();
            if (graphicsType == Sc.GraphicsType.D2D && rootNode != null)
            {
                unsafe
                {
                    RectangleF clip = rootNode.clipRect;
                    clip.X = (int)(clip.X / sizeScale.Width);
                    clip.Y = (int)(clip.Y / sizeScale.Height);
                    clip.Width = (int)(clip.Width / sizeScale.Width);
                    clip.Height = (int)(clip.Height / sizeScale.Height);

                    Rectangle bitmapRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    BitmapData srcBmData = bitmap.LockBits(bitmapRect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    byte* ptr = (byte*)srcBmData.Scan0;
                    ptr += (int)clip.Y * srcBmData.Stride + (int)clip.X * 4;
                    DataPointer dataPtr = new DataPointer(ptr, (int)clip.Height * srcBmData.Stride);
                    RawBox box = new RawBox((int)clip.X, (int)clip.Y, (int)clip.Width, (int)clip.Height);
                    wicBitmap.CopyPixels(box, srcBmData.Stride, dataPtr);
                    bitmap.UnlockBits(srcBmData);
                }
            }
        }


        public void Paint(PaintEventArgs e)
        {
            if (rootScLayer == null || graphics == null)
                return;

            _PaintByD2D(e.ClipRectangle);
        }

        void _PaintByD2D(Rectangle refreshArea)
        {
            Sc.D2DGraphics d2dGraphics = (Sc.D2DGraphics)graphics;

            if (drawType == Sc.DrawType.NoImage)
            {
                WindowRenderTarget wRT = (WindowRenderTarget)d2dGraphics.RenderTarget;
                WindowState wstate = wRT.CheckWindowState();
                if (wstate != WindowState.Occluded)
                    _RenderByD2D(d2dGraphics, refreshArea);
            }
            else
            {
                _RenderByD2D(d2dGraphics, refreshArea);
            }
        }

        void _RenderByD2D(Sc.D2DGraphics graph, Rectangle refreshArea)
        {
            try
            {
                graph.BeginDraw();
                reDrawTree.ReCreateReDrawTree(rootScLayer, refreshArea);
                reDrawTree.Draw(graph);
                graph.EndDraw();
            }
            catch (Exception ex)
            {
                CreateD2D();
                control.Refresh();
                control.Update();
            }
        }

        public ScLayer GetRootLayer()
        {
            return rootScLayer;
        }
        public void Refresh()
        {
            Refresh(Rectangle.Empty);
        }
        public void Refresh(RectangleF refreshArea)
        {
            Rectangle rect;

            if (rootScLayer == null)
                return;

            if (refreshArea == Rectangle.Empty)
            {
                rect = new Rectangle(
                0,
                0,
                (int)rootScLayer.Width,
                (int)rootScLayer.Height);
            }
            else
            {
                rect = new Rectangle(
                    (int)refreshArea.X - 1,
                    (int)refreshArea.Y - 1,
                    (int)Math.Round(refreshArea.Width) + 2,
                    (int)Math.Round(refreshArea.Height) + 2);
            }

            if (controlType == ControlType.UpdateLayerForm)
            {
                ((UpdateLayerFrm)control).Invalidate(rect, true);
            }
            else
            {
                control.Invalidate(rect, true);
            }
        }


        public void Update()
        {
            control.Update();
        }


        private void RootScLayer_MouseUp(object sender, ScMouseEventArgs e)
        {
            // 修改鼠标状态isMouseDown的值
            // 确保只有鼠标左键按下并移动时，才移动窗体
            if (e.Button == MouseButtons.Left)
            {
                //松开鼠标时，停止移动
                isMouseDown = false;
                //Top高度小于0的时候，等于0
                if (control.Top < 0)
                {
                    control.Top = 0;
                }
            }
        }

        private void RootScLayer_MouseMove(object sender, ScMouseEventArgs e)
        {
            //确定开启了移动模式后
            if (isMouseDown)
            {
                //移动的位置计算
                Point mousePos = Control.MousePosition;

                mousePos.X -= mouseOrgLocation.X;
                mousePos.Y -= mouseOrgLocation.Y;

                Point frmPos = controlOrgLocation;
                frmPos.X += mousePos.X;
                frmPos.Y += mousePos.Y;

                control.Location = frmPos;
            }
        }

        private void RootScLayer_MouseDown(object sender, ScMouseEventArgs e)
        {
            //点击窗体时，记录鼠标位置，启动移动
            if (e.Button == MouseButtons.Left)
            {
                controlOrgLocation = control.Location;
                mouseOrgLocation = Control.MousePosition;
                isMouseDown = true;
            }
        }

        private void Control_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (mouseMoveScControlList == null)
                return;

            PointF ptf;
            PointF scMouseLocation;
            ScMouseEventArgs mouseEventArgs;

            foreach (ScLayer control in mouseMoveScControlList)
            {
                Point pt = new Point((int)(e.Location.X * sizeScale.Width), (int)(e.Location.Y * sizeScale.Height));
                ptf = control.TransGlobalToLocal(pt);
                scMouseLocation = new PointF(ptf.X, ptf.Y);
                mouseEventArgs = new ScMouseEventArgs(e.Button, scMouseLocation);
                control.ScMouseDoubleClick(mouseEventArgs);
            }
        }

        private void Control_SizeChanged(object sender, EventArgs e)
        {
            ReBulid();
        }

        public void ReBulid()
        {
            if (control.Width <= 0 || control.Height <= 0)
            {
                rootScLayer = null;
                return;
            }

            if (GraphicsType == GraphicsType.D2D)
            {
                ReBulidD2D();
                rootScLayer = cacheRootScLayer;
            }

            rootScLayer.SuspendLayout();
            rootScLayer.DirectionRect = new RectangleF(0, 0, control.Width * sizeScale.Width, control.Height * sizeScale.Height);
            rootScLayer.DrawBox = rootScLayer.DirectionRect;
            rootScLayer.ResumeLayout(true);

        }

        public void Show()
        {
            UpdateLayerFrm frm = control as UpdateLayerFrm;
            frm.Show();
        }

        private void Control_ImeStringEvent(string imeString)
        {
            FocusScControl?.ScImeStringEvent(imeString);
        }

        private void Control_CharEvent(char c)
        {
            FocusScControl?.ScCharEvent(c);
        }

        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            FocusScControl?.ScKeyUp(e);
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            FocusScControl?.ScKeyDown(e);
        }

        private void Control_LostFocus(object sender, EventArgs e)
        {
            FocusScControl?.ScLostFocus(e);
        }

        private void Control_GotFocus(object sender, EventArgs e)
        {
            FocusScControl?.ScGotFocus(e);
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            if (mouseMoveScControlList == null)
                return;

            foreach (ScLayer control in mouseMoveScControlList)
            {
                control.ScMouseLeave();
            }

            mouseMoveScControlList = null;
            oldMouseMoveScControlList = null;
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (mouseMoveScControlList == null)
                return;

            PointF ptf;
            PointF scMouseLocation;
            ScMouseEventArgs mouseEventArgs;

            foreach (ScLayer control in mouseMoveScControlList)
            {
                if (control.Visible == false)
                    continue;

                Point pt = new Point((int)(e.Location.X * sizeScale.Width), (int)(e.Location.Y * sizeScale.Height));
                ptf = control.TransGlobalToLocal(pt);
                scMouseLocation = new PointF(ptf.X, ptf.Y);
                mouseEventArgs = new ScMouseEventArgs(e.Button, scMouseLocation);
                control.ScMouseDown(mouseEventArgs);
            }
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseMoveScControlList == null)
                return;

            PointF ptf;
            PointF scMouseLocation;
            ScMouseEventArgs mouseEventArgs;

            foreach (ScLayer control in mouseMoveScControlList)
            {
                if (control.Visible == false)
                    continue;

                Point pt = new Point((int)(e.Location.X * sizeScale.Width), (int)(e.Location.Y * sizeScale.Height));
                ptf = control.TransGlobalToLocal(pt);
                scMouseLocation = new PointF(ptf.X, ptf.Y);
                mouseEventArgs = new ScMouseEventArgs(e.Button, scMouseLocation);
                control.ScMouseUp(mouseEventArgs);
            }
        }

        private void Control_MouseWheel(object sender, MouseEventArgs e)
        {
            if (mouseMoveScControlList == null)
                return;

            PointF ptf;
            PointF scMouseLocation;
            ScMouseEventArgs mouseEventArgs;

            foreach (ScLayer control in mouseMoveScControlList)
            {
                if (control.Visible == false)
                    continue;

                Point pt = new Point((int)(e.Location.X * sizeScale.Width), (int)(e.Location.Y * sizeScale.Height));
                ptf = control.TransGlobalToLocal(pt);
                scMouseLocation = new PointF(ptf.X, ptf.Y);
                mouseEventArgs = new ScMouseEventArgs(e.Button, scMouseLocation, e.Delta);
                control.ScMouseWheel(mouseEventArgs);
            }
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ScMouseMove(e);
                return;
            }

            Point pt = new Point((int)(e.Location.X * sizeScale.Width), (int)(e.Location.Y * sizeScale.Height));
            CheckScControlMouseMove(pt);

            ScMouseLeave();
            ScMouseEnter(e);
            ScMouseMove(e);
        }


        void CheckScControlMouseMove(Point mouseLocation)
        {
            if (rootScLayer == null)
                return;

            if (mouseMoveScControlList != null)
                oldMouseMoveScControlList = mouseMoveScControlList;

            mouseMoveScControlList = new List<ScLayer>();

            bool isChildHitThrough = CheckChildScControlMouseMove(rootScLayer, mouseLocation);

            if (isChildHitThrough)
                mouseMoveScControlList.Add(rootScLayer);
        }



        bool CheckChildScControlMouseMove(Sc.ScLayer parent, Point mouseLocation)
        {
            Sc.ScLayer layer;
            bool isChildHitThrough;
            bool ret = true;

            for (int i = parent.controls.Count() - 1; i >= 0; i--)
            {
                layer = parent.controls[i];
                if (layer.Visible == false)
                    continue;

                // 确定根Layout层矩形是否与扫描到Layout矩形相交
                if (!rootScLayer.DrawBox.IntersectsWith(layer.DrawBox))
                {
                    continue;
                }

                if (layer.FillContainsPoint(mouseLocation))
                {
                    ret = true;
                    isChildHitThrough = CheckChildScControlMouseMove(layer, mouseLocation);

                    if (isChildHitThrough)
                    {
                        mouseMoveScControlList.Add(layer);

                        if (layer.IsHitThrough)
                            continue;
                        else
                            ret = false;
                    }
                    else
                        ret = false;

                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// 将鼠标当前的移动位置传递给 所有可见的 Layout Control
        /// </summary>
        void ScMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            PointF ptf;
            PointF scMouseLocation;
            Sc.ScMouseEventArgs mouseEventArgs;
            if (mouseMoveScControlList == null)
                return;

            foreach (Sc.ScLayer control in mouseMoveScControlList)
            {
                if (control.Visible == false)
                    continue;

                Point pt = new Point((int)(e.Location.X * sizeScale.Width), (int)(e.Location.Y * sizeScale.Height));
                ptf = control.TransGlobalToLocal(pt);
                scMouseLocation = new PointF(ptf.X, ptf.Y);
                mouseEventArgs = new Sc.ScMouseEventArgs(e.Button, scMouseLocation);
                control.ScMouseMove(mouseEventArgs);
            }
        }

        /// <summary>
        /// 通过对比现在的鼠标所在可见的Layout Control和以前鼠标所在可见的Layout Control，
        /// 遍历找到在以前但不在现在的执行  ScMouseLeave()
        /// </summary>
        void ScMouseLeave()
        {
            if (oldMouseMoveScControlList == null)
                return;

            bool isFind = false;

            foreach (Sc.ScLayer oldControl in oldMouseMoveScControlList)
            {
                if (oldControl.Visible == false)
                    continue;

                isFind = false;

                foreach (Sc.ScLayer newControl in mouseMoveScControlList)
                {
                    if (newControl.Visible == false)
                        continue;

                    if (oldControl == newControl)
                    {
                        isFind = true;
                        break;
                    }
                }

                if (isFind == false)
                {
                    oldControl.ScMouseLeave();
                }
            }
        }

        /// <summary>
        /// 通过对比现在的鼠标所在可见的Layout Control和以前鼠标所在可见的Layout Control，
        /// 遍历找到在现在但不在以前的执行  ScMouseEnter(Sc.ScMouseEventArgs mouseEventArgs) 区别于 ScMouseEnter(System.Windows.Forms.MouseEventArgs e)
        /// </summary>
        void ScMouseEnter(System.Windows.Forms.MouseEventArgs e)
        {
            bool isFind = false;
            PointF ptf;
            PointF scMouseLocation;
            Sc.ScMouseEventArgs mouseEventArgs;
            if (mouseMoveScControlList == null)
                return;

            foreach (Sc.ScLayer newControl in mouseMoveScControlList)
            {
                if (newControl.Visible == false)
                    continue;
                isFind = false;

                if (oldMouseMoveScControlList != null)
                {
                    foreach (Sc.ScLayer oldControl in oldMouseMoveScControlList)
                    {
                        if (oldControl.Visible == false)
                            continue;

                        if (newControl == oldControl)
                        {
                            isFind = true;
                            break;
                        }
                    }
                }

                if (isFind == false)
                {
                    Point pt = new Point((int)(e.Location.X * sizeScale.Width), (int)(e.Location.Y * sizeScale.Height));
                    ptf = newControl.TransGlobalToLocal(pt);
                    scMouseLocation = new PointF(ptf.X, ptf.Y);
                    mouseEventArgs = new Sc.ScMouseEventArgs(e.Button, scMouseLocation);
                    newControl.ScMouseEnter(mouseEventArgs);
                }
            }
        }


        private void RegControlEvent()
        {
            if (control == null)
                return;

            control.MouseDown += Control_MouseDown;
            control.MouseLeave += Control_MouseLeave;
            control.MouseUp += Control_MouseUp;
            control.MouseMove += Control_MouseMove;
            control.MouseWheel += Control_MouseWheel;
            control.MouseDoubleClick += Control_MouseDoubleClick;

            control.GotFocus += Control_GotFocus;
            control.LostFocus += Control_LostFocus;
            control.KeyDown += Control_KeyDown;
            control.KeyUp += Control_KeyUp;
            control.SizeChanged += Control_SizeChanged;


            // 不同Control还需要注册额外的事件
            if (controlType == ControlType.StdControl)
            {
                ((Sc.ScLayerControl)control).CharEvent += Control_CharEvent;
                ((Sc.ScLayerControl)control).ImeStringEvent += Control_ImeStringEvent;
                ((Sc.ScLayerControl)control).DirectionKeyEvent += Control_KeyDown;
            }
            else
            {
                ((Sc.UpdateLayerFrm)control).CharEvent += Control_CharEvent;
                ((Sc.UpdateLayerFrm)control).ImeStringEvent += Control_ImeStringEvent;
                ((Sc.UpdateLayerFrm)control).DirectionKeyEvent += Control_KeyDown;

                rootScLayer.MouseDown += RootScLayer_MouseDown;
                rootScLayer.MouseUp += RootScLayer_MouseUp;
                rootScLayer.MouseMove += RootScLayer_MouseMove;
            }
        }



        public void Dispose()
        {
            graphics?.Dispose();
            if (dot9BitmaShadowDict != null)
            {
                foreach (KeyValuePair<string, Utils.Dot9BitmapD2D> item in dot9BitmaShadowDict)
                {
                    item.Value.Dispose();
                }

                dot9BitmaShadowDict.Clear();
            }

            bitmap?.Dispose();
            bitmap = null;

            wicBitmap?.Dispose();
            wicBitmap = null;

            rebulidLayerList?.Clear();

            rootParent?.Dispose();
        }
    }
}
