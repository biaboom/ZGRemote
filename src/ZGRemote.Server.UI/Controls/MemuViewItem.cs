using HandyControl.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ZGRemote.Server.UI.Controls
{
    public class MemuViewItem : TreeViewItem
    {
        public static RoutedCommand CloseClick { get; } = new(nameof(CloseClick), typeof(MemuViewItem));

        public MemuViewItem()
        {
            CommandBindings.Add(new CommandBinding(CloseClick, (s, e) => OnCloseButtonClick(s, e)));
        }

        

        // 是否显示关闭按钮
        public bool HasClose
        {
            get { return (bool)GetValue(HasCloseProperty); }
            set { SetValue(HasCloseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasClose.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasCloseProperty =
            DependencyProperty.Register("HasClose", typeof(bool), typeof(MemuViewItem), new PropertyMetadata(false));


        // Register a custom routed event using the Bubble routing strategy.
        public static readonly RoutedEvent CloseButtonClickEvent = EventManager.RegisterRoutedEvent(
            name: "CloseButtonClick",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(MemuViewItem));

        // Provide CLR accessors for assigning an event handler.
        public event RoutedEventHandler CloseButtonClick
        {
            add { AddHandler(CloseButtonClickEvent, value); }
            remove { RemoveHandler(CloseButtonClickEvent, value); }
        }

        void RaiseCloseButtonClickRoutedEvent()
        {
            // Create a RoutedEventArgs instance.
            RoutedEventArgs routedEventArgs = new(routedEvent: CloseButtonClickEvent);

            // Raise the event, which will bubble up through the element tree.
            RaiseEvent(routedEventArgs);
        }



        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            RaiseCloseButtonClickRoutedEvent();
        }
    }
}
