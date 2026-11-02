# Trip GPS route sync — API and column codec

This document describes how a client uploads a **compressed GPS trace** for a completed trip. It is **platform-neutral**: the same rules apply to the Loadly Driver app, backend services, admin tools, and any other producer or consumer of this payload.

---

## 1. HTTP API

| Item | Value |
|------|--------|
| **Method** | `POST` |
| **Path** | `/trips/{tripId}/route-sync` |
| **Content-Type** | `application/json` |
| **Auth** | Same as other driver trip APIs (e.g. bearer token). |

`tripId` in the URL must match the trip the trace belongs to.

### 1.1 Request body (top level)

| Field | Type | Description |
|-------|------|-------------|
| `distanceMeters` | **integer** | Total driven distance for the trip, in **meters**, **rounded to the nearest whole meter** (e.g. `1251`, not `1250.5`). This is the trip odometer total, not necessarily the sum of per-point segment distances. |
| `startTime` | string | Trip trace / session start, **UTC**, ISO-8601 (e.g. `2026-10-13T10:00:00.000Z`). |
| `endTime` | string | Trip end, **UTC**, ISO-8601. |
| `gpsPacketCount` | integer | Count of **logical** GPS samples after §2.1 preprocessing (sort + per-second dedupe), **before** upload downsampling. Must satisfy `gpsPacketCount >= route.n`. |
| `route` | object | Column-compressed trace (see §3–§5). |

### 1.2 Example request

```json
{
  "distanceMeters": 1251,
  "startTime": "2026-10-13T10:00:00.000Z",
  "endTime": "2026-10-13T10:15:00.000Z",
  "gpsPacketCount": 3,
  "route": {
    "n": 3,
    "t0": 1728813600,
    "dt": [0, 5, 5],
    "lat": [3001022, 1, 1],
    "lng": [3123500, 1, 1],
    "spd": [0, 45, 5],
    "hdg": [0, 90, 95],
    "acc": [5, 4, 4],
    "dsm": [0, 420, 10]
  }
}
```

---

## 2. Logical GPS point (before encoding)

Each stored sample is a **point** with these fields:

| Field | Type | Units / range | Meaning |
|-------|------|----------------|---------|
| `t` | integer | Unix time, **seconds**, UTC | When the fix was recorded. |
| `lat` | number | WGS-84 degrees | Latitude. |
| `lng` | number | WGS-84 degrees | Longitude. |
| `spd` | number | **km/h** | Speed at this sample. If the device reports m/s, convert: `spd_kmh = spd_mps × 3.6`. |
| `hdg` | integer | `[0, 360)` | Heading clockwise from north. Normalize: `hdg = ((hdg % 360) + 360) % 360`. |
| `acc` | number | **meters** | Horizontal accuracy (e.g. GPS reported accuracy). |
| `dsm` | number | **meters** | **Distance from the previous stored point** along the trace (0 for the first point). Typically great-circle distance between consecutive stored coordinates, not cumulative trip distance. |

### 2.1 Preprocessing (recommended before encode)

1. **Sort** points by `t` ascending.
2. **Deduplicate time**: if multiple points share the same `t` (same Unix second), keep **one** (first wins). This avoids ambiguous `dt` columns.
3. **Reject empty traces**: encoding requires at least one point.

Producers may subsample fixes before storage and again before upload (e.g. cap at 30 points by path distance). After upload downsampling, recompute each encoded point’s `dsm` as the geodesic distance from the **previous kept** point (not from the dense trace). **`gpsPacketCount`** is the dense count (after per-second dedupe); **`route.n`** is the downsampled count on the wire (≤ upload cap).

---

## 3. Constants

| Name | Value | Notes |
|------|--------|--------|
| `LAT_LNG_SCALE` | `100000` (`1e5`) | Multiply degrees by this value and **round to integer** before lat/lng deltas. One unit ≈ 1.1 m at the equator. **If this changes, all clients and decoders must agree on a new version.** |

Internal reference: format version `1` (not sent in JSON today; optional future field `route.v`).

---

## 4. Encoded `route` object

The trace is stored as **parallel integer arrays** (columnar format), length `n`.

| Field | Type | Role |
|-------|------|------|
| `n` | integer | Point count; every array below has length `n`. |
| `t0` | integer | Unix seconds of the **first** point (same as decoded `t` of index `0`). |
| `dt` | int[] | Time deltas (seconds). |
| `lat` | int[] | Latitude deltas (quantized). |
| `lng` | int[] | Longitude deltas (quantized). |
| `spd` | int[] | Speed deltas (km/h, rounded). |
| `hdg` | int[] | **Absolute** heading per point (degrees). |
| `acc` | int[] | **Absolute** accuracy per point (meters, rounded). |
| `dsm` | int[] | **Absolute** segment distance from the previous point (meters, rounded). |

