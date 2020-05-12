# RyuBook Documentation

RyuBook is a ePub generator written in C#. It uses [Markdown](https://daringfireball.net/projects/markdown/syntax) as a source and [Pandoc](https://pandoc.org/) for generation.

## Directory Structure

```txt
+-- src
|   +-- book.md
|   +-- title.txt
```

The book contents exists in the ``/src`` directory. Both ``book.md`` and ``title.txt`` are required. RyuBook currently can't build multiple Markdown files.

## Usage

```txt
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

```txt
USAGE:
    ryubook build [FLAGS]

FLAGS:
  -t, --title
  -f  --format
  -d  --dir
```

Using the ``build`` command alone will build the book as is and output ``book.epub``. You can also build formats other then the default ``epub`` by passing `-f` or ``--format``. In build, ``-d`` or ``--dir`` allows for building a book outside of the current directory.

Note the book name is currently grabbed from the current or specified directory and not from ``title.txt``. This is a limitation in Ryubook and NOT Pandoc.

### Supported formats

In order to avoid installing additional dependencies, Ryubook doesn't support outputting to all of Pandoc's supported formats.

- ``odt``
- ``docx``
- ``rtf``
- ``html``

Note that passing ``doc`` will always output to ``docx``. This is a limitation of Pandoc and not Ryubook. If you still need to output to the legacy Word document format, then you will have to use another tool or word processor that supports saving to that format.

## Init

```txt
USAGE:
    ryubook init [FLAGS]

FLAGS:
  -t --title
  -a  --author
  -d  --dir
```

Init will create ``/src/title.txt``, ``/src/01-helloworld.md``, and a ``.gitignore``. By passing ``-t`` or ``-a`` Ryubook will write the title and author, respectfully, to the ``title.txt``. In Init, ``-d`` or ``--dir`` allows for creating a book project of the current directory.

### title.txt

```txt
---
title: Book Title
author: Lorem Ipsum
language: en-US
---
```

The book's metadata is contained in ``title.txt`` and will be read by Pandoc. Ryubook assumes the language based on your relative location. In this example, it choose ``en-US`` because my native language is English and I live in the US.
