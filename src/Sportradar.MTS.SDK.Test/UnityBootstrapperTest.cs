/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.API;
using Sportradar.MTS.SDK.API.Internal;
using Sportradar.MTS.SDK.API.Internal.RabbitMq;
using Sportradar.MTS.SDK.API.Internal.Senders;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Internal;
using Unity;
// ReSharper disable NotAccessedField.Local

namespace Sportradar.MTS.SDK.Test
{
    [TestClass]
    public class UnityBootstrapperTest
    {
        private static IUnityContainer _childContainer1;
        private static IUnityContainer _childContainer2;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            var container = new UnityContainer().EnableDiagnostic();
            var config = new SdkConfiguration("test", "test", "10.27.26.83");
            container.RegisterTypes(config, null, null);
            _childContainer1 = ((IUnityContainer) container).CreateChildContainer();
            _childContainer2 = ((IUnityContainer) container).CreateChildContainer();
        }

        [TestMethod]
        public void MtsSdkIsConstructedSuccessfully()
        {
            var config = _childContainer1.Resolve<ISdkConfiguration>();
            Assert.IsNotNull(config);

            var sdk = new MtsSdk(config, null, null);
            Assert.IsNotNull(sdk);
        }

        [TestMethod]
        public void RabbitServerBaseObjectsAreResolvedCorrectly()
        {
            var server = _childContainer1.Resolve<IRabbitServer>();
            Assert.IsNotNull(server, "Resolved IRabbitServer cannot be a null reference");
            Assert.IsInstanceOfType(server, typeof(RabbitServer), "item must be instance of RabbitServer");

            var cv = _childContainer1.Resolve<ConnectionValidator>();
            Assert.IsNotNull(cv, "Resolved ConnectionValidator cannot be a null reference");
            Assert.IsInstanceOfType(cv, typeof(ConnectionValidator), "item must be instance of ConnectionValidator");

            var ccf = _childContainer1.Resolve<ConfiguredConnectionFactory>();
            Assert.IsNotNull(ccf, "Resolved ConfiguredConnectionFactory cannot be a null reference");
            Assert.IsInstanceOfType(ccf, typeof(ConfiguredConnectionFactory), "item must be instance of ConfiguredConnectionFactory");

            var cf = _childContainer1.Resolve<IChannelFactory>();
            Assert.IsNotNull(cf, "Resolved IChannelFactory cannot be a null reference");
            Assert.IsInstanceOfType(cf, typeof(ChannelFactory), "item must be instance of ChannelFactory");

            var rs = _childContainer1.Resolve<IRabbitMqChannelSettings>("TicketChannelSettings");
            Assert.IsNotNull(rs, "Resolved IRabbitMqChannelSettings cannot be a null reference");
            Assert.IsInstanceOfType(rs, typeof(RabbitMqChannelSettings), "item must be instance of RabbitMqChannelSettings");
        }

        [TestMethod]
        public void TicketConsumersAreResolvedCorrectly()
        {
            var tcc = _childContainer1.Resolve<IRabbitMqConsumerChannel>("TicketConsumerChannel");
            Assert.IsNotNull(tcc, "Resolved IRabbitMqConsumerChannel cannot be a null reference");
            Assert.IsInstanceOfType(tcc, typeof(RabbitMqConsumerChannel), "item must be instance of RabbitMqConsumerChannel");

            var tccc = _childContainer1.Resolve<IRabbitMqConsumerChannel>("TicketCancelConsumerChannel");
            Assert.IsNotNull(tccc, "Resolved IRabbitMqConsumerChannel cannot be a null reference");
            Assert.IsInstanceOfType(tccc, typeof(RabbitMqConsumerChannel), "item must be instance of RabbitMqConsumerChannel");

            var trmr = _childContainer1.Resolve<IRabbitMqMessageReceiver>("TicketResponseMessageReceiver");
            Assert.IsNotNull(trmr, "Resolved IRabbitMqMessageReceiver cannot be a null reference");
            Assert.IsInstanceOfType(trmr, typeof(RabbitMqMessageReceiver), "item must be instance of RabbitMqMessageReceiver");

            var tcrmr = _childContainer1.Resolve<IRabbitMqMessageReceiver>("TicketCancelResponseMessageReceiver");
            Assert.IsNotNull(tcrmr, "Resolved IRabbitMqMessageReceiver cannot be a null reference");
            Assert.IsInstanceOfType(tcrmr, typeof(RabbitMqMessageReceiver), "item must be instance of RabbitMqMessageReceiver");
        }

