using System;
using System.Collections.Generic;
using System.Net;
using Noobot.Domain.MessagingPipeline.Middleware;
using Noobot.Domain.MessagingPipeline.Request;
using Noobot.Domain.MessagingPipeline.Response;
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
                    ValidHandles = new []{"/calc", "calc"},
                    Description = "Calculate mathematical expressions - usage: calc ((1+2)*3)/4",
                    EvaluatorFunc = CalculateHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> CalculateHandler(IncomingMessage message, string matchedHandle)
        {
            string response;
            // TODO: Remove HtmlDecode if messages are Decoded elsewhere.
            string expression = WebUtility.HtmlDecode(message.FormatTextTargettedAtBot().Substring(matchedHandle.Length).Trim());
            Parser parser = new Parser();

            try
            {
                IExpression parsedExpression = parser.Parse(expression);                
                object result = parsedExpression.Calculate();                
                response = $"{parsedExpression} = {result}";
            }
            catch (Exception e)
            {
                response = $"Who taught you maths? {e.Message}";
            }

            yield return message.ReplyToChannel($"@{message.Username}: {response}");           
        }
    }
}