using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Threading;

namespace HrtzAudioMixer.Extensions
{
    public class ObservableList<T> : ObservableCollection<T>
    {
        bool _isInAddRange;
        private readonly Dispatcher _currentDispatcher;

        /// <summary>
        /// Creates a new empty ObservableList of the provided type. 
        /// </summary>
        public ObservableList()
        {
            //Assign the current Dispatcher (owner of the collection) 
            _currentDispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Executes this action in the right thread
        /// </summary>
        ///<param name="action">The action which should be executed</param>
        private void DoDispatchedAction(Action action)
        {
            if (_currentDispatcher.CheckAccess())
                action.Invoke();
            else
                _currentDispatcher.Invoke(DispatcherPriority.DataBind, action);
        }

        /// <summary>
        /// Handles the event when a collection has changed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // intercept this when it gets called inside the AddRange method.
            if (!_isInAddRange)
                DoDispatchedAction(() => base.OnCollectionChanged(e));
        }

        /// <summary>
        /// Adds a collection of items to the ObservableList.
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<T> items)
        {
            _isInAddRange = true;
            var enumerable = items as T[] ?? items.ToArray();
            foreach (var item in enumerable.Where(item => item != null))
                Add(item);

            _isInAddRange = false;

            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, enumerable.ToList());
            DoDispatchedAction(() => base.OnCollectionChanged(e));
        }
    }
}
