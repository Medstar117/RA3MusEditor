# 红警3 Mus 文件修改器

红警3的音乐看上去是保存在两个文件里的：
`track-mem.mus`（`data\static\cdata\a70bc1d6.9f12cf33.240021e2.dca9eaab.cdata`）
`track.mus`（`data\static\cdata\a70bc1d6.9f12cf33.2d97c0ca.6d02956b.cdata`）

游戏通过文件`pc.mpf`控制PathMusic音乐播放。mpf文件的结构尚不清楚，因此目前只能修改mus文件。

此外附上了Jonwil对Mus文件的研究（主要是`pathmusic.txt`、`mus.cpp`和`mus.exe`），以及Ben Moench的EALayer3解码器（EALayer3是一种类似MP3的编码，EA使用这种编码来储存音乐）可供参考使用。

## 理论上的使用方法：
1. 用`Mus.Main.exe`打开任意.mus文件，它会自动把mus文件解析为XML，并提取出所有的MP3文件
    ```
    Mus.Main track.mus
    ```

2. ~~【把MP3文件替换成你自己想要使用的音乐后，使用`Mus.Main.exe`打开XML文件，它会自动生成新的mus文件并把所有的MP3文件加进去】~~ ←实际上不一定有用，见下文
    ```
    Mus.Main track.xml
    ```

本工具（`Mus.Main.exe`）会调用Ben Moench的EALayer3解码/编码器来提取mus文件中的音乐、以及将mus文件里的音乐替换成自定义音乐。但是，可能是由于我调用编码器的姿势不对，生成的音乐并不能被游戏识别（甚至可能导致游戏崩溃），因此实际上只能使用游戏自带的音乐来进行替换，比如把某个EALayer3文件替换成另一个EALayer3文件。
为了防止这个工具自动把MP3编码成（并不能被游戏识别的）EALayer3文件，你需要在cache.xml里把相应的EALayer3以及MP3的`ForceCache`选项设为`true`，这样`Mus.Main.exe`就会使用现成的EALayer3文件，而不是重新从MP3开始编码。

EALayer3文件的大小必须与XML里的DataSize相吻合，因此你还需要手动修改XML里相应的DataSize，否则工具将会报错。

~~【修改过的mus文件可以像示例里的文件pc_pathmusicassets.xml那样，引用到你的Mod里。】~~ ←由于未知原因这没有用，目前你必须手动替换StaticStream里面的cdata。

相关链接：
- EALayer3解码编码器（mus文件里的音乐使用EALayer3编码） https://forum.xentax.com/viewtopic.php?t=4922
- EALayer3格式（已知mus文件里的音乐是以“Headerless Streamed Chunk”格式存储的）：https://wiki.multimedia.cx/index.php/EA_SAGE_Audio_Files
- 关于RA3 PathMusic的一些信息：https://forum.xentax.com/viewtopic.php?t=14927
- 关于MPF格式：https://forum.xentax.com/viewtopic.php?t=9841

有任何问题也欢迎在红警3吧（https://tieba.baidu.com/ra3）发帖
