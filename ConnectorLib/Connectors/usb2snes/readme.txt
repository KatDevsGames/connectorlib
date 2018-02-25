This is an alpha version of usb2snes using a modified version of the sd2snes FW 0.1.7e.

*** PLEASE BACKUP YOUR SD CARD BEFORE TRYING THIS ***
*** PLEASE BACKUP YOUR SD CARD BEFORE TRYING THIS ***
*** PLEASE BACKUP YOUR SD CARD BEFORE TRYING THIS ***

============================================================================
SETUP
============================================================================

[1] SD2SNES 0.1.7e-usb-v<N> USB Firmware Setup
----------------------------------------------
a) Copy the contents of the sd2snes/ folder to your SD card's sd2snes/ folder.  You will overwrite some of these files:
- firmware.img
- fpga_base.bit, fpga_*.bit, etc.
b) Boot up the sd2snes with the SD card installed.
c) Check the firmware version ([X]->System Information [A]->Firmware version).  It should read 0.1.7e-usb-v<NUMBER>.

If you have any problems you should try the unmodified 0.1.7e FW available at: https://sd2snes.de/blog/downloads.  It doesn't have USB support, but testing it will help eliminate the USB changes as a cause of the problem.

[2] Windows setup
-----------------
Plug in the sd2snes usb port to your windows machine
a) Install driver
   - If windows7, update your driver in Device Manager to the driver in the subdirectory: win7_driver/.
   - If windows8, it may work without installing a driver.  If it doesn't, you will need to disable signed driving checks and install the driver in win7_driver.  See: https://learn.sparkfun.com/tutorials/disabling-driver-signature-on-windows-8/disabling-signed-driver-enforcement-on-windows-8
   - If windows10, the correct driver should already be included with windows.
b) Check if the sd2snes is recognized.
   - In Device Manager check 'Ports (COM & LPT)' and look for your sd2snes.
     - In win7 it should say sd2snes.
     - In win10 it may have a generic name.
     - If there is a ! symbol over it there is a problem with the driver or the firmware has not been properly copied to the SD card (see step [1]).

You should hear the usb connection sound if it's properly connected and recognized.
- Make sure there are no unrecognized/errored devices in 'Universal Serial Bus controllers' section.
- If there are, make sure you install the driver if you're on win7.

WHEN UPDATING A PRIOR VERSION OF USB2SNES
-----------------------------------------
- Always make sure to copy the contents of sd2snes/ subdirectory from the usb2snes zip to your SD card's sd2snes/ directory.  Often time changes in both the firmware (firmware.img) and fpga code (fpga_*.bit) are required for the latest usb2snes to work.
- If you copy any executables in the usb2snes/ directory to other locations you also copy DLLs and other files from the same directory or it may not work.



********************************************************************************************
^^^^^^^ Everything above only needs to be done once for each new version of USB2SNES. ^^^^^^
********************************************************************************************

********************************************************************************************
If you are using an application designed to connect to SD2SNES via USB (e.g. older versions of multitroid) you can probably stop here _unless_ that application also requires the tray app or something called websockets.
********************************************************************************************



============================================================================
USB2SNES TRAY APP
============================================================================
[3] Try the usb2snes tray app.
The application opens as a tool tray app in the windows tool tray.  Look for a green circle with white lines inside it.
- Make sure your snes is on, the USB port is connected to your PC, and all the steps under the SETUP section are complete.
- Click on the green circle to open a menu.
  - You should see a sd2snes section that opens up and displays COM#.  If it doesn't, your SD2SNES is not properly connected.  See the SETUP section.
- Highlight "Clients" to get a list of applications available.
  - The FileViewer allows access to the SD card and a few select menu options to control the SD2SNES.
  - The InputViewer is a NintendoSpy-based button viewer that works for a small set of games.
  - The MemoryViewer is a debugging tool to view the current SNES context (ROM, WRAM, OAM, CGRAM, etc).
  - The ZeldaHUD is a HUD with autotracking support.

In the FileViewer:
- upload lets you upload roms to the current directory.
- boot lets you start up a snes rom.  some roms (msu?) don't seem to work, yet.  (double click a ROM file in right window to boot it)
- patch lets you apply a IPS patch to a running game.
- get/set state will save or load the save state RAM to your computer.  Make sure the appropriate game is running and the save state patch is applied.  It does not currently support automatically restoring the state in the game.  You'll have to use the load state button combination to do that after setting the state.  Getting state is only valid if you have saved the state at least once in the game.
- other features are available: download, delete, rename, etc.  Right-click in the right window to see options.

The program should work both while in the sd2snes menu or in a game.

In the memory viewer:
- Change the memory space with the upper left.
- The range space supports a start/offset (see boxes in the lower left) on the entire contents of the 16MB RAM.
- It's now possible to modify the WRAM, VRAM, and a few other regions one nibble at a time.

============================================================================
PATCHES
============================================================================
Patches can be applied to running games.  It takes a IPS patch formatted file and directly writes the 16MB region in SD2SNES RAM.  At game start the sd2snes copies the game ROM to its 16MB RAM.  It then provides a way (address mapper) for accessing the 16MB (24b) space.  SNES game save state (SRAM) is stored at offset $E00000.  The menu uses some of the upper regions in memory, too.  There is currently no way to halt a running game so patching the ROM region directly may result in crashes.  There is more work in order to make a general patching service to apply on running games.

SAVE STATE PATCH
----------------
The save state patch applies a non-game specific IPS patch to an unused region of the 16MB SD2SNES RAM to add support for saving and loading.  It then patches the cheat section of the snescmd memory region in order to call the new code on every frame (typically).  The default controls are:

Select+R = Save
Select+L = Load

Exceptions have been made for the following games:
SMW, DKC[1-3] - X+R, X+L
Super Metroid - Select+Y+R, Select+Y+L

Save state issues:
- Most games are untested and likely to have bugs.  Some untested games will lock immediately due to audio processor desync with cached state in WRAM.  Several games (mostly Capcom) are patched to work around this.
- There are still races in the code that can cause lockups and other things.  It's better not to load/save repeatedly in cutscenes/menus.
- Some games either don't use NMIs (Mystic Quest) or don't seem to be triggering the button code as expected (Final Fight 2) and won't save/load.
- Try the US version if the JP or other version doesn't work properly.  I've tried to include fixes for common US and JP versions of games that require them, but there are many that are still broken.

============================================================================
KNOWN BUGS
============================================================================
- Save state freezes.
- Multiple apps can now be used at the same time, however, there are some cases where apps will stall and the whole usb2snes app may have to be killed/restarted along with the snes.

============================================================================
THANKS
============================================================================
Thanks to ikari for making the sd2snes.
Thanks to saturnu for the usb firmware source.  A lot of it has been rewritten, but the CDC/block transfer code still remains and other changes used the original code as reference.
Thanks to total for help with DKC save states and for the SM save state code which the IPS patch was based on.
Thanks to oste_hovel for help and developing multitroid.
Thanks to wildanaconda69 for help with testing and other feedback.
