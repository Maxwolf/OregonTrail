# Wolf Curses

Provides an abstraction of one or more windows that maps onto the console. Each window is represented by a character matrix. The programmer sets up each window to look as they want the display to look, and then tells wolf curses to update the screen. The library determines a minimal set of changes needed to update the display and then executes these using the terminal's specific capabilities and control sequences.

In short, this means that the programmer simply creates a character matrix of how the screen should look and lets wolf curses handle the work.

## Compilation Instructions ##

You *should* be able to run the Cake build script by invoking the bootstrapper.

### Windows ###

```powershell
./build.ps1
```

If script execution fail due to the execution policy, you might have to
tell PowerShell to allow running scripts. You do this by
[changing the execution policy](https://technet.microsoft.com/en-us/library/ee176961.aspx).

### Linux/OS X ###

To be able to execute the bash script on Linux or OS X you should
give the owner of the script permission to execute it.

```bash
chmod +x build.sh
```

When this have been done, you should be able to run your Cake script
by invoking the bootstrapper.

```bash
./build.sh
```

## Features ##

### Stuff ###
 * Things
 * Stuff with things

### Things ###
 * Stuff
 * Things with stuff 

## Purpose ##

The purpose of this project was to replicate the concept of the curses library created by Ken Arnold and originally released with BSD UNIX, where it was used for several games, most notably [Rogue](https://en.wikipedia.org/wiki/Rogue_(video_game) "Rogue (video game)").

## Curses-based software ##

The original curses project was designed to facilitate GUI-like functionality on a text-only device, such as a PC running in console mode, a hardware ANSI terminal, a [Telnet](https://en.wikipedia.org/wiki/Telnet "Telnet") or [SSH](https://en.wikipedia.org/wiki/Secure_Shell "Secure Shell") client, or similar.

Curses-based programs often have a [user interface](https://en.wikipedia.org/wiki/User_interface "User interface") that resembles a traditional graphical user interface, including '[widgets](https://en.wikipedia.org/wiki/Widget_(computing) "Widget (GUI)")' such as text boxes and scrollable lists, rather than the [command line interface](https://en.wikipedia.org/wiki/Command-line_interface "Command-line interface") (CLI) most commonly found on text-only devices. This can make them more user-friendly than a CLI-based program, while still being able to run on text-only devices. Curses-based software can also have a lighter resource footprint and operate on a wider range of systems (both in terms of hardware and software) than their GUI-based counterparts. This includes old pre-1990 machines along with modern embedded systems using text-only displays.

However, not all Curses-based software employs a [text user interface](https://en.wikipedia.org/wiki/Text-based_user_interface "Text-based user interface") which resembles a graphical user interface. One counterexample would be the popular vi text editor, which while not being CLI-based, uses memorized keyboard commands almost exclusively, rather than the prompting TUI/GUI style, which relies more on recognition than recall.

Curses is most commonly associated with Unix-like operating systems, although implementations for Microsoft Windows also exist.

## History ##

There are several other projects that have come after curses such as pcurses, PDCurses, and more recently ncurses which is used by most Linux text-mode installers to this day (2016 time of writing).

I am not affiliated with these other projects at all. I wanted to re-imagine these libraries for modern object-oriented languages without using a wrapper.