        [TestMethod]
        public void RabbitPublishersAreResolvedCorrectly()
        {
            var rpc = _childContainer1.Resolve<IRabbitMqPublisherChannel>("TicketPublisherChannel");
            Assert.IsNotNull(rpc, "Resolved IRabbitMqPublisherChannel cannot be a null reference");
            Assert.IsInstanceOfType(rpc, typeof(RabbitMqPublisherChannel), "item must be instance of RabbitMqPublisherChannel");

            var rapc = _childContainer1.Resolve<IRabbitMqPublisherChannel>("TicketAckPublisherChannel");
            Assert.IsNotNull(rapc, "Resolved IRabbitMqPublisherChannel cannot be a null reference");
            Assert.IsInstanceOfType(rapc, typeof(RabbitMqPublisherChannel), "item must be instance of RabbitMqPublisherChannel");

            var rcpc = _childContainer1.Resolve<IRabbitMqPublisherChannel>("TicketCancelPublisherChannel");
            Assert.IsNotNull(rcpc, "Resolved IRabbitMqPublisherChannel cannot be a null reference");
            Assert.IsInstanceOfType(rcpc, typeof(RabbitMqPublisherChannel), "item must be instance of RabbitMqPublisherChannel");

            var rcapc = _childContainer1.Resolve<IRabbitMqPublisherChannel>("TicketCancelAckPublisherChannel");
            Assert.IsNotNull(rcapc, "Resolved IRabbitMqPublisherChannel cannot be a null reference");
            Assert.IsInstanceOfType(rcapc, typeof(RabbitMqPublisherChannel), "item must be instance of RabbitMqPublisherChannel");
        }

        [TestMethod]
        public void TicketSendersAreResolvedCorrectly()
        {
            var tst = _childContainer1.Resolve<ITicketSender>("TicketSender");
            Assert.IsNotNull(tst, "Resolved ITicketSender<ITicket> cannot be a null reference");
            Assert.IsInstanceOfType(tst, typeof(TicketSender), "item must be instance of TicketSender");

            var tsta = _childContainer1.Resolve<ITicketSender>("TicketAckSender");
            Assert.IsNotNull(tsta, "Resolved ITicketSender<ITicketAck> cannot be a null reference");
            Assert.IsInstanceOfType(tsta, typeof(TicketAckSender), "item must be instance of TicketAckSender");

            var tstc = _childContainer1.Resolve<ITicketSender>("TicketCancelSender");
            Assert.IsNotNull(tstc, "Resolved ITicketSender<ITicketCancel> cannot be a null reference");
            Assert.IsInstanceOfType(tstc, typeof(TicketCancelSender), "item must be instance of TicketCancelSender");

            var tstca = _childContainer1.Resolve<ITicketSender>("TicketCancelAckSender");
            Assert.IsNotNull(tstca, "Resolved ITicketSender<ITicketCancelAck> cannot be a null reference");
            Assert.IsInstanceOfType(tstca, typeof(TicketCancelAckSender), "item must be instance of TicketCancelAckSender");
        }

        [TestMethod]
        public void MtsChannelSettingsExchangeTypeSetProperly()
        {
            Assert.AreEqual(ExchangeType.Fanout, _childContainer1.Resolve<IMtsChannelSettings>("TicketChannelSettings").ExchangeType);
            Assert.AreEqual(ExchangeType.Topic, _childContainer1.Resolve<IMtsChannelSettings>("TicketCancelChannelSettings").ExchangeType);
            Assert.AreEqual(ExchangeType.Topic, _childContainer1.Resolve<IMtsChannelSettings>("TicketAckChannelSettings").ExchangeType);
            Assert.AreEqual(ExchangeType.Topic, _childContainer1.Resolve<IMtsChannelSettings>("TicketCancelAckChannelSettings").ExchangeType);
            Assert.AreEqual(ExchangeType.Topic, _childContainer1.Resolve<IMtsChannelSettings>("TicketReofferCancelChannelSettings").ExchangeType);
            Assert.AreEqual(ExchangeType.Topic, _childContainer1.Resolve<IMtsChannelSettings>("TicketCashoutChannelSettings").ExchangeType);
            Assert.AreEqual(ExchangeType.Topic, _childContainer1.Resolve<IMtsChannelSettings>("TicketResponseChannelSettings").ExchangeType);
            Assert.AreEqual(ExchangeType.Topic, _childContainer1.Resolve<IMtsChannelSettings>("TicketCancelResponseChannelSettings").ExchangeType);
            Assert.AreEqual(ExchangeType.Topic, _childContainer1.Resolve<IMtsChannelSettings>("TicketCashoutResponseChannelSettings").ExchangeType);
        }
    }
}