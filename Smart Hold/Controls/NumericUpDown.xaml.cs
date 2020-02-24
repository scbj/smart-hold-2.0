using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Smart_Hold.Controls
{
    /// <summary>
    /// Logique d'interaction pour NumericUpDown.xaml
    /// </summary>
    [Description("Affiche une valeur numérique que l'utilisateur peut incrémenter et décrémenter en cliquand sur les boutons déroulants du contrôle.")]
    public partial class NumericUpDown : UserControl
    {
        public decimal Increment
        {
            get { return (decimal)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }
        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(decimal), typeof(NumericUpDown));

        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumProperty); }
            set
            {
                if (Value > value)
                    Value = value;
                SetValue(MaximumProperty, value);
            }
        }
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(decimal), typeof(NumericUpDown));

        public decimal Minimum
        {
            get { return (decimal)GetValue(MinimumProperty); }
            set
            {
                if (Value < value)
                    Value = value;
                SetValue(MinimumProperty, value);
            }
        }
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(decimal), typeof(NumericUpDown));

        [Bindable(true)]
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set
            {
                if (value < Minimum || value > Maximum)
                    throw new ArgumentOutOfRangeException("Value", $"La valeur '{value}' n'est pas valide pour 'Value'. 'Value' doit être compris entre 'Minimum' et 'Maximum'.");
                SetValue(ValueProperty, value);
            }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(decimal), typeof(NumericUpDown), new FrameworkPropertyMetadata(default(decimal), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValue));

        public NumericUpDown()
        {
            Increment = 1;
            Maximum = 100;
            Minimum = 0;

            InitializeComponent();
        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            if (Value < Maximum)
                Value = Value + Increment;
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            if (Value > Minimum)
                Value = Value - Increment;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;

            numericUpDown.OnValueChanged((decimal)e.OldValue, (decimal)e.NewValue);
        }
        protected virtual void OnValueChanged(decimal oldValue, decimal newValue)
        {
            if (oldValue != newValue)
                textBox_Value.Text = newValue.ToString();
        }
        private static object CoerceValue(DependencyObject d, object value)
        {
            if (value == null)
            {
                return null;
            }

            var numericUpDown = (NumericUpDown)d;
            decimal val = ((decimal?)value).Value;

            if (val < numericUpDown.Minimum)
            {
                return numericUpDown.Minimum;
            }
            if (val > numericUpDown.Maximum)
            {
                return numericUpDown.Maximum;
            }
            return val;
        }
    }
}
