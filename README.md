# Markdown Support for Unity TMP Components

This package adds the MarkdownFormatter component. Add this to any game objects that you you want to have a TMP component with Markdown Support

# Features
The parser is based of the [Markdown Cheat Sheet](https://www.markdownguide.org/cheat-sheet/) and supports most of it

## Basic Syntax
### Almost All Basic Syntax from the Cheat Sheet is included, that means:
* Headers 1 to 6
* Bold and Italic Text with \* and \_ for openers and closers
* Quotes with \>
* Ordered List with \* or \- to enumerate items
* Code lines and code blocks with \` and \`\`\` respectively
* Links and Images



## Extended Syntax
### Extended Synxtax has less supported features. Support Includes:
* Strikethrough Text with \~\~
* Highlighted Text with \=\=
* Subscript and Superscript with \~ and \^ respectivelly

## Unique Syntax
* Underline Text with \~\~\~
* For images you can use \!\[image\] without fall back text (Not recommended though)

## Unsupported Markdown
* Horizontal Rules are not possible in TMP and therefore not supported
* Ordered Lists do not receive any additional formatting and will just be plain text
* Tables
* Footnotes
* Heading IDs (not needed in Unity)
* Definition List
* Task List
* Emojis (Unless implemented as Images)

## Requirements
Your project should already have the 2D Sprites and Unity UI packages
You should already have install TMP Essentials

## Installing
Just download the .unitypackage and import it to your project. Import all the files it asks for. Then add a MarkdownFormatter component to a game object that has TMP components on it

## Configuration
You can currently configure the following settings:
* ***Auto Convert On Change***: If true the parser will execute the frame after the TMP text was changed from its last value. If false you will need to call MarkdownFormatter.ConvertToTMPCompatibleText() from the component instance manually
* ***Code Background Color***: The background color when in a code block. Default is #39312C
* ***Highlighted Text Background Color***: The background color when text is highlighted. Default is #FFFF00AA

Created by kstelakis  
For more info and example see the [wiki](https://github.com/Stelios-Kourlis/MD-For-TMP/wiki)
