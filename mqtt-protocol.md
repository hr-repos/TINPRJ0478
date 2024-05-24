# verkeerslichten

- `vkl/1/{commando}`
- `vkl/2/{commando}`

### `{commando}`

- `verander` (vanuit dashboard)
- `terugkoppeling` (vanuit ESP32)

### :bericht

- `0` (uit)
- `1` (alleen rood aan)
- `2` (alleen geel aan)
- `3` (alleen groen aan)
- `4` (alleen geel knipperen)

# afsluitbomen

- `asb/1/{commando}`
- `asb/2/{commando}`

### `{commando}`

- `verander` (vanuit dashboard)
  - :bericht
    - 0 openen
    - 1 normaal sluiten
    - 2 geforceerd sluiten
    - 3 reset noodstop
- `terugkoppeling` (vanuit ESP32)
  - :bericht
    - 0 wordt geopend
    - 1 wordt gesloten
    - 2 is geopend
    - 3 is normaal gesloten
    - 4 is geforceerd gesloten
    - 5 normaal sluiten mislukt (object detected)
    - 6 reset uitgevoerd
    - 7 bericht niet herkend

# globaal

- `reset`
