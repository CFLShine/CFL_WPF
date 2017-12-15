using System;
using System.Windows;
using System.Windows.Controls;

namespace ShLayouts
{

    /// <summary>
    /// <see cref="BoxLayout"/> est un layout permetant d'aranger les éléments sur une ligne ou une colone.
    /// Pour imposer un espace entre deux éléments, ajouter un <see cref="Spacer"/>, 
    /// </summary>
    public abstract class BoxLayout : Grid
    {
        public BoxLayout(Orientation _orientation)
        {
            Orientation = _orientation;
            Model = new BoxLayoutModel(this);
            Init();
            ShowGridLines = true;
        }

        public BoxLayoutModel Model { get; private set; }

        public Orientation Orientation
        {
            get;
            protected set;
        }

        public bool IsPerpendicularMinimized
        {
            get => Model.IsPerpendicularMinimized;
            set => Model.IsPerpendicularMinimized = value;
        }

        public void Add(FrameworkElement e)
        {
            Model.Add(e);
        }

        /// <summary>
        /// Insert dans le layout l'élément e à l'index index.
        /// Provoque une exeption si l'index est en dehors des limites [0 , Count].
        /// </summary>
        public virtual void Insert(int index, FrameworkElement e)
        {
            if(index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("index");
            Model.Insert(index, e);
        }

        /// <summary>
        /// Suprime du layout l'élément e, s'il existe dans le layout,
        /// sinon, ne provoque pas d'exeption (Remove(null) est permis).
        /// </summary>
        public void Remove(FrameworkElement e)
        {
            Model.Remove(e);
        }

        public int Count
        {
            get => Model.Count;
        }

        public void Clear()
        {
            Model.Clear();
            Init();
        }

        private void Init()
        {
            RowDefinitions.Clear();
            ColumnDefinitions.Clear();
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    RowDefinitions.Add(new RowDefinition());
                    break;
                
                case Orientation.Vertical:
                    ColumnDefinitions.Add(new ColumnDefinition());
                    break;
            }
        }
    }

    public class VBoxLayout : BoxLayout
    {
        public VBoxLayout()
            :base(Orientation.Vertical)
        { }
    }

    public class HBoxLayout : BoxLayout
    {
        public HBoxLayout()
            :base(Orientation.Horizontal)
        { }
    }
}
