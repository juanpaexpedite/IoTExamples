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

namespace SPIThermometer.Views.Controls
{
    public sealed partial class Potentiometer : UserControl
    {
        #region Resistance
        public double Resistance
        {
            get { return (double)GetValue(ResistanceProperty); }
            set { SetValue(ResistanceProperty, value); }
        }

        public static readonly DependencyProperty ResistanceProperty =
            DependencyProperty.Register(nameof(Resistance), typeof(double), typeof(Potentiometer), new PropertyMetadata(0.0, Update));
        #endregion

        #region Minimum
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(Potentiometer), new PropertyMetadata(0.0, Update));
        #endregion

        #region Maximum
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(Potentiometer), new PropertyMetadata(10.0, Update));
        #endregion

        #region Update

        private static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var control = d as Potentiometer;

            control.Header.Text = $"{control.Resistance:0.###} KΩ";

            if (control.Maximum - control.Minimum > 0)
            {
                var Shape = control.Shape;
                var Clip = control.Clip;

                var a = Shape.Width / (control.Maximum - control.Minimum);
                var b = -a * control.Minimum;
                var width = a * control.Resistance + b;

                Clip.Rect = new Rect(0, 0, width, Clip.Rect.Height);
            }

        }
        #endregion

        public Potentiometer()
        {
            this.InitializeComponent();
        }
    }
}
