using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class MessageQueue 
{
    private static BlockingCollection<Action> queue;

    public static void Enqueue<T>(T command) where T : ICommand
    {
        var commandHandler = (ICommandHandler<T>) Activator.CreateInstance(typeof(ICommandHandler<T>));
        queue.Add(()=> commandHandler.Handle(command));
    }

    public static void Complete() => queue.CompleteAdding();

    public static void Start()
    {
        queue = new BlockingCollection<Action>();
        Task.Run(() =>
               {
                   while (!queue.IsCompleted)
                   {
                       try
                       {
                           var command = queue.Take();

                           command();
                           //command.Execute(command);
                       }
                       catch
                       {

                       }
                   }
               });
    }
}