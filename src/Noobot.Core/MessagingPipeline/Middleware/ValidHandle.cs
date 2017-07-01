using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noobot.Core.MessagingPipeline.Middleware
{
    public class ValidHandle
    {

        public enum ValidHandleMatchType
        {
            ProcessAll,
            ExactMatch,
            StartsWith,
            Contains,
            RegEx
        }

        /// <summary>
        /// Defaults to ProcessAll so that the handler will process ALL messages
        /// </summary>
        public ValidHandleMatchType MatchType { get; set; }

        /// <summary>
        /// The text to use for StartWith, ExactMatch, or RegEx expression
        /// </summary>
        public string MatchText { get; set; }


        /// <summary>
        /// Shorthand for making a list of handles
        /// </summary>
        /// <param name="matchType"></param>
        /// <param name="matchTextList"></param>
        /// <returns></returns>
        public static ValidHandle[] CreateValidHandleList(ValidHandle.ValidHandleMatchType matchType, string[] matchTextList)
        {
            var validHandles = new ValidHandle[matchTextList.Length];
            for (int i = 0; i <= matchTextList.Length - 1; i++)
            {
                validHandles[i] = new ValidHandle();
                validHandles[i].MatchType = matchType;
                validHandles[i].MatchText = matchTextList[i];
            }

            return validHandles;

        }

    }
}
