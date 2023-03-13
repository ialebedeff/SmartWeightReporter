using System;

namespace SmartWeight.Updater.Pages.Updates.ViewModels
{
    public class UpdateException : Exception
    {
        public UpdateException(
            string message,
            UpdateState updateState) : base(message)
        {
            UpdateState = updateState;
        }

        public UpdateState UpdateState { get; set; }
    }
}
