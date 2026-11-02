import 'dart:io';
import 'package:dio/dio.dart';
import 'package:flutter/cupertino.dart';
import 'package:path_provider/path_provider.dart';

extension ImageUrlListConverter on List<String> {
  // Convert all URLs to files
  Future<List<File>> toFiles() async {
    final Dio dio = Dio();
    final List<File> files = [];

    for (final url in this) {
      try {
        final file = await url.toFile(dio: dio);
        if (file != null) {
          files.add(file);
        }
      } catch (e) {
        debugPrint('Error converting $url: $e');
      }
    }

    return files;
  }
}

extension ImageUrlConverter on String {
  // Convert single URL to file
  Future<File?> toFile({Dio? dio}) async {
    final Dio client = dio ?? Dio();

    try {
      final directory = await getTemporaryDirectory();
      final fileName = split('/').last;
      final filePath = '${directory.path}/$fileName';

      await client.download(this, filePath);

      return File(filePath);
    } catch (e) {
      debugPrint('Error: $e');
      return null;
    }
  }
}
