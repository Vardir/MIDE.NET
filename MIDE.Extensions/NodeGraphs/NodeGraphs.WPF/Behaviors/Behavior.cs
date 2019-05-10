using System;
using System.Windows;

namespace NodeGraphs.WPF.Behaviors
{
    public abstract class Behavior
    {
        public static void Attach(Behavior behavior, object element)
        {
            if (behavior == null || element == null)
                throw new ArgumentNullException();

            behavior.Attach(element);
        }

        public abstract void Attach(object obj);
        public abstract void Detach();
    }

    public class Behavior<T> : Behavior
        where T : UIElement
    {
        public T AssociatedObject { get; private set; }

        public override void Attach(object obj)
        {
            if (obj == null || !(obj is T element))
                throw new ArgumentException();
            Attach(element);
        }
        public void Attach(T element)
        {
            if (element == null)
                throw new ArgumentNullException();

            AssociatedObject = element;
            OnAttached();
        }
        public override void Detach()
        {
            AssociatedObject = null;
            OnDetached();
        }

        protected virtual void OnAttached() { }
        protected virtual void OnDetached() { }
    }
}