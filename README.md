# PLRenewer

This repo is a storage of my 8-year-old (at least!) project that I decided to update and adapt to modern smartphones because of reasons.

Previous location (2010-2015): https://code.google.com/archive/p/plrenewer/ (readonly archive)
Previous location (2008-2010): http://forum.ichip.ru (inaccessible now), http://exchip.ru (inaccessible now)
Lol it seems whereever I publish this code, the hosting dies. 

It includes 

- *PLRenewer*, an application to build random playlist from your audio file collection and push it to a device (oldfags, do you remember the hardware MP3 players?). Currently adapted to work over MTP (WPD) protocol which is required by modern smartphones not representing themselves as a flash drive.
- *MTP Support* folder contains Managed C++ wrapper over native Win32 API and C# wrapper over that C++ wrapper so it can be called from VB. Yes I know I could just declare classes and interfaces and `<ComInterface>` and so on... was just curious about IJW/Managed C++. Plus testing app.
- *Tagging library* supporting reading and writing tags of 3 popular audio formats. Plus test app.
- *Licensing library* allowing reading my supercool license format and an application to build such a license.
- *Archiving library* allowing to read my supercool HDZ archive format and an application to build such an archive.
- *Updater application* allowing to auto-update PLRenewer, supporting not just exe<->exe but entire update HDZ with update script inside. Script in turn supports custom syntax.

Just for fun I also plan to fix the updater because it was designed (if this term is applicable to anything written in VB at all) for Google Code.
