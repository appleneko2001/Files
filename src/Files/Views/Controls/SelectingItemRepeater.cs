using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Controls.Selection;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Files.Views.Controls.Events;

namespace Files.Views.Controls
{
    public class SelectingItemRepeater : ItemsRepeater
    {
        public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
            RoutedEvent.Register<SelectingItemRepeater, SelectionChangedEventArgs>(
                "SelectionChanged",
                RoutingStrategies.Bubble);

        public event EventHandler<SelectionChangedEventArgs> SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        public event EventHandler<AdditionalEventArgs> DoubleTappedItemEvent;

        private readonly SelectionModel<object?> _selection = new()
        {
            SingleSelect = false
        };

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

            var prop = e.GetCurrentPoint(child).Properties;

            if (child is not ContentControl)
            {
                var shift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
                var ctrl = e.KeyModifiers.HasFlag(KeyModifiers.Control);

                // Is multiselect
                if (shift && !Selection.SingleSelect && prop.IsLeftButtonPressed)
                {
                    var itemsCast = new List<object>(Items.Cast<object>());
                    
                    var startObject = Selection.SelectedItem ?? itemsCast.First();
                    if (startObject == null)
                    {
                        base.OnPointerPressed(e);
                        return;
                    }

                    var endObject = child.DataContext;

                    long startPos = -1, endPos = -1;

                    long index = 0;
                    foreach (var o in itemsCast)
                    {
                        if (ReferenceEquals(o, startObject))
                            startPos = index;
                        else if (ReferenceEquals(o, endObject))
                            endPos = index;

                        index++;

                        if (startPos == -1)
                            continue;
                        
                        if (endPos == -1)
                            continue;
                        
                        Selection.Deselect((int)startPos);

                        if(endPos > startPos)
                            Selection.SelectRange((int)endPos, (int)startPos);
                        else
                            Selection.SelectRange((int)startPos, (int)endPos);
                        break;
                    }
                }
                else
                {
                    // Single select

                    bool IsSelected(IVisual visual) => Selection.SelectedItems.Contains(child.DataContext);
                    
                    if (!ctrl && prop.IsLeftButtonPressed)
                    {
                        Selection.Clear();
                
                        Selection.SelectedItem = child.DataContext;
                    }
                    else if (!IsSelected(child) && prop.IsRightButtonPressed || !prop.IsRightButtonPressed)
                    {
                        // keep multiselect and pick more by clicking 
                        /*
                        var itemsCast = new List<object>(Items.Cast<object>());

                        var index = 0;
                        foreach (var target in itemsCast.Select(o => 
                                     ReferenceEquals(o, child.DataContext) ? index : -1))
                        {
                            index++;
                            
                            if(target == -1)
                                continue;

                            var isSelected = Selection.IsSelected(target);
                            
                            if(isSelected)
                                Selection.Deselect(target);
                            else
                                Selection.Select(target);
                        }*/
                        Selection.SelectedItem = child.DataContext;
                    }
                }
            }
            
            base.OnPointerPressed(e);

            if (!prop.IsRightButtonPressed)
                return;
            
            if(ContextMenu is not ContextMenu menu)
                return;

            menu.PlacementAnchor = PopupAnchor.None;
            menu.PlacementGravity = PopupGravity.None;
            menu.PlacementTarget = child as Control;
            menu.DataContext = Selection.SelectedItem;
            menu.Open();
        }

        private void OnDoubleTapped(object sender, RoutedEventArgs e)
        {
            e.Handled = false;

            var child = GetChildBySource(e.Source);
            if (child == null)
                return;

            DoubleTappedItemEvent?.Invoke(this, new AdditionalEventArgs
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

            RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, e.DeselectedItems.ToList(),
                e.SelectedItems.ToList()));
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