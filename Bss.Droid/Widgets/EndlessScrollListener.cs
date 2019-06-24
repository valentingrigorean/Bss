using Android.Support.V7.Widget;

namespace Bss.Droid.Widgets
{
    public abstract class EndlessScrollListener : RecyclerView.OnScrollListener
    {
        private readonly RecyclerView.LayoutManager _layoutManager;
        // The minimum amount of items to have below your current scroll position
        // before loading more.
        private readonly int _visibleThreshold;
        // Sets the starting page index
        private int _startingPageIndex;
        // The current offset index of data you have loaded
        private int _currentPage;
        // The total number of items in the dataset after the last load
        private int _previousTotalItemCount = 0;
        // True if we are still waiting for the last set of data to load.
        private bool _loading;

        public EndlessScrollListener(RecyclerView.LayoutManager layoutManager) : this(layoutManager, 5, 0)
        {

        }

        public EndlessScrollListener(RecyclerView.LayoutManager layoutManager, int visibleThreshold) : this(layoutManager, visibleThreshold, 0)
        {

        }

        public EndlessScrollListener(RecyclerView.LayoutManager layoutManager, int visibleThreshold, int startPage)
        {
            _layoutManager = layoutManager;
            if (layoutManager is GridLayoutManager)
                _visibleThreshold = visibleThreshold * ((GridLayoutManager)layoutManager).SpanCount;
            else if (layoutManager is StaggeredGridLayoutManager)
                _visibleThreshold = ((StaggeredGridLayoutManager)layoutManager).SpanCount;
            else
                _visibleThreshold = visibleThreshold;
            _startingPageIndex = startPage;
        }

        public int GetLastVisibleItem(int[] lastVisibleItemPositions)
        {
            int maxSize = 0;
            for (int i = 0; i < lastVisibleItemPositions.Length; i++)
            {
                if (i == 0)
                {
                    maxSize = lastVisibleItemPositions[i];
                }
                else if (lastVisibleItemPositions[i] > maxSize)
                {
                    maxSize = lastVisibleItemPositions[i];
                }
            }
            return maxSize;
        }

        // Call this method whenever performing new searches
        public void ResetState()
        {
            _currentPage = _startingPageIndex;
            _previousTotalItemCount = 0;
            _loading = true;
        }

        // This happens many times a second during a scroll, so be wary of the code you place here.
        // We are given a few useful parameters to help us work out if we need to load some more data,
        // but first we check if we are waiting for the previous load to finish.
        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {

            int lastVisibleItemPosition = 0;
            int totalItemCount = _layoutManager.ItemCount;

            if (_layoutManager is StaggeredGridLayoutManager)
            {
                int[] lastVisibleItemPositions = ((StaggeredGridLayoutManager)_layoutManager).FindLastVisibleItemPositions(null);
                // get maximum element within the list
                lastVisibleItemPosition = GetLastVisibleItem(lastVisibleItemPositions);
            }
            else if (_layoutManager is GridLayoutManager)
            {
                lastVisibleItemPosition = ((GridLayoutManager)_layoutManager).FindLastVisibleItemPosition();
            }
            else if (_layoutManager is LinearLayoutManager)
            {
                lastVisibleItemPosition = ((LinearLayoutManager)_layoutManager).FindLastVisibleItemPosition();
            }

            // If the total item count is zero and the previous isn't, assume the
            // list is invalidated and should be reset back to initial state
            if (totalItemCount < _previousTotalItemCount)
            {
                _currentPage = _startingPageIndex;
                _previousTotalItemCount = totalItemCount;
                if (totalItemCount == 0)
                {
                    _loading = true;
                }
            }
            // If it’s still loading, we check to see if the dataset count has
            // changed, if so we conclude it has finished loading and update the current page
            // number and total item count.
            if (_loading && (totalItemCount > _previousTotalItemCount))
            {
                _loading = false;
                _previousTotalItemCount = totalItemCount;
            }

            // If it isn’t currently loading, we check to see if we have breached
            // the visibleThreshold and need to reload more data.
            // If we do need to reload some more data, we execute onLoadMore to fetch the data.
            // threshold should reflect how many total columns there are too
            if (!_loading && (lastVisibleItemPosition + _visibleThreshold) > totalItemCount)
            {
                _currentPage++;
                OnLoadMore(_currentPage, totalItemCount, recyclerView);
                _loading = true;
            }
        }

        public abstract void OnLoadMore(int page, int totalItemsCount, RecyclerView recyclerView);
    }
}

