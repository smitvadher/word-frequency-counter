# Word Frequency Counter

The Word Frequency Counter is a console application built on .NET Standard 2.1 and .NET 8. It efficiently counts the occurrences of words within large text files and provides an output file that lists each word along with its frequency.

## Features

- Count word frequencies in a given text file.
- Process files in chunks for memory efficiency.
- Write results to an output file.

## File Structure

- /Counter.Core # Core functionality and interfaces
- /CounterApp # Console application
- /Counter.Core.Tests # Unit tests for core functionality

## Usage

To count word frequencies in a text file and save the result:

```bash
$ CounterApp <inputFilePath> <outputFilePath>
```
- `<inputFilePath>`: Path to the input text file.
- `<outputFilePath>`: Path to the output file where results will be saved.

## Dependencies
- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
