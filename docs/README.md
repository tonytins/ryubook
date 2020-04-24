# RyuBook Documentation

RyuBook is a ePub generator written in C#. It uses [Markdown](https://daringfireball.net/projects/markdown/syntax) as a source and [Pandoc](https://pandoc.org/) for generation.

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

### Build

```txt
USAGE:
    ryubook build [FLAGS]

FLAGS:
  -t, --title
```

Using the ``build`` command alone will build the book as is and output ``book.epub``. If ``ryubook.toml`` is present or ``--title`` is used then name of your choice will be used.

## Directory Structure

```txt
+-- src
|   +-- book.md
|   +-- title.txt
```

The book contents exists in the ``/src`` directory. Both ``book.md`` and ``title.txt`` are required. RyuBook currently can't build multiple Markdown files.

### title.txt

```txt
% Book Title
% Author
```

The book's metadata is contained in ``title.txt``. Title and author are on seperate lines with a ``%`` before each word.

## ryubook.toml

``ryubook.toml`` is the project file that can optionally be generated when using the ``init`` command. Currently, the only option is ``Title``. Ryubook will grab the directory's name and use that as the title in the project file.
