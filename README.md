# Noobot
Noobot is a SlackBot for C# built on the intention of extensibility; building a bot should be fun.

## About
I wanted to build a bot host/framework that was easy to work with, but also has the potential of being super powerful. 

Noobot supports DI out of the box for all custom elements to ensure all elements could be easily tested and predictable. 

## Setup
Please note that you will need to create a config.json file with your bot's api key. This should live under:
`src/Noobot.Runner/Configuration`

To implement a new chat related feature, you simply need to create a new Middleware and plug it into `src/Noobot.Runner/PipelineManager.cs`. Please remember, the ordering of the Pipeline is very important, ensure you put new Middleware between the `BeginMessageMiddleware` and `UnhandledMessageMiddleware`.

Your Middleware simply needs to implement the interface `IMiddleware`, however if you want to inherit 
some helper/wrapper methods for quick development you can inherit from `MiddlewareBase`.

## How to customise


### Pipeline & Middleware
When a message is detected by Noobot a `pipeline` is created, it then passes the `incoming message` through the `pipeline`. The `pipeline` is formed of `middleware` that can intercept a message, modify a message, respond to it or ignore it (*or do all of them*).

Using the `pipeline` pattern gives `middleware` lots of power over how `incoming messages` are handled, and using the base classes provided allow you to get up and running really quickly (however they are not required).

Any `middleware` have to simply implement the interface `IMiddleware` to be compatible.

`Middleware` are built with `StructureMap,` so it fully supports DI. This allows you to communicate easily with any `plugins` you have built. (*`plugins` are ` singletons`, so when you are always communicating with the same `plugin`*)

#### Why would I want to use MiddlewareBase?
MiddlewareBase is not required for your `middleware` to be supported - you will just have to handle incoming messages and meta information yourself - make sure you don't break the `middleware chain`.

#### How to I setup my `pipeline`?
Within the project you will find a class called `src/Noobot.Custom.PipelineManager`, simply add your `middleware` to the `Initialise()` function. 

```
protected override void Initialise()
{
    Use<WelcomeMiddleware>();
    Use<JokeMiddleware>();
    Use<YieldTestMiddleware>();
    Use<PingMiddleware>();
    Use<FlickrMiddleware>();
}
```

**Please note:** the ordering of the pipeline is important.

### Plugins
Plugins are
