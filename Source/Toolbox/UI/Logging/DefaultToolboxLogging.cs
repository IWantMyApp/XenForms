using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.Targets;
using XenForms.Core;
using XenForms.Core.Diagnostics;
using XenForms.Core.Toolbox;
using XenForms.Core.Toolbox.Shell;
using XenForms.Toolbox.UI.Resources;

namespace XenForms.Toolbox.UI.Logging
{
    public class DefaultToolboxLogging : ToolboxLogging
    {
        private readonly ILogger _logger;
        private readonly XenFormsEnvironment _environment;
        private const string EndOfSection = "-------------- End --------------";


        public DefaultToolboxLogging(ILogger logger, XenFormsEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }


        public override void Event(XenLogLevel level, string message, params object[] args)
        {
            LogLevel nLogLevel;

            switch (level)
            {
                case XenLogLevel.Off:
                    nLogLevel = LogLevel.Off;
                    break;
                case XenLogLevel.Error:
                    nLogLevel = LogLevel.Error;
                    break;
                case XenLogLevel.Info:
                    nLogLevel = LogLevel.Info;
                    break;
                case XenLogLevel.Trace:
                    nLogLevel = LogLevel.Trace;
                    break;
                case XenLogLevel.Warn:
                    nLogLevel = LogLevel.Warn;
                    break;
                default:
                    nLogLevel = LogLevel.Trace;
                    break;
            }

            _logger.Log(nLogLevel, Beautify(message, args));
        }


        public override void Event(Exception ex, string message, params object[] args)
        {
            var msg = Beautify(ex + Environment.NewLine + message + Environment.NewLine + ex, args);
            Event(XenLogLevel.Error, msg);
        }


        public override void Info(string message, params object[] args)
        {
            _logger.Info(Beautify(message, args));
        }


        public override void Trace(string message, params object[] args)
        {
            _logger.Trace(Beautify(message, args));
        }


        public override void Warn(string message, params object[] args)
        {
            _logger.Warn(Beautify(message, args));
        }


        public override void Error(string message, params object[] args)
        {
            _logger.Error(Beautify(message, args));
        }


        public override void Error(Exception ex, string message, params object[] args)
        {
            var msg = Beautify(ex + Environment.NewLine + message + Environment.NewLine + ex, args);
            Event(XenLogLevel.Error, msg);
        }


        public override string[] LiveLog
        {
            get
            {
                var target = (MemoryTarget) LogManager.Configuration.FindTargetByName(LoggingResource.LogName);
                return target.Logs.ToArray();
            }
        }


        public override IEnumerable<LogItem> LiveLogItems
        {
            get
            {
                return LiveLog.Select(raw => raw.Split('|')).Select(split => new LogItem
                {
                    EventTime = DateTime.Parse(split[0]),
                    Severity = split[1],
                    Component = split[2],
                    Description = split[3]
                });
            }
        }


        public override void ClearLiveLog()
        {
            var target = (MemoryTarget) LogManager.Configuration.FindTargetByName(LoggingResource.LogName);
            target.Logs.Clear();
        }


        public override void ConfigureTraceLog(ISettingsStore store)
        {
            var disabled = false;
            var traceEnabled = store.GetBool(UserSettingKeys.Builtin.TraceLoggingEnabled);

            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                if (traceEnabled == null || !traceEnabled.Value)
                {
                    disabled = true;
                    rule.DisableLoggingForLevel(LogLevel.Trace);
                }
                else
                {
                    rule.EnableLoggingForLevel(LogLevel.Trace);
                }
            }

            Event(XenLogLevel.Info,
                disabled
                    ? "Trace logging has been disabled. Future trace log events will be ignored."
                    : "Trace logging is enabled.");

            LogManager.ReconfigExistingLoggers();
        }


        public override string[] GetDetailedLogInformation(string extra)
        {
            var logs = LiveLogItems
                .Select(l => l.ToString())
                .ToList();

            if (!string.IsNullOrWhiteSpace(extra))
            {
                logs.Add(EndOfSection);
                logs.Add(Environment.NewLine);
                logs.Add(extra);
            }

            try
            {
                logs.Add(EndOfSection);
                logs.Add(_environment.GetOsInformation());
                logs.Add(_environment.GetMemoryUsage());
            }
            catch (Exception)
            {
                // ignored
            }

            logs.Add($"XenForms Toolbox Version: {XenFormsEnvironment.ToolboxVersion}");
            logs.Add($"Supports Designer Version: {XenFormsEnvironment.DesignerVersion}");

            return logs.ToArray();
        }


        private string Beautify(string message, params object[] args)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message)) return message;
                if (!message.Contains("{0}")) return message;

                var formatted = String.Format(message, args);
                var period = formatted.EndsWith(".", StringComparison.CurrentCulture);

                return period ? formatted : formatted + ".";
            }
            catch (Exception)
            {
                return message;
            }
        }
    }
}