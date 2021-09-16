using Microsoft.AspNetCore.SignalR;
using SignalR_Ex.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// I, Nakshi Dudhiya, certify that all code submitted is my own work; 
//  that I have not copied it from any other source.
//  I also certify that I have not allowed my work to be copied by others.

namespace SignalR_Ex
{
    public class ChatHub: Hub
    {
        private readonly AppDbContext _dbContext;
        //A dictionary for current users
        private static ConcurrentDictionary<string, string> CurrentUsers = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, ChatRoom> ChatRooms = new ConcurrentDictionary<string, ChatRoom>();

        /// <summary>
        /// Contructor that passed the DbContext
        /// </summary>
        /// <param name="dbContext"></param>
        public ChatHub(AppDbContext dbContext) {
            _dbContext = dbContext;
        }

        /// <summary>
        /// It sends the msg when a user connects
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "Chat Hub", DateTimeOffset.Now,
                "Welcome to Chat Hub!");
            await base.OnConnectedAsync();

        }
        /// <summary>
        /// This method send the msg all the clients connected 
        /// to the current RoomName
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="userName"></param>
        /// <param name="textMessage"></param>
        /// <returns></returns>
        public async Task SendMessage(string roomName,string userName, string textMessage)
        { 
            //checking whether currentUser exists or not
            if (!CurrentUsers.ContainsKey(Context.ConnectionId))
            {
                CurrentUsers.TryAdd(Context.ConnectionId, userName);
            }
            var message = new ChatMessage
            {
                UserName = userName,
                Message = textMessage,
                TimeStamp = DateTimeOffset.Now
            };

            // add to DB
            await _dbContext.AddAsync(message);
            //Sending to the group of ppl who contacted
            await Clients.Group(roomName.ToLower()).SendAsync("ReceiveMessage",message.UserName,
                message.TimeStamp,message.Message);

        }

        /// <summary>
        /// This method gets called when a client joins the room
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roomName"></param>
        /// <returns></returns>
        public async Task JoinRoom(string userName,string roomName)
        {
            roomName = roomName.ToLower();
            string currentConnectionId = Context.ConnectionId;

            //If chatroom doesnt exist
            if (!ChatRooms.ContainsKey(roomName))
            {
                //add a new room
                ChatRoom newRoom = new ChatRoom() {
                    RoomName = roomName,
                    ConnectionIds = new List<string>()
                };

                //after making a room, add the current connection id to the new room
                newRoom.ConnectionIds.Add(currentConnectionId);
                ChatRooms.TryAdd(roomName, newRoom);
            }
            else
            {
                //if the room exists already then we are getting the room and adding the current 
                //connection id to the room.
                ChatRoom existingRoom = new ChatRoom();
                ChatRooms.TryGetValue(roomName, out existingRoom);
                existingRoom.ConnectionIds.Add(currentConnectionId); //try not to add to the dictionary
                ChatRooms.TryAdd(roomName, existingRoom);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            //Notifies the other users
            await Clients.GroupExcept(roomName, currentConnectionId).SendAsync("ReceiveMessage", "Chat Hub", DateTimeOffset.Now, $"{userName} connected ({DateTime.Now.ToShortTimeString()}).");
            //Sending to the specific client who  contacted
            await Clients.Caller.SendAsync("ReceiveMessage", "Chat Hub",DateTimeOffset.Now,
                $"You have joined {roomName}");
        }

        /// <summary>
        /// A method calls when the user leaves the room
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roomName"></param>
        /// <returns></returns>
        public async Task LeaveRoom(string userName, string roomName)
        {
            roomName = roomName.ToLower();
            string currentConnectionId = Context.ConnectionId;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            //Notifies the other users
            await Clients.GroupExcept(roomName, currentConnectionId).SendAsync("ReceiveMessage", "Chat Hub", DateTimeOffset.Now, $"{userName} Disconnected ({DateTime.Now.ToShortTimeString()}).");

        }

        /// <summary>
        /// A method that sends message to all the connected clients
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="textMessage"></param>
        /// <returns></returns>
        public async Task SendAllMessage(string userName, string textMessage)
        {
            var message = new ChatMessage {
                UserName = userName,
                Message = textMessage,
                TimeStamp = DateTimeOffset.Now
            };

            //Sending to every client/user
            await Clients.All.SendAsync("ReceiveMessage", message.UserName, 
                message.TimeStamp, message.Message);
        }
    }
}
