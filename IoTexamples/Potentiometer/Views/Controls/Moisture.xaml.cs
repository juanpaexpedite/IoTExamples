using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Potentiometer.Views.Controls
{
    public sealed partial class Moisture : UserControl
    {
        #region Level
        public double Level
        {
            get { return (double)GetValue(ResistanceProperty); }
            set { SetValue(ResistanceProperty, value); }
        }

        public static readonly DependencyProperty ResistanceProperty =
            DependencyProperty.Register(nameof(Level), typeof(double), typeof(Moisture), new PropertyMetadata(0.0, Update));
        #endregion

        #region Minimum
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(Moisture), new PropertyMetadata(0.0, Update));
        #endregion

        #region Maximum
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(Moisture), new PropertyMetadata(10.0, Update));
        #endregion

        #region Update

        private static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var control = d as Moisture;

            control.Header.Text = $"{control.Level:0.###} %";

            if (control.Maximum - control.Minimum > 0)
            {
                var Shape = control.Shape;
                var Clip = control.Clip;

                var a = Shape.Height / (control.Maximum - control.Minimum);
                var b = -a * control.Minimum;
                var height = a * control.Level + b;

                Clip.Rect = new Rect(0, Shape.Height - height, Clip.Rect.Width, height);
            }

        }
        #endregion

        public Moisture()
        {
            this.InitializeComponent();
        }
    }
}
