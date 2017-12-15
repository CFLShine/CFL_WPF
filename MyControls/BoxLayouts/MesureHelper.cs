using System.Windows;
using MSTD;

namespace ShLayouts
{
    public static class MesureHelper
    {
        /// <summary>
        /// Un MinHeight non défini est à 0.
        /// Un Height non définit est à NaN.
        /// 
        /// Si e.MinHeight est à 0, il n'a peut-être pas été défini, 
        /// dans ce cas, si Height est défini, e.Height est retourné,
        /// sinon e.MinHeight est retourné
        /// </summary>
        public static double MinHeight(FrameworkElement e)
        {
            if(e.MinHeight == 0 && !double.IsNaN(e.Height))
                return e.Height;
            return e.MinHeight;
        }

        /// <summary>
        /// Un MinWidth non défini est à 0.
        /// Un Width non définit est à NaN.
        /// 
        /// Si e.MinWidth est à 0, il n'a peut-être pas été défini, 
        /// dans ce cas, si Width est défini, e.Width est retourné,
        /// sinon e.MinWidth est retourné
        /// </summary>
        public static double MinWidth(FrameworkElement e)
        {
            if(e.MinWidth == 0 && !double.IsNaN(e.Width))
                return e.Width;
            return e.MinWidth;
        }

        /// <summary>
        /// Un MaxHeight non défini est à PositiveInfinity.
        /// Un Height non défini est à Nan.
        /// 
        /// Si e.MaxHeight est infini, il n'a peut-être pas été définit et dans ce cas
        /// e.Height est retourné si e.Height est défini,
        /// sinon, retourne e.MaxHeight.
        /// </summary>
        public static double MaxHeight(FrameworkElement e)
        {
            if(double.IsPositiveInfinity(e.MaxHeight) && !double.IsNaN(e.Height))
                return e.Height;
            return e.MaxHeight;
        }

        /// <summary>
        /// Un MaxWidth non défini est à PositiveInfinity.
        /// Un Width non défini est à Nan.
        /// 
        /// Si e.MaxWidth est infini, il n'a peut-être pas été définit et dans ce cas
        /// e.Width est retourné si e.Width est défini,
        /// sinon, retourne e.MaxWidth.
        /// </summary>
        public static double MaxWidth(FrameworkElement e)
        {
            if(double.IsPositiveInfinity(e.MaxWidth) && !double.IsNaN(e.Width))
                return e.Width;
            return e.MaxWidth;
        }
    
        public static double MinHeight(params FrameworkElement[] elements)
        {
            double _minHeight = double.NaN;

            foreach(FrameworkElement e in elements)
            {
                if(DoubleHelper.Bigger(e.MinHeight, _minHeight))
                    _minHeight = e.MinHeight;
            }
            return _minHeight;
        }

        public static double MaxHeight(params FrameworkElement[] elements)
        {
            double _maxHeight = double.NaN;

            foreach(FrameworkElement e in elements)
            {
                if(DoubleHelper.Bigger(e.MaxHeight, _maxHeight))
                {
                    _maxHeight = e.MaxHeight;
                }
            }
            return _maxHeight;
        }

        public static double MaxHeightNotInfinity(params FrameworkElement[] elements)
        {
            double _maxHeight = double.NaN;

            foreach(FrameworkElement e in elements)
            {
                if(!double.IsPositiveInfinity(e.MaxHeight) && DoubleHelper.Bigger(e.MaxHeight, _maxHeight))
                {
                    _maxHeight = e.MaxHeight;
                }
            }
            return _maxHeight;
        }

        public static double MinWidth(params FrameworkElement[] elements)
        {
            double _minWidth = double.NaN;

            foreach(FrameworkElement e in elements)
            {
                if(DoubleHelper.Bigger(e.MinWidth, _minWidth))
                    _minWidth = e.MinWidth;
            }
            return _minWidth;
        }

        public static double MaxWidth(params FrameworkElement[] elements)
        {
            double _maxWidth = double.NaN;

            foreach(FrameworkElement e in elements)
            {
                if(DoubleHelper.Bigger(e.MaxWidth, _maxWidth))
                    _maxWidth = e.MaxWidth;
            }
            return _maxWidth;
        }

        public static double MaxWidthNotInfinity(params FrameworkElement[] elements)
        {
            double _maxWidth = double.NaN;

            foreach(FrameworkElement e in elements)
            {
                if(!double.IsPositiveInfinity(e.MaxWidth) && DoubleHelper.Bigger(e.MaxWidth, _maxWidth))
                    _maxWidth = e.MaxWidth;
            }
            return _maxWidth;
        }

    }
    
}
