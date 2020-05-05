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
```

Using the ``build`` command alone will build the book as is and output ``book.epub``. If ``ryubook.toml`` is present or ``--title`` is used then name of your choice will be used. You can also build formats other then the default ``epub`` by passing `-f` or ``--format``.

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
```

By passing ``-t`` or ``-a`` you can Ryubook will write metadata of the book to ``title.txt``.

### title.txt

```txt
% Book Title
% Author
```

The book's metadata is contained in ``title.txt``. Title and author are on separate lines with a ``%`` before each word.


