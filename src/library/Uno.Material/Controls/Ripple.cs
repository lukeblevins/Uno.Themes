﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Uno.Material.Controls
{
	/*
	 * Starting implementation acknowledgement from project https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/blob/2740f14a814896d42032ae0013b765a8a0ec04c3/MaterialDesignThemes.Uwp/Ripple.cs
	 */
	public partial class Ripple : ContentControl, INotifyPropertyChanged
	{
		private double _rippleSize;
		private double _rippleX;
		private double _rippleY;

		public Ripple()
		{
			DefaultStyleKey = typeof(Ripple);
			SizeChanged += OnSizeChanged;
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
		{
			RippleSize = Math.Max(sizeChangedEventArgs.NewSize.Width, sizeChangedEventArgs.NewSize.Height) * RippleSizeMultiplier;
			Clip = new RectangleGeometry { Rect = new Windows.Foundation.Rect(0, 0, sizeChangedEventArgs.NewSize.Width, sizeChangedEventArgs.NewSize.Height) };
		}

		public static readonly DependencyProperty FeedbackProperty = DependencyProperty.Register(
			"Feedback", typeof(Brush), typeof(Ripple), new PropertyMetadata(default(Brush)));

		public Brush Feedback
		{
			get { return (Brush)GetValue(FeedbackProperty); }
			set { SetValue(FeedbackProperty, value); }
		}

		/// <summary>
		/// Opacity of the feedback ripple effect
		/// </summary>
		/// <remarks>Using the Opacity property would affect it's content, while this property only affects the feedback</remarks>
		public double FeedbackOpacity
		{
			get { return (double)GetValue(FeedbackOpacityProperty); }
			set { SetValue(FeedbackOpacityProperty, value); }
		}

		public static readonly DependencyProperty FeedbackOpacityProperty =
			DependencyProperty.Register("FeedbackOpacity", typeof(double), typeof(Ripple), new PropertyMetadata(1));

		protected override void OnApplyTemplate()
		{
			VisualStateManager.GoToState(this, "Normal", false);

			base.OnApplyTemplate();
		}

		protected override void OnPointerPressed(PointerRoutedEventArgs e)
		{
			if (IsAutoRippleEnabled)
			{
				StartRippling(e);
			}

			base.OnPointerPressed(e);
		}

		/// <summary>
		/// On WASM when the Ripple control is nested into a control that does capture the pointer,
		/// we won't receive the pointer events as we should.
		/// ** Noticeably, it's the case for all buttons. **
		/// To work around this issue, you can turn this flag on and then,
		/// invoke the "StartRippling" in the OnPointerPressed of the parent control (i.e. the Button).
		/// </summary>
		internal bool IsAutoRippleEnabled { get; set; } = true;

		internal void StartRippling(PointerRoutedEventArgs e)
		{
			var point = e.GetCurrentPoint(this);

			RippleX = point.Position.X - RippleSize / 2;
			RippleY = point.Position.Y - RippleSize / 2;

			VisualStateManager.GoToState(this, "Normal", true);
			VisualStateManager.GoToState(this, "Pressed", true);
		}

		// Uncomment for hover effect
		//protected override void OnPointerMoved(PointerRoutedEventArgs e)
		//{
		//	var point = e.GetCurrentPoint(this);

		//	RippleX = point.Position.X - RippleSize / 2;
		//	RippleY = point.Position.Y - RippleSize / 2;

		//	VisualStateManager.GoToState(this, "Normal", true);
		//	VisualStateManager.GoToState(this, "PointerOver", true);

		//	base.OnPointerMoved(e);
		//}

		public static readonly DependencyProperty RippleSizeMultiplierProperty = DependencyProperty.Register(
			"RippleSizeMultiplier", typeof(double), typeof(Ripple), new PropertyMetadata(8.0));

		public double RippleSizeMultiplier
		{
			get { return (double)GetValue(RippleSizeMultiplierProperty); }
			set { SetValue(RippleSizeMultiplierProperty, value); }
		}

		public double RippleSize
		{
			get { return _rippleSize; }
			private set
			{
				if (_rippleSize == value) return;
				_rippleSize = value;
				OnPropertyChanged();
			}
		}

		public double RippleY
		{
			get { return _rippleY; }
			private set
			{
				if (_rippleY == value) return;
				_rippleY = value;
				OnPropertyChanged();
			}
		}

		public double RippleX
		{
			get { return _rippleX; }
			private set
			{
				if (_rippleX == value) return;
				_rippleX = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
