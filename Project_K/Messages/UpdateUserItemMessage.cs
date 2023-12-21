using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Project_K.Messages
{
    public class UpdateUserItemMessage : ValueChangedMessage<string> //Pub/Sub Messenger
    {
        public UpdateUserItemMessage(string value) : base(value)
        {
        }
    }
}
