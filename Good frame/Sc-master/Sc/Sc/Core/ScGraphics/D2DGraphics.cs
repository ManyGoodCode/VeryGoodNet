using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class D2DGraphics : ScGraphics
    {
        static public SharpDX.Direct2D1.Factory d2dFactory = null;
        static public SharpDX.DirectWrite.Factory dwriteFactory = null;

        public SharpDX.Direct2D1.RenderTarget renderTarget;
        public SharpDX.Direct2D1.GdiInteropRenderTarget gdiRenderTarget;

        SharpDX.Mathematics.Interop.RawMatrix3x2 matrix = new RawMatrix3x2(
             1.0f, 0.0f,
             0.0f, 1.0f,
             0.0f, 0.0f
             );

        System.Windows.Forms.Control control = null;
        SharpDX.WIC.Bitmap wicBitmap;
        Sc.RenderTargetMode renderTargetMode;

        public D2DGraphics(Control control)
        {
            renderTargetMode = Sc.RenderTargetMode.Hwnd;
            CreateDeviceIndependentResource();
            if (control.Width <= 0 || control.Height <= 0)
                return;

            this.control = control;
            CreateDeviceDependentResource();
        }

        public D2DGraphics(SharpDX.WIC.Bitmap wicBitmap)
        {
            renderTargetMode = Sc.RenderTargetMode.Wic;
            CreateDeviceIndependentResource();

            this.wicBitmap = wicBitmap;
            CreateDeviceDependentResource();
        }

        /// <summary>
        /// 设备独立资源
        /// </summary>
        /// <returns></returns>
        bool CreateDeviceIndependentResource()
        {
            if (d2dFactory == null)
                d2dFactory = new SharpDX.Direct2D1.Factory(SharpDX.Direct2D1.FactoryType.MultiThreaded);

            if (dwriteFactory == null)
                dwriteFactory = new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Shared);

            return true;
        }

        /// <summary>
        /// 设备依赖资源
        /// </summary>
        /// <returns></returns>
        bool CreateDeviceDependentResource()
        {
            switch (renderTargetMode)
            {
                case Sc.RenderTargetMode.Hwnd:
                    CreateRenderTarget(control);
                    break;

                case Sc.RenderTargetMode.Wic:
                    CreateRenderTarget(wicBitmap);
                    break;
            }

            return true;
        }

        public void CreateRenderTarget(System.Windows.Forms.Control control)
        {
            if (renderTarget != null)
                return;

            SharpDX.Direct2D1.HwndRenderTargetProperties properties = new HwndRenderTargetProperties
            {
                Hwnd = control.Handle,
                PixelSize = new SharpDX.Size2(control.Width, control.Height),
                PresentOptions = PresentOptions.Immediately | PresentOptions.RetainContents
            };


            SharpDX.Direct2D1.RenderTargetProperties rtProps = new RenderTargetProperties();
            rtProps.Usage = SharpDX.Direct2D1.RenderTargetUsage.GdiCompatible;

            renderTarget = new SharpDX.Direct2D1.WindowRenderTarget(d2dFactory, rtProps, properties)
            {
                AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive,
                TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Cleartype
            };


            IntPtr gdirtPtr;
            renderTarget.QueryInterface(Guid.Parse("e0db51c3-6f77-4bae-b3d5-e47509b35838"), out gdirtPtr);
            gdiRenderTarget = new SharpDX.Direct2D1.GdiInteropRenderTarget(gdirtPtr);

        }

        public void CreateRenderTarget(SharpDX.WIC.Bitmap wicBitmap)
        {
            if (renderTarget != null)
                return;
            float dpiX = 96, dpiY = 96;
            using (System.Drawing.Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
            }

            SharpDX.Direct2D1.RenderTargetProperties renderTargetProperties = new SharpDX.Direct2D1.RenderTargetProperties(
                SharpDX.Direct2D1.RenderTargetType.Default,
                new SharpDX.Direct2D1.PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Unknown),
                dpiX, dpiY, 
                SharpDX.Direct2D1.RenderTargetUsage.GdiCompatible, FeatureLevel.Level_DEFAULT);

            renderTarget = new WicRenderTarget(d2dFactory, wicBitmap, renderTargetProperties);

            IntPtr gdirtPtr;
            renderTarget.QueryInterface(Guid.Parse("e0db51c3-6f77-4bae-b3d5-e47509b35838"), out gdirtPtr);
            gdiRenderTarget = new GdiInteropRenderTarget(gdirtPtr);
        }


        public override void ReSize(int width, int height)
        {
            if (control != null)
            {
                SharpDX.Direct2D1.WindowRenderTarget wrt = (WindowRenderTarget)renderTarget;
                wrt.Resize(new SharpDX.Size2(width, height));
            }
        }

        public SharpDX.Direct2D1.RenderTarget RenderTarget
        {
            get { return renderTarget; }
        }

        public Graphics CreateGdiGraphics()
        {
            IntPtr hdc = gdiRenderTarget.GetDC(SharpDX.Direct2D1.DeviceContextInitializeMode.Copy);
            System.Drawing.Graphics gdiGraphics = Graphics.FromHdc(hdc);
            gdiGraphics.Transform = layer.GlobalMatrix;
            return gdiGraphics;
        }

        public void RelaseGdiGraphics(System.Drawing.Graphics gdiGraphics)
        {
            gdiRenderTarget.ReleaseDC();
            gdiGraphics.Dispose();
        }

        public override Sc.GraphicsType GetGraphicsType()
        {
            return GraphicsType.D2D;
        }
        public override void BeginDraw()
        {
            renderTarget.BeginDraw();
        }

        public override void EndDraw()
        {
            renderTarget.EndDraw();
        }

        public override void ResetClip()
        {
            renderTarget.PopAxisAlignedClip();
        }

        public override void ResetTransform()
        {
            renderTarget.Transform = matrix;
        }

        public override void SetClip(System.Drawing.RectangleF clipRect)
        {
            SharpDX.Mathematics.Interop.RawRectangleF rawRectF = TransRectFToRawRectF(clipRect);
            renderTarget.PushAxisAlignedClip(rawRectF, AntialiasMode.PerPrimitive);
        }

        public override void TranslateTransform(float dx, float dy)
        {
            System.Drawing.Drawing2D.Matrix m = Sc.GDIDataD2DUtils.TransRawMatrix3x2ToMatrix(renderTarget.Transform);
            m.Translate(dx, dy);
            renderTarget.Transform = Sc.GDIDataD2DUtils.TransMatrixToRawMatrix3x2(m);
        }

        public override System.Drawing.Drawing2D.Matrix Transform
        {
            get { return Sc.GDIDataD2DUtils.TransRawMatrix3x2ToMatrix(renderTarget.Transform); }

            set { renderTarget.Transform = Sc.GDIDataD2DUtils.TransMatrixToRawMatrix3x2(value); }
        }


        SharpDX.Mathematics.Interop.RawRectangleF TransRectFToRawRectF(System.Drawing.RectangleF clipRect)
        {
            RawRectangleF rawRectF =
                new RawRectangleF(
                clipRect.Left, clipRect.Top,
                clipRect.Right, clipRect.Bottom);

            return rawRectF;
        }

        public override void Dispose()
        {
            if (gdiRenderTarget != null)
            {
                gdiRenderTarget.Dispose();
                gdiRenderTarget = null;
            }

            if (renderTarget != null)
            {
                renderTarget.Dispose();
                renderTarget = null;
            }
        }
    }
}
