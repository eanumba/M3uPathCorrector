# M3uPathCorrector
Utility for M3Us playlists that replaces one part of a path with another for all of the files. Useful if your entire music library moves.

## Usage
First, select the .m3u file you want to correct by clicking on the "..." button under "M3U File." The longest file path common to all files will populate under "Source Path."

Edit "Source Path" to match the path were the music files *were* stored, and edit "Destination Path" to match the path were the music is currently stored.

E.g. if all of the files in the playlist were originally stored at "C:/My Music/Storage" but now they are in "C:/Users/Music" set up the following:

* **Source Path:** "C:/My Music/Storage"
* **Destination Path:** "C:/Users/Music"

**Note:** ensure that if you end the source path with a slash ("\" or "/"), that the destination path also ends with a slash.

Press "Fix" and choose a location to store the new .m3u