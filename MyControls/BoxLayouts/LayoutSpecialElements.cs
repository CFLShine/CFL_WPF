
using System;
using System.Windows;
using System.Windows.Media;

namespace ShLayouts
{
    /// <summary>
    /// <see cref="Spacer"/> ajoute un espace
    /// entre deux éléments dans un layout.
    /// </summary>
    public class Spacer : FrameworkElement
    {
        public Spacer(double minSpace = 0, double maxSpace = double.PositiveInfinity)
        {
            MinSpace = minSpace;
            MaxSpace = maxSpace;
        }

        public double MinSpace
        {
            get => __minSpace;
            set
            {
                if(double.IsPositiveInfinity(value) || double.IsNegativeInfinity(value))
                    throw new Exception("MinSpace ne peut être infinie.");
                if(value > MaxSpace)
                    throw new Exception("MinSpace ne peut être supérieure à MaxSpace.");

                if(double.IsNaN(value))
                    __minSpace = 0;
                else
                    __minSpace = value;
            }
        }

        public double MaxSpace
        {
            get => __maxSpace;
            set
            {
                if(value < MinSpace)
                    throw new Exception("MaxSpace ne peut être inferieur à MinSpace.");
                if(double.IsNegativeInfinity(value))
                    throw new Exception("MaxSpace ne peut être NegativeInfinity.");

                if(double.IsNaN(value))
                    __maxSpace = double.PositiveInfinity;
                else
                    __maxSpace = value;
            }
        }

        public Brush BackGround { get; set; } = null;

        private double __minSpace = 0;
        private double __maxSpace = double.PositiveInfinity;
    }

    public class FixedSpacer : Spacer
    {
        public FixedSpacer(double _space)
            :base(_space, _space)
        {}

        public double Space
        {
            get => MinSpace;
        }
    }

}
