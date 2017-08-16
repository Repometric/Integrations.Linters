namespace Linterhub.Core.Schema
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Option = System.Collections.Generic.KeyValuePair<string, string>;
    using static Runtime.EngineWrapper;

    public class CommandFactory
    {
        public string GetAnalyzeCommand(
            Context context,
            string argSeparator = " ")
        {
            var valueSeparator = context.Specification.Schema.OptionsDelimiter ?? " ";
            var options = MergeOptions(context.ConfigOptions, context.Specification);

            if (context.Stdin == Context.stdinType.UseWithEngine)
            {
                options.AddRange(context.Specification.Schema.Stdin);
            }

            var args = options.Select(x => BuildArg(context.RunOptions, x, valueSeparator, context.Stdin)).Where(x => !string.IsNullOrEmpty(x));
            var command = context.Specification.Schema.Executable ?? context.Specification.Schema.Name + " " + string.Join(argSeparator, args);


            if (context.Specification.Schema.Postfix != null)
            {
                var postfix = context.Specification.Schema.Postfix ?? "";
                foreach (var runtimeOption in context.RunOptions)
                {
                    postfix = postfix?.Replace(runtimeOption.Key, runtimeOption.Value);
                }
                command = string.Join(" ", command, postfix);
            }

            return command;
        }

        public string GetVersionCommand(EngineSpecification specification)
        {
            return "--version";
        }

        private List<Option> MergeOptions(
            EngineOptions configOptions,
            EngineSpecification specification)
        {
            var sortedOptions = new List<Option>();
            foreach (var option in specification.OptionsSchema)
            {
                var key = option.Key;
                string value;
                if (option.Value.Id == "{path}")
                {
                    value = option.Value.Id;
                }
                else
                {
                    value = configOptions.GetValueOrDefault(key) ??
                            specification.Schema.Defaults.GetValueOrDefault(key);
                }

                if (value != null || (option.Value.Type == "null" && specification.Schema.Defaults.ContainsKey(key)))
                {
                    sortedOptions.Add(new Option(key, value));
                }
            }

            return sortedOptions;
        }

        private string BuildArg(
            EngineOptions runtimeOptions,
            Option option,
            string valueSeparator = " ", Context.stdinType stdin = Context.stdinType.NotUse)
        {
            if(stdin == Context.stdinType.UseWithEngine && (option.Value ?? "").Contains("{path}"))
            {
                return "";
            }

            var value = BuildArgValue(runtimeOptions, option.Value);
            var key = option.Key;
            var parts = new [] { key, value }.Where(x => !string.IsNullOrEmpty(x));
            return string.Join(valueSeparator, parts);
        }

        private string BuildArgValue(
            EngineOptions runtimeOptions,
            string value)
        {
            foreach (var runtimeOption in runtimeOptions)
            {
                value = value?.Replace(runtimeOption.Key, runtimeOption.Value);
            }
            return value;
        }
    }
}