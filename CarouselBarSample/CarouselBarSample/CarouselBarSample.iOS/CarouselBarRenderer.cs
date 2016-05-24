using CarouselBarSample;
using CarouselBarSample.iOS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CarouselBar), typeof(CarouselBarRenderer))]
namespace CarouselBarSample.iOS
{
    public class CarouselBarRenderer : ScrollViewRenderer
    {
        UIScrollView _native;
        private nfloat _previousHorizontalOffset;

        public CarouselBarRenderer()
        {
            ShowsHorizontalScrollIndicator = false;
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null) return;

            _native = (UIScrollView)NativeView;
            _native.DraggingStarted += NativeDraggingStarted;
            _native.DraggingEnded += NativeDraggingEnded;
            _native.DecelerationStarted += NativeDecelerationStarted;

            e.NewElement.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var carousel = (CarouselBar)Element;
            if (e.PropertyName == nameof(carousel.SelectedIndex))
            {
                ScrollNative();
            }
        }

        private void NativeDraggingStarted(object sender, EventArgs e)
        {
            _previousHorizontalOffset = _native.ContentOffset.X;
        }

        private void NativeDraggingEnded(object sender, DraggingEventArgs e)
        {
            var carousel = (CarouselBar)Element;
            var childs = carousel.Children;
            var selectedIndex = carousel.SelectedIndex;

            if (_native.ContentOffset.X > _previousHorizontalOffset)
            {
                selectedIndex = Math.Min(selectedIndex + 1, carousel.Buttons.Count - 1);
            }
            else
            {
                selectedIndex = Math.Max(selectedIndex - 1, 0);
            }

            carousel.SelectedIndex = selectedIndex;
            ScrollNative();
        }

        private void NativeDecelerationStarted(object sender, EventArgs e)
        {
            ScrollNative();
        }

        private void ScrollNative()
        {
            var carousel = (CarouselBar)Element;

            var offset = 0d;
            for (int i = 0; i < carousel.SelectedIndex; i++)
            {
                offset += carousel.Children[i].Width + carousel.Spacing;
            }

            _native.SetContentOffset(new CoreGraphics.CGPoint(offset, _native.ContentOffset.Y), true);
        }
    }
}
