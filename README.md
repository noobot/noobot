# Noobot
Noobot is a SlackBot for C# built on the intention of extensibility; building a bot should be fun.

## About
I wanted to build a bot host/framework that was easy to work with, but also has the potential of being super powerful. 

Noobot supports DI out of the box for all custom elements to ensure all elements could be easily tested and predictable. 

## Setup
Please note that you will need to create a config.json file with your bot's api key. This should live under:
`src/Noobot.Runner/Configuration`

## How to customise
### Pipeline & Middleware
When a message is detected by Noobot a `pipeline` is created, it then passes the `incoming message` through the `pipeline`. The `pipeline` is formed of `middleware` that can intercept a message, modify a message, respond to it or ignore it (*or do all of them*).

Using the `pipeline` pattern gives `middleware` lots of power over how `incoming messages` are handled, and using the base classes provided allow you to get up and running really quickly (however they are not required).

Any `middleware` have to simply implement the interface `IMiddleware` to be compatible.

`Middleware` are built with `StructureMap,` so it fully supports DI. This allows you to communicate easily with any `plugins` you have built. (*`plugins` are ` singletons`, so when you are communicating with a `plugin` it is always the same `plugin`*)

#### How are messages sent?
Using the super handy `yield return` method, you can return messages in real time when long running processes are executing without having to wait for an operation to execute:
```
yield return incomingMessage.ReplyToChannel("Waiting before");
Thread.Sleep(TimeSpan.FromSeconds(3));
yield return incomingMessage.ReplyToChannel("Waiting after");
```

#### Why would I want to use MiddlewareBase?
MiddlewareBase is not required for your `middleware` to be supported - you will just have to handle incoming messages and meta information yourself - make sure you don't break the `middleware chain`.

#### How to I setup my `pipeline`?
Within the project you will find a class called `src/Noobot.Custom/PipelineManager`, simply add your `middleware` to the `Initialise()` function. 
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

#### Async?
Well, kinda. The `pipeline` is contained within a `TPL Task` which means a response can take as long as it needs.

### Plugins
`Plugins` are initialised and run once the bot is connected to Slack. All `plugins` must implement the `IPlugin` interface and are defined to be used in the `PluginManager` found in `src/Noobot.Custom/PluginManager` in a similar fashion to the `PipelineManager`

```
protected override void Initialise()
{
    Use<PingPlugin>();
}
```
**Please note:** the ordering of the plugins are **NOT** important.

Plugins have a `Start` and a `Stop` method and are only `constructed` **ONCE**, this means if you add a dependency to a `plugin` in `middleware`, then you will always get the same `plugin`. This enables you to communicate, store and manipulate data in a single place for a domain.

For `plugins` to communicate with Slack, it must take a `dependency` on `ISlackConnector` which will give the `plugin` methods for sending messages, getting channel ids and user ids *(more functionality will be added when required)*. 
