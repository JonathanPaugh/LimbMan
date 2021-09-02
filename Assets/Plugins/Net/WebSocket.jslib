var webSocket = {
	$Socket: {
		connection: undefined,
	},

    WebSocketConnect: function() {
        if (Socket.connection != null) { return; }

	    console.log("WebSocket Starting");
	    
	    Socket.connection = new WebSocket("ws://127.0.0.1:8000");
	    Socket.connection.binaryType = "arraybuffer";

	    Socket.connection.onopen = function() {
	        console.log("WebSocket Open");
	    };

	    Socket.connection.onclose = function() {
	        console.log("WebSocket Closed");
	    };

	    Socket.connection.onerror = function(error) {
	        console.error(error);
	    };

	    Socket.connection.onmessage = function(event) {
	    	WebSocketReceive(event.data);
	    };
    },

    WebSocketDisconnect: function() {
        if (Socket.connection == null) { return; }
	    if (Socket.connection.readyState != 1) { return; }

	    Socket.connection.close();
    },

    WebSocketSend: function(data, length) {
        if (Socket.connection == null) { return; }
	    if (Socket.connection.readyState != 1) { return; }

	    sendData = new Uint8Array(length);
	    for (var i = 0; i < length; i++) {
	    	sendData[i] = HEAP8[data + i];
	    }

	    Socket.connection.send(sendData);
    },

    $WebSocketReceive: function(data) {
	    if (data.byteLength < 1) { return; }
	    SendMessage("WebManager", "WebSocketReceiveOpen", data.byteLength);
	    buffer = _malloc(data.byteLength);
	    heap = new Uint8Array(HEAPU8.buffer, buffer, data.byteLength);
	    heap.set(new Uint8Array(data));
		SendMessage("WebManager", "WebSocketReceiveClose", buffer);
		
    },
};

autoAddDeps(webSocket, "$Socket");
autoAddDeps(webSocket, "$WebSocketReceive");

mergeInto(LibraryManager.library, webSocket);