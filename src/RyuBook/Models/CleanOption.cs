namespace RyuBook.Models;

using RyuBook.Interface;

[Verb("clean", HelpText = "Removes all books in the /build directory.")] class CleanOption : BaseOptions, IOptions { }
