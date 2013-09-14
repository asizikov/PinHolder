// (c) Copyright 2010 Microsoft Corporation.
// This source is subject to the Microsoft Public License (MS-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
//
// Author: Jason Ginchereau - jasongin@microsoft.com - http://blogs.msdn.com/jasongin/
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;

namespace PinHolder.Conrols
{
    /// <summary>
    ///     Extends ListBox to enable drag-and-drop reorder within the list.
    /// </summary>
    [TemplatePart(Name = ScrollViewerPart, Type = typeof (ScrollViewer))]
    [TemplatePart(Name = DragIndicatorPart, Type = typeof (Image))]
    [TemplatePart(Name = DragInterceptorPart, Type = typeof (Canvas))]
    [TemplatePart(Name = RearrangeCanvasPart, Type = typeof (Canvas))]
    public class ReorderListBox : ListBox
    {
        #region Template part name constants

        public const string ScrollViewerPart = "ScrollViewer";
        public const string DragIndicatorPart = "DragIndicator";
        public const string DragInterceptorPart = "DragInterceptor";
        public const string RearrangeCanvasPart = "RearrangeCanvas";

        #endregion

        private const string ScrollViewerScrollingVisualState = "Scrolling";
        private const string ScrollViewerNotScrollingVisualState = "NotScrolling";

        private const string IsReorderEnabledPropertyName = "IsReorderEnabled";

        #region Private fields

        private Image dragIndicator;
        private Canvas dragInterceptor;
        private Rect dragInterceptorRect;
        private object dragItem;
        private ReorderListBoxItem dragItemContainer;
        private double dragScrollDelta;
        private int dropTargetIndex;
        private bool isDragItemSelected;
        private Panel itemsPanel;
        private Canvas rearrangeCanvas;
        private Queue<KeyValuePair<Action, Duration>> rearrangeQueue;
        private ScrollViewer scrollViewer;

        #endregion

        /// <summary>
        ///     Creates a new ReorderListBox and sets the default style key.
        ///     The style key is used to locate the control template in Generic.xaml.
        /// </summary>
        public ReorderListBox()
        {
            DefaultStyleKey = typeof (ReorderListBox);
        }

        #region IsReorderEnabled DependencyProperty

        public static readonly DependencyProperty IsReorderEnabledProperty = DependencyProperty.Register(
            IsReorderEnabledPropertyName, typeof (bool), typeof (ReorderListBox),
            new PropertyMetadata(false, (d, e) => ((ReorderListBox) d).OnIsReorderEnabledChanged(e)));

        /// <summary>
        ///     Gets or sets a value indicating whether reordering is enabled in the listbox.
        ///     This also controls the visibility of the reorder drag-handle of each listbox item.
        /// </summary>
        public bool IsReorderEnabled
        {
            get { return (bool) GetValue(IsReorderEnabledProperty); }
            set { SetValue(IsReorderEnabledProperty, value); }
        }

        protected void OnIsReorderEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if (dragInterceptor != null)
            {
                dragInterceptor.Visibility = (bool) e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }

            InvalidateArrange();
        }

        #endregion

        #region AutoScrollMargin DependencyProperty

        public static readonly DependencyProperty AutoScrollMarginProperty = DependencyProperty.Register(
            "AutoScrollMargin", typeof (int), typeof (ReorderListBox), new PropertyMetadata(32));

        /// <summary>
        ///     Gets or sets the size of the region at the top and bottom of the list where dragging will
        ///     cause the list to automatically scroll.
        /// </summary>
        public double AutoScrollMargin
        {
            get { return (int) GetValue(AutoScrollMarginProperty); }
            set { SetValue(AutoScrollMarginProperty, value); }
        }

        #endregion

        #region ItemsControl overrides

        /// <summary>
        ///     Applies the control template, gets required template parts, and hooks up the drag events.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            scrollViewer = (ScrollViewer) GetTemplateChild(ScrollViewerPart);
            dragInterceptor = GetTemplateChild(DragInterceptorPart) as Canvas;
            dragIndicator = GetTemplateChild(DragIndicatorPart) as Image;
            rearrangeCanvas = GetTemplateChild(RearrangeCanvasPart) as Canvas;

            if (scrollViewer != null && dragInterceptor != null && dragIndicator != null)
            {
                dragInterceptor.Visibility = IsReorderEnabled ? Visibility.Visible : Visibility.Collapsed;

                dragInterceptor.ManipulationStarted += dragInterceptor_ManipulationStarted;
                dragInterceptor.ManipulationDelta += dragInterceptor_ManipulationDelta;
                dragInterceptor.ManipulationCompleted += dragInterceptor_ManipulationCompleted;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ReorderListBoxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ReorderListBoxItem;
        }

        /// <summary>
        ///     Ensures that a possibly-recycled item container (ReorderListBoxItem) is ready to display a list item.
        /// </summary>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var itemContainer = (ReorderListBoxItem) element;
            itemContainer.ApplyTemplate(); // Loads visual states.

