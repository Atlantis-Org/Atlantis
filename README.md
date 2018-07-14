# Atlantis - 一个DDD+CQS+Grpc+RabbitMQ的快速开发框架

框架采用了 DDD 理论模型开发，其中分C端（写端）和Q端（读端）两个部分。C端使用DDD，Q端直接使用数据库，其目的是为了严谨对写的操作，而Q端作为查询需要快速返回数据，故直接数据库操作返回。框架默认接入了Grpc与RabbitMQ实现，当然你也可以扩展接入更多（例如 Webapi）。数据库操作分C端和Q端，默认C端使用EF，Q端使用Dapper。C端框架已支持事物机制。在这一整套框架中，你可以很方便的扩展任何部分。
其默认使用组件：Grpc、RabbitMQ、Autofac、EF、Dapper、SerilLog、Newtonsoft.json。

 ## 安装
 建议使用源码引用。

## 使用
