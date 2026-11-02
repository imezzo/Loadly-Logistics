import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../l10n/gen/app_localizations.dart';

class FavoriteListSection extends StatelessWidget {
  const FavoriteListSection({
    super.key,
    required this.requestedCount,
    required this.bookedCount,
    required this.onRequestedPreview,
    required this.onBookedPreview,
  });

  final int requestedCount;
  final int bookedCount;
  final VoidCallback onRequestedPreview;
  final VoidCallback onBookedPreview;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.favoriteList,
          style: const TextStyle(
            fontSize: 16,
            fontWeight: FontWeight.bold,
            color: AppColors.darkGray,
          ),
        ),
        const SizedBox(height: 10),
        _FavoriteListCard(
          title: l10n.requestedTransfersCount(requestedCount.toString()),
          iconAsset: 'assets/images/requests.svg',
          showBadge: requestedCount > 0,
          emphasized: true,
          onPreview: onRequestedPreview,
        ),
        const SizedBox(height: 8),
        _FavoriteListCard(
          title: l10n.bookedTransfersCount(bookedCount.toString()),
          iconAsset: 'assets/images/newspaper.svg',
          showBadge: false,
          emphasized: false,
          iconHighlighted: false,
          onPreview: onBookedPreview,
        ),
      ],
    );
  }
}

class _FavoriteListCard extends StatelessWidget {
  const _FavoriteListCard({
    required this.title,
    required this.iconAsset,
    required this.showBadge,
    required this.emphasized,
    this.iconHighlighted = false,
    required this.onPreview,
  });

  final String title;
  final String iconAsset;
  final bool showBadge;
  final bool emphasized;
  final bool iconHighlighted;
  final VoidCallback onPreview;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.lightGray),
      ),
      child: Row(
        children: [
          _IconBadge(
            iconAsset: iconAsset,
            showBadge: showBadge,
            highlighted: emphasized || iconHighlighted,
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Text(
              title,
              textAlign: TextAlign.start,
              style: TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w600,
                color: emphasized ? AppColors.mainBlue : AppColors.darkGray,
              ),
            ),
          ),
          const SizedBox(width: 10),
          _PreviewButton(label: l10n.preview, onTap: onPreview),
        ],
      ),
    );
  }
}

class _PreviewButton extends StatelessWidget {
  const _PreviewButton({required this.label, required this.onTap});

  final String label;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
        decoration: BoxDecoration(
          color: AppColors.white,
          borderRadius: BorderRadius.circular(20),
          border: Border.all(color: AppColors.mainBlue),
        ),
        child: Text(
          label,
          style: const TextStyle(
            fontSize: 13,
            fontWeight: FontWeight.w600,
            color: AppColors.mainBlue,
          ),
        ),
      ),
    );
  }
}

class _IconBadge extends StatelessWidget {
  const _IconBadge({
    required this.iconAsset,
    required this.showBadge,
    required this.highlighted,
  });

  final String iconAsset;
  final bool showBadge;
  final bool highlighted;

  @override
  Widget build(BuildContext context) {
    final iconColor = AppColors.mainBlue;
    final backgroundColor = highlighted
        ? const Color(0xFFEFF7FF)
        : const Color(0xFFF2F4F6);

    return SizedBox(
      width: 40,
      height: 40,
      child: Stack(
        clipBehavior: Clip.none,
        children: [
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              color: backgroundColor,
              borderRadius: BorderRadius.circular(10),
              border: highlighted
                  ? Border.all(color: AppColors.mainBlue, width: 1)
                  : null,
            ),
            child: Padding(
              padding: const EdgeInsets.all(7),
              child: SvgPicture.asset(
                iconAsset,
                fit: BoxFit.contain,
                colorFilter: ColorFilter.mode(iconColor, BlendMode.srcIn),
              ),
            ),
          ),
          if (showBadge)
            const Positioned(
              top: -2,
              right: -2,
              child: DecoratedBox(
                decoration: BoxDecoration(
                  color: AppColors.mainBlue,
                  shape: BoxShape.circle,
                ),
                child: SizedBox(width: 8, height: 8),
              ),
            ),
        ],
      ),
    );
  }
}
