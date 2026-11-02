class DriverDocumentInput {
  const DriverDocumentInput({
    required this.documentId,
    required this.filePath,
    required this.expiryDate,
    this.status = 0,
  });

  final int documentId;
  final String filePath;
  final DateTime expiryDate;
  final int status;
}
