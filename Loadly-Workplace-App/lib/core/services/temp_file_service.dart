import 'dart:io';

import 'package:flutter/cupertino.dart';
import 'package:path_provider/path_provider.dart';

class TempFileService {
  Future<void> cleanOnStartup() async {
    try {
      final tempDir = await getTemporaryDirectory();
      // Async + recursive + filtered
      await for (final entity in tempDir.list(recursive: true)) {
        if (entity is File && _isImageFile(entity.path)) {
          await entity.delete();
        }
      }
      debugPrint('Cleanup succeded');
    } catch (e) {
      debugPrint('Cleanup failed: $e');
    }
  }

  bool _isImageFile(String path) {
    const extensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp'];
    return extensions.any((ext) => path.toLowerCase().endsWith(ext));
  }
}
// 
// // Call this when app starts
// void main() async {
//   WidgetsFlutterBinding.ensureInitialized();
//   await TempFileManager.cleanupAllTempFiles(); // Clear all temps on app start
//   runApp(MyApp());
// }