# TODO

## General
* [x] Map command line arguments to a method to execute.
* [x] Handle parsing errors in a friendly manner.
* [x] Support boolean actions and report proper exit code.

## Command Line Options

* [x] Support boolean switches
  * [ ] Support --switch and --no-switch argument styles for boolean arguments
* [x] Support implicit ordered parameters
* [x] Support the concept of a default action
* [x] Support sub-comamnds
* [ ] Support aliases on controllers
* [ ] Support aliases on methods
* [x] Support aliases on parameters
* [x] Support argument parsing for primitive types
  * [x] bool
  * [x] int
  * [x] long
  * [x] DateTime
  * [x] decimal
  * [x] double
  * [x] Enum
* [x] Support argument parsing for nullable primitive types
  * [x] bool
  * [x] int
  * [x] long
  * [x] DateTime
  * [x] decimal
  * [x] double
  * [x] Enum
* [x] Support custom argument parsers
    * [x] Support custom argument parsers with aliases
* [ ] Support array arguments
  * [ ] bool
  * [ ] bool?
  * [ ] int
  * [ ] int?
  * [ ] long
  * [ ] long?
  * [ ] DateTime
  * [ ] DateTime?
  * [ ] decimal
  * [ ] decimal?
  * [ ] double
  * [ ] double?
  * [ ] Enum
  * [ ] Enum?

## Extensibility

* [x] Support an injectable Logger
* [x] Support customizable conventions
  * [x] provide /argment:value style convention
  * [x] provide /argument=value style convention

## Help

* [x] Generate help using reflection
  * [x] Aliases should be emitted by the help
  * [ ] Help should display available enum options
  * [ ] Help should support an output file parameter
  * [ ] Help generation should be customizable
  * [ ] Display default values using reflection

## Testability
* [x] Provide testability of command structure separate from execution of command structure.
* [ ] Validation
  * [ ] Validate no more than one default action per command
  * [ ] Validate no more than one of the same alias on a method
  * [ ] Detect conflicts between subcommands and action names

## Deployment
* [x] Create a nuget package
* [x] Setup AppVeyor Build

## Documentation
* [x] Create a demo project to show how to use the library.
