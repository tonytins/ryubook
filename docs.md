# Using RyuBook

RyuBook is an ePub generator tool written in C# that uses Markdown as a source and [Pandoc](https://pandoc.org/) for generation.

## Directory Structure

```
+-- src
|   +-- book.md
|   +-- title.txt
```

The book contents exists in the ``/src`` directory. Both ``book.md`` and ``title.txt`` are required. RyuBook currently can't build multiple Markdown files.

## Usage

```
USAGE:
    ryubook [SUBCOMMAND]
SUBCOMMANDS:
  init       Creates a new project.
  build      Compiles the book as a ePub.
  clean      Removes all books in the /build directory.
  help       Display more information on a specific command.
  version    Display version information.
```


## Build

```
USAGE:
    ryubook build [FLAGS]
FLAGS:
  -t --title [TITLE]
  -f  --format [FORMAT]
  -d  --dir [DIRECTORY]
```

Using the ``build`` command alone will build the book as is and output ``book.ePub``. You can also build formats other then the default ``ePub`` by passing `-f` or ``--format``. In build, ``-d`` or ``--dir`` allows for building a book outside of the current directory.

---

> ℹ️ The book name is grabbed from the current or specified directory and not from ``title.txt``. This is a limitation in RyuBook and NOT Pandoc.

### Supported Formats

If you wish to output to something other an ePub, RyuBook does support a few formats.

- ``odt``
- ``docx``
- ``rtf``
- ``html``
- ``pdf``

---

> ℹ️ In order to avoid dependency hell (see PDF requirement), RyuBook doesn't support all of what Pandoc has to offer.

> ℹ️ Even if you choose ``doc``, it will always output to ``docx``.

> ⚠️ PDF support requires [wkhtmltopdf](https://wkhtmltopdf.org/) (recommended), [weasyprint](https://weasyprint.org/) or [prince](https://www.princexml.com/).

## Init

```
USAGE:
    ryubook init [FLAGS]
FLAGS:
  -t --title  -a  --author  -d  --dir```

Init will create ``/src/title.txt``, ``/src/01-helloworld.md``, and a ``.gitignore``. By passing ``-t`` or ``-a`` Ryubook will write the title and author, respectfully, to the ``title.txt``. In Init, ``-d`` or ``--dir`` allows for creating a book project of the current directory.
```

### title.txt

```
---
title: Book Title
author: Lorem Ipsum
language: en-US
---
```

The book's metadata is contained in ``title.txt`` and will be read by Pandoc.

---

> ℹ️ RyuBook assumes the language based on your relative location. In this example, it choose `en-US` because my native language is English because I live in the US.