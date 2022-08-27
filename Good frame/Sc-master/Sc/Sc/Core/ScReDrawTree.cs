using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System.Collections.Generic;
using System.Drawing;

namespace Sc
{
    public partial class ScReDrawTree
    {
        Sc.ScMgr scmgr;
        public Sc.ScDrawNode root;

        static SharpDX.Mathematics.Interop.RawMatrix3x2 identityMatrix = new RawMatrix3x2(
            m11: 1.0f, m12: 0.0f,
            m21: 0.0f, m22: 1.0f,
            m31: 0.0f, m32: 0.0f);

        public void Draw(Sc.ScGraphics g)
        {
            if (root == null)
                return;

            g.SetClip(root.clipRect);
            switch (g.GetGraphicsType())
            {
                case Sc.GraphicsType.D2D:

                    if (root.isRender)
                        D2DPaint(g, root);
                    DrawChildNodeD2D(root, g);
                    break;
            }

            g.ResetClip();
        }


        void DrawChildNodeD2D(Sc.ScDrawNode parent, Sc.ScGraphics g)
        {
            SharpDX.Direct2D1.Layer d2dLayer;
            foreach (Sc.ScDrawNode node in parent.nodes)
            {
                if (node.layer.IsComputedStraight)
                {
                    if (node.isRender && node.layer.IsRender)
                        D2DPaint(g, node);
                    DrawChildNodeD2D(node, g);
                }
                else
                {
                    d2dLayer = PushLayer((Sc.D2DGraphics)g, node.layer);
                    D2DPaint(g, node);
                    DrawChildNodeD2D(node, g);
                    PopLayer((Sc.D2DGraphics)g);
                    d2dLayer.Dispose();
                }
            }
        }


        void D2DPaint(Sc.ScGraphics g, Sc.ScDrawNode node)
        {
            Sc.ScLayer layer = node.layer;
            g.SetClip(node.clipRect);
            g.Transform = layer.GlobalMatrix;
            g.layer = layer;
            layer.OnD2DPaint(g);
            g.layer = null;
            g.ResetTransform();
            g.ResetClip();
        }


        public SharpDX.Direct2D1.Layer PushLayer(Sc.D2DGraphics g, Sc.ScLayer sclayer)
        {
            SharpDX.Direct2D1.Layer d2dLayer = new Layer(g.RenderTarget);
            SharpDX.Direct2D1.LayerParameters layerParameters = new LayerParameters();
            layerParameters.ContentBounds = GDIDataD2DUtils.TransToRawRectF(sclayer.DrawBox);
            layerParameters.LayerOptions = LayerOptions.InitializeForCleartype;
            layerParameters.MaskAntialiasMode = AntialiasMode.PerPrimitive;

            //应用到GeometricMask上的变换，这个变换可能已经在计算布局的时候已经计算到了sclayer.TransLastHitPathGeometry上
            //所以不需要应用变换
            layerParameters.MaskTransform = identityMatrix;
            layerParameters.Opacity = sclayer.Opacity;
            layerParameters.GeometricMask = sclayer.TransLastHitPathGeometry;

            g.RenderTarget.PushLayer(ref layerParameters, d2dLayer);
            return d2dLayer;
        }

        public void PopLayer(Sc.D2DGraphics g)
        {
            g.RenderTarget.PopLayer();
        }


        public Sc.ScDrawNode ReCreateReDrawTree(Sc.ScLayer rootLayer, Rectangle refreshArea)
        {
            if (rootLayer.Visible == false)
                return null;

            scmgr = rootLayer.ScMgr;
            RectangleF clipRect = new RectangleF(refreshArea.X, refreshArea.Y, refreshArea.Width, refreshArea.Height);
            root = _AddChildReDrawScLayer(null, clipRect, new List<Sc.ScLayer> { rootLayer });
            return root;

        }


        void AddChildReDrawScLayer(Sc.ScDrawNode parentDrawNode, RectangleF parentClipRect)
        {
            Sc.ScLayer parentScLayer = parentDrawNode.layer;
            _AddChildReDrawScLayer(parentDrawNode, parentClipRect, parentScLayer.controls);
            _AddChildReDrawScLayer(parentDrawNode, parentClipRect, parentScLayer.DirectClipChildLayerList);
        }

        Sc.ScDrawNode _AddChildReDrawScLayer(Sc.ScDrawNode parentDrawNode, RectangleF parentClipRect, List<Sc.ScLayer> childLayerList)
        {
            if (childLayerList == null)
                return null;

            RectangleF rect;
            Sc.ScDrawNode drawNode = null;

            foreach (Sc.ScLayer childLayer in childLayerList)
            {
                if (childLayer.Visible == false ||
                    childLayer.IsNotAtRootDrawBoxBound)
                    continue;

                rect = childLayer.DrawBox;
                Sc.ScDrawNode clipDrawNode;
                RectangleF clipRect;
                clipDrawNode = parentDrawNode;
                clipRect = parentClipRect;

                if (clipRect.IntersectsWith(rect))
                {
                    rect.Intersect(clipRect);

                    drawNode = new ScDrawNode();
                    drawNode.layer = childLayer;
                    drawNode.clipRect = rect;
                    drawNode.parent = clipDrawNode;

                    if (clipDrawNode != null)
                        clipDrawNode.nodes.Add(drawNode);

                    //子层完全覆盖了父层，父层将不再绘制
                    if (childLayer.BackgroundColor != null &&
                        childLayer.BackgroundColor.Value.A == 255 &&
                        childLayer.IsComputedStraight && clipDrawNode.clipRect.Equals(rect))
                    {
                        clipDrawNode.isRender = false;

                        for (ScDrawNode parentNode = clipDrawNode.parent; parentNode != null; parentNode = parentNode.parent)
                        {
                            if (!parentNode.clipRect.Equals(rect) || parentNode.layer.IsComputedStraight)
                                break;

                            parentNode.isRender = false;
                        }
                    }

                    AddChildReDrawScLayer(drawNode, drawNode.clipRect);
                }
            }

            return drawNode;
        }
    }
}
