using System;
using System.Collections.Generic;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;
using RestSharp.Extensions;
using xFunc.Maths;
using xFunc.Maths.Expressions;

namespace Noobot.Custom.Pipeline.Middleware
{
    public class CalculatorMiddleware : MiddlewareBase
    {
        public CalculatorMiddleware(IMiddleware next) : base(next)
        {
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{"calc"},
                    Description = "Calculate mathematical expressions - usage: calc ((1+2)*3)/4",
                    EvaluatorFunc = CalculateHandler,
                    MessageShouldTargetBot = false
                },
                new HandlerMapping
                {
                    ValidHandles = new []{""},
                    Description = "Try to calculate mathematical expressions without the 'calc' prefix - usage: ((1+2)*3)/4",
                    EvaluatorFunc = CalculateHandler,
                    MessageShouldTargetBot = false,
                    ShouldContinueProcessing = true
                }
            };
        }

        private IEnumerable<ResponseMessage> CalculateHandler(IncomingMessage message, string matchedHandle)
        {
            string response = string.Empty;

            if (matchedHandle != null)
            {
                string expression = message.FullText.Substring(matchedHandle.Length).Trim();
                Parser parser = new Parser();

                try
                {
                    IExpression parsedExpression = parser.Parse(expression);
                    object result = parsedExpression.Calculate();
                    response = $"{parsedExpression} = {result}";
                }
                catch (Exception e)
                {
                    bool showErrors = !string.IsNullOrEmpty(matchedHandle);

                    if (showErrors)
                    {
                        response = $"Who taught you maths? {e.Message}";
                    }
                }
            }

            if (!string.IsNullOrEmpty(response))
            {
                yield return message.ReplyToChannel($"@{message.Username}: {response}");
            }
        }
    }
}