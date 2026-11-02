import 'package:flutter/cupertino.dart';

import '../theme/app_colors.dart';

class AppCheckbox extends StatelessWidget {
  final String label;
  final bool checked;
  final VoidCallback onTap;
  const AppCheckbox({
    super.key,
    required this.label,
    required this.checked,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Row(
        children: [
          AnimatedContainer(
            duration: const Duration(milliseconds: 150),
            width: 15,
            height: 15,
            decoration: BoxDecoration(
              color: checked ? AppColors.mainBlue : CupertinoColors.white,
              borderRadius: BorderRadius.circular(5),
              border: Border.all(
                color: checked ? AppColors.mainBlue : AppColors.lightGray,
                width: 1,
              ),
            ),
            child: checked
                ? const Icon(
                    CupertinoIcons.checkmark,
                    color: CupertinoColors.white,
                    size: 12,
                  )
                : null,
          ),
          const SizedBox(width: 10),
          Text(
            label,
            style: const TextStyle(fontSize: 15, color: CupertinoColors.black),
          ),
        ],
      ),
    );
  }
}
