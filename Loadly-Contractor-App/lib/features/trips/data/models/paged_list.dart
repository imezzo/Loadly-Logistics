class PagedList<T> {
  const PagedList({required this.items, required this.totalCount});

  final List<T> items;
  final int totalCount;
}
