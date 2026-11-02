import { setOptions, importLibrary } from '@googlemaps/js-api-loader'
import mapMarkerTruckSvg from '@/assets/svg/map-marker-truck.svg?raw'
import type { TripTrackItem } from '@/types/admin-dashboard/response/trip/trip'

function buildMarkerPinHtml(
  trackItem: TripTrackItem,
  primaryColor = '#167CF2',
  svgContent?: string,
): string {
  return `
  <div
    id="${trackItem.id}_marker"
    class="relative"
    style="color: ${primaryColor};"
  >
    ${svgContent ?? mapMarkerTruckSvg}
  </div>`
}

export function useGoogleMap(
  mapEl: Ref<HTMLElement | undefined>,
  trackList: Ref<TripTrackItem[]>,
) {
  const selectedPinItem = ref<TripTrackItem | null>(null)
  const mapError = ref<string | null>(null)
  const isMapLoading = ref(false)

  let map: google.maps.Map | null = null
  let infoWindow: google.maps.InfoWindow | null = null
  let markers: google.maps.marker.AdvancedMarkerElement[] = []

  async function initMap(): Promise<void> {
    if (!mapEl.value) return

    isMapLoading.value = true
    mapError.value = null

    try {
      setOptions({
        key: import.meta.env.VITE_GOOGLE_MAPS_API_KEY as string,
        v: 'weekly',
      })

      const { Map: GoogleMap } = await importLibrary('maps')
      await importLibrary('marker')

      map = new GoogleMap(mapEl.value, {
        mapId: 'company-trips-map',
      })

      infoWindow = new google.maps.InfoWindow()

      map.addListener('click', () => {
        selectedPinItem.value = null
      })

      syncMarkers()
    } catch (error) {
      // Differentiates network/cert errors from API key / quota errors
      if (error instanceof Error) {
        const isCertOrNetworkError =
          error.message.includes('could not load') ||
          error.message.includes('ERR_CERT') ||
          error.message.includes('Failed to fetch')

        mapError.value = isCertOrNetworkError
          ? 'Unable to load Google Maps due to a network or certificate issue. Please check your connection or contact your IT administrator.'
          : `Google Maps error: ${error.message}`
      } else {
        mapError.value = 'An unexpected error occurred while loading the map.'
      }

      console.error('[useGoogleMap] initMap failed:', error)
    } finally {
      isMapLoading.value = false
    }
  }

  function syncMarkers(): void {
    if (!map || !infoWindow) return

    for (const marker of markers) {
      marker.map = null
    }
    markers = []

    const bounds = new google.maps.LatLngBounds()
    let hasBounds = false

    for (const item of trackList.value) {
      if (!item.currentLocation?.latitude || !item.currentLocation?.longitude) continue
      const { latitude, longitude } = item.currentLocation

      const position = { lat: latitude, lng: longitude }

      const pin = document.createElement('div')
      pin.innerHTML = buildMarkerPinHtml(item)
      pin.style.cursor = 'pointer'

      const marker = new google.maps.marker.AdvancedMarkerElement({
        map,
        position,
        title: item.vehiclePlateNumber || item.driverName,
        content: pin,
      })

      marker.addListener('gmp-click', () => {
        selectedPinItem.value = item
      })

      markers.push(marker)
      bounds.extend(position)
      hasBounds = true
    }

    if (hasBounds) {
      map.fitBounds(bounds, 60)
    }
  }

  watch(trackList, syncMarkers, { deep: true })

  return {
    initMap,
    selectedPinItem,
    syncMarkers,
    mapError,
    isMapLoading
  }
}
