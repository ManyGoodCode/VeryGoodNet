using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace Sc
{
    public class ScTextBox : ScLayer
    {
        ScTextViewBox textBox;
        public delegate void TextBoxKeyDownEventHandler(object sender, KeyEventArgs e);
        public event TextBoxKeyDownEventHandler TextBoxKeyDownEvent;

        public delegate void ValueChangedEventHandler(object sender, object value);
        public event ValueChangedEventHandler ValueChangedEvent = null;

        public delegate void TextViewLostFocusEventHandler(object sender, EventArgs e);
        public event TextViewLostFocusEventHandler TextViewLostFocusEvent = null;

        Margin margin = new Margin(5, 5, 5, 5);

        public string BackGroundText
        {
            get { return textBox.BackGroundText; }
            set { textBox.BackGroundText = value; }
        }


        public Color BackGroundTextColor
        {
            get { return textBox.BackGroundTextColor; }
            set { textBox.BackGroundTextColor = value; }
        }


        public ScTextBox(ScMgr scmgr = null)
             : base(scmgr)
        {
            Type = "ScTextBoxEx";

            textBox = new ScTextViewBox(scmgr);
            textBox.ForeColor = Color.Black;
            ForeFont = new D2DFont("微软雅黑", 12, SharpDX.DirectWrite.FontWeight.Regular);
            textBox.TextViewLostFocusEvent += TextBox_TextViewLostFocusEvent;
            Add(childLayer: textBox);

            SizeChanged += ScTextBox_SizeChanged;
            D2DPaint += ScTextBoxEx_D2DPaint;

            textBox.TextViewKeyDownEvent += TextBox_TextViewKeyDownEvent;
            textBox.ValueChangedEvent += TextBox_ValueChangedEvent;
        }

        private void TextBox_TextViewLostFocusEvent(object sender, EventArgs e)
        {
            TextViewLostFocusEvent?.Invoke(this, e);
        }

        private void TextBox_ValueChangedEvent(object sender)
        {
            ValueChangedEvent?.Invoke(this, Text);
        }

        private void TextBox_TextViewKeyDownEvent(object sender, KeyEventArgs e)
        {
            TextBoxKeyDownEvent?.Invoke(this, e);
        }

        public D2DFont ForeFont
        {
            get { return textBox.ForeFont; }
            set
            {
                textBox.ForeFont = value;
                Height = margin.top + textBox.Height + margin.bottom;
            }
        }


        public Color ForeColor
        {
            get { return textBox.ForeColor; }
            set { textBox.ForeColor = value; }
        }
        public string Text
        {
            get { return textBox.Text; }
            set
            {
                textBox.Text = value;
                Refresh();
            }
        }

        public bool IsOnlyRead
        {
            get { return textBox.IsOnlyRead; }
            set { textBox.IsOnlyRead = value; }
        }


        private void ScTextBoxEx_D2DPaint(D2DGraphics g)
        {
            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;
            RawRectangleF rect = new RawRectangleF(2, 2, Width - 1, Height - 1);
            RoundedRectangle roundedRect = new RoundedRectangle()
            {
                RadiusX = 4,
                RadiusY = 4,
                Rect = rect
            };

            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 200, 200, 200));
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.DrawRectangle(rect, brush, 1f);
        }


        private void ScTextBox_SizeChanged(object sender, SizeF oldSize)
        {
            RectangleF rect = new RectangleF(0, 0, Width, Height);
            textBox.Width = rect.Width - margin.left - margin.right;
            textBox.BoxWidth = textBox.Width;
            float x = rect.Width / 2 - textBox.Width / 2;
            float y = rect.Height / 2 - textBox.Height / 2;
            textBox.Location = new PointF((float)Math.Ceiling(x), (float)Math.Ceiling(y));
        }
    }
}
