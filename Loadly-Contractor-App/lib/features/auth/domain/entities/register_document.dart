class RegisterDocument {
  final int documentId;
  final String filePath;
  final DateTime expiryDate;

  const RegisterDocument({
    required this.documentId,
    required this.filePath,
    required this.expiryDate,
  });
}
