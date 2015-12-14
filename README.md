unFARC
==================

Utilities for unpacking tool for FARC format on Pokemon mystery dungeon 3DS games. This tool can unpacking FARC with SIR0 FAT 0x4 and 0x5 (and with/out filenames), and also automatically convert portrait graphic, message file and cte Image format to PNG files from FARC archive.

This tools can :
- Unpack FARC file on PMD 3DS game as raw file
- Automatic convert some file into readable format
	- convert message format to *.txt file (with external code handle on json file)
	- convert portrait graphic to *.png file
	- convert *.*cte image format to *.png file 

Special Thanks to :
- [Gericom](https://github.com/Gericom/EveryFileExplorer) for EFE library for handle 3DS texture
- [psy_commando](http://projectpokemon.org/forums/member.php?49403-psy_commando) for PMD Research thread on [pp.org forum](http://projectpokemon.org/forums/)
- [evandixon](http://projectpokemon.org/forums/member.php?182-evandixon) for sky editor and much more
- [json.net](http://www.newtonsoft.com/json) for handle json file
- and other is not mentioned in here.

Tools for build this :
- Microsoft Visual Studio 2015 Comunity Edition Update 1
- .Net Framework 4.0, C# 5.0, it might work with mono

![](http://s17.postimg.org/hfg06g817/614.png) > I hope this tool usefull for you