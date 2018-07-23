namespace Followme.AspNet.Core.FastCommon.ThirdParty.RabbitMQ
{
    public interface IRabbitMessagePublisher
    {
        void Publish<T>(T message);
    }     
}
