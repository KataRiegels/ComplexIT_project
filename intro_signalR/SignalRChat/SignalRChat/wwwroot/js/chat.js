"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

connection.on("ReceiveGroupName", function (message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `room name:  ${message}`;
});


connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


//connection.stop().then(function () {
//    document.getElementById("sendButton").disabled = false;
//}).catch(function (err) {
//    return console.error(err.toString());
//});

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("userInput").value;
//    var message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", user, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});


document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage",  message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});


document.getElementById("createRoom").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var name = document.getElementById("messageInput").value;
    connection.invoke("CreateRoom", user).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("joinRoom").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var name = document.getElementById("messageInput").value;
    connection.invoke("JoinRoom", user, name).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});