### 4.1 Delta vs absolute columns

| Column | Encoding | Decode |
|--------|----------|--------|
| `dt` | **Delta** | Prefix sum + `t0` → time per point |
| `lat` | **Delta** | Prefix sum → quantized lat; ÷ `LAT_LNG_SCALE` → degrees |
| `lng` | **Delta** | Same as `lat` |
| `spd` | **Delta** | Prefix sum → speed in **km/h** (integer) |
| `hdg` | **Absolute** | Use value as-is (degrees) |
| `acc` | **Absolute** | Use value as-is (meters, integer) |
| `dsm` | **Absolute** | Use value as-is (meters from previous stored point; `0` at index `0`) |

**Delta rule:** For each delta column, the encoder keeps a “previous quantized value” `prev` (initial value `0` for lat/lng/spd; for `dt`, the first row uses `prevT = t0` so `dt[0] = 0`). For each point it computes a current quantized value `q`, stores `column[i] = q - prev`, then sets `prev = q`.

**Important:** For `lat`/`lng`, `prev` tracks **quantized** coordinates, not degrees. The **first** `lat[i]` and `lng[i]` are therefore the full quantized coordinates (because `prev` starts at 0).

---

## 5. Encode algorithm (pseudocode)

```
LAT_LNG_SCALE = 100000

function encodeRoute(points):
  if points is empty: error

  points = sort(points, by t ascending)
  n = length(points)

  allocate dt, lat, lng, spd, hdg, acc, dsm arrays of length n

  prevT = points[0].t
  prevLatQ = 0
  prevLngQ = 0
  prevSpdQ = 0

  for i in 0 .. n-1:
    p = points[i]

    latQ = round(p.lat * LAT_LNG_SCALE)
    lngQ = round(p.lng * LAT_LNG_SCALE)
    spdQ = round(p.spd)          // km/h
    accQ = round(p.acc)          // meters
    dsmQ = round(p.dsm)          // meters, segment from previous point

    dt[i]  = p.t - prevT
    lat[i] = latQ - prevLatQ
    lng[i] = lngQ - prevLngQ
    spd[i] = spdQ - prevSpdQ
    hdg[i] = normalizeHeading(p.hdg)
    acc[i] = accQ                // absolute, not delta
    dsm[i] = dsmQ                // absolute segment meters, not delta

    prevT = p.t
    prevLatQ = latQ
    prevLngQ = lngQ
    prevSpdQ = spdQ

  return {
    n: n,
    t0: points[0].t,
    dt, lat, lng, spd, hdg, acc, dsm
  }
```

---

## 6. Decode algorithm (pseudocode)

```
function decodeRoute(route):
  n = route.n
  assert length(route.dt) == n
  assert length(route.lat) == n
  // ... same for lng, spd, hdg, acc, dsm

  points = []
  t = route.t0
  latQ = 0
  lngQ = 0
  spdQ = 0

  for i in 0 .. n-1:
    t    += route.dt[i]
    latQ += route.lat[i]
    lngQ += route.lng[i]
    spdQ += route.spd[i]

    points.push({
      t: t,
      lat: latQ / LAT_LNG_SCALE,
      lng: lngQ / LAT_LNG_SCALE,
      spd: spdQ,                    // km/h
      hdg: route.hdg[i],
      acc: route.acc[i],            // meters
      dsm: route.dsm[i]             // meters, segment from previous
    })

  return points
```

---

## 7. Worked example (decode check)

Given the example in §1.2:

**Times (`t0` + `dt`):**  
`1728813600`, `1728813605`, `1728813610` (5 s steps).

**Positions (`lat` / `lng` prefix sums):**

- `lat`: 3001022 → 3001023 → 3001024 → **30.01022°, 30.01023°, 30.01024°**
- `lng`: 3123500 → 3123501 → 3123502 → **31.23500°, 31.23501°, 31.23502°**

**Speed (`spd` prefix, km/h):** `0`, `45`, `50`.

**Heading (`hdg`, absolute):** `0°`, `90°`, `95°`.

**Accuracy (`acc`, absolute meters):** `5`, `4`, `4` (third logical point had 3.5 m before round).

**Segment distance (`dsm`, absolute meters):** `0`, `420`, `430` (same values on the wire; no prefix sum).

