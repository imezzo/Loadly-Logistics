import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/core/utils/google_reverse_geocoding.dart';

void main() {
  group('formattedAddressFromReverseGeocodeResponse', () {
    test('drops plus code and postal code from POI result', () {
      final address = formattedAddressFromReverseGeocodeResponse({
        'results': [
          {
            'address_components': [
              {
                'long_name': 'خلف المسجد العمرى',
                'types': ['establishment', 'point_of_interest'],
              },
              {
                'long_name': 'J2Q2+F36',
                'types': ['plus_code'],
              },
              {
                'long_name': 'Batbas',
                'types': ['administrative_area_level_3', 'political'],
              },
              {
                'long_name': 'Shibin el Kom',
                'types': ['administrative_area_level_2', 'political'],
              },
              {
                'long_name': 'Menofia Governorate',
                'types': ['administrative_area_level_1', 'political'],
              },
              {
                'long_name': 'Egypt',
                'types': ['country', 'political'],
              },
              {
                'long_name': '6125372',
                'types': ['postal_code'],
              },
            ],
            'formatted_address':
                'J2Q2+F36 خلف المسجد العمرى، Batbas, Shibin el Kom, Menofia Governorate 6125372, Egypt',
            'types': ['establishment', 'point_of_interest'],
          },
        ],
        'status': 'OK',
      });

      expect(
        address,
        'خلف المسجد العمرى, Batbas, Shibin el Kom, Menofia Governorate',
      );
    });

    test('skips plus_code-only results', () {
      final address = formattedAddressFromReverseGeocodeResponse({
        'results': [
          {
            'formatted_address': 'JXQX+CR Shibin el Kom, Egypt',
            'types': ['plus_code'],
          },
          {
            'address_components': [
              {
                'long_name': 'Tanta El-Bagour',
                'types': ['route'],
              },
              {
                'long_name': 'Batbas',
                'types': ['administrative_area_level_3', 'political'],
              },
              {
                'long_name': 'Egypt',
                'types': ['country', 'political'],
              },
            ],
            'formatted_address':
                'Tanta El-Bagour, Batbas, Shibin el Kom, Menofia Governorate, Egypt',
            'types': ['route'],
          },
        ],
      });

      expect(address, 'Tanta El-Bagour, Batbas');
    });
  });
}
