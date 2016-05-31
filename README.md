# PDFInspector

Programming Language: C#

IDE: Visual Studio 2015

Platform: Windows with .Net Framework 4.0


Description:

An application designed to find the broken PDF files.  User need to enter or select a path to start a check query. The program will check all the recursive sub folders under that path. A DFS traversal is used here since the depth of the file system architecture is much smaller compared with the width of some levels.  The program uses a third-party DLL named ITEXT to quickly check a PDF without opening it.
