import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';

import '../theme/app_colors.dart';

/// Bottom page-number navigation (groups of 5 with prev/next chevrons).
class PaginationBar extends StatelessWidget {
  const PaginationBar({
    super.key,
    required this.currentPage,
    required this.totalCount,
    required this.pageSize,
    required this.onPageSelected,
  });

  final int currentPage;
  final int totalCount;
  final int pageSize;
  final ValueChanged<int> onPageSelected;

  static const int _pagesPerGroup = 5;

  @override
  Widget build(BuildContext context) {
    if (pageSize <= 0 || totalCount <= 0) {
      return const SizedBox.shrink();
    }

    final calculatedPages = (totalCount / pageSize).ceil();
    if (calculatedPages <= 1) {
      return const SizedBox.shrink();
    }

    final totalPages = calculatedPages;

    final groupStart =
        ((currentPage - 1) ~/ _pagesPerGroup) * _pagesPerGroup + 1;

    final proposedEnd = groupStart + _pagesPerGroup - 1;
    final groupEnd = proposedEnd > totalPages ? totalPages : proposedEnd;

    return Padding(
      padding: const EdgeInsets.only(top: 20, bottom: 10),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          IconButton(
            onPressed: groupStart > 1
                ? () => onPageSelected(groupStart - 1)
                : null,
            icon: const Icon(Icons.chevron_left),
          ),
          for (int page = groupStart; page <= groupEnd; page++)
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 2),
              child: Container(
                width: 36,
                height: 36,
                decoration: BoxDecoration(
                  border: Border.all(color: AppColors.mainBlue),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: CupertinoButton(
                  padding: EdgeInsets.zero,
                  color: page == currentPage
                      ? AppColors.mainBlue
                      : AppColors.white,
                  borderRadius: BorderRadius.circular(8),
                  onPressed: () {
                    if (page != currentPage) {
                      onPageSelected(page);
                    }
                  },
                  child: Text(
                    '$page',
                    style: TextStyle(
                      color: page == currentPage
                          ? AppColors.white
                          : AppColors.mainBlue,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
              ),
            ),
          IconButton(
            onPressed: groupEnd < totalPages
                ? () => onPageSelected(groupEnd + 1)
                : null,
            icon: const Icon(Icons.chevron_right),
          ),
        ],
      ),
    );
  }
}
