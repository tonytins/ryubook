# RyuBook

RyuBook is a ePub generator written in C#. It uses [Markdown](https://daringfireball.net/projects/markdown/syntax) as a source and [Pandoc](https://pandoc.org/) for generation. To easily build as a self-contained standalone executable, use the ``publish.sh`` or ``publish.ps1`` scripts.

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

## Authors

- **Anthony Foxclaw** - _Initial work_ - [tonytins](https://github.com/tonytins)

See also the list of [contributors](https://github.com/tonytins/RyuBook/contributors) who participated in this project.

## License

This project is licensed under the MPL 2.0 license - see the [LICENSE](LICENSE) file for details.