            // Set this state before binding to avoid showing the visual transition in this case.
            string reorderState = IsReorderEnabled
                ? ReorderListBoxItem.ReorderEnabledState
                : ReorderListBoxItem.ReorderDisabledState;
            VisualStateManager.GoToState(itemContainer, reorderState, false);

            itemContainer.SetBinding(ReorderListBoxItem.IsReorderEnabledProperty,
                new Binding(IsReorderEnabledPropertyName) {Source = this});

            if (item == dragItem)
            {
                itemContainer.IsSelected = isDragItemSelected;
                VisualStateManager.GoToState(itemContainer, ReorderListBoxItem.DraggingState, false);

                if (dropTargetIndex >= 0)
                {
                    // The item's dragIndicator is currently being moved, so the item itself is hidden. 
                    itemContainer.Visibility = Visibility.Collapsed;
                    dragItemContainer = itemContainer;
                }
                else
                {
                    itemContainer.Opacity = 0;
                    Dispatcher.BeginInvoke(() => AnimateDrop(itemContainer));
                }
            }
            else
            {
                VisualStateManager.GoToState(itemContainer, ReorderListBoxItem.NotDraggingState, false);
            }
        }

        /// <summary>
        ///     Called when an item container (ReorderListBoxItem) is being removed from the list panel.
        ///     This may be because the item was removed from the list or because the item is now outside
        ///     the virtualization region (because ListBox uses a VirtualizingStackPanel as its items panel).
        /// </summary>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            var itemContainer = (ReorderListBoxItem) element;
            if (itemContainer == dragItemContainer)
            {
                dragItemContainer.Visibility = Visibility.Visible;
                dragItemContainer = null;
            }
        }

        #endregion

        #region Drag & drop reorder

        /// <summary>
        ///     Called when the user presses down on the transparent drag-interceptor. Identifies the targed
        ///     drag handle and list item and prepares for a drag operation.
        /// </summary>
        private void dragInterceptor_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (dragItem != null)
            {
                return;
            }

            if (itemsPanel == null)
            {
                var scrollItemsPresenter = (ItemsPresenter) scrollViewer.Content;
                itemsPanel = (Panel) VisualTreeHelper.GetChild(scrollItemsPresenter, 0);
            }

            GeneralTransform interceptorTransform = dragInterceptor.TransformToVisual(
                Application.Current.RootVisual);
            Point targetPoint = interceptorTransform.Transform(e.ManipulationOrigin);
            targetPoint = GetHostCoordinates(targetPoint);

            List<UIElement> targetElements = VisualTreeHelper.FindElementsInHostCoordinates(
                targetPoint, itemsPanel).ToList();
            ReorderListBoxItem targetItemContainer = targetElements.OfType<ReorderListBoxItem>().FirstOrDefault();
            if (targetItemContainer != null && targetElements.Contains(targetItemContainer.DragHandle))
            {
                VisualStateManager.GoToState(targetItemContainer, ReorderListBoxItem.DraggingState, true);

                GeneralTransform targetItemTransform = targetItemContainer.TransformToVisual(dragInterceptor);
                Point targetItemOrigin = targetItemTransform.Transform(new Point(0, 0));
                Canvas.SetLeft(dragIndicator, targetItemOrigin.X);
                Canvas.SetTop(dragIndicator, targetItemOrigin.Y);
                dragIndicator.Width = targetItemContainer.RenderSize.Width;
                dragIndicator.Height = targetItemContainer.RenderSize.Height;

                dragItemContainer = targetItemContainer;
                dragItem = dragItemContainer.Content;
                isDragItemSelected = dragItemContainer.IsSelected;

                dragInterceptorRect = interceptorTransform.TransformBounds(
                    new Rect(new Point(0, 0), dragInterceptor.RenderSize));

                dropTargetIndex = -1;
            }
        }

        /// <summary>
        ///     Called when the user drags on (or from) the transparent drag-interceptor.
        ///     Moves the item (actually a rendered snapshot of the item) according to the drag delta.
        /// </summary>
        private void dragInterceptor_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (Items.Count <= 1 || dragItem == null)
            {
                return;
            }

            if (dropTargetIndex == -1)
            {
                if (dragItemContainer == null)
                {
                    return;
                }

                // When the drag actually starts, swap out the item for the drag-indicator image of the item.
                // This is necessary because the item itself may be removed from the virtualizing panel
                // if the drag causes a scroll of considerable distance.
                Size dragItemSize = dragItemContainer.RenderSize;
                var writeableBitmap = new WriteableBitmap(
                    (int) dragItemSize.Width, (int) dragItemSize.Height);

                // Swap states to force the transition to complete.
                VisualStateManager.GoToState(dragItemContainer, ReorderListBoxItem.NotDraggingState, false);
                VisualStateManager.GoToState(dragItemContainer, ReorderListBoxItem.DraggingState, false);
                writeableBitmap.Render(dragItemContainer, null);

                writeableBitmap.Invalidate();
                dragIndicator.Source = writeableBitmap;

                dragIndicator.Visibility = Visibility.Visible;
                dragItemContainer.Visibility = Visibility.Collapsed;

                if (itemsPanel.Children.IndexOf(dragItemContainer) < itemsPanel.Children.Count - 1)
                {
                    UpdateDropTarget(Canvas.GetTop(dragIndicator) + dragIndicator.Height + 1, false);
                }
                else
                {
                    UpdateDropTarget(Canvas.GetTop(dragIndicator) - 1, false);
                }
            }

            double dragItemHeight = dragIndicator.Height;

            var translation = (TranslateTransform) dragIndicator.RenderTransform;
            double top = Canvas.GetTop(dragIndicator);

            // Limit the translation to keep the item within the list area.
            // Use different targeting for the top and bottom edges to allow taller items to
            // move before or after shorter items at the edges.
            double y = top + e.CumulativeManipulation.Translation.Y;
            if (y < 0)
            {
                y = 0;
                UpdateDropTarget(0, true);
            }
            else if (y >= dragInterceptorRect.Height - dragItemHeight)
            {
                y = dragInterceptorRect.Height - dragItemHeight;
                UpdateDropTarget(dragInterceptorRect.Height - 1, true);
            }
            else
            {
                UpdateDropTarget(y + dragItemHeight/2, true);
            }

            translation.Y = y - top;

            // Check if we're within the margin where auto-scroll needs to happen.
            bool scrolling = (dragScrollDelta != 0);
            double autoScrollMargin = AutoScrollMargin;
            if (autoScrollMargin > 0 && y < autoScrollMargin)
            {
                dragScrollDelta = y - autoScrollMargin;
                if (!scrolling)
                {
                    VisualStateManager.GoToState(scrollViewer, ScrollViewerScrollingVisualState, true);
                    Dispatcher.BeginInvoke(() => DragScroll());
                }
            }
            else if (autoScrollMargin > 0 && y + dragItemHeight > dragInterceptorRect.Height - autoScrollMargin)
            {
                dragScrollDelta = (y + dragItemHeight - (dragInterceptorRect.Height - autoScrollMargin));
                if (!scrolling)
                {
                    VisualStateManager.GoToState(scrollViewer, ScrollViewerScrollingVisualState, true);
                    Dispatcher.BeginInvoke(() => DragScroll());
                }
            }
            else
            {
                // We're not within the auto-scroll margin. This ensures any current scrolling is stopped.
                dragScrollDelta = 0;
            }
        }

        /// <summary>
        ///     Called when the user releases a drag. Moves the item within the source list and then resets everything.
        /// </summary>
        private void dragInterceptor_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (dragItem == null)
            {
                return;
            }

            if (dropTargetIndex >= 0)
            {
                MoveItem(dragItem, dropTargetIndex);
            }

            if (dragItemContainer != null)
            {
                dragItemContainer.Visibility = Visibility.Visible;
                dragItemContainer.Opacity = 0;
                AnimateDrop(dragItemContainer);
                dragItemContainer = null;
            }

            dragScrollDelta = 0;
            dropTargetIndex = -1;
            ClearDropTarget();
        }

        /// <summary>
        ///     Slides the drag indicator (item snapshot) to the location of the dropped item,
        ///     then performs the visibility swap and removes the dragging visual state.
        /// </summary>
        private void AnimateDrop(ReorderListBoxItem itemContainer)
        {
            GeneralTransform itemTransform = itemContainer.TransformToVisual(dragInterceptor);
            Rect itemRect = itemTransform.TransformBounds(new Rect(new Point(0, 0), itemContainer.RenderSize));
            double delta = Math.Abs(itemRect.Y - Canvas.GetTop(dragIndicator) -
                                    ((TranslateTransform) dragIndicator.RenderTransform).Y);
            if (delta > 0)
            {
                // Adjust the duration based on the distance, so the speed will be constant.

                //Anton Sizikov: an OverflowExceptionOverflow when itemRect.Height equals to zero.
                TimeSpan duration;
                if (itemRect.Height == 0.0d)
                {
                    duration = TimeSpan.FromSeconds(0);
                }
                else
                {
                    double seconds = 0.25*delta/itemRect.Height;
                    duration = TimeSpan.FromSeconds(seconds);
                }

                var dropStoryboard = new Storyboard();
                var moveToDropAnimation = new DoubleAnimation();
                Storyboard.SetTarget(moveToDropAnimation, dragIndicator.RenderTransform);
                Storyboard.SetTargetProperty(moveToDropAnimation, new PropertyPath(TranslateTransform.YProperty));
                moveToDropAnimation.To = itemRect.Y - Canvas.GetTop(dragIndicator);
                moveToDropAnimation.Duration = duration;
                dropStoryboard.Children.Add(moveToDropAnimation);

                dropStoryboard.Completed += delegate
                {
                    dragItem = null;
                    itemContainer.Opacity = 1;
                    dragIndicator.Visibility = Visibility.Collapsed;
                    dragIndicator.Source = null;
                    ((TranslateTransform) dragIndicator.RenderTransform).Y = 0;
                    VisualStateManager.GoToState(itemContainer, ReorderListBoxItem.NotDraggingState, true);
                };
                dropStoryboard.Begin();
            }
            else
            {
                // There was no need for an animation, so do the visibility swap right now.
                dragItem = null;
                itemContainer.Opacity = 1;
                dragIndicator.Visibility = Visibility.Collapsed;
                dragIndicator.Source = null;
                VisualStateManager.GoToState(itemContainer, ReorderListBoxItem.NotDraggingState, true);
            }
        }

        /// <summary>
        ///     Automatically scrolls for as long as the drag is held within the margin.
        ///     The speed of the scroll is adjusted based on the depth into the margin.
        /// </summary>
        private void DragScroll()
        {
            if (dragScrollDelta != 0)
            {
                double scrollRatio = scrollViewer.ViewportHeight/scrollViewer.RenderSize.Height;
                double adjustedDelta = dragScrollDelta*scrollRatio;
                double newOffset = scrollViewer.VerticalOffset + adjustedDelta;
                scrollViewer.ScrollToVerticalOffset(newOffset);

                Dispatcher.BeginInvoke(() => DragScroll());

                double dragItemOffset = Canvas.GetTop(dragIndicator) +
                                        ((TranslateTransform) dragIndicator.RenderTransform).Y +
                                        dragIndicator.Height/2;
                UpdateDropTarget(dragItemOffset, true);
            }
            else
            {
                VisualStateManager.GoToState(scrollViewer, ScrollViewerNotScrollingVisualState, true);
            }
        }

        /// <summary>
        ///     Updates spacing (drop target indicators) surrounding the targeted region.
        /// </summary>
        /// <param name="dragItemOffset">Vertical offset into the items panel where the drag is currently targeting.</param>
        /// <param name="showTransition">True if the drop-indicator transitions should be shown.</param>
        private void UpdateDropTarget(double dragItemOffset, bool showTransition)
        {
            Point dragPoint = GetHostCoordinates(
                new Point(dragInterceptorRect.Left, dragInterceptorRect.Top + dragItemOffset));
            IEnumerable<UIElement> targetElements = VisualTreeHelper.FindElementsInHostCoordinates(dragPoint, itemsPanel);
            ReorderListBoxItem targetItem = targetElements.OfType<ReorderListBoxItem>().FirstOrDefault();
            if (targetItem != null)
            {
                GeneralTransform targetTransform = targetItem.DragHandle.TransformToVisual(dragInterceptor);
                Rect targetRect =
                    targetTransform.TransformBounds(new Rect(new Point(0, 0), targetItem.DragHandle.RenderSize));
                double targetCenter = (targetRect.Top + targetRect.Bottom)/2;

                int targetIndex = itemsPanel.Children.IndexOf(targetItem);
                int childrenCount = itemsPanel.Children.Count;
                bool after = dragItemOffset > targetCenter;

                ReorderListBoxItem indicatorItem = null;
                if (!after && targetIndex > 0)
                {
                    var previousItem = (ReorderListBoxItem) itemsPanel.Children[targetIndex - 1];
                    if (previousItem.Tag as string == ReorderListBoxItem.DropAfterIndicatorState)
                    {
                        indicatorItem = previousItem;
                    }
                }
                else if (after && targetIndex < childrenCount - 1)
                {
                    var nextItem = (ReorderListBoxItem) itemsPanel.Children[targetIndex + 1];
                    if (nextItem.Tag as string == ReorderListBoxItem.DropBeforeIndicatorState)
                    {
                        indicatorItem = nextItem;
                    }
                }
                if (indicatorItem == null)
                {
                    targetItem.DropIndicatorHeight = dragIndicator.Height;
                    string dropIndicatorState = after
                        ? ReorderListBoxItem.DropAfterIndicatorState
                        : ReorderListBoxItem.DropBeforeIndicatorState;
                    VisualStateManager.GoToState(targetItem, dropIndicatorState, showTransition);
                    targetItem.Tag = dropIndicatorState;
                    indicatorItem = targetItem;
                }

                for (int i = targetIndex - 5; i <= targetIndex + 5; i++)
                {
                    if (i >= 0 && i < childrenCount)
                    {
                        var nearbyItem = (ReorderListBoxItem) itemsPanel.Children[i];
                        if (nearbyItem != indicatorItem)
                        {
                            VisualStateManager.GoToState(nearbyItem, ReorderListBoxItem.NoDropIndicatorState,
                                showTransition);
                            nearbyItem.Tag = ReorderListBoxItem.NoDropIndicatorState;
                        }
                    }
                }

                UpdateDropTargetIndex(targetItem, after);
            }
        }

        /// <summary>
        ///     Updates the targeted index -- that is the index where the item will be moved to if dropped at this point.
        /// </summary>
        private void UpdateDropTargetIndex(ReorderListBoxItem targetItemContainer, bool after)
        {
            int dragItemIndex = Items.IndexOf(dragItem);
            int targetItemIndex = Items.IndexOf(targetItemContainer.Content);

            int newDropTargetIndex;
            if (targetItemIndex == dragItemIndex)
            {
                newDropTargetIndex = dragItemIndex;
            }
            else
            {
                newDropTargetIndex = targetItemIndex + (after ? 1 : 0) - (targetItemIndex >= dragItemIndex ? 1 : 0);
            }

            if (newDropTargetIndex != dropTargetIndex)
            {
                dropTargetIndex = newDropTargetIndex;
            }
        }

        /// <summary>
        ///     Hides any drop-indicators that are currently visible.
        /// </summary>
        private void ClearDropTarget()
        {
            foreach (ReorderListBoxItem itemContainer in itemsPanel.Children)
            {
                VisualStateManager.GoToState(itemContainer, ReorderListBoxItem.NoDropIndicatorState, false);
                itemContainer.Tag = null;
            }
        }

        /// <summary>
        ///     Moves an item to a specified index in the source list.
        /// </summary>
        private bool MoveItem(object item, int toIndex)
        {
            object itemsSource = ItemsSource;

            var sourceList = itemsSource as IList;
            if (!(sourceList is INotifyCollectionChanged))
            {
                // If the source does not implement INotifyCollectionChanged, then there's no point in
                // changing the source because changes to it will not be synchronized with the list items.
                // So, just change the ListBox's view of the items.
                sourceList = Items;
            }

            int fromIndex = sourceList.IndexOf(item);
            if (fromIndex != toIndex)
            {
                double scrollOffset = scrollViewer.VerticalOffset;

                sourceList.RemoveAt(fromIndex);
                sourceList.Insert(toIndex, item);

                if (fromIndex <= scrollOffset && toIndex > scrollOffset)
                {
                    // Correct the scroll offset for the removed item so that the list doesn't appear to jump.
                    scrollViewer.ScrollToVerticalOffset(scrollOffset - 1);
                }
                return true;
            }
            return false;
        }

        #endregion

        #region View range detection

        /// <summary>
        ///     Gets the indices of the first and last items in the view based on the current scroll position.
        /// </summary>
        /// <param name="includePartial">
        ///     True to include items that are partially obscured at the top and bottom,
        ///     false to include only items that are completely in view.
        /// </param>
        /// <param name="firstIndex">Returns the index of the first item in view (or -1 if there are no items).</param>
        /// <param name="lastIndex">Returns the index of the last item in view (or -1 if there are no items).</param>
        public void GetViewIndexRange(bool includePartial, out int firstIndex, out int lastIndex)
        {
            if (Items.Count > 0)
            {
                firstIndex = 0;
                lastIndex = Items.Count - 1;

                if (scrollViewer != null && Items.Count > 1)
                {
                    var scrollViewerPadding = new Thickness(
                        scrollViewer.BorderThickness.Left + scrollViewer.Padding.Left,
                        scrollViewer.BorderThickness.Top + scrollViewer.Padding.Top,
                        scrollViewer.BorderThickness.Right + scrollViewer.Padding.Right,
                        scrollViewer.BorderThickness.Bottom + scrollViewer.Padding.Bottom);

                    GeneralTransform scrollViewerTransform = scrollViewer.TransformToVisual(
                        Application.Current.RootVisual);
                    Rect scrollViewerRect = scrollViewerTransform.TransformBounds(
                        new Rect(new Point(0, 0), scrollViewer.RenderSize));

                    Point topPoint = GetHostCoordinates(new Point(
                        scrollViewerRect.Left + scrollViewerPadding.Left,
                        scrollViewerRect.Top + scrollViewerPadding.Top));
                    IEnumerable<UIElement> topElements = VisualTreeHelper.FindElementsInHostCoordinates(
                        topPoint, scrollViewer);
                    ReorderListBoxItem topItem = topElements.OfType<ReorderListBoxItem>().FirstOrDefault();
                    if (topItem != null)
                    {
                        GeneralTransform itemTransform = topItem.TransformToVisual(Application.Current.RootVisual);
                        Rect itemRect = itemTransform.TransformBounds(new Rect(new Point(0, 0), topItem.RenderSize));

                        firstIndex = ItemContainerGenerator.IndexFromContainer(topItem);
                        if (!includePartial && firstIndex < Items.Count - 1 &&
                            itemRect.Top < scrollViewerRect.Top && itemRect.Bottom < scrollViewerRect.Bottom)
                        {
                            firstIndex++;
                        }
                    }

                    Point bottomPoint = GetHostCoordinates(new Point(
                        scrollViewerRect.Left + scrollViewerPadding.Left,
                        scrollViewerRect.Bottom - scrollViewerPadding.Bottom - 1));
                    IEnumerable<UIElement> bottomElements = VisualTreeHelper.FindElementsInHostCoordinates(
                        bottomPoint, scrollViewer);
                    ReorderListBoxItem bottomItem = bottomElements.OfType<ReorderListBoxItem>().FirstOrDefault();
                    if (bottomItem != null)
                    {
                        GeneralTransform itemTransform = bottomItem.TransformToVisual(Application.Current.RootVisual);
                        Rect itemRect = itemTransform.TransformBounds(
                            new Rect(new Point(0, 0), bottomItem.RenderSize));

                        lastIndex = ItemContainerGenerator.IndexFromContainer(bottomItem);
                        if (!includePartial && lastIndex > firstIndex &&
                            itemRect.Bottom > scrollViewerRect.Bottom && itemRect.Top > scrollViewerRect.Top)
                        {
                            lastIndex--;
                        }
                    }
                }
            }
            else
            {
                firstIndex = -1;
                lastIndex = -1;
            }
        }

        #endregion

        #region Rearrange

        /// <summary>
        ///     Animates movements, insertions, or deletions in the list.
        /// </summary>
        /// <param name="animationDuration">Duration of the animation.</param>
        /// <param name="rearrangeAction">Performs the actual rearrange on the list source.</param>
        /// <remarks>
        ///     The animations are as follows:
        ///     - Inserted items fade in while later items slide down to make space.
        ///     - Removed items fade out while later items slide up to close the gap.
        ///     - Moved items slide from their previous location to their new location.
        ///     - Moved items which move out of or in to the visible area also fade out / fade in while sliding.
        ///     <para>
        ///         The rearrange action callback is called in the middle of the rearrange process. That
        ///         callback may make any number of changes to the list source, in any order. After the rearrange
        ///         action callback returns, the net result of all changes will be detected and included in a dynamically
        ///         generated rearrange animation.
        ///     </para>
        ///     <para>
        ///         Multiple calls to this method in quick succession will be automatically queued up and executed in turn
        ///         to avoid any possibility of conflicts. (If simultaneous rearrange animations are desired, use a single
        ///         call to AnimateRearrange with a rearrange action callback that does both operations.)
        ///     </para>
        /// </remarks>
        public void AnimateRearrange(Duration animationDuration, Action rearrangeAction)
        {
            if (rearrangeAction == null)
            {
                throw new ArgumentNullException("rearrangeAction");
            }

            if (rearrangeCanvas == null)
            {
                throw new InvalidOperationException("ReorderListBox control template is missing " +
                                                    "a part required for rearrange: " + RearrangeCanvasPart);
            }

            if (rearrangeQueue == null)
            {
                rearrangeQueue = new Queue<KeyValuePair<Action, Duration>>();
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset); // Stop scrolling.
                Dispatcher.BeginInvoke(() =>
                    AnimateRearrangeInternal(rearrangeAction, animationDuration));
            }
            else
            {
                rearrangeQueue.Enqueue(new KeyValuePair<Action, Duration>(rearrangeAction, animationDuration));
            }
        }

        /// <summary>
        ///     Orchestrates the rearrange animation process.
        /// </summary>
        private void AnimateRearrangeInternal(Action rearrangeAction, Duration animationDuration)
        {
            // Find the indices of items in the view. Animations are optimzed to only include what is visible.
            int viewFirstIndex, viewLastIndex;
            GetViewIndexRange(true, out viewFirstIndex, out viewLastIndex);

            // Collect information about items and their positions before any changes are made.
            RearrangeItemInfo[] rearrangeMap = BuildRearrangeMap(viewFirstIndex, viewLastIndex);

            // Call the rearrange action callback which actually makes the changes to the source list.
            // Assuming the source list is properly bound, the base class will pick up the changes.
            rearrangeAction();

            rearrangeCanvas.Visibility = Visibility.Visible;

            // Update the layout (positions of all items) based on the changes that were just made.
            UpdateLayout();

            // Find the NEW last-index in view, which may have changed if the items are not constant heights
            // or if the view includes the end of the list.
            viewLastIndex = FindViewLastIndex(viewFirstIndex);

            // Collect information about the NEW items and their NEW positions, linking up to information
            // about items which existed before.
            RearrangeItemInfo[] rearrangeMap2 = BuildRearrangeMap2(rearrangeMap,
                viewFirstIndex, viewLastIndex);

            // Find all the movements that need to be animated.
            IEnumerable<RearrangeItemInfo> movesWithinView = rearrangeMap
                .Where(rii => !Double.IsNaN(rii.FromY) && !Double.IsNaN(rii.ToY));
            IEnumerable<RearrangeItemInfo> movesOutOfView = rearrangeMap
                .Where(rii => !Double.IsNaN(rii.FromY) && Double.IsNaN(rii.ToY));
            IEnumerable<RearrangeItemInfo> movesInToView = rearrangeMap2
                .Where(rii => Double.IsNaN(rii.FromY) && !Double.IsNaN(rii.ToY));
            IEnumerable<RearrangeItemInfo> visibleMoves =
                movesWithinView.Concat(movesOutOfView).Concat(movesInToView);

            // Set a clip rect so the animations don't go outside the listbox.
            rearrangeCanvas.Clip = new RectangleGeometry {Rect = new Rect(new Point(0, 0), rearrangeCanvas.RenderSize)};

            // Create the animation storyboard.
            Storyboard rearrangeStoryboard = CreateRearrangeStoryboard(visibleMoves, animationDuration);
            if (rearrangeStoryboard.Children.Count > 0)
            {
                // The storyboard uses an overlay canvas with item snapshots.
                // While that is playing, hide the real items.
                scrollViewer.Visibility = Visibility.Collapsed;

                rearrangeStoryboard.Completed += delegate
                {
                    rearrangeStoryboard.Stop();
                    rearrangeCanvas.Children.Clear();
                    rearrangeCanvas.Visibility = Visibility.Collapsed;
                    scrollViewer.Visibility = Visibility.Visible;

                    AnimateNextRearrange();
                };

                Dispatcher.BeginInvoke(() => rearrangeStoryboard.Begin());
            }
            else
            {
                rearrangeCanvas.Visibility = Visibility.Collapsed;
                AnimateNextRearrange();
            }
        }

        /// <summary>
        ///     Checks if there's another rearrange action waiting in the queue, and if so executes it next.
        /// </summary>
        private void AnimateNextRearrange()
        {
            if (rearrangeQueue.Count > 0)
            {
                KeyValuePair<Action, Duration> nextRearrange = rearrangeQueue.Dequeue();
                Dispatcher.BeginInvoke(() =>
                    AnimateRearrangeInternal(nextRearrange.Key, nextRearrange.Value));
            }
            else
            {
                rearrangeQueue = null;
            }
        }

        /// <summary>
        ///     Collects information about items and their positions before any changes are made.
        /// </summary>
        private RearrangeItemInfo[] BuildRearrangeMap(int viewFirstIndex, int viewLastIndex)
        {
            var map = new RearrangeItemInfo[Items.Count];

            for (int i = 0; i < map.Length; i++)
            {
                object item = Items[i];

                var info = new RearrangeItemInfo
                {
                    Item = item,
                    FromIndex = i,
                };

                // The precise item location is only important if it's within the view.
                if (viewFirstIndex <= i && i <= viewLastIndex)
                {
                    var itemContainer = (ReorderListBoxItem)
                        ItemContainerGenerator.ContainerFromIndex(i);
                    if (itemContainer != null)
                    {
                        GeneralTransform itemTransform = itemContainer.TransformToVisual(rearrangeCanvas);
                        Point itemPoint = itemTransform.Transform(new Point(0, 0));
                        info.FromY = itemPoint.Y;
                        info.Height = itemContainer.RenderSize.Height;
                    }
                }

                map[i] = info;
            }

            return map;
        }

        /// <summary>
        ///     Collects information about the NEW items and their NEW positions after changes were made.
        /// </summary>
        private RearrangeItemInfo[] BuildRearrangeMap2(RearrangeItemInfo[] map,
            int viewFirstIndex, int viewLastIndex)
        {
            var map2 = new RearrangeItemInfo[Items.Count];

            for (int i = 0; i < map2.Length; i++)
            {
                object item = Items[i];

                // Try to find the same item in the pre-rearrange info.
                RearrangeItemInfo info = map.FirstOrDefault(rii => rii.ToIndex < 0 && rii.Item == item);
                if (info == null)
                {
                    info = new RearrangeItemInfo
                    {
                        Item = item,
                    };
                }

                info.ToIndex = i;

                // The precise item location is only important if it's within the view.
                if (viewFirstIndex <= i && i <= viewLastIndex)
                {
                    var itemContainer = (ReorderListBoxItem)
                        ItemContainerGenerator.ContainerFromIndex(i);
                    if (itemContainer != null)
                    {
                        GeneralTransform itemTransform = itemContainer.TransformToVisual(rearrangeCanvas);
                        Point itemPoint = itemTransform.Transform(new Point(0, 0));
                        info.ToY = itemPoint.Y;
                        info.Height = itemContainer.RenderSize.Height;
                    }
                }

                map2[i] = info;
            }

            return map2;
        }

        /// <summary>
        ///     Finds the index of the last visible item by starting at the first index and
        ///     comparing the bounds of each following item to the ScrollViewer bounds.
        /// </summary>
        /// <remarks>
        ///     This method is less efficient than the hit-test method used by GetViewIndexRange() above,
        ///     but it works when the controls haven't actually been rendered yet, while the other doesn't.
        /// </remarks>
        private int FindViewLastIndex(int firstIndex)
        {
            int lastIndex = firstIndex;

            GeneralTransform scrollViewerTransform = scrollViewer.TransformToVisual(
                Application.Current.RootVisual);
            Rect scrollViewerRect = scrollViewerTransform.TransformBounds(
                new Rect(new Point(0, 0), scrollViewer.RenderSize));

            while (lastIndex < Items.Count - 1)
            {
                var itemContainer = (ReorderListBoxItem)
                    ItemContainerGenerator.ContainerFromIndex(lastIndex + 1);
                if (itemContainer == null)
                {
                    break;
                }

                GeneralTransform itemTransform = itemContainer.TransformToVisual(
                    Application.Current.RootVisual);
                Rect itemRect = itemTransform.TransformBounds(new Rect(new Point(0, 0), itemContainer.RenderSize));
                itemRect.Intersect(scrollViewerRect);
                if (itemRect == Rect.Empty)
                {
                    break;
                }

                lastIndex++;
            }

            return lastIndex;
        }

        /// <summary>
        ///     Creates a storyboard to animate the visible moves of a rearrange.
        /// </summary>
        private Storyboard CreateRearrangeStoryboard(IEnumerable<RearrangeItemInfo> visibleMoves,
            Duration animationDuration)
        {
            var storyboard = new Storyboard();

            ReorderListBoxItem temporaryItemContainer = null;

            foreach (RearrangeItemInfo move in visibleMoves)
            {
                var itemSize = new Size(rearrangeCanvas.RenderSize.Width, move.Height);

                ReorderListBoxItem itemContainer = null;
                if (move.ToIndex >= 0)
                {
                    itemContainer = (ReorderListBoxItem) ItemContainerGenerator.ContainerFromIndex(move.ToIndex);
                }
                if (itemContainer == null)
                {
                    if (temporaryItemContainer == null)
                    {
                        temporaryItemContainer = new ReorderListBoxItem();
                    }

                    itemContainer = temporaryItemContainer;
                    itemContainer.Width = itemSize.Width;
                    itemContainer.Height = itemSize.Height;
                    rearrangeCanvas.Children.Add(itemContainer);
                    PrepareContainerForItemOverride(itemContainer, move.Item);
                    itemContainer.UpdateLayout();
                }

                var itemSnapshot = new WriteableBitmap((int) itemSize.Width, (int) itemSize.Height);
                itemSnapshot.Render(itemContainer, null);
                itemSnapshot.Invalidate();

                var itemImage = new Image();
                itemImage.Width = itemSize.Width;
                itemImage.Height = itemSize.Height;
                itemImage.Source = itemSnapshot;
                itemImage.RenderTransform = new TranslateTransform();
                rearrangeCanvas.Children.Add(itemImage);

                if (itemContainer == temporaryItemContainer)
                {
                    rearrangeCanvas.Children.Remove(itemContainer);
                }

                if (!Double.IsNaN(move.FromY) && !Double.IsNaN(move.ToY))
                {
                    Canvas.SetTop(itemImage, move.FromY);
                    if (move.FromY != move.ToY)
                    {
                        var moveAnimation = new DoubleAnimation();
                        moveAnimation.Duration = animationDuration;
                        Storyboard.SetTarget(moveAnimation, itemImage.RenderTransform);
                        Storyboard.SetTargetProperty(moveAnimation, new PropertyPath(TranslateTransform.YProperty));
                        moveAnimation.To = move.ToY - move.FromY;
                        storyboard.Children.Add(moveAnimation);
                    }
                }
                else if (Double.IsNaN(move.FromY) != Double.IsNaN(move.ToY))
                {
                    if (move.FromIndex >= 0 && move.ToIndex >= 0)
                    {
                        var moveAnimation = new DoubleAnimation();
                        moveAnimation.Duration = animationDuration;
                        Storyboard.SetTarget(moveAnimation, itemImage.RenderTransform);
                        Storyboard.SetTargetProperty(moveAnimation, new PropertyPath(TranslateTransform.YProperty));

                        const double animationDistance = 200;
                        if (!Double.IsNaN(move.FromY))
                        {
                            Canvas.SetTop(itemImage, move.FromY);
                            if (move.FromIndex < move.ToIndex)
                            {
                                moveAnimation.To = animationDistance;
                            }
                            else if (move.FromIndex > move.ToIndex)
                            {
                                moveAnimation.To = -animationDistance;
                            }
                        }
                        else
                        {
                            Canvas.SetTop(itemImage, move.ToY);
                            if (move.FromIndex < move.ToIndex)
                            {
                                moveAnimation.From = -animationDistance;
                            }
                            else if (move.FromIndex > move.ToIndex)
                            {
                                moveAnimation.From = animationDistance;
                            }
                        }

                        storyboard.Children.Add(moveAnimation);
                    }

                    var fadeAnimation = new DoubleAnimation();
                    fadeAnimation.Duration = animationDuration;
                    Storyboard.SetTarget(fadeAnimation, itemImage);
                    Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(OpacityProperty));

                    if (Double.IsNaN(move.FromY))
                    {
                        itemImage.Opacity = 0.0;
                        fadeAnimation.To = 1.0;
                        Canvas.SetTop(itemImage, move.ToY);
                    }
                    else
                    {
                        itemImage.Opacity = 1.0;
                        fadeAnimation.To = 0.0;
                        Canvas.SetTop(itemImage, move.FromY);
                    }

                    storyboard.Children.Add(fadeAnimation);
                }
            }

            return storyboard;
        }

        /// <summary>
        ///     Private helper class for keeping track of each item involved in a rearrange.
        /// </summary>
        private class RearrangeItemInfo
        {
            public int FromIndex = -1;
            public double FromY = Double.NaN;
            public double Height = Double.NaN;
            public object Item;
            public int ToIndex = -1;
            public double ToY = Double.NaN;
        }

        #endregion

        #region Private utility methods

        /// <summary>
        ///     Gets host coordinates, adjusting for orientation. This is helpful when identifying what
        ///     controls are under a point.
        /// </summary>
        private static Point GetHostCoordinates(Point point)
        {
            var frame = (PhoneApplicationFrame) Application.Current.RootVisual;
            switch (frame.Orientation)
            {
                case PageOrientation.LandscapeLeft:
                    return new Point(frame.RenderSize.Width - point.Y, point.X);
                case PageOrientation.LandscapeRight:
                    return new Point(point.Y, frame.RenderSize.Height - point.X);
                default:
                    return point;
            }
        }

        #endregion
    }
}