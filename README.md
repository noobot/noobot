# noobot
An extensible Slackbot build in C#

## About
Want to build a useful team slack bot...or just something to make funny jokes? The Noobot host allows you to build your own middleware really easily.

Noobot supports long running requests (need to execute and wait for something to finish?).

## Setup
Please note that you will need to create a config.json file with your bot's api key. This should live under:
`src/Noobot.Runner/Configuration`

To implement a new chat related feature, you simply need to create a new Middleware and plug it into `src/Noobot.Runner/PipelineManager.cs`. Please remember, the ordering of the Pipeline is very important, ensure you put new Middleware between the `BeginMessageMiddleware` and `UnhandledMessageMiddleware`.

Your Middleware simply needs to implement the interface `IMiddleware`, however if you want to inherit some helper/wrapper methods for quick development you can inherit from `MiddlewareBase`.

### Why would I want to use MiddlewareBase?
