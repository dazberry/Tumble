using System;
using System.Collections.Generic;
using System.Text;

namespace Tumble.Core.Notifications
{
    public class Notification
    {
        public IPipelineHandler Handler { get; private set; }

        public string ErrorMessage { get; private set; }

        public Notification(IPipelineHandler handler, string errorMessage)
        {
            Handler = handler;
            ErrorMessage = errorMessage;
        }
    }

    public static class NotificationExtensions
    {
        public static PipelineContext AddNotification(this PipelineContext context, IPipelineHandler handler, string errorMessage) =>        
            context.Add(new Notification(handler, errorMessage));
    }
}
