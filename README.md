# Noobot
Noobot is a SlackBot for C# built on the intention of extensibility; building a bot should be fun.

[![Build status](https://ci.appveyor.com/api/projects/status/lvhpuswmafwv84kw?svg=true)](https://ci.appveyor.com/project/Workshop2/noobot)[![Nuget.org](https://img.shields.io/nuget/v/Noobot.Core.svg?style=flat)](https://www.nuget.org/packages/Noobot.Core)

<p align="center">
<img src="https://github.com/noobot/noobot/blob/master/img/noobot-small.png" alt="Noobot" />
</p>

```
Install-Package Noobot.Core
```

## About
I wanted to build a bot host/framework that was easy to work with, but also has the potential of being super powerful. 

Noobot supports DI out of the box for all custom elements to ensure all elements could be easily tested and predictable. 

Noobot is available as a Nuget package or standalone Console/Windows Service app.

## Features

 - Is super extensible
 - DI support out of the box
 - Is super easy to install as a [Windows Service](https://github.com/noobot/noobot/wiki/Install-as-Windows-Service) (thanks to [TopShelf](https://github.com/Topshelf/Topshelf))
 - Automatically builds up `help` text with all supported commands
 - Middleware can send multiple messages for each message received
 - Supports long running processes (async)
 - Typing Indicator - indicate to the end user that the bot has received the message and is processing the request

## Examples
You can find some examples of how to use the Nuget package in different scenarios at [Noobot.Examples](https://github.com/noobot/Noobot.Examples)

## Download & setup
We have compilled releases ready for you to use, all you need to do is fill out the `Configuration/config.json` file found in the zip file. Run over to [releases](https://github.com/noobot/noobot/releases) section to download the latest build.

## Setup for development
Please note that you will need to create a config.json file with your bot's api key. This should live under:
`src/Noobot.Runner/Configuration`

Read how to get Noobot up and running quickly on the [wiki](https://github.com/noobot/noobot/wiki/Getting-Started-With-Noobot#get-noobot-up-and-running-quickly).

## How to customise
To customise Noobot please have a look at our [wiki: https://github.com/noobot/noobot/wiki](https://github.com/noobot/noobot/wiki)
