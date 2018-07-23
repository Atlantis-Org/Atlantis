using System;

namespace Followme.AspNet.Core.FastCommon.Infrastructure
{
    public class BaseMessage
    {
        private MessageExecutingType ExecutingType { get; set; }

        private string TypeFullName { get; set; }

        public void SetTypeFullName(string typeFullName)
        {
            TypeFullName = typeFullName;
        }

        public string GetTypeFullName()
        {
            if(string.IsNullOrWhiteSpace(TypeFullName))
            {
                TypeFullName=this.GetType().FullName;
            }
            return TypeFullName;
        }

        public void SetMessageExecutingType(MessageExecutingType executingType)
        {
            ExecutingType = executingType;
        }

        public MessageExecutingType GetMessageExecutingType()=>ExecutingType;
    }

    public enum MessageExecutingType
    {
        Command,
        Query
    }
}
