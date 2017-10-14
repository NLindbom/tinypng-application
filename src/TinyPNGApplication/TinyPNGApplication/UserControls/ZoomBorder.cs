using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TinyPNGApplication.UserControls
{
    /// <summary>
    /// Custom control with child pan/zoom support credit to Wiesław Šoltés: https://stackoverflow.com/questions/741956/pan-zoom-image
    /// </summary>
    public class ZoomBorder : Border
    {
        private UIElement child = null;
        private Point origin;
        private Point start;

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != Child)
                    Initialize(value);

                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            child = element;

            if (child != null)
            {
                var transformGroup = new TransformGroup()
                {
                    Children =
                    {
                        new ScaleTransform(),
                        new TranslateTransform()
                    }
                };

                child.RenderTransform = transformGroup;

                child.RenderTransformOrigin = new Point(0.0, 0.0);

                this.MouseWheel += child_MouseWheel;
                this.MouseLeftButtonDown += child_MouseLeftButtonDown;
                this.MouseLeftButtonUp += child_MouseLeftButtonUp;
                this.MouseMove += child_MouseMove;
                this.PreviewMouseRightButtonDown += child_PreviewMouseRightButtonDown;
            }
        }

        public void Reset()
        {
            if (child != null)
            {
                // reset zoom
                var scaleTransform = GetScaleTransform(child);
                scaleTransform.ScaleX = 1.0;
                scaleTransform.ScaleY = 1.0;

                // reset pan
                var translateTransform = GetTranslateTransform(child);
                translateTransform.X = 0.0;
                translateTransform.Y = 0.0;
            }
        }

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (element.RenderTransform as TransformGroup).Children.First(tr => tr is TranslateTransform) as TranslateTransform;
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (element.RenderTransform as TransformGroup).Children.First(tr => tr is ScaleTransform) as ScaleTransform;
        }

        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (child != null)
            {
                var scaleTransform = GetScaleTransform(child);
                var translateTransform = GetTranslateTransform(child);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (e.Delta <= 0 && (scaleTransform.ScaleX < .4 || scaleTransform.ScaleY < .4))
                    return;

                var relative = e.GetPosition(child);

                double abosuluteX = relative.X * scaleTransform.ScaleX + translateTransform.X;
                double abosuluteY = relative.Y * scaleTransform.ScaleY + translateTransform.Y;

                scaleTransform.ScaleX += zoom;
                scaleTransform.ScaleY += zoom;

                translateTransform.X = abosuluteX - relative.X * scaleTransform.ScaleX;
                translateTransform.Y = abosuluteY - relative.Y * scaleTransform.ScaleY;
            }
        }

        private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                var translateTransform = GetTranslateTransform(child);

                start = e.GetPosition(this);
                origin = new Point(translateTransform.X, translateTransform.Y);

                this.Cursor = Cursors.Hand;

                child.CaptureMouse();
            }
        }

        private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                child.ReleaseMouseCapture();

                this.Cursor = Cursors.Arrow;
            }
        }

        void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Reset();
        }

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (child != null)
            {
                if (child.IsMouseCaptured)
                {
                    var translateTransform = GetTranslateTransform(child);

                    Vector v = start - e.GetPosition(this);

                    translateTransform.X = origin.X - v.X;
                    translateTransform.Y = origin.Y - v.Y;
                }
            }
        }
    }
}
