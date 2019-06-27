mergeInto(LibraryManager.library, {
    JSConnectToServer: function (s1, s2, s3, s4, serverPort) {
	serverIp = s1 + "." + s2 + "." + s3 + "." + s4;
	console.log("RJ Opening a webserver connection to "+serverIp+":"+serverPort);
	var wsImpl = window.WebSocket || window.MozWebSocket;
	window.ws = new wsImpl("ws://"+serverIp+":"+serverPort);
	ws.onmessage = function (evt) {
		console.log("RJ on message from server");
		return false;
	}
	ws.onopen = function () {
		console.log("RJ on connection open");
		return false;
	}
	ws.onclose = function () {
		console.log("RJ on connection close");
		return false;
	}
    },

    JSSendJoystickToServer : function (x, y) {
        var msg = "move,"+ x + ","+ y;
	    console.log("RJ SendJoystickToServer:"+x+","+y+"\r\n");
	    ws.send(msg);
    }

    JSSendFireToServer : function() {
	var msg = "fire";
	ws.send(msg);
    }

    JSSendShieldToServer : function() {
	var msg = "shield";
	ws.send(msg);
    }
	
});


/*
    var start = function () {
            var inc = document.getElementById('incomming');
            var wsImpl = window.WebSocket || window.MozWebSocket;
            var form = document.getElementById('sendForm');
            var input = document.getElementById('sendText');
            
            inc.innerHTML += "connecting to server ..<br/>";
            // create a new websocket and connect
            window.ws = new wsImpl('ws://localhost:8181/');
            // when data is comming from the server, this metod is called
            ws.onmessage = function (evt) {
                inc.innerHTML += evt.data + '<br/>';
            };
            // when the connection is established, this method is called
            ws.onopen = function () {
                inc.innerHTML += '.. connection open<br/>';
            };
            // when the connection is closed, this method is called
            ws.onclose = function () {
                inc.innerHTML += '.. connection closed<br/>';
            }
            
			form.addEventListener('submit', function(e){
				e.preventDefault();
				var val = input.value;
				ws.send(val);
				input.value = "";
			});
            
        }
        window.onload = start;
*/
