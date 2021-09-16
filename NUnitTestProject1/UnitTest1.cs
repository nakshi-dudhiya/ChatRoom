using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using SignalR_Ex;
using SignalR_Ex.Models;
using System.Threading.Tasks;

namespace NUnitTestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// To test the connection of signalR
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Signal_R_OnConnectedAsync_InvalidCaller()
        {
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.Caller).Returns(mockClientProxy.Object).Verifiable();
            ChatHub simpleHub = new ChatHub(new AppDbContext())
            {
                Clients = mockClients.Object
            };
            await simpleHub.OnConnectedAsync();
            Assert.Throws<MockException>(
                () => mockClients.Verify(clients => clients.Others, Times.Once));
        }
        [Test]
        public async Task SignalR_SendAll()
        {
            // arrange
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            ChatHub simpleHub = new ChatHub(new AppDbContext())
            {
                Clients = mockClients.Object
            };
            // act
            await simpleHub.SendAllMessage("userName", "message");
            // assert
            mockClients.Verify(clients => clients.All, Times.Once);

        }

        [Test]
        public async Task SignalR_OnConnectedAsync()
        {
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.Caller).Returns(mockClientProxy.Object);
            ChatHub simpleHub = new ChatHub(new AppDbContext())
            {
                Clients = mockClients.Object
            };
            await simpleHub.OnConnectedAsync();
            mockClients.Verify(clients => clients.Caller, Times.Once);
        }

    }
}