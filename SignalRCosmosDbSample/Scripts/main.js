$(function () {
	// Declare a proxy to reference the hub.
	var chat = $.connection.fanHub;
	// Create a function that the hub can call to broadcast messages.
	chat.client.broadcastMessage = function (name, message) {
		// Html encode display name and message.
		var encodedName = $('<div />').text(name).html();
		var encodedMsg = $('<div />').text(message).html();
		// Add the message to the page.
		$('#discussion').append('<li><strong>' + encodedName
			+ '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
	};

	// Create a random number to serve as the username
	var userId = Math.floor((Math.random() * 10000) + 1);

	$('#displayname').val(userId);
	// Set initial focus to message input box.
	$('#message').focus();
	// Start the connection.
	$.connection.hub.start().done(function () {
		$('#sendmessage').click(function () {
			// Call the Send method on the hub.
			chat.server.send($('#displayname').val(), $('#message').val());
			// Clear text box and reset focus for next comment.
			$('#message').val('').focus();
		});
	});
});

function sendMessage() {

	var chat = $.connection.fanHub;

	// Create a random number to serve as the OrderID
	var orderId = Math.floor((Math.random() * 10000) + 1);

	chat.server.sendOrder(orderId);

	$('#discussion').append('<li>OrderId ' + orderId + ' Sent</li>');
}