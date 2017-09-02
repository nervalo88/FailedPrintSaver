# FailedPrintSaver
When a print fails, expecially a long one (i.e out of filament, or communication issue), 
I usually edit the gcode to restart it from the layer it failed.

This software build a new gCode file, starting from the specified Z, it just needs:
- the failed gCode
- the Z height reached before failure.

You can modify the start.gcode file to include you own gcode (added to the beginning of the generated file).

Works fine from cura Gcode, for my prusa.
