
namespace OxyPlot
{
    using System;
    using System.Collections.Generic;

    public abstract partial class Element 
    {
        private Selection selection;

        [Obsolete("May be removed in v4.0 (#111)")]
        public event EventHandler SelectionChanged;
        public bool Selectable { get; set; } = true;
        public SelectionMode SelectionMode { get; set; } = SelectionMode.All;

#if X
        public bool IsEverythingSelected()
        {
            return this.selection.IsEverythingSelected();
        }
#endif
        protected OxyColor ActualSelectedColor
        {
            get
            {
                if (this.Parent != null)
                {
                    return this.Parent.SelectionColor.GetActualColor(Model.DefaultSelectionColor);
                }

                return Model.DefaultSelectionColor;
            }
        }

        public bool IsSelected()
        {
            return this.selection != null;
        }

        public IEnumerable<int> GetSelectedItems()
        {
            this.EnsureSelection();
            return this.selection.GetSelectedItems();
        }

        public void ClearSelection()
        {
            this.selection = null;
            this.OnSelectionChanged();
        }

        public void Unselect()
        {
            this.selection = null;
            this.OnSelectionChanged();
        }
        
        public bool IsItemSelected(int index)
        {
            if (this.selection == null)
            {
                return false;
            }

            if (index == -1)
            {
                return this.selection.IsEverythingSelected();
            }

            return this.selection.IsItemSelected(index);
        }

        public void Select()
        {
            this.selection = Selection.Everything;
            this.OnSelectionChanged();
        }

        public void SelectItem(int index)
        {
            if (this.SelectionMode == SelectionMode.All)
            {
                throw new InvalidOperationException("Use the Select() method when using SelectionMode.All");
            }

            this.EnsureSelection();
            if (this.SelectionMode == SelectionMode.Single)
            {
                this.selection.Clear();
            }

            this.selection.Select(index);
            this.OnSelectionChanged();
        }

        public void UnselectItem(int index)
        {
            if (this.SelectionMode == SelectionMode.All)
            {
                throw new InvalidOperationException("Use the Unselect() method when using SelectionMode.All");
            }

            this.EnsureSelection();
            this.selection.Unselect(index);
            this.OnSelectionChanged();
        }

        protected OxyColor GetSelectableColor(OxyColor originalColor, int index = -1)
        {
            // TODO: rename to GetActualColor (33 usages)
            if (originalColor.IsUndefined())
            {
                return OxyColors.Undefined;
            }

            if (this.IsItemSelected(index))
            {
                return this.ActualSelectedColor;
            }

            return originalColor;
        }

        protected OxyColor GetSelectableFillColor(OxyColor originalColor, int index = -1)
        {
            return this.GetSelectableColor(originalColor, index);
        }

        private void EnsureSelection()
        {
            if (this.selection == null)
            {
                this.selection = new Selection();
            }
        }

        private void OnSelectionChanged(EventArgs args = null)
        {
            var e = this.SelectionChanged;
            if (e != null)
            {
                e(this, args);
            }
        }
    }
}
