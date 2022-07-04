# EDNA – An Elite Dangerous System Data Library

EDNA is a library for third party apps that want to access data about star
systems, stations and CMDRs of the game Elite Dangerous. It can query
[EDSM](https://edsm.net), [Spansh](https://spansh.uk) and
[EDTS](http://edts.thargoid.space) for data.

The general idea is that you usually do not care _where_ your data comes from,
so this will get it for you wherever it can depending on data availability and
API features.

**THIS IS A HEAVY WIP PROJECT AND *NOT* READY FOR PRODUCTION USE.** Just saying.

## Why EDNA⁈

Why not. I find it quite funny that most Elite-related project are 4 letter
acronyms starting with “ED”. So I stuck to that, and I had just played “Edna &
Harvey: The Breakout” with the kids. It’s not actually an acronym though because
I couldn’t come up with a meaning for NA.

## TODO for first proper release

- [x] README
- [ ] Documentation
- [ ] Spansh API
  - [ ] system data
  - [ ] nearest system
  - [ ] station data
    - [ ] outdated stations
    - [ ] rest (probably out of scope for now)
- [ ] EDSM API
  - [x] Systems
  - [x] Logs (CMDR data)
  - [ ] Stations (probably out of scope for now)
- [x] EDTS API
  - [x] system position
- [ ] convenience layer / unification
- [ ] sanity check / refactoring
- [ ] nuget package
- [ ] Github Actions build / package automation

[![GitHub Sponsors](https://img.shields.io/github/sponsors/alterNERDtive?style=for-the-badge)](https://github.com/sponsors/alterNERDtive)
[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/S6S1DLYBS)
