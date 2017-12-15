
using System;
using System.Windows;
using System.Windows.Media;

namespace ShLayouts
{
    /// <summary>
    /// <see cref="Spacer"/> ajoute un espace
    /// entre deux éléments dans un layout.
    /// Si <see cref="MinSpace"/> est à double.NaN, <see cref="Spacer"/> occupe le plus d'espace
    /// possible, réduisant l'espace des autre élément à leur MinHeight ou MinWidth, Height ou Width,
    /// selon <see cref="BoxLayout.Orientation"/>.
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
                    __maxSpace = 0;
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

    /// <summary>
    /// Contrairement à <see cref="Spacer"/>, <see cref="Glue"/> limite l'espace entre deux
    /// éléments d'un layout à <see cref="MaxSpace"/>.
    /// Une valeur à 0 (valeur par défaut), permet donc de coller ensemble des éléments, 
    /// ou de coller un premier élément au bord du layout.
    /// Le <see cref="Glue"/> agit en limitant la prochaine cellule au ElementMinimalVisibility de son élément
    /// contenu.
    /// </summary>
    //public class Glue : FrameworkElement
    //{
    //    public Glue(double maxSpace = 0)
    //    {
    //        MaxSpace = maxSpace;
    //    }

    //    public double MaxSpace
    //    {
    //        get => __maxSpace;
    //        set
    //        {
    //            if(double.IsNegativeInfinity(value) || double.IsPositiveInfinity(value))
    //                throw new Exception("MaxSpace ne peut pas être infini.");
                
    //            if(double.IsNaN(value))
    //                __maxSpace = 0;
    //            else
    //                __maxSpace = value;
    //        }
    //    } 

    //    public Brush BackGround { get; set; } = null;

    //    private double __maxSpace = 0;
    //}

}
