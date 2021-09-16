// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.


//This is a connection between a client and a server
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

var userName;
var roomName;

//adding the connection to an event handler
//on receiving a call to method ReceiveMessage it will call the method displayMessage
connection.on('ReceiveMessage', displayMessage);
connection.start();

//on submitting a form, it will call the following method
var msgForm = document.forms.msgForm;

msgForm.addEventListener('submit', function (e) {
    e.preventDefault();
    var userMessage = document.getElementById('usermessage');
    var text = userMessage.value;
    var userName = document.getElementById('username').value;
    roomName = document.getElementById('roomName').value;
    sendMessage(roomName, userName, text);
    //sendMessage(userName, text)
});

function sendMessage(roomName, userName, message) {
    if (message && message.length) {
        connection.invoke("SendMessage", roomName, userName, message);
    }
}

function displayMessage(name, time, message) {
    var friendlyTime = moment(time).format('H:mm:ss');

    var userLi = document.createElement("li");
    userLi.className = 'userLi list-group-item';
    userLi.textContent = friendlyTime + ", " + name + " says:";

    var messageLi = document.createElement("li");
    userLi.className = 'messageLi list-group-item pl-5';
    messageLi.textContent = message;

    var chatHistoryUI = document.getElementById('chatHistory');
    chatHistoryUI.appendChild(userLi);
    chatHistoryUI.appendChild(messageLi);

    chatHistoryUI.animate({ scrollTop: $('#chatHistory').prop('scrollHeight')}, 50)

}

document.getElementById("btnJoin").addEventListener('click', function (e) {
    e.preventDefault();
    var userName = document.getElementById('username').value;
    var roomName = document.getElementById('roomName').value;
    var showError = document.getElementById("error");
    if (roomName && roomName.length && (userName.match("^[a-zA-Z]+$"))) {
        document.getElementById("btnJoin").disabled = true;
        document.getElementById("btnLeave").disabled = false;
        showError.textContent = '';
        connection.invoke('JoinRoom', userName, roomName);
    }
    else {

        showError.style.color = "red";
        showError.textContent = 'Username contains only letters!';

    }
});

document.getElementById("btnLeave").addEventListener('click', function (e) {
    e.preventDefault();
    var userName = document.getElementById('username').value;
    var roomName = document.getElementById('roomName').value;
    if (roomName && roomName.length) {
        document.getElementById("btnJoin").disabled = false;
        document.getElementById("btnLeave").disabled = true;
        connection.invoke('LeaveRoom', userName, roomName);
    }

});