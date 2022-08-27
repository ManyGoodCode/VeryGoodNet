using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using Sc;
using SharpDX.DirectWrite;

namespace Demo2
{
    public class TestLayer : ScLayer
    {
        Sc.ScShadow shadow;
        System.Drawing.PointF mousePos;
        public TestLayer(Sc.ScMgr scmgr = null)
            : base(scmgr)
        {
            //设置层的阴影
            shadow = new Sc.ScShadow(scmgr)
            {
                CornersRadius = 4,
                ShadowRadius = 15,
                ShadowColor = Color.Black
            };

            ShadowLayer = shadow;


            SizeChanged += ScPanel_SizeChanged;
            D2DPaint += ScPanel_D2DPaint;
            MouseMove += TestLayer_MouseMove;
        }

        private void TestLayer_MouseMove(object sender, ScMouseEventArgs e)
        {
            mousePos = e.Location;
            Refresh();
        }

        private void ScPanel_SizeChanged(object sender, SizeF oldSize)
        {
            //设置阴影层的尺寸
            shadow.DirectionRect = new RectangleF(
                       x: DirectionRect.X - shadow.ShadowRadius,
                       y: DirectionRect.Y - shadow.ShadowRadius,
                       width: DirectionRect.Width + shadow.ShadowRadius * 2,
                       height: DirectionRect.Height + shadow.ShadowRadius * 2);

        }

        void DrawParamInfo(Sc.D2DGraphics g, TextFormat textFormat)
        {
            // 画矩形
            SharpDX.Direct2D1.SolidColorBrush brush = new SolidColorBrush(
                renderTarget: g.RenderTarget,
                color: new RawColor4(r: 1, g: 0, b: 0, a: 1));
            SharpDX.Mathematics.Interop.RawRectangleF rect = new RawRectangleF(
                left: 0,
                top: 0,
                right: 100,
                bottom: 200);
            g.RenderTarget.FillRectangle(rect: rect, brush: brush);

            // 画文字
            SharpDX.Direct2D1.SolidColorBrush blackBrush = new SolidColorBrush(
                renderTarget: g.RenderTarget,
                color: new RawColor4(r: 0, g: 0, b: 0, a: 1));

            string info = "LayerWidth:" + Width + "\n" + "LayerHeight:" + Height;
            g.RenderTarget.DrawText(
                text: info,
                textFormat: textFormat,
                layoutRect: rect,
                defaultForegroundBrush: blackBrush,
                options: DrawTextOptions.Clip);
        }


        private void ScPanel_D2DPaint(Sc.D2DGraphics g)
        {
            g.RenderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
            SharpDX.DirectWrite.TextFormat textFormat = new TextFormat(D2DGraphics.dwriteFactory, "微软雅黑", 10)
            {
                TextAlignment = TextAlignment.Center,
                ParagraphAlignment = ParagraphAlignment.Center,
                WordWrapping = WordWrapping.Wrap
            };

            DrawParamInfo(g: g, textFormat: textFormat);
            SharpDX.Direct2D1.SolidColorBrush brush1 = new SolidColorBrush(g.RenderTarget, new RawColor4(1, 0, 0, 1));
            SharpDX.Direct2D1.SolidColorBrush brush2 = new SolidColorBrush(g.RenderTarget, new RawColor4(0, 0, 0, 1));

            SharpDX.Mathematics.Interop.RawRectangleF rect = new RawRectangleF(100, 100, 200, 100 + 20);
            g.RenderTarget.FillRectangle(rect, brush1);
            g.RenderTarget.DrawText(mousePos.ToString(), textFormat, rect, brush2, DrawTextOptions.Clip);
        }
    }
}
