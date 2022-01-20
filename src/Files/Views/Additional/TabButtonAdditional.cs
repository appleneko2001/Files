using Avalonia;
using Avalonia.Controls;

namespace Files.Views.Additional
{
    public class TabButtonAdditional : AvaloniaObject
    {
        public static readonly AttachedProperty<bool> IsSelectedProperty = AvaloniaProperty.RegisterAttached<TabButtonAdditional, Panel, bool>(
            "IsSelected");
        
        public static void SetIsSelected(AvaloniaObject element, bool value)
        {
            element.SetValue(IsSelectedProperty, value);
        }

        public static bool GetIsSelected(AvaloniaObject element)
        {
            return element.GetValue(IsSelectedProperty);
        }
    }
}