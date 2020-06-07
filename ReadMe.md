# Red Alert 3 Mus File Modifier

Red Alert 3's music appears to be saved in two files:
- `track-mem.mus`（`data\static\cdata\a70bc1d6.9f12cf33.240021e2.dca9eaab.cdata`）
- `track.mus`（`data\static\cdata\a70bc1d6.9f12cf33.2d97c0ca.6d02956b.cdata`）

The game controls PathMusic audio playback through the file `pc.mpf`. The structure of the .mpf file is not yet clear, so currently only the .mus files can be modified.

In addition, Jonwil's research on Mus files (mainly `pathmusic.txt`, `mus.cpp` and `mus.exe`) and Ben Moench’s EALayer3 decoder (EALayer3 is an MP3-like encoding; EA used this codec to store music) have been included for reference.

## Theoretical Use:
1. Open any .mus file with `Mus.Main.exe`; it will automatically parse the .mus file into XML and extract all MP3 files
    ```
    Mus.Main track.mus
    ```

2. ~~【After replacing the MP3 file with the music you want, use `Mus.Main.exe` to open the XML file; it will automatically generate a new .mus file and add all the MP3 files you added into it】~~ ←Actually not very useful, see below
    ```
    Mus.Main track.xml
    ```

This tool (`Mus.Main.exe`) will call Ben Moench's EALayer3 decoder/encoder to extract the music in the .mus file and replace the extracted music with one's own custom music. However, due to some unknown encoding error, the generated music cannot be recognized by the game (it may even cause the game to crash); with this in mind, it is currently only possible to replace pieces of music with tracks that came with the game.

__TL;DR:__ You can only replace extracted EALayer3 files with other extracted EALayer3 files from the game. The EALayer3 files that you generate from MP3 files using Ben Moench's tool cannot be read by the game.

In order to prevent this tool from automatically encoding MP3 into an EALayer3 file (the faulty process explained above), you need to set the corresponding EALayer3 and MP3 `ForceCache` option to `true` in cache.xml, so that `Mus.Main.exe` will use the ready-made EALayer3 file instead of re-encoding from MP3.

The size of the EALayer3 file __must__ match the DataSize in the XML; you need to manually modify the corresponding DataSize section in the XML, or the tool will report an error.

~~【The modified .mus file can then be referenced in your mod, like how you would with the pc_pathmusicassets.xml file in the example.】~~ ←This is useless for unknown reasons. At present, you must manually replace the cdata in StaticStream.

## Related Links:
- EALayer3 decoder/encoder（music in .mus files are encoded with EALayer3） https://forum.xentax.com/viewtopic.php?t=4922
- EALayer3 format（music in .mus files are known to be stored as “Headerless Streamed Chunks”）：https://wiki.multimedia.cx/index.php/EA_SAGE_Audio_Files
- Some information about RA3's PathMusic：https://forum.xentax.com/viewtopic.php?t=14927
- Information about the .MPF format：https://forum.xentax.com/viewtopic.php?t=9841

If you have any questions, please ask them at this Red Alert 3 post（https://tieba.baidu.com/ra3）
