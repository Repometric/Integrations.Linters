namespace Linterhub.Cli.Runtime
{
    using System;
    using Engine.Exceptions;

    public class LinterhubWrapper
    {
        public RunContext Context { get; }

        public LinterhubWrapper(RunContext context)
        {
            Context = context;
        }

        private static CmdWrapper.RunResults Run(string terminal, string command, string workingDirectory = null)
        {
            var wrapper = new CmdWrapper();
            var run = wrapper.RunExecutable(terminal, command, workingDirectory);
            return run;
        }

        public string Run(string command)
        {
            var cmd =  string.Format(Context.CliConfig.Terminal.Command, Context.CliConfig.Linterhub.Command + " " + command);
            var run = Run(Context.CliConfig.Terminal.Path, cmd, Context.CliConfig.Linterhub.Path);
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
            var result = Run(Context.CliConfig.Command.Info);
            return result;
        }

        public string Version()
        {
            var result = Run(Context.CliConfig.Command.Version);
            return result;
        }

        public string Analyze(string name, string command, string path)
        {
            var cmd = string.Format(Context.CliConfig.Command.Analyze, name, command, path);
            var result = Run(cmd);
            return result;
        }

        public string LinterVersion(string name, string command)
        {
            var cmd = string.Format(Context.CliConfig.Command.LinterVersion, name, command);
            var result = Run(cmd);
            return result;
        }
    }
}