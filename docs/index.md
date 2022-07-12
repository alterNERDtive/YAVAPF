# YAVAPF – Yet Another VoiceAttack Plugin Framework

This is a framework for implementing VoiceAttack plugins. Simply put I had two
issues with the plugins I have been working on over the last couple years:

1. The plugin API is … functional, but not great. I want to provide one that is
   more pleasant to work with.
2. I have noticed that I keep re-implementing certain things for each and every
   plugin that I write. For example, every single one of them wants to log to
   VoiceAttack’s event log. A shared framework means writing the code once.

The goal is to get you up & running with as little code and as little knowledge of
the inner workings of VoiceAttack as possible.

You can find an [example plugin on
Github](https://github.com/alterNERDtive/YAVAPF/tree/release/ExamplePlugin).

## Current Implementation Status

* [x] VoiceAttack plugin API
* [x] Handlers for Init/Invoke/Exit/StopCommand
* [x] Plugin contexts
* [x] Handlers for variable changed events
* [x] Logging to the VoiceAttack event log
* [ ] Logging to a log file
    * [ ] separate full debug log
* [x] Wrapper for executing commands
* [ ] Plugin options, separate from handling “normal” variables
    * [ ] default values
    * [ ] descriptions
    * [ ] auto save / load between VoiceAttack runs¹
        * [ ] profile specific
        * [ ] global
    * [ ] bootstrapping voice commands for changing options
    * [ ] GUI support
* [ ] Miscellaneous VoiceAttack proxy functionality
* [ ] Full unit test coverage 😬

¹ Will probably require changes in VoiceAttack that I have already requested.

## Need Help / Want to Contribute?

Have a look at [the FAQ](faq.md). If your problem persists, please [file an
issue](https://github.com/alterNERDtive/YAVAPF/issues/new). Thanks! :)

You can also [say “Hi” on Discord](https://discord.gg/3pWdJwfJc5) if that is
your thing.

[![GitHub Sponsors](https://img.shields.io/github/sponsors/alterNERDtive?style=for-the-badge)](https://github.com/sponsors/alterNERDtive)
[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/S6S1DLYBS)
