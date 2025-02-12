﻿using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScButton : ScLayer
    {
        Color color = Color.FromArgb(red: 234, green: 232, blue: 233);
        Color fontColor = Color.FromArgb(234, 232, 233);
        Color normalColor = Color.FromArgb(234, 232, 233);
        Color enterColor = Color.FromArgb(248, 248, 245);
        Color downColor = Color.FromArgb(153, 114, 49);
        Color disableColor = Color.FromArgb(153, 114, 49);
        Color normalFontColor = Color.FromArgb(255, 191, 152, 90);
        Color enterFontColor = Color.FromArgb(248, 248, 245);
        Color downFontColor = Color.FromArgb(255, 233, 233, 233);
        Color disableFontColor = Color.FromArgb(255, 233, 233, 233);

        public Color NormalColor
        {
            get { return normalColor; }
            set
            {
                normalColor = value;
                color = normalColor;
            }
        }


        public Color EnterColor
        {
            get { return enterColor; }
            set { enterColor = value; }
        }

        public Color DownColor
        {
            get { return downColor; }
            set { downColor = value; }
        }


        public Color DisableColor
        {
            get { return disableColor; }
            set { disableColor = value; }
        }

        public Color NormalFontColor
        {
            get { return normalFontColor; }
            set
            {
                normalFontColor = value;
                fontColor = normalFontColor;
            }
        }

        public Color EnterFontColor
        {
            get { return enterFontColor; }
            set { enterFontColor = value; }
        }

        public Color DownFontColor
        {
            get { return downFontColor; }
            set { downFontColor = value; }
        }

        public Color DisableFontColor
        {
            get { return disableFontColor; }
            set { disableFontColor = value; }
        }

        public bool IsUseShadow
        {
            get { return ShadowLayer != null; }
            set { ShadowLayer = value ? shadow : null; }
        }


        public Color CurtStateColor
        {
            get { return color; }
        }

        public bool UseDefaultPaint = true;


        ScAnimation scAnim;
        ScAnimation scFontColorAnim;

        ScLinearAnimation linearR;
        ScLinearAnimation linearG;
        ScLinearAnimation linearB;

        ScLinearAnimation linearFontR;
        ScLinearAnimation linearFontG;
        ScLinearAnimation linearFontB;

        public string Text = "";
        public float RadiusX = 4;
        public float RadiusY = 4;
        public int animMS = 10;
        public int SideShadowAlpha = 20;


        ScShadow shadow;
        public int AnimMS
        {
            set
            {
                animMS = value;
                scAnim.animMS = value;
                scFontColorAnim.animMS = value;
            }
        }


        D2DFont foreFont = new D2DFont("微软雅黑", 12);

        public D2DFont ForeFont
        {
            get { return foreFont; }
            set { foreFont = value; }
        }

        public delegate void AnimalStopEventHandler(ScButton button);
        public event AnimalStopEventHandler AnimalStopEvent = null;

        public delegate void PaintEventHandler(D2DGraphics g, RawRectangleF rect);
        public event PaintEventHandler PaintEvent = null;

        public ScButton(ScMgr scmgr = null)
            : base(scmgr)
        {
            shadow = new ScShadow(scmgr);
            shadow.CornersRadius = 6;
            shadow.ShadowRadius = 3;
            shadow.ShadowColor = Color.FromArgb(15, 0, 0, 0);
            ShadowLayer = shadow;

            this.MouseDown += ScButton_MouseDown;
            this.MouseUp += ScButton_MouseUp;
            this.MouseEnter += ScButton_MouseEnter;
            this.MouseLeave += ScButton_MouseLeave;

            this.D2DPaint += ScButton_D2DPaint;

            scAnim = new ScAnimation(layer: this, animMS: animMS, autoRest: true);
            scAnim.AnimationEvent += ScAnim_AnimationEvent;

            scFontColorAnim = new ScAnimation(this, animMS, true);
            scFontColorAnim.AnimationEvent += ScFontColorAnim_AnimationEvent;

            IsUseOrgHitGeometry = false;

            SizeChanged += ScButton_SizeChanged;
            LocationChanged += ScButton_LocationChanged;
        }

        private void ScButton_D2DPaint(D2DGraphics g)
        {
            if (UseDefaultPaint)
            {
                FillItemGeometry(g);
                DrawString(g);
            }
            if (PaintEvent != null)
            {
                RawRectangleF rect = new RawRectangleF(0, 0, Width, Height);
                PaintEvent(g, rect);
            }
        }

        void FillItemGeometry(D2DGraphics g)
        {
            if (Enable == false)
                color = Color.DarkGray;

            g.RenderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
            RawRectangleF rect = new RawRectangleF(1, 1, Width - 1, Height - 1);
            RoundedRectangle roundedRect = new RoundedRectangle()
            {
                RadiusX = this.RadiusX,
                RadiusY = this.RadiusY,
                Rect = rect
            };

            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(color);
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.FillRoundedRectangle(roundedRect, brush);

            rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(SideShadowAlpha, 0, 0, 0));
            brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.DrawRoundedRectangle(roundedRect, brush);
        }

        void DrawString(D2DGraphics g)
        {
            if (string.IsNullOrWhiteSpace(Text))
                return;
            g.RenderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
            RawRectangleF rect = new RawRectangleF(0, 0, Width - 1, Height - 1);
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, GDIDataD2DUtils.TransToRawColor4(fontColor));
            TextFormat textFormat = new TextFormat(D2DGraphics.dwriteFactory, foreFont.FamilyName, foreFont.Weight, foreFont.Style, foreFont.Size)
            {
                TextAlignment = TextAlignment.Center,
                ParagraphAlignment = ParagraphAlignment.Center
            };

            textFormat.WordWrapping = WordWrapping.Wrap;
            g.RenderTarget.DrawText(Text, textFormat, rect, brush, DrawTextOptions.Clip);
        }


        private void ScButton_SizeChanged(object sender, SizeF oldSize)
        {
            if (ShadowLayer != null)
            {
                ScShadow shadow = (ScShadow)ShadowLayer;
                shadow.DirectionRect = new RectangleF(DirectionRect.X - shadow.ShadowRadius, DirectionRect.Y + shadow.ShadowRadius + 2, DirectionRect.Width + shadow.ShadowRadius * 2, DirectionRect.Height);
            }
        }

        private void ScButton_LocationChanged(object sender, PointF oldLocation)
        {
            if (ShadowLayer != null)
            {
                ScShadow shadow = (ScShadow)ShadowLayer;
                shadow.Location = new PointF(DirectionRect.X - shadow.ShadowRadius, DirectionRect.Y + shadow.ShadowRadius + 2);
            }
        }

        protected override Geometry CreateHitGeometryByD2D(D2DGraphics g)
        {
            RawRectangleF rect = new RawRectangleF(0, 0, Width - 1, Height - 1);
            RoundedRectangle roundedRect = new RoundedRectangle()
            {
                RadiusX = this.RadiusX,
                RadiusY = this.RadiusY,
                Rect = rect
            };

            RoundedRectangleGeometry roundedRectGeo = new RoundedRectangleGeometry(D2DGraphics.d2dFactory, roundedRect);
            return roundedRectGeo;
        }

        private void ScButton_MouseDown(object sender, ScMouseEventArgs e)
        {
            StartAnim(downColor);
            StartFontColorAnim(downFontColor);
        }

        private void ScButton_MouseUp(object sender, ScMouseEventArgs e)
        {
            StartAnim(enterColor);
            StartFontColorAnim(enterFontColor);
        }

        private void ScButton_MouseEnter(object sender, ScMouseEventArgs e)
        {

            StartAnim(enterColor);
            StartFontColorAnim(enterFontColor);
        }


        private void ScButton_MouseLeave(object sender)
        {
            StartAnim(normalColor);
            StartFontColorAnim(normalFontColor);
        }


        public void StartAnim(Color stopColor)
        {
            scAnim.Stop();

            linearR = new ScLinearAnimation(color.R, stopColor.R, scAnim);
            linearG = new ScLinearAnimation(color.G, stopColor.G, scAnim);
            linearB = new ScLinearAnimation(color.B, stopColor.B, scAnim);
            scAnim.Start();
        }

        private void ScAnim_AnimationEvent(ScAnimation scAnimation)
        {
            int r = (int)linearR.GetCurtValue();
            int g = (int)linearG.GetCurtValue();
            int b = (int)linearB.GetCurtValue();

            color = Color.FromArgb(r, g, b);

            Refresh();
            Update();

            if (linearR.IsStop && linearG.IsStop && linearB.IsStop)
            {
                scAnimation.Stop();
                // 当前线程执行
                AnimalStopEvent?.Invoke(this);
            }
        }

        public void StartFontColorAnim(Color stopFontColor)
        {
            scFontColorAnim.Stop();
            linearFontR = new ScLinearAnimation(fontColor.R, stopFontColor.R, scFontColorAnim);
            linearFontG = new ScLinearAnimation(fontColor.G, stopFontColor.G, scFontColorAnim);
            linearFontB = new ScLinearAnimation(fontColor.B, stopFontColor.B, scFontColorAnim);
            scFontColorAnim.Start();
        }

        private void ScFontColorAnim_AnimationEvent(ScAnimation scAnimation)
        {
            int r = (int)linearFontR.GetCurtValue();
            int g = (int)linearFontG.GetCurtValue();
            int b = (int)linearFontB.GetCurtValue();

            fontColor = Color.FromArgb(r, g, b);
            Refresh();
            Update();

            if (linearFontR.IsStop && linearFontG.IsStop && linearFontB.IsStop)
            {
                scAnimation.Stop();
                AnimalStopEvent?.Invoke(this);
            }
        }
    }
}
