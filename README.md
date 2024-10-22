# GenericPlaybook
Messing with the Idea to have a book as a game, transfering it to a computer environment
## Idea
Inspired on the Books by Jackson, Steve / Livingstone, Ian, like the "Duell der Piraten" I wantto trans fer it to a computer enwirionment. As far as I know this is not the first time it got transfered. Motliely it will not be the best port. With this projects I want train my development skills. The Project will be split into serveral parts:
* The Generic handling, this repositry
* Content of the actual Playbook (second private repositry)
* Non public third party librarys (third private repositry)
## Documentation
can be found in [Documentation](https://s-streulicht.github.io/GenericPlaybook/index.html) it reflects the state of the master
### Manually create a doxyge documantation
Download Doxygen version 1.12; copy to main\Docu folder (same than the Doxfile)
https://www.doxygen.nl/download.html
Download graphviz-2.38; extract to main\Docu folder
https://www.graphviz.org/download/
change doxygen file with:
DOT_PATH               = ./graphvizInstalation/bin
execute createDocu.bat this will call doxygen which itself creates a html documentation.
This documentation will be ziped to docu.zip
