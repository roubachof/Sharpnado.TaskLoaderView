using System;
using System.Linq;

namespace Sharpnado.TaskLoaderView
{
    public partial class CompositeTaskLoaderNotifier
    {
        public class Builder
        {
            public TaskLoaderCommandBase[] Commands { get; private set; } = Array.Empty<TaskLoaderCommandBase>();

            public ITaskLoaderNotifier[] Loaders { get; private set; } = Array.Empty<ITaskLoaderNotifier>();

            public Builder WithLoaders(params ITaskLoaderNotifier[] loaders)
            {
                Loaders = loaders;
                return this;
            }

            public Builder WithCommands(params TaskLoaderCommandBase[] commands)
            {
                Commands = commands;
                return this;
            }

            public CompositeTaskLoaderNotifier Build()
            {
                return new CompositeTaskLoaderNotifier(Loaders, Commands.Select(c => c.Notifier).ToArray());
            }
        }
    }
}
