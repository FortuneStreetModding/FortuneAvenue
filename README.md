FortuneAvenue
=============

Fortune Street editor.  Continuation of the editor released at <http://www.gamefaqs.com/boards/632973-fortune-street/61369903?page=7>.

<dl>
  <a href="https://github.com/FortuneStreetModding/FortuneAvenue/releases/latest"><img src="https://upload.wikimedia.org/wikipedia/commons/b/bd/Download_Button.svg"/></a>
</dl>

![Forunte Avenue Screenshot](FortuneAvenue.png "Screenshot of FortuneAvenue - A map editor for Fortune Street")

## Changelog

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

### [Unreleased]

### [v8] - 2020-06-05
**Added**
- More textures for square tiles
- Possibility to edit unknown values of square tiles 
- Possibility to edit the lift type value for square tiles

**Changed**
- Prevent door tiles from being autopathed

### v7 - 2019-10-06
**Changed**
- Fix bug which causes new squares not to calculate value and price automatically

### v6 - 2019-07-18
**Added**
- The SquareType 0x2E can now be selected in the Map Editor. This is needed for the ASM hacked main.dol
- The yield of each shop is now automatically calculated and displayed. The formula of yield is:
yield = -0.15 * 0.2 ^ (0.005*value) + 0.2
- When choosing a shop model the default values are now loaded.
- Add additional sanity checks to prevent crashes.

[Unreleased]: https://github.com/FortuneStreetModding/FortuneAvenue/compare/v8...HEAD
[v8]: https://github.com/FortuneStreetModding/FortuneAvenue/compare/v7...v8