---

## 8. Reference JavaScript (encode / decode)

Suitable for Node backends or browser admin tools. Adjust only if `LAT_LNG_SCALE` or column rules change.

```javascript
const LAT_LNG_SCALE = 1e5;

function normalizeHeading(deg) {
  let h = deg % 360;
  if (h < 0) h += 360;
  return Math.round(h);
}

function encodeRoute(points) {
  if (!points.length) throw new Error("No points.");

  const seen = new Set();
  const pts = [...points]
    .sort((a, b) => a.t - b.t)
    .filter((p) => !seen.has(p.t) && seen.add(p.t));

  const n = pts.length;
  const dt = [], lat = [], lng = [], spd = [], hdg = [], acc = [], dsm = [];

  let prevT = pts[0].t;
  let prevLatQ = 0, prevLngQ = 0, prevSpdQ = 0;

  for (let i = 0; i < n; i++) {
    const p = pts[i];
    const latQ = Math.round(p.lat * LAT_LNG_SCALE);
    const lngQ = Math.round(p.lng * LAT_LNG_SCALE);
    const spdQ = Math.round(p.spd);
    const accQ = Math.round(p.acc);
    const dsmQ = Math.round(p.dsm);

    dt.push(p.t - prevT);
    lat.push(latQ - prevLatQ);
    lng.push(lngQ - prevLngQ);
    spd.push(spdQ - prevSpdQ);
    hdg.push(normalizeHeading(p.hdg));
    acc.push(accQ);
    dsm.push(dsmQ);

    prevT = p.t;
    prevLatQ = latQ;
    prevLngQ = lngQ;
    prevSpdQ = spdQ;
  }

  return { n, t0: pts[0].t, dt, lat, lng, spd, hdg, acc, dsm };
}

function decodeRoute(route) {
  const n = route.n;
  const out = [];
  let t = route.t0;
  let latQ = 0, lngQ = 0, spdQ = 0;

  for (let i = 0; i < n; i++) {
    t += route.dt[i];
    latQ += route.lat[i];
    lngQ += route.lng[i];
    spdQ += route.spd[i];
    out.push({
      t,
      lat: latQ / LAT_LNG_SCALE,
      lng: lngQ / LAT_LNG_SCALE,
      spd: spdQ,
      hdg: route.hdg[i],
      acc: route.acc[i],
      dsm: route.dsm[i],
    });
  }
  return out;
}
```

**Speed from m/s (producers only):** `spd_kmh = spd_mps * 3.6`.

---

## 9. Validation checklist (backend)

When ingesting `POST /trips/{tripId}/route-sync`:

1. `gpsPacketCount >= route.n` and `route.n` equals the length of every column array in `route`.
2. All seven arrays exist and have length `route.n`.
3. `dt[0] === 0` (convention when using `t0` anchor).
4. Decoded times are strictly non-decreasing if duplicates were stripped at encode time.
5. `distanceMeters` is an integer; compare to your business rules vs. sum of `dsm` or map-matched distance (they may differ slightly from filtered GPS odometer).
6. `startTime` / `endTime` should be consistent with trip lifecycle (allow small skew vs. `t0` and last decoded `t`).

---

## 10. Common pitfalls

| Pitfall | Correct approach |
|---------|------------------|
| Treating `acc`, `hdg`, or `dsm` as deltas | They are **absolute** per index. |
| Using m/s in `spd` | Encode **km/h** (integer round, then delta). |
| Using cm for `acc` | Encode **meters** (integer round, absolute). |
| Storing degrees in `lat`/`lng` arrays | Store **deltas of quantized** `round(deg × 1e5)`. |
| Duplicate Unix seconds | Deduplicate before encode or `dt` becomes ambiguous. |
| `distanceMeters` as float | Send **rounded integer** meters. |

---

## 11. Versioning

- **Codec rules in this document:** version **1** (`LAT_LNG_SCALE = 100000`, column delta/absolute split as in §4.1).
- If the wire format changes, add e.g. `route.v: 2` and document migrations; until then, absence of `v` implies version 1.

---

## 12. Loadly Driver reference

The driver app builds this payload after trip completion and POSTs it to `/trips/{tripId}/route-sync`. The column codec is implemented in the repository as `trip_route_codec` (encode) and `trip_route_sync_body` (HTTP body assembly). Automated tests in `test/core/database/trip_route_codec_test.dart` lock the §7 example.

For API authentication, base URL, and error handling, see the main driver API documentation and `documentation/ARCHITECTURE.md`.
