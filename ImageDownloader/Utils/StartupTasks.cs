using Caliburn.Micro;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace ImageDownloader.Utils
{
    public delegate void StartupTask();

    public class StartupTasks
    {
        [Export(typeof(StartupTask))]
        public void ApplyBindingScopeOverride()
        {
            var get_named_elements = BindingScope.GetNamedElements;
            BindingScope.GetNamedElements = o =>
            {
                var metroWindow = o as MetroWindow;
                if (metroWindow == null)
                {
                    return get_named_elements(o);
                }

                var list = new List<FrameworkElement>(get_named_elements(o));
                var type = o.GetType();
                var fields = o.GetType()
                              .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                              .Where(f => f.DeclaringType == type);
                var flyouts = fields.Where(f => f.FieldType == typeof(FlyoutsControl))
                                    .Select(f => f.GetValue(o))
                                    .Cast<FlyoutsControl>();
                var commands = fields.Where(f => f.FieldType == typeof(WindowCommands))
                                    .Select(f => f.GetValue(o))
                                    .Cast<WindowCommands>();
                list.AddRange(flyouts);
                list.AddRange(commands);

                if (!commands.Any())
                {
                    var contained_commands = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                 .Where(p => p.PropertyType == typeof(WindowCommands))
                                                 .Select(p => p.GetValue(o))
                                                 .Cast<WindowCommands>();
                    foreach (var command in contained_commands)
                        list.AddRange(get_named_elements(command));
                }

                return list;
            };
        }

        [Export(typeof(StartupTask))]
        public void ApplyParserOverride()
        {
            var current_parser = Parser.CreateTrigger;
            Parser.CreateTrigger = (target, trigger_text) => InputBindingParser.CanParse(trigger_text)
                                                             ? InputBindingParser.CreateTrigger(trigger_text)
                                                             : current_parser(target, trigger_text);
        }
    }
}
