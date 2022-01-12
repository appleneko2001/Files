using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Files.Views.Controls.Events;

namespace Files.Views.Controls
{
    public class SelectingItemRepeater : ItemsRepeater
    {
        public event EventHandler<AdditionalEventArgs> DoubleTappedItemEvent; 

        private readonly SelectionModel<object?> _selection = new();

        public SelectionModel<object?> Selection => _selection;

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            _selection.IndexesChanged += OnSelectionIndexesChanged;
            _selection.LostSelection += OnSelectionLostSelection;
            _selection.SelectionChanged += OnSelectionSelectionChanged;
            _selection.SourceReset += OnSelectionSourceReset;
            
            DoubleTapped += OnDoubleTapped;
            
            base.OnAttachedToLogicalTree(e);
        }

        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            _selection.IndexesChanged -= OnSelectionIndexesChanged;
            _selection.LostSelection -= OnSelectionLostSelection;
            _selection.SelectionChanged -= OnSelectionSelectionChanged;
            _selection.SourceReset -= OnSelectionSourceReset;
            
            DoubleTapped -= OnDoubleTapped;
            
            base.OnDetachedFromLogicalTree(e);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            if (change.Property == ItemsProperty)
            {
                Selection.Source = Items as IEnumerable<object?>;
            }
            
            base.OnPropertyChanged(change);
        }
        
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            var child = GetChildBySource(e.Source);

            if (child == null)
            {
                base.OnPointerPressed(e);
                return;
            }
            
            if (child is not ContentControl)
            {
                Selection.SelectedItem = child.DataContext;
            }
            
            base.OnPointerPressed(e);
        }
        
        private void OnDoubleTapped(object sender, TappedEventArgs e)
        {
            e.Handled = false;
            
            var child = GetChildBySource(e.Source);
            if (child == null) 
                return;

            DoubleTappedItemEvent?.Invoke(this,  new AdditionalEventArgs
            {
                Argument = child,
                Source = e
            });
        }

        private Visual? GetChildBySource(IInteractive? source)
        {
            if (source is not Visual s)
                return null;
            
            var target = s;
            while (true)
            {
                if (target == null)
                    throw new ArgumentNullException(nameof(target));
                    
                if (!Equals(target.Parent, this))
                    target = target.Parent as Visual;
                    
                else
                    break;
            }

            return target;
        }
        
        private void OnSelectionSelectionChanged(object sender, SelectionModelSelectionChangedEventArgs<object?> e)
        {
            foreach (var item in e.DeselectedItems)
            {
                if (item is ISelectable i)
                    i.IsSelected = false;
            }
            
            foreach (var item in e.SelectedItems)
            {
                if (item is ISelectable i)
                    i.IsSelected = true;
            }
        }
        
        private void OnSelectionIndexesChanged(object sender, SelectionModelIndexesChangedEventArgs e)
        {
        }
        
        private void OnSelectionLostSelection(object sender, EventArgs e)
        {
        }
        
        private void OnSelectionSourceReset(object sender, EventArgs e)
        {
        }
    }
}