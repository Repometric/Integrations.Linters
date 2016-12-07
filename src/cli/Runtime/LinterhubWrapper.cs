namespace Linterhub.Cli.Runtime
{
    using System;
    using Engine;
    using Engine.Exceptions;

    public class LinterhubWrapper
    {
        public RunContext Context { get; }
        public LinterEngine Engine { get; }

        public LinterhubWrapper(RunContext context, LinterEngine engine)
        {
            Context = context;
            Engine = engine;
        }

        public string Run(string command)
        {
            var cmd = string.Format(Context.CliConfig.TerminalCommand, command);
            var run = Engine.Run(Context.CliConfig.Terminal, cmd, Context.CliConfig.Linterhub);
            if (run.RunException != null)
            {
                throw new LinterEngineException("Runtime Exception", run.RunException);
            }

            var error = run.Error?.ToString().Trim();
            if (!string.IsNullOrEmpty(error))
            {
                throw new LinterEngineException(
                    "Linter stderr: " + error + Environment.NewLine +
                    "Linter stdout: " + run.Output?.ToString());
            }

            return run.Output?.ToString();
        }

        public string Info()
        {
            var result = Run(Context.CliConfig.CommandInfo);
            return result;
        }

        public string Version()
        {
            var result = Run(Context.CliConfig.CommandInfo.Replace("info", "version"));
            return result;
        }

        public string Analyze(string name, string command, string path)
        {
            var cmd = string.Format(Context.CliConfig.Command, name, command, path);
            var result = Run(cmd);
            return result;
        }
    }